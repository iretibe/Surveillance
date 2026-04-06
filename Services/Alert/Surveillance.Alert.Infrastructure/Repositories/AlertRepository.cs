using Microsoft.EntityFrameworkCore;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.Alert.Infrastructure.Data;

namespace Surveillance.Alert.Infrastructure.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AlertDbContext _context;

        public AlertRepository(AlertDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Domain.Entities.Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
        }

        public async Task DeleteAsync(Guid id)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == id);
            if (alert != null)
                _context.Alerts.Remove(alert);
        }

        public async Task<IEnumerable<Domain.Entities.Alert>> GetAllAsync()
        {
            return await _context.Alerts.ToListAsync();
        }

        public async Task<Domain.Entities.Alert?> GetByIdAsync(Guid id)
        {
            return await _context.Alerts.FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task UpdateAsync(Domain.Entities.Alert alert)
        {
            _context.Alerts.Update(alert);
            return Task.CompletedTask;
        }
    }
}