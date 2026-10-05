using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.UserRoleAssignments;

public class AssignUserRoleCmdHdlr : ICommandHandler<AssignUserRoleCmd, int>
{
    private readonly IUserRep _userRepository;
    private readonly IRoleRep _roleRepository;
    private readonly ITenantRep _tenantRepository;
    private readonly IBranchRep _branchRepository;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabaseExceptionTranslator _databaseExceptionTranslator;

    public AssignUserRoleCmdHdlr(
        IUserRep userRepository,
        IRoleRep roleRepository,
        ITenantRep tenantRepository,
        IBranchRep branchRepository,
        IUserRoleAssignmentRep userRoleAssignmentRepository,
        IUnitOfWork unitOfWork,
        IDatabaseExceptionTranslator databaseExceptionTranslator)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tenantRepository = tenantRepository;
        _branchRepository = branchRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
        _unitOfWork = unitOfWork;
        _databaseExceptionTranslator = databaseExceptionTranslator;
    }

    public async Task<Result<int>> HandleAsync(
        AssignUserRoleCmd command,
        CancellationToken cancellationToken = default)
    {
        var assignmentResult = UserRoleAssignment.Create(
            command.UserId,
            command.RoleId,
            command.TenantId,
            command.BranchId);

        if (assignmentResult.IsFailure)
        {
            return Result<int>.Failure(assignmentResult.Error);
        }

        // check if user exists    
        var userResult = await _userRepository.GetByIdAsync(command.UserId);
        if (userResult.IsFailure)
        {
            return Result<int>.Failure(userResult.Error);
        }

        // check if role exists
        var roleResult = await _roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
        if (roleResult.IsFailure)
        {
            return Result<int>.Failure(roleResult.Error);
        }

        // check if tenant exists
        var tenantResult = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
        if (tenantResult.IsFailure)
        {
            return Result<int>.Failure(tenantResult.Error);
        }

        // check if branch exists
        if (command.BranchId.HasValue)
        {
            // check if branch exists for the tenant
            var branchResult = await _branchRepository.GetByTenantAndIdAsync(
                command.TenantId,
                command.BranchId.Value,
                cancellationToken);

            if (branchResult.IsFailure)
            {
                return Result<int>.Failure(branchResult.Error);
            }
        }

        var alreadyExists = await _userRoleAssignmentRepository.ExistsAsync(
            command.UserId,
            command.RoleId,
            command.TenantId,
            command.BranchId,
            cancellationToken);

        if (alreadyExists)
        {
            return Result<int>.Failure(DomainErrors.UserRoleAssignment.AlreadyExists);
        }

        var assignment = assignmentResult.Value;
        _userRoleAssignmentRepository.Add(assignment);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var translated = _databaseExceptionTranslator.FromSaveChanges(
                ex,
                new DatabaseExceptionContext(DomainErrors.UserRoleAssignment.AlreadyExists));
            if (translated is not null)
            {
                return Result<int>.Failure(translated);
            }

            throw;
        }

        return Result<int>.Success(assignment.Id);
    }
}
