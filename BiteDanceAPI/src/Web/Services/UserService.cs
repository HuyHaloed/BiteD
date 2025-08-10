using BiteDanceAPI.Application.Common.Exceptions;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiteDanceAPI.Web.Services
{
    public class UserService(
        ICurrentUser currentUser,
        IApplicationDbContext dbContext,
        ILogger<UserService> logger
    ) : IUserService
    {
        public async Task<User> GetFromDatabaseOrCreateAsync(
            CancellationToken cancellationToken,
            bool includeAssignedLocations = false
        )
        {
            IQueryable<User> query = dbContext.Users;
            if (includeAssignedLocations)
            {
                query = query.Include(u => u.AssignedLocations);
            }

            var user = await query.FirstOrDefaultAsync(
                x => x.Id == currentUser.Id,
                cancellationToken
            );

            if (user is not null)
            {
                return user;
            }

            if (currentUser.Id is null || currentUser.Email is null)
            {
                throw new ForbiddenAccessException();
            }

            // Create new user
            user = new User()
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                Name = currentUser.Name ?? currentUser.Email
            };
            logger.LogInformation("Creating new user {@user}", user);
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

       
    }
}
