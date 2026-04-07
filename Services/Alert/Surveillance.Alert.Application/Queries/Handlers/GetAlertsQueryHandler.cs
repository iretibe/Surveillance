using MediatR;
using Microsoft.EntityFrameworkCore;
using Surveillance.Alert.Domain.Dtos;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.Alert.Infrastructure.Data;
using Surveillance.EventBus.Caching;

namespace Surveillance.Alert.Application.Queries.Handlers
{
    public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, List<AlertDto>>
    {
        private readonly IAlertRepository _repository;
        private readonly CacheService _cache;
        private readonly AlertDbContext _context;

        public GetAlertsQueryHandler(
            IAlertRepository repository,
            CacheService cache,
            AlertDbContext context)
        {
            _repository = repository;
            _cache = cache;
            _context = context;
        }

        public async Task<List<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"alerts_user_{request.UserId}";

            // Try to get from cache
            var cached = await _cache.GetAsync<List<AlertDto>>(cacheKey);
            if (cached != null)
                return cached;

            // Get alerts for specific user
            var alerts = await _context.Alerts
                .Where(a => a.UserId == request.UserId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    Message = a.Message,
                    CreatedAt = a.CreatedAt,
                    UserId = a.UserId
                })
                .ToListAsync(cancellationToken);

            // Cache for 5 minutes
            await _cache.SetAsync(cacheKey, alerts);

            return alerts;
        }
    }
}
