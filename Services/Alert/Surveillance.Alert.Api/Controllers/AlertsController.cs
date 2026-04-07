using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Surveillance.Alert.Application.Commands;
using Surveillance.Alert.Application.Queries;

namespace Surveillance.Alert.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlerts()
        {
            var userId = User.FindFirst("userId")?.Value;
            var alerts = await _mediator.Send(new GetAlertsQuery(Guid.Parse(userId!)));
            return Ok(alerts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlert(CreateAlertCommand command)
        {
            var userId = User.FindFirst("userId")?.Value;
            var result = await _mediator.Send(command with { UserId = Guid.Parse(userId!) });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(Guid id)
        {
            await _mediator.Send(new DeleteAlertCommand(id));
            return NoContent();
        }
    }
}
