using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<User> GetFromDatabaseOrCreateAsync(
            CancellationToken cancellationToken,
            bool includeAssignedLocations = false
        );
    }
}
