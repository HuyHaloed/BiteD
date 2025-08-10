// using BiteDanceAPI.Application.Common.Interfaces;
// using BiteDanceAPI.Domain.Entities;
// using BiteDanceAPI.Domain.Enums;
//
// namespace BiteDanceAPI.Application.Checkins.Commands;
//
// public record CheckinPurpleQrCommand : IRequest
// {
//     public int LocationId { get; init; }
//     public int PurpleScanCodeId { get; init; }
// }
//
// public class CheckinPurpleQrCommandValidator : AbstractValidator<CheckinPurpleQrCommand>
// {
//     private readonly IApplicationDbContext _context;
//     private readonly IUserService _userService;
//     private readonly TimeProvider _timeProvider;
//
//     public CheckinPurpleQrCommandValidator(
//         IApplicationDbContext context,
//         IUserService userService,
//         TimeProvider timeProvider
//     )
//     {
//         _context = context;
//         _userService = userService;
//         _timeProvider = timeProvider;
//
//         RuleFor(v => v.LocationId)
//             .MustAsync(AdminAssignedToLocation)
//             .WithMessage("Admin is not assigned to the location.");
//
//         RuleFor(v => v.PurpleScanCodeId)
//             .MustAsync(CodeExists)
//             .WithMessage("Code does not exist.")
//             .MustAsync(CodeNotExpired)
//             .WithMessage("Code is expired.")
//             .MustAsync(CodeMaxScansNotExceeded)
//             .WithMessage("Code max number of scans exceeded.")
//             .MustAsync(CodeNotDisabled)
//             .WithMessage("Code is disabled.");
//     }
//
//     private async Task<bool> AdminAssignedToLocation(
//         int locationId,
//         CancellationToken cancellationToken
//     )
//     {
//         var currentUser = await _userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
//         return currentUser.AssignedLocations.Any(l => l.Id == locationId);
//     }
//
//     private async Task<bool> CodeExists(int codeId, CancellationToken cancellationToken)
//     {
//         return await _context.PurpleScanCodes.AnyAsync(c => c.Id == codeId, cancellationToken);
//     }
//
//     private async Task<bool> CodeNotExpired(int codeId, CancellationToken cancellationToken)
//     {
//         var code = await _context.PurpleScanCodes.FindAsync([codeId], cancellationToken);
//         return code != null && code.ExpirationDate > _timeProvider.GetLocalNow();
//     }
//
//     private async Task<bool> CodeMaxScansNotExceeded(
//         int codeId,
//         CancellationToken cancellationToken
//     )
//     {
//         var code = await _context.PurpleScanCodes.FindAsync([codeId], cancellationToken);
//         if (code == null)
//             return false;
//
//         var scanCount = await _context.PurpleCheckins.CountAsync(
//             c => c.ScanCodeId == codeId,
//             cancellationToken
//         );
//         return scanCount < code.MaxNumScans;
//     }
//
//     private async Task<bool> CodeNotDisabled(int codeId, CancellationToken cancellationToken)
//     {
//         var code = await _context.PurpleScanCodes.FindAsync([codeId], cancellationToken);
//         return code != null && !code.IsDisabled;
//     }
// }
//
// public class CheckinPurpleQrCommandHandler(IApplicationDbContext context, TimeProvider timeProvider)
//     : IRequestHandler<CheckinPurpleQrCommand>
// {
//     public async Task Handle(CheckinPurpleQrCommand request, CancellationToken cancellationToken)
//     {
//         var location = await context.Locations.FindAsync(request.LocationId, cancellationToken);
//         Guard.Against.NotFound(request.LocationId, location);
//
//         var scanCode = await context.PurpleScanCodes.FindAsync(
//             request.PurpleScanCodeId,
//             cancellationToken
//         );
//         Guard.Against.NotFound(request.PurpleScanCodeId, scanCode);
//
//         var checkin = new PurpleCheckin
//         {
//             Location = location,
//             LocationId = request.LocationId,
//             ScanCode = scanCode,
//             ScanCodeId = request.PurpleScanCodeId,
//             Datetime = timeProvider.GetLocalNow(),
//             Type = CodeType.Purple,
//         };
//
//         context.PurpleCheckins.Add(checkin);
//         await context.SaveChangesAsync(cancellationToken);
//     }
// }
