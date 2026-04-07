using MediatR;
using Surveillance.Alert.Domain.Dtos;

namespace Surveillance.Alert.Application.Queries
{
    public record GetAlertsQuery(Guid UserId) : IRequest<List<AlertDto>>;
}
