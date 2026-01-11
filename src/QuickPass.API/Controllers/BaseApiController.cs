using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QuickPass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid AccountId
        {
            get
            {
                var idClaim = User.FindFirst("accountId")?.Value;

                if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var guid))
                {
                    return Guid.Empty;
                }
                return guid;
            }
        }

        protected bool IsAdmin => User.IsInRole("Administrador");

        protected bool IsTech => User.IsInRole("Tecnico");
    }
}
