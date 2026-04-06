using System.Diagnostics;
using MediatR;
using Surveillance.Alert.Application.Commands;
using Surveillance.Alert.Domain.Events;
using Surveillance.Alert.Domain.Saga;
using Surveillance.Alert.Infrastructure.Data;
using Surveillance.EventBus.Events;
using Surveillance.EventBus.Events.Notifications;
using Surveillance.Saga;

namespace Surveillance.Alert.Application.Sagas
{
    public class AlertSaga : ISaga
    {
        private readonly IEventBus _bus;
        private readonly AlertDbContext _db;
        private readonly IMediator _mediator;
        private static readonly ActivitySource ActivitySource = new("Saga");

        public AlertSaga(IEventBus bus, AlertDbContext db, IMediator mediator)
        {
            _bus = bus;
            _db = db;
            _mediator = mediator;
        }

        public async Task HandleAsync(object @event, CancellationToken ct = default)
        {
            using var activity = ActivitySource.StartActivity("Saga.Handle");
            activity?.SetTag("event.type", @event.GetType().Name);

            switch (@event)
            {
                case AlertCreatedEvent e:
                    activity?.SetTag("alert.id", e.Id.ToString());
                    await StartSaga(e, ct);
                    break;

                case NotificationFailedEvent e:
                    await Compensate(e, ct);
                    break;
            }
        }

        private async Task StartSaga(AlertCreatedEvent e, CancellationToken ct)
        {
            var saga = new SagaStateEntity
            {
                Id = e.Id,
                CurrentStep = "Notification",
                IsCompleted = false,
                IsFailed = false
            };

            _db.SagaStates.Add(saga);
            await _db.SaveChangesAsync(ct);

            await _bus.PublishAsync(new SendNotificationCommand(e.Id, e.Message));
        }

        private async Task Compensate(NotificationFailedEvent e, CancellationToken ct)
        {
            var saga = await _db.SagaStates.FindAsync(new object[] { e.AlertId }, ct);
            if (saga != null)
            {
                saga.IsFailed = true;
                saga.CurrentStep = "Compensated";
            }

            await _mediator.Send(new DeleteAlertCommand(e.AlertId), ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
