namespace Surveillance.Alert.Domain.Repositories
{
    public interface IAlertRepository
    {
        Task<IEnumerable<Entities.Alert>> GetAllAsync();
        Task<Entities.Alert?> GetByIdAsync(Guid id);
        Task AddAsync(Entities.Alert alert);
        Task UpdateAsync(Entities.Alert alert);
        Task DeleteAsync(Guid id);
    }
}
