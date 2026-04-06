using Surveillance.Alert.Application.Commands;
using Surveillance.Alert.Domain.Events;
using Surveillance.Alert.Domain.Saga;
using Surveillance.Alert.Infrastructure.Data;
using Surveillance.EventBus.Events;
using Surveillance.EventBus.Events.Notifications;
using Surveillance.Saga;
using System.Diagnostics;

namespace Surveillance.Alert.Application.Sagas
{
    public class AlertSaga : ISaga
    {
        private readonly IEventBus _bus;
        private readonly AlertDbContext _db;

        private static readonly ActivitySource ActivitySource = new("Saga");

        public AlertSaga(IEventBus bus, AlertDbContext db)
        {
            _bus = bus;
            _db = db;
        }

        public async Task HandleAsync(object @event, CancellationToken ct)
        {
            using var activity = ActivitySource.StartActivity("Saga.Handle");
            activity?.SetTag("event.type", @event.GetType().Name);

            switch (@event)
            {
                //case AlertCreatedEvent e:
                //    await StartSaga(e, ct);
                //    break;

                case AlertCreatedEvent e:
                    activity?.SetTag("alert.id", e.Id);
                    await HandleAlertCreated(e);
                    break;

                case NotificationFailedEvent e:
                    await Compensate(e, ct);
                    break;
            }
        }

        private async Task HandleAlertCreated(AlertCreatedEvent e)
        {
            throw new NotImplementedException();
        }

        private async Task StartSaga(AlertCreatedEvent e, CancellationToken ct)
        {
            var saga = new SagaStateEntity
            {
                Id = e.Id,
                CurrentStep = "Notification",
            };

            _db.SagaStates.Add(saga);
            await _db.SaveChangesAsync(ct);

            await _bus.PublishAsync(new SendNotificationCommand(e.Id, e.Message));
        }

        private async Task Compensate(NotificationFailedEvent e, CancellationToken ct)
        {
            var saga = await _db.SagaStates.FindAsync(e.AlertId);

            saga!.IsFailed = true;

            await _bus.PublishAsync(new DeleteAlertCommand(e.AlertId));

            await _db.SaveChangesAsync(ct);
        }
    }
}
