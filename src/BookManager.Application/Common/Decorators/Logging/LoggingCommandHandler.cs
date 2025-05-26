using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BookManager.Application.Common.Decorators.Logging;

internal sealed class LoggingCommandBaseHandler<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    ILogger<LoggingCommandBaseHandler<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command {CommandType}", typeof(TCommand).Name);
        var result = await innerHandler.Handle(command, cancellationToken);
        logger.LogInformation("Handle command {CommandType} with result: {IsSuccess}", typeof(TCommand).Name, result.IsSuccess);

        return result;
    }
}

internal sealed class LoggingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ILogger<LoggingCommandHandler<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command {CommandType}", typeof(TCommand).Name);
        var result = await innerHandler.Handle(command, cancellationToken);
        logger.LogInformation("Handled command {CommandType} with result: {IsSuccess}", typeof(TCommand).Name, result.IsSuccess);

        return result;
    }
}