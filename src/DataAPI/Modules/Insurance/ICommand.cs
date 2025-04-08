namespace DataAPI.Modules.Insurance
{
    public interface ICommand<T> where T : class
    {
        Task<bool> ExecuteAsync(T context);
    }
}
