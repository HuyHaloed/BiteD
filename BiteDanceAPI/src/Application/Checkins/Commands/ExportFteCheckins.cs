using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BiteDanceAPI.Application.Checkins.Commands;

[Authorize(RequireAdmin = true)]
public record ExportFteCheckinsCommand(int LocationId, int Year, int Month) : IRequest<byte[]>;

public class ExportFteCheckinsCommandHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<ExportFteCheckinsCommand, byte[]>
{
    public async Task<byte[]> Handle(
        ExportFteCheckinsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Admin
        var currentUser = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        currentUser.AuthorizeAdminOrThrow(request.LocationId);

        // Fetch blue checkins
        var blueCheckins = await context.BlueCheckins
            .Where(c => c.LocationId == request.LocationId && c.Datetime.Year == request.Year && c.Datetime.Month == request.Month)
            .Include(c => c.User)
            .ToListAsync(cancellationToken);

        // Fetch green checkins
        var greenCheckins = await context.GreenCheckins
            .Where(c => c.LocationId == request.LocationId && c.Datetime.Year == request.Year && c.Datetime.Month == request.Month)
            .Include(c => c.User)
            .Include(c => c.ShiftOrder)
            .ToListAsync(cancellationToken);

        // Calculate checkin counts
        var userCheckinCounts = blueCheckins
            .GroupBy(c => c.UserId)
            .Select(g => new
            {
                UserEmail = g.First().User!.Email,
                UserName = g.First().User!.Name,
                Shift1Count = g.Count(c => c.Shift == ShiftType.Shift1),
                Shift2Count = g.Count(c => c.Shift == ShiftType.Shift2),
                Shift3Count = g.Count(c => c.Shift == ShiftType.Shift3)
            })
            .ToList();

        foreach (var greenGroup in greenCheckins.GroupBy(c => c.UserId))
        {
            var userCheckin = userCheckinCounts.FirstOrDefault(u => u.UserEmail == greenGroup.First().User!.Email);
            if (userCheckin != null)
            {
                var updatedUserCheckin = new
                {
                    userCheckin.UserEmail,
                    userCheckin.UserName,
                    Shift1Count = userCheckin.Shift1Count + greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift1),
                    Shift2Count = userCheckin.Shift2Count + greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift2),
                    Shift3Count = userCheckin.Shift3Count + greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift3)
                };
                userCheckinCounts.Remove(userCheckin);
                userCheckinCounts.Add(updatedUserCheckin);
            }
            else
            {
                userCheckinCounts.Add(new
                {
                    UserEmail = greenGroup.First().User!.Email,
                    UserName = greenGroup.First().User!.Name,
                    Shift1Count = greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift1),
                    Shift2Count = greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift2),
                    Shift3Count = greenGroup.Count(c => c.ShiftOrder.ShiftType == ShiftType.Shift3)
                });
            }
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("FTE Checkins");

        // Define headers
        var headers = new[] { "User Email", "Name", "Shift 1 Checkins", "Shift 2 Checkins", "Shift 3 Checkins" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Add data
        for (int i = 0; i < userCheckinCounts.Count; i++)
        {
            var row = i + 2;
            var userCheckin = userCheckinCounts[i];
            worksheet.Cell(row, 1).Value = userCheckin.UserEmail;
            worksheet.Cell(row, 2).Value = userCheckin.UserName;
            worksheet.Cell(row, 3).Value = userCheckin.Shift1Count;
            worksheet.Cell(row, 4).Value = userCheckin.Shift2Count;
            worksheet.Cell(row, 5).Value = userCheckin.Shift3Count;
        }

        // Resize columns to fit content
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
