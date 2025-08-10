using BiteDanceAPI.Application.Common.Exceptions;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Common.Security;

public static class AdminAuthorization
{
    public static bool AuthorizeAdmin(this User admin, int locationId)
    {
        return admin.AssignedLocations.Any(l => l.Id == locationId);
    }

    public static void AuthorizeAdminOrThrow(this User admin, int locationId)
    {
        if (!admin.AuthorizeAdmin(locationId))
        {
            throw new ForbiddenAccessException();
        }
    }
}
