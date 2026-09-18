using TalyerApp.Domain.Shared;

namespace TalyerApp.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsActive => RevokedAt == null && !IsExpired;

    private RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<RefreshToken> Create(Guid userId, string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<RefreshToken>.Failure(DomainErrors.RefreshToken.InvalidTokenFormat);

        if (expiresAt <= DateTime.UtcNow)
            return Result<RefreshToken>.Failure(DomainErrors.RefreshToken.InvalidExpirationDate);

        return Result<RefreshToken>.Success(new RefreshToken(userId, token, expiresAt));
    }
}