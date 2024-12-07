namespace ElgatoLightControl.ApiClient.Queries
{
    internal interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
    }
}