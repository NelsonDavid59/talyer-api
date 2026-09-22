namespace TalyerApp.Application.Common.Interfaces.CQRS;

public interface IQuery
{
    
}

public interface IQuery<out TResponse> : IQuery
{
}