using TalyerApp.Application.Dto;

namespace TalyerApp.Application.Common.Interfaces.Identity;

public interface ITalyerSessionTokenIssuer
{
    string Issue(SessionDto session);
}
