using BiteDanceAPI.Domain.Constants;

namespace BiteDanceAPI.Application.System.Queries;

public record GetConfigsQuery : IRequest<ConfigDto>;

public class GetConfigsQueryHandler : IRequestHandler<GetConfigsQuery, ConfigDto>
{
    public Task<ConfigDto> Handle(GetConfigsQuery request, CancellationToken cancellationToken)
    {
        var config = new ConfigDto
        {
            AllowedCities = LocationConst.CityAllowList.ToList(),
            AllowedCountries = LocationConst.CountryAllowList.ToList()
        };

        return Task.FromResult(config);
    }
}

public class ConfigDto
{
    public List<string> AllowedCities { get; set; } = [];
    public List<string> AllowedCountries { get; set; } = [];
}
