using CarpooliDotTN.Models;

namespace CarpooliDotTN.DAL.IServices
{
    public interface ICarpoolService
    {
        Task<IQueryable<Carpool>> GetQueryableCarpoolsAsync();
        Task<IQueryable<Carpool>> GetQueryableCarpoolsWithDemandsAsync();
        Task<Carpool> GetCarpoolByIdAsync(Guid id);
        Task<IEnumerable<string>> GetDistinctCitiesAsync();
        Task AddCarpoolAsync(Carpool carpool);
        Task UpdateCarpoolAsync(Carpool carpool);
        Task DeleteCarpoolAsync(Guid id);
        bool CarpoolExists(Guid id);
    }


}
