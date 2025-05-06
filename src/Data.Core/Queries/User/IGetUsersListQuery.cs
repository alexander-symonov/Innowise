namespace Data.Core.Queries.User
{   
    public interface IGetUsersListQuery
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<DTO.QueryModels.User.User>> ExecuteAsync(CancellationToken cancellationToken);
    }
}
