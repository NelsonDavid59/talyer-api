using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Features.UserRoleAssignments;
using TalyerApp.Application.Interfaces;
using TalyerApp.Domain.Authorization;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class PlatformOwnerBootstrapSeeder : IPlatformOwnerBootstrapSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;
    private readonly PlatformOwnerBootstrapOptions _options;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<PlatformOwnerBootstrapSeeder> _logger;

    public PlatformOwnerBootstrapSeeder(
        ApplicationDbContext context,
        ICommandDispatcher commandDispatcher,
        IUserRoleAssignmentRep userRoleAssignmentRepository,
        IOptions<PlatformOwnerBootstrapOptions> options,
        IHostEnvironment hostEnvironment,
        ILogger<PlatformOwnerBootstrapSeeder> logger)
    {
        _context = context;
        _commandDispatcher = commandDispatcher;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
        _options = options.Value;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        await EnsureReferenceDataExistsAsync(cancellationToken);

        var provider = _options.Provider.Trim();

        var registerResult = await _commandDispatcher.DispatchAsync<RegisterExternalIdentityCmd, Guid>(
            new RegisterExternalIdentityCmd(
                Provider: provider,
                ProviderUserId: _options.ProviderUserId.Trim(),
                Username: _options.Username.Trim(),
                FirstName: _options.FirstName.Trim(),
                LastName: _options.LastName.Trim(),
                Email: _options.Email.Trim()),
            cancellationToken);

        if (registerResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to register platform owner external identity ({registerResult.Error.Code}).");
        }

        var userId = registerResult.Value;
        _logger.LogInformation(
            "Platform owner user linked. UserId: {UserId}, Provider: {Provider}, ProviderUserId: {ProviderUserId}",
            userId,
            provider,
            _options.ProviderUserId);

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

        if (string.IsNullOrWhiteSpace(_options.Provider))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.Provider));
        }

        if (string.IsNullOrWhiteSpace(_options.ProviderUserId))
        {
            missing.Add(nameof(PlatformOwnerBootstrapOptions.ProviderUserId));
        }

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
            : "Set environment variables Bootstrap__PlatformOwner__* (e.g. Bootstrap__PlatformOwner__ProviderUserId).";

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
