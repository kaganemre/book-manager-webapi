using System.Text.Json;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BookManager.Application.Common.Decorators.Logging;

internal sealed class LoggingQueryHandler<TQuery, TResponse>(
    ILogger<LoggingQueryHandler<TQuery, TResponse>> logger,
    IQueryHandler<TQuery, TResponse> innerHandler)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;
        var queryJson = JsonSerializer.Serialize(query);

        logger.LogInformation("🔎 Handling query: {QueryName} - Payload: {QueryPayload}", queryName, queryJson);

        var result = await innerHandler.Handle(query, cancellationToken);

        if (result.IsSuccess)
            logger.LogInformation("✅ Query {QueryName} handled successfully.", queryName);
        else
            logger.LogWarning("❌ Query {QueryName} failed with errors: {Errors}",
                queryName, string.Join(", ", result.Errors.Select(e => e.Message)));

        return result;
    }
}