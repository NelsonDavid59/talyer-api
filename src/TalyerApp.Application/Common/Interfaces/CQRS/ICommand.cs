namespace TalyerApp.Application.Common.Interfaces.CQRS;

public interface ICommand
{
}

public interface ICommand<out TResponse> : ICommand
{
}