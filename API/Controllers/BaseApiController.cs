using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IMediator Mediator
        {
            get
            {
                var mediatorValue = HttpContext.RequestServices?.GetService<IMediator>();
                return mediatorValue ?? throw new InvalidOperationException("The IMediator service is not available.");
            }
        }
    }
}
