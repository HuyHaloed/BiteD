using BiteDanceAPI.Application.Common;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Orders.Commands;

[Authorize(DenySupplier = true)]
public record CreateOrderCommand : IRequest<int>
{
    public int LocationId { get; init; }
    public int MonthlyMenuId { get; init; }
    public DateOnly Date { get; init; }
    public IReadOnlyList<ShiftOrderDto> ShiftOrders { get; init; } = new List<ShiftOrderDto>();
}

public record ShiftOrderDto
{
    public ShiftType ShiftType { get; init; }
    public IReadOnlyList<int> DishIds { get; init; } = new List<int>();
}

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUser _currentUser;

    public CreateOrderCommandValidator(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        ICurrentUser currentUser
    )
    {
        _context = context;
        _timeProvider = timeProvider;
        _currentUser = currentUser;

        RuleFor(v => v.LocationId)
            .MustAsync(LocationExists)
            .WithMessage("Location does not exist.");

        RuleFor(v => v)
            .MustAsync(MonthlyMenuValid)
            .WithMessage("No available monthly menu for the specified location and date.")
            .Must(UniqueDishIds)
            .WithMessage("Duplicate dish IDs are not allowed.")
            .MustAsync(ValidDishes)
            .WithMessage("Invalid dishes.")
            // .MustAsync(ValidDishSelection)
            // .WithMessage("Invalid dish selection for the location and shift.")
            
            .Must(ValidOrderTime)
            .WithMessage("Orders must be placed before 4pm GMT+7 of the previous day.")
            .MustAsync(OneActiveOrderPerDay)
            .WithMessage("You can only order once per location per day.");

        RuleFor(v => v.ShiftOrders)
            .NotEmpty()
            .WithMessage("At least one shift order must be provided.");

        RuleForEach(v => v.ShiftOrders)
            .ChildRules(shiftOrder =>
            {
                shiftOrder
                    .RuleFor(s => s.DishIds)
                    .NotEmpty()
                    .WithMessage("At least one dish must be selected.");
            });
    }

    private async Task<bool> LocationExists(int locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(l => l.Id == locationId, cancellationToken);
    }

    private async Task<bool> MonthlyMenuValid(
        CreateOrderCommand command,
        CancellationToken cancellationToken
    )
    {
        var monthlyMenu = await _context.MonthlyMenus.FindAsync(
            [command.MonthlyMenuId],
            cancellationToken
        );

        return monthlyMenu != null
            && monthlyMenu.LocationId == command.LocationId
            && monthlyMenu.Year == command.Date.Year
            && monthlyMenu.Month == command.Date.Month
            && monthlyMenu.IsPublished;
    }

    private static bool UniqueDishIds(CreateOrderCommand command)
    {
        return command.ShiftOrders.All(shiftOrder =>
            shiftOrder.DishIds.Distinct().Count() == shiftOrder.DishIds.Count
        );
    }
    private bool ValidOrderTime(CreateOrderCommand command)
    {
        var currentTime = _timeProvider.GetUtcNow();
        return OrderTimeValidator.IsValidOrderTime(command.Date, currentTime);
    }
    private async Task<bool> OneActiveOrderPerDay(
        CreateOrderCommand command,
        CancellationToken cancellationToken
    )
    {
        return !await _context.DailyOrders.AnyAsync(
            o =>
                o.UserId == _currentUser.Id
                && o.LocationId == command.LocationId
                && o.Date == command.Date
                && o.Status == DailyOrderStatus.Ordered,
            cancellationToken
        );
    }

    private async Task<bool> ValidDishes(
        CreateOrderCommand command,
        CancellationToken cancellationToken
    )
    {
        var dailyMenu = await _context
            .DailyMenus.Include(d => d.ShiftMenus)
            .ThenInclude(s => s.Dishes)
            .FirstOrDefaultAsync(
                d => d.MonthlyMenuId == command.MonthlyMenuId && d.Date == command.Date,
                cancellationToken
            );
        if (dailyMenu == null)
            return false;

        foreach (var shiftOrder in command.ShiftOrders)
        {
            var shiftMenu = dailyMenu.ShiftMenus.FirstOrDefault(s =>
                s.Shift == shiftOrder.ShiftType
            );
            if (shiftMenu == null)
                return false;

            var validDishIds = shiftMenu.Dishes.Select(d => d.Id).ToHashSet();
            if (!shiftOrder.DishIds.All(dishId => validDishIds.Contains(dishId)))
                return false;
        }
        return true;
    }

    // private async Task<bool> ValidDishSelection(
    //     OrderMealCommand command,
    //     CancellationToken cancellationToken
    // )
    // {
    //     var location = await _context.Locations.FindAsync(
    //         new object[] { command.LocationId },
    //         cancellationToken
    //     );
    //     if (location == null)
    //         return false;
    //
    //     var dailyMenu = await _context
    //         .DailyMenus.Include(d => d.ShiftMenus)
    //         .ThenInclude(s => s.Dishes)
    //         .FirstOrDefaultAsync(
    //             d => d.MonthlyMenuId == command.MonthlyMenuId && d.Date == command.Date,
    //             cancellationToken
    //         );
    //     if (dailyMenu == null)
    //         return false;
    //
    //     foreach (var shiftOrder in command.ShiftOrders)
    //     {
    //         var shiftMenu = dailyMenu.ShiftMenus.FirstOrDefault(s =>
    //             s.Shift == shiftOrder.ShiftType
    //         );
    //         if (shiftMenu == null)
    //             return false;
    //
    //         var validDishIds = shiftMenu.Dishes.Select(d => d.Id).ToHashSet();
    //         var selectedDishes = shiftMenu.Dishes.Where(d => validDishIds.Contains(d.Id)).ToList();
    //
    //         switch (location.Type)
    //         {
    //             case LocationType.HeadOffice
    //                 when shiftOrder.ShiftType == ShiftType.Shift2
    //                     && !(
    //                         selectedDishes.Count == 1
    //                         && (
    //                             selectedDishes.Count(d => d.Type == DishType.Main) == 1
    //                             || selectedDishes.Count(d => d.Type == DishType.Vegetarian) == 1
    //                         )
    //                     ):
    //                 return false;
    //             case LocationType.Brewery:
    //             {
    //                 switch (shiftOrder.ShiftType)
    //                 {
    //                     case ShiftType.Shift1:
    //                     case ShiftType.Shift2:
    //                     {
    //                         if (
    //                             !(
    //                                 selectedDishes.Count == 1
    //                                     && (
    //                                         selectedDishes.Count(d => d.Type == DishType.Soup) == 1
    //                                         || selectedDishes.Count(d =>
    //                                             d.Type == DishType.Vegetarian
    //                                         ) == 1
    //                                     )
    //                                 || selectedDishes.Count == 2
    //                                     && selectedDishes.Count(d => d.Type == DishType.Main) == 2
    //                             )
    //                         )
    //                             return false;
    //                         break;
    //                     }
    //                     case ShiftType.Shift3
    //                         when !(
    //                             selectedDishes.Count == 1
    //                             && selectedDishes.Count(d => d.Type == DishType.Main) == 1
    //                         ):
    //                         return false;
    //                     default:
    //                         return false;
    //                 }
    //
    //                 break;
    //             }
    //             default:
    //                 return false;
    //         }
    //     }
    //     return true;
    // }
}

public class CreateOrderCommandHandler(IApplicationDbContext context, IUserService userService)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.GetFromDatabaseOrCreateAsync(cancellationToken);
        var location = await context.Locations.FindOrNotFoundExceptionAsync(
            request.LocationId,
            cancellationToken
        );

        var dailyMenu = await context
            .DailyMenus.Include(d => d.ShiftMenus)
            .ThenInclude(s => s.Dishes)
            .FirstOrDefaultAsync(
                d => d.MonthlyMenuId == request.MonthlyMenuId && d.Date == request.Date,
                cancellationToken
            );
        Guard.Against.NotFound(request.MonthlyMenuId, dailyMenu);

        var dailyOrder = new DailyOrder
        {
            Date = request.Date,
            User = user,
            UserId = user.Id,
            Location = location,
            LocationId = request.LocationId,
            ShiftOrders = new List<ShiftOrder>()
        };
        dailyOrder.SetStatus(DailyOrderStatus.Ordered);

        foreach (var shiftOrderDto in request.ShiftOrders)
        {
            var shiftMenu = dailyMenu.ShiftMenus.FirstOrDefault(s =>
                s.Shift == shiftOrderDto.ShiftType
            );
            Guard.Against.NotFound(request.LocationId, shiftMenu);

            var shiftOrder = new ShiftOrder
            {
                Status = ShiftOrderStatus.Ordered,
                DailyOrder = dailyOrder,
                ShiftMenu = shiftMenu,
                ShiftType = shiftMenu.Shift,
                Location = location,
                LocationId = request.LocationId,
                Dishes = shiftMenu.Dishes.Where(d => shiftOrderDto.DishIds.Contains(d.Id)).ToList()
            };

            dailyOrder.ShiftOrders.Add(shiftOrder);
        }

        context.DailyOrders.Add(dailyOrder);
        await context.SaveChangesAsync(cancellationToken);

        return dailyOrder.Id;
    }
}
