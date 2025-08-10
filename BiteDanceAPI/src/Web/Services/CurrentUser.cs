using System.Security.Claims;
using BiteDanceAPI.Application.Common.Interfaces;

namespace BiteDanceAPI.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? Id =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Upn); // Note: email claim depends on entra id config
    public string? Name => httpContextAccessor.HttpContext?.User?.Identity?.Name;
}
