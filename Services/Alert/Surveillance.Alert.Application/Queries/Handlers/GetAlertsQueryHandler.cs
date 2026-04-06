using MediatR;
using Surveillance.Alert.Domain.Dtos;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.EventBus.Caching;

namespace Surveillance.Alert.Application.Queries.Handlers
{
    public class GetAlertsQueryHandler
        : IRequestHandler<GetAlertsQuery, List<AlertDto>>
    {
        private readonly IAlertRepository _repository;
        private readonly CacheService _cache;

        public GetAlertsQueryHandler(IAlertRepository repository, 
            CacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellation)
        {
            var cached = await _cache.GetAsync<List<AlertDto>>("alerts");

            if (cached != null)
                return cached;

            var alerts = (await _repository.GetAllAsync())
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    Message = a.Message,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            await _cache.SetAsync("alerts", alerts);

            return alerts;
        }
    }
}
