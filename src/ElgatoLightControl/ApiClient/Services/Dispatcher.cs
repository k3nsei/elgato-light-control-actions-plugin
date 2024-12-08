namespace ElgatoLightControl.ApiClient.Services
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
            if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
            {
                var ex = new InvalidOperationException($"No handler registered for {typeof(TCommand).Name}");

                Logger.Error(ex, ex.Message);
                throw ex;
            }

            try
            {
                await ((ICommandHandler<TCommand>)handler).Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error while handling command {typeof(TCommand).Name}");
                throw;
            }
        }

        internal static async Task<TResult> Send<TCommand, TResult>(TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>
        {
            if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
            {
                var ex = new InvalidOperationException($"No command handler registered for {typeof(TCommand).Name}");

                Logger.Error(ex, ex.Message);
                throw ex;
            }

            try
            {
                return await ((ICommandHandler<TCommand, TResult>)handler).Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error while handling command {typeof(TCommand).Name}");
                throw;
            }
        }

        internal static async Task<TResult> Query<TQuery, TResult>(TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (!_handlers.TryGetValue(typeof(TQuery), out var handler))
            {
                var ex = new InvalidOperationException($"No query handler registered for {typeof(TQuery).Name}");

                Logger.Error(ex, ex.Message);
                throw ex;
            }

            try
            {
                return await ((IQueryHandler<TQuery, TResult>)handler).Handle(query, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error while handling query {typeof(TQuery).Name}");
                throw;
            }
        }
    }
}