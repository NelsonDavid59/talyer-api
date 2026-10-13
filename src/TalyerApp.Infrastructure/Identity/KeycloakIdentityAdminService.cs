using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Identity;

public class KeycloakIdentityAdminService : IIdentityAdminService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _options;
    private readonly ILogger<KeycloakIdentityAdminService> _logger;

    public KeycloakIdentityAdminService(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger<KeycloakIdentityAdminService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<string>> CreateUserAsync(
        string email,
        string username,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result<string>.Failure(tokenResult.Error);
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/admin/realms/{_options.Realm}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        request.Content = JsonContent.Create(new
        {
            username,
            email,
            firstName,
            lastName,
            enabled = true,
            emailVerified = false
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return Result<string>.Failure(DomainErrors.IdentityAdmin.UserAlreadyExists);
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Keycloak create user failed. Status: {Status}. Body: {Body}",
                (int)response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrWhiteSpace(location))
        {
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var id = location.TrimEnd('/').Split('/').Last();
        return Result<string>.Success(id);
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            _logger.LogError("Keycloak token failed while checking email existence.");
            return false;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/admin/realms/{_options.Realm}/users?email={Uri.EscapeDataString(email)}&exact=true");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Keycloak lookup user by email failed. Status: {Status}",
                (int)response.StatusCode);
            return false;
        }

        var users = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        return users.ValueKind == JsonValueKind.Array && users.GetArrayLength() > 0;
    }

    public async Task DeleteUserAsync(string providerUserId, CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            _logger.LogError(
                "Keycloak token failed while deleting user {ProviderUserId}.",
                providerUserId);
            return;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/admin/realms/{_options.Realm}/users/{providerUserId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
        {
            _logger.LogError(
                "Failed to delete Keycloak user {ProviderUserId}. Status: {Status}",
                providerUserId,
                (int)response.StatusCode);
        }
    }

    public async Task<Result> SendExecuteActionsEmailAsync(
        string providerUserId,
        IReadOnlyList<string> actions,
        CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure(tokenResult.Error);
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/admin/realms/{_options.Realm}/users/{providerUserId}/execute-actions-email");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        request.Content = JsonContent.Create(actions);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Keycloak execute-actions-email failed for {ProviderUserId}. Status: {Status}. Body: {Body}",
                providerUserId,
                (int)response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
            return Result.Failure(DomainErrors.MembershipRequest.ApprovalSystemError);
        }

        return Result.Success();
    }

    private async Task<Result<string>> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            _logger.LogError("Keycloak:Admin ClientId or ClientSecret is missing.");
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/realms/{_options.Realm}/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Keycloak client credentials failed. Status: {Status}. Body: {Body}",
                (int)response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (!payload.TryGetProperty("access_token", out var tokenElement))
        {
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var token = tokenElement.GetString();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result<string>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        return Result<string>.Success(token);
    }
}
