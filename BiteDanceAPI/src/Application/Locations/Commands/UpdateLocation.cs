using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Locations.Commands;

[Authorize(RequireSuperAdmin = true)]
public record UpdateLocationCommand : IRequest
{
    public required int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public LocationType Type { get; init; }
    public bool EnableShift1 { get; init; }
    public bool EnableShift2 { get; init; }
    public bool EnableShift3 { get; init; }
    public bool EnableWeekday { get; init; }
    public bool EnableWeekend { get; init; }
    public IReadOnlyCollection<string> AdminEmails { get; init; } = [];
    public int? SupplierId { get; init; }
}

public class UpdateLocationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateLocationCommand>
{
    public async Task Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await context
            .Locations.Include(l => l.Admins)
            .ThenInclude(a => a.AssignedLocations)
            .FindOrNotFoundExceptionAsync(request.Id, cancellationToken);

        location.Name = request.Name;
        location.Description = request.Description;
        location.City = request.City;
        location.Country = request.Country;
        location.Type = request.Type;
        location.EnableShift1 = request.EnableShift1;
        location.EnableShift2 = request.EnableShift2;
        location.EnableShift3 = request.EnableShift3;
        location.EnableWeekday = request.EnableWeekday;
        location.EnableWeekend = request.EnableWeekend;

        if (request.SupplierId.HasValue)
        {
            location.Supplier = await context.Suppliers.FindOrNotFoundExceptionAsync(
                request.SupplierId.Value,
                cancellationToken
            );
        }
        else
        {
            location.Supplier = null;
        }

        var currentAdminEmails = location.Admins.Select(a => a.Email).ToList();
        var newAdminEmails = request.AdminEmails.Except(currentAdminEmails).ToList();
        var removedAdminEmails = currentAdminEmails.Except(request.AdminEmails).ToList();

        foreach (var email in newAdminEmails)
        {
            var user = await context.Users.FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken
            );
            Guard.Against.NotFound(email, user);
            user.IsAdmin = true;
            location.Admins.Add(user);
        }

        foreach (var email in removedAdminEmails)
        {
            var user = location.Admins.FirstOrDefault(a => a.Email == email);
            if (user == null)
            {
                continue;
            }

            location.Admins.Remove(user);
            if (user.AssignedLocations.Count == 1)
            {
                user.IsAdmin = false;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
