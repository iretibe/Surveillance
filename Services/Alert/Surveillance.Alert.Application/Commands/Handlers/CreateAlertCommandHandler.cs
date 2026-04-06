using MediatR;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.EventBus.Events;
using Surveillance.SharedKernel;

namespace Surveillance.Alert.Application.Commands.Handlers
{
    public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, Guid>
    {
        private readonly IAlertRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IEventBus _bus;

        public CreateAlertCommandHandler(IAlertRepository repo, 
            IUnitOfWork uow, IEventBus bus)
        {
            _repo = repo;
            _uow = uow;
            _bus = bus;
        }

        public async Task<Guid> Handle(CreateAlertCommand request, CancellationToken ct)
        {
            var alert = Domain.Entities.Alert.Create(request.Message);

            await _repo.AddAsync(alert);
            await _uow.SaveChangesAsync(ct);

            foreach (var e in alert.Events)
                await _bus.PublishAsync(e);

            alert.ClearEvents();

            return alert.Id;
        }
    }
}
