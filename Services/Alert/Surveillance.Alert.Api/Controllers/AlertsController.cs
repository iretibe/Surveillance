using MediatR;
using Microsoft.AspNetCore.Mvc;
using Surveillance.Alert.Application.Commands;

namespace Surveillance.Alert.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlert(CreateAlertCommand command)
            => Ok(await _mediator.Send(command));
    }
}
