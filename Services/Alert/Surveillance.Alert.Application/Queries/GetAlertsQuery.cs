using MediatR;
using Surveillance.Alert.Domain.Dtos;

namespace Surveillance.Alert.Application.Queries
{
    public record GetAlertsQuery() : IRequest<List<AlertDto>>;
}
