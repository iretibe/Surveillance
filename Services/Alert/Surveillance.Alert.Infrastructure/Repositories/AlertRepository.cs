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
            var alert = await _context.Alerts.SingleOrDefaultAsync(alert => alert.Id == id);

            _context.Alerts.Remove(alert!);
        }

        public Task<IEnumerable<Domain.Entities.Alert>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Alert?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Domain.Entities.Alert alert)
        {
            throw new NotImplementedException();
        }
    }
}
