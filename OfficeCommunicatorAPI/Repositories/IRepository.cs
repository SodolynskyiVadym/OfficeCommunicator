using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.Repositories
{
    public interface IRepository<T, in TDto, in TUpdateDto>
    {
        Task<T?> GetByIdWithIncludeAsync(int id);
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(TDto entity);
        Task<bool> UpdateAsync(TUpdateDto entity);
        Task<bool> DeleteAsync(int id);
    }
}
