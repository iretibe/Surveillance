using MediatR;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.SharedKernel;

namespace Surveillance.Alert.Application.Commands.Handlers
{
    public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand>
    {
        private readonly IAlertRepository _repository;
        private readonly IUnitOfWork _uow;

        public DeleteAlertCommandHandler(IAlertRepository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.Id);

            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
