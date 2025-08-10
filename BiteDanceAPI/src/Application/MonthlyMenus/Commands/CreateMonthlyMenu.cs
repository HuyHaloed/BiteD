using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.Services;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.MonthlyMenus.Commands;

[Authorize(RequireAdmin = true)]
public record CreateMonthlyMenuCommand : IRequest<int>
{
    public int LocationId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public IReadOnlyCollection<ShiftMenuDto> ShiftMenus { get; init; } = [];
}

public record ShiftMenuDto
{
    public DateOnly DayOfMonth { get; init; }
    public ShiftType ShiftType { get; init; }
    public IReadOnlyCollection<string> MainDishes { get; init; } = [];
    public IReadOnlyCollection<string> Soups { get; init; } = [];
    public IReadOnlyCollection<string> VegetarianCombos { get; init; } = [];
    public IReadOnlyCollection<string> SideDishes { get; init; } = [];
}

public class CreateMonthlyMenuCommandValidator : AbstractValidator<CreateMonthlyMenuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHolidayService _holidayService;
    private readonly IUserService _userService;

    public CreateMonthlyMenuCommandValidator(
        IApplicationDbContext context,
        IHolidayService holidayService,
        IUserService userService
    )
    {
        _context = context;
        _holidayService = holidayService;
        _userService = userService;

        RuleFor(v => v.LocationId)
            .MustAsync(BeExistingLocation)
            .WithMessage("Location does not exist.")
            .MustAsync(BeActiveLocation)
            .WithMessage("Location is not active.")
            .MustAsync(HaveAssignedSupplier)
            .WithMessage("Location does not have an assigned supplier.")
            .MustAsync(HaveActiveSupplier)
            .WithMessage("Location's assigned supplier is not active.")
            .MustAsync(BeAssignedToAdmin)
            .WithMessage("Current admin is not assigned to this location.");

        RuleFor(v => v)
            .MustAsync(NotHaveExistingMonthlyMenu)
            .WithMessage("A monthly menu for this year, month, and location already exists.")
            .MustAsync(CoverAllRequiredDays)
            .WithMessage("Shift menus must cover all required days of the month.")
            .MustAsync(HaveRequiredDishes)
            .WithMessage("Shift menus must contain the required number of dishes.")
            .MustAsync(CoverAllRequiredShifts)
            .WithMessage("Shift menus must cover all required shifts for the location.");

        RuleForEach(v => v.ShiftMenus)
            .Must(IsValidDayOfMonth)
            .WithMessage("DayOfMonth must be within the specified year and month.");
    }

    private async Task<bool> BeExistingLocation(int locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(l => l.Id == locationId, cancellationToken);
    }

    private async Task<bool> BeActiveLocation(int locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(
            l => l.Id == locationId && l.IsActive,
            cancellationToken
        );
    }

    private async Task<bool> HaveAssignedSupplier(
        int locationId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Locations.AnyAsync(
            l => l.Id == locationId && l.SupplierId.HasValue,
            cancellationToken
        );
    }

    private async Task<bool> HaveActiveSupplier(int locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(
            l => l.Id == locationId && l.Supplier != null && l.Supplier.IsActive,
            cancellationToken
        );
    }

    private async Task<bool> BeAssignedToAdmin(int locationId, CancellationToken cancellationToken)
    {
        var admin = await _userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        return admin.AuthorizeAdmin(locationId);
    }

    private async Task<bool> NotHaveExistingMonthlyMenu(
        CreateMonthlyMenuCommand command,
        CancellationToken cancellationToken
    )
    {
        return !await _context.MonthlyMenus.AnyAsync(
            m =>
                m.Location.Id == command.LocationId
                && m.Year == command.Year
                && m.Month == command.Month,
            cancellationToken
        );
    }

    private async Task<bool> CoverAllRequiredDays(
        CreateMonthlyMenuCommand command,
        CancellationToken cancellationToken
    )
    {
        var location = await _context.Locations.FindAsync([command.LocationId], cancellationToken);
        if (location == null)
            return false;

        var daysInMonth = DateTime.DaysInMonth(command.Year, command.Month);
        var requiredDays = new HashSet<DateOnly>();

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(command.Year, command.Month, day);
            var isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

            if ((isWeekend && location.EnableWeekend) || (!isWeekend && location.EnableWeekday))
            {
                if (!_holidayService.IsPublicHoliday(date))
                {
                    requiredDays.Add(DateOnly.FromDateTime(date));
                }
            }
        }

        var providedDays = command.ShiftMenus.Select(sm => sm.DayOfMonth).ToHashSet();

        // Ensure all required days are included in the provided days
        return requiredDays.IsSubsetOf(providedDays);
    }

    private async Task<bool> HaveRequiredDishes(
        CreateMonthlyMenuCommand command,
        CancellationToken cancellationToken
    )
    {
        var location = await _context.Locations.FindAsync([command.LocationId], cancellationToken);
        if (location == null)
            return false;

        foreach (var shiftMenu in command.ShiftMenus)
        {
            if (
                !(
                    (location.Type, shiftMenu.ShiftType) switch
                    {
                        (LocationType.Brewery, ShiftType.Shift1 or ShiftType.Shift2)
                            => shiftMenu.MainDishes.Count >= 3
                                && shiftMenu.Soups.Count >= 1
                                && shiftMenu.VegetarianCombos.Count >= 1,

                        (LocationType.Brewery, ShiftType.Shift3) => shiftMenu.MainDishes.Count >= 1,

                        (LocationType.HeadOffice, ShiftType.Shift2)
                            => shiftMenu.MainDishes.Count >= 2
                                && shiftMenu.VegetarianCombos.Count >= 1,

                        _ => true // For any other combination, we don't have specific requirements
                    }
                )
            )
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> CoverAllRequiredShifts(
        CreateMonthlyMenuCommand command,
        CancellationToken cancellationToken
    )
    {
        var location = await _context.Locations.FindAsync([command.LocationId], cancellationToken);
        if (location == null)
            return false;

        var requiredShifts = new HashSet<ShiftType>();
        if (location.EnableShift1)
            requiredShifts.Add(ShiftType.Shift1);
        if (location.EnableShift2)
            requiredShifts.Add(ShiftType.Shift2);
        if (location.EnableShift3)
            requiredShifts.Add(ShiftType.Shift3);

        var providedShifts = command.ShiftMenus.Select(sm => sm.ShiftType).ToHashSet();
        return requiredShifts.SetEquals(providedShifts);
    }

    private bool IsValidDayOfMonth(CreateMonthlyMenuCommand command, ShiftMenuDto shiftMenu)
    {
        var daysInMonth = DateTime.DaysInMonth(command.Year, command.Month);
        return shiftMenu.DayOfMonth.Year == command.Year
            && shiftMenu.DayOfMonth.Month == command.Month
            && shiftMenu.DayOfMonth.Day <= daysInMonth;
    }
}

public class CreateMonthlyMenuCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateMonthlyMenuCommand, int>
{
    public async Task<int> Handle(
        CreateMonthlyMenuCommand request,
        CancellationToken cancellationToken
    )
    {
        var location = await context
            .Locations.Include(l => l.Supplier)
            .FindOrNotFoundExceptionAsync(request.LocationId, cancellationToken);

        if (location.Supplier is null)
        {
            throw new ValidationException("Location doesn't have any Supplier");
        }

        var monthlyMenu = new MonthlyMenu
        {
            Year = request.Year,
            Month = request.Month,
            Location = location,
            LocationId = location.Id,
            Supplier = location.Supplier,
            SupplierId = location.Supplier.Id,
            IsPublished = false
        };

        var dailyMenus = new Dictionary<DateOnly, DailyMenu>();
        var existingDishes = await context
            .Dishes.Where(d =>
                d.LocationId == request.LocationId && d.SupplierId == location.SupplierId
            )
            .ToListAsync(cancellationToken);
        var newDishes = new Dictionary<string, Dish>();

        foreach (var shiftMenuDto in request.ShiftMenus)
        {
            var date = shiftMenuDto.DayOfMonth;

            if (!dailyMenus.TryGetValue(date, out var dailyMenu))
            {
                dailyMenu = new DailyMenu
                {
                    Date = date,
                    MonthlyMenu = monthlyMenu,
                    MonthlyMenuId = monthlyMenu.Id
                };
                dailyMenus[date] = dailyMenu;
                monthlyMenu.DailyMenus.Add(dailyMenu);
            }

            var shiftMenu = new ShiftMenu { Shift = shiftMenuDto.ShiftType, DailyMenu = dailyMenu };

            // Main dishes
            shiftMenu.Dishes.AddRange(GetOrCreateDishes(shiftMenuDto.MainDishes, DishType.Main));
            // Soups
            shiftMenu.Dishes.AddRange(GetOrCreateDishes(shiftMenuDto.Soups, DishType.Soup));
            // Vegetarians
            shiftMenu.Dishes.AddRange(
                GetOrCreateDishes(shiftMenuDto.VegetarianCombos, DishType.Vegetarian)
            );
            // Desserts
            shiftMenu.Dishes.AddRange(GetOrCreateDishes(shiftMenuDto.SideDishes, DishType.Dessert));

            dailyMenu.ShiftMenus.Add(shiftMenu);
        }

        context.MonthlyMenus.Add(monthlyMenu);
        await context.SaveChangesAsync(cancellationToken);

        return monthlyMenu.Id;

        IEnumerable<Dish> GetOrCreateDishes(IEnumerable<string> dishNames, DishType dishType)
        {
            var dishes = new List<Dish>();

            foreach (var dishName in dishNames)
            {
                var normalizedDishName = dishName.ToLower();
                var dish = existingDishes.FirstOrDefault(x =>
                    x.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase)
                    && x.Type == dishType
                );

                if (dish == null)
                {
                    if (!newDishes.TryGetValue(normalizedDishName, out dish))
                    {
                        dish = new Dish
                        {
                            Name = dishName,
                            Type = dishType,
                            Location = location,
                            Supplier = location.Supplier!
                        };
                        newDishes[normalizedDishName] = dish;
                    }
                    context.Dishes.Add(dish);
                }

                dishes.Add(dish);
            }

            return dishes;
        }
    }
}
