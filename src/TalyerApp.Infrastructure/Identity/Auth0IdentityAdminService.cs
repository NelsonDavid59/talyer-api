using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Identity;

public class Auth0IdentityAdminService : IIdentityAdminService
{
    private readonly HttpClient _httpClient;
    private readonly Auth0AdminOptions _options;
    private readonly ILogger<Auth0IdentityAdminService> _logger;

    public Auth0IdentityAdminService(
        HttpClient httpClient,
        IOptions<Auth0AdminOptions> options,
        ILogger<Auth0IdentityAdminService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public string Provider => "auth0";

    public async Task<Result<CreatedIdentityUser>> CreateUserAsync(
        CreateIdentityUserRequest createRequest,
        CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result<CreatedIdentityUser>.Failure(tokenResult.Error);
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/admin/realms/{_options.Realm}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        request.Content = JsonContent.Create(new
        {
            email = createRequest.Email,
            nickname = createRequest.Username,
            enabled = true,
            emailVerified = false
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return Result<CreatedIdentityUser>.Failure(DomainErrors.IdentityAdmin.UserAlreadyExists);
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Auth0 create user failed. Status: {Status}. Body: {Body}",
                (int)response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
            return Result<CreatedIdentityUser>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrWhiteSpace(location))
        {
            return Result<CreatedIdentityUser>.Failure(DomainErrors.IdentityAdmin.OperationFailed);
        }

        var id = location.TrimEnd('/').Split('/').Last();
        return Result<CreatedIdentityUser>.Success(new CreatedIdentityUser(Provider, id));
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var tokenResult = await GetAccessTokenAsync(cancellationToken);
        if (tokenResult.IsFailure)
        {
            _logger.LogError("Auth0 token failed while checking email existence.");
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
                "Auth0 lookup user by email failed. Status: {Status}",
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
                "Auth0 token failed while deleting user {ProviderUserId}.",
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
                "Failed to delete Auth0 user {ProviderUserId}. Status: {Status}",
                providerUserId,
                (int)response.StatusCode);
        }
    }

    private async Task<Result<string>> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            _logger.LogError("Auth0:Admin ClientId or ClientSecret is missing.");
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
                "Auth0 client credentials failed. Status: {Status}. Body: {Body}",
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
