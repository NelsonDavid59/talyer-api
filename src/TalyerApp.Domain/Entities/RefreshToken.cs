using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public DateTime? RevokedAt { get; private set; }
    public bool IsActive => RevokedAt == null && !IsExpired;

    private RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        Id = Guid.NewGuid();}

    public static Result<RefreshToken> Create(Guid userId, string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<RefreshToken>.Failure(DomainErrors.RefreshToken.InvalidTokenFormat);

        if (expiresAt <= DateTime.UtcNow)
            return Result<RefreshToken>.Failure(DomainErrors.RefreshToken.InvalidExpirationDate);

        return Result<RefreshToken>.Success(new RefreshToken(userId, token, expiresAt));
    }
}