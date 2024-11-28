namespace ElgatoLightApiClient.Services
{
    using Commands;

    using Queries;

    internal static class Dispatcher
    {
        private static readonly Dictionary<Type, Object> _handlers = new();

        internal static void RegisterHandler<T>(Object handler) => _handlers[typeof(T)] = handler;

        internal static async Task Send<TCommand>(TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if (_handlers.TryGetValue(typeof(TCommand), out var handler))
            {
                await ((ICommandHandler<TCommand>)handler).Handle(command, cancellationToken);
            }

            throw new InvalidOperationException($"No handler registered for {typeof(TCommand).Name}");
        }

        internal static async Task<TResult> Send<TCommand, TResult>(TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>
        {
            if (_handlers.TryGetValue(typeof(TCommand), out var handler))
            {
                return await ((ICommandHandler<TCommand, TResult>)handler).Handle(command, cancellationToken);
            }

            throw new InvalidOperationException($"No handler registered for {typeof(TCommand).Name}");
        }

        internal static async Task<TResult> Query<TQuery, TResult>(TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (_handlers.TryGetValue(typeof(TQuery), out var handler))
            {
                return await ((IQueryHandler<TQuery, TResult>)handler).Handle(query, cancellationToken);
            }

            throw new InvalidOperationException($"No handler registered for {typeof(TQuery).Name}");
        }
    }
}