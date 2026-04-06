using MediatR;
using Microsoft.AspNetCore.Mvc;
using Surveillance.Identity.Application.Commands;

namespace Surveillance.Identity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserCommand command)
            => Ok(await _mediator.Send(command));
    }
}
