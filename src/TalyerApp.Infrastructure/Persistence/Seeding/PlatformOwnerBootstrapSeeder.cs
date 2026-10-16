using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Features.UserRoleAssignments;
using TalyerApp.Application.Interfaces;
using TalyerApp.Domain.Authorization;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class PlatformOwnerBootstrapSeeder : IPlatformOwnerBootstrapSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserRep _userRepository;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PlatformOwnerBootstrapOptions _options;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<PlatformOwnerBootstrapSeeder> _logger;

    public PlatformOwnerBootstrapSeeder(
        ApplicationDbContext context,
        ICommandDispatcher commandDispatcher,
        IUserRep userRepository,
        IUserRoleAssignmentRep userRoleAssignmentRepository,
        IUnitOfWork unitOfWork,
        IOptions<PlatformOwnerBootstrapOptions> options,
        IHostEnvironment hostEnvironment,
        ILogger<PlatformOwnerBootstrapSeeder> logger)
    {
        _context = context;
        _commandDispatcher = commandDispatcher;
        _userRepository = userRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        await EnsureReferenceDataExistsAsync(cancellationToken);

        var email = _options.Email.Trim();
        var userResult = await _userRepository.GetByEmailAsync(email);
        Guid userId;
        if (userResult.IsSuccess)
        {
            userId = userResult.Value.Id;
            _logger.LogInformation(
                "Platform owner user already exists. UserId: {UserId}, Email: {Email}",
                userId,
                email);
        }
        else
        {
            if (userResult.Error.Code != ErrorCodes.User.NotFound)
            {
                throw new InvalidOperationException(
                    $"Failed to look up platform owner ({userResult.Error.Code}).");
            }

            var createResult = User.Create(
                email,
                _options.Username.Trim(),
                _options.FirstName.Trim(),
                _options.LastName.Trim());
            if (createResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to create platform owner user ({createResult.Error.Code}).");
            }

            _userRepository.Add(createResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            userId = createResult.Value.Id;
            _logger.LogInformation(
                "Platform owner user created. UserId: {UserId}, Email: {Email}",
                userId,
                email);
        }

        var role = await _context.Roles
            .AsNoTracking()
            .SingleAsync(r => r.Code == RoleCodes.PlatformOwner, cancellationToken);

        var tenant = await _context.Tenants
            .AsNoTracking()
            .SingleAsync(t => t.Code == PlatformTenantCatalog.Code, cancellationToken);

        var assignmentExists = await _userRoleAssignmentRepository.ExistsAsync(
            userId,
            role.Id,
            tenant.Id,
            branchId: null,
            cancellationToken);

        if (assignmentExists)
        {
            _logger.LogInformation(
                "Platform owner role assignment already exists for UserId {UserId} on tenant '{TenantCode}'.",
                userId,
                PlatformTenantCatalog.Code);
            return;
        }

        var assignResult = await _commandDispatcher.DispatchAsync<AssignUserRoleCmd, int>(
            new AssignUserRoleCmd(userId, role.Id, tenant.Id, BranchId: null),
            cancellationToken);

        if (assignResult.IsSuccess)
        {
            _logger.LogInformation(
                "Platform owner role assigned. UserRoleAssignmentId: {AssignmentId}, UserId: {UserId}, TenantCode: {TenantCode}",
                assignResult.Value,
                userId,
                PlatformTenantCatalog.Code);
            return;
        }

        if (assignResult.Error.Code == ErrorCodes.UserRoleAssignment.AlreadyExists)
        {
            _logger.LogInformation(
                "Platform owner role assignment already exists for UserId {UserId} on tenant '{TenantCode}'.",
                userId,
                PlatformTenantCatalog.Code);
            return;
        }

        throw new InvalidOperationException(
            $"Failed to assign platform owner role ({assignResult.Error.Code}).");
    }

    private void ValidateOptions()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.Email));
        }

        if (string.IsNullOrWhiteSpace(_options.Username))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.Username));
        }

        if (string.IsNullOrWhiteSpace(_options.FirstName))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.FirstName));
        }

        if (string.IsNullOrWhiteSpace(_options.LastName))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.LastName));
        }

        if (missing.Count == 0)
        {
            return;
        }

        var missingList = string.Join(", ", missing);
        var hint = _hostEnvironment.IsDevelopment()
            ? "Copy src/TalyerApp.Api/usersecrets.example.json to usersecrets.json and set Bootstrap:PlatformOwner values."
            : "Set environment variables Bootstrap__PlatformOwner__* (e.g. Bootstrap__PlatformOwner__Email).";

        throw new InvalidOperationException(
            $"Platform owner bootstrap configuration is incomplete. Missing: {missingList}. {hint}");
    }

    private async Task EnsureReferenceDataExistsAsync(CancellationToken cancellationToken)
    {
        var roleExists = await _context.Roles
            .AnyAsync(r => r.Code == RoleCodes.PlatformOwner, cancellationToken);

        var tenantExists = await _context.Tenants
            .AnyAsync(t => t.Code == PlatformTenantCatalog.Code, cancellationToken);

        if (roleExists && tenantExists)
        {
            return;
        }

        throw new InvalidOperationException(
            "RBAC reference data or platform tenant is missing. Run 'dotnet run -- seed' after applying migrations.");
    }
}
