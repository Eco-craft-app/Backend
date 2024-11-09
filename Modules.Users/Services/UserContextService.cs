using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Services;
internal interface IUserContextService
{
    public string? GetUserId();
}
internal class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public string? GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId;
    }
}
