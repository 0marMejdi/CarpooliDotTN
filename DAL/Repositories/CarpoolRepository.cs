using CarpooliDotTN.DAL.IRepositories;
using CarpooliDotTN.Models;
using Microsoft.EntityFrameworkCore;

namespace CarpooliDotTN.DAL.Repositories
{
    public class CarpoolRepository : ICarpoolRepository
    {
        private readonly CarpooliDbContext _context;

        public CarpoolRepository(CarpooliDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<Carpool>> GetQueryableCarpoolsAsync()
        {
            return _context.carpools.Include(c => c.Demands).Include(c => c.Owner).AsQueryable();
        }

        public async Task<IQueryable<Carpool>> GetQueryableCarpoolsWithDemandsAsync()
        {
            return _context.carpools.Include(c => c.Demands).AsQueryable();
        }

        public async Task<Carpool> GetCarpoolByIdAsync(Guid id)
        {
            return await _context.carpools.Include(c => c.Demands).Include(c => c.Owner).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<string>> GetDistinctCitiesAsync()
        {
            return await _context.carpools.Include(c => c.Demands).Include(c => c.Owner).Select(c => c.DepartureCity).Distinct().ToListAsync();
        }

        public async Task AddCarpoolAsync(Carpool carpool)
        {
            _context.carpools.Add(carpool);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarpoolAsync(Carpool updatedCarpool)
        {
            // Retrieve the existing carpool from the database
            var existingCarpool = await _context.carpools.Include(c => c.Demands).Include(c => c.Owner).Where(c=> c.Id == updatedCarpool.Id).FirstOrDefaultAsync();

            if (existingCarpool == null)
            {
                // Handle the case where the carpool is not found
                throw new Exception("Carpool not found");
            }

            // Update the properties of the existing carpool with values from updatedCarpool
            existingCarpool.DepartureTime = updatedCarpool.DepartureTime;
            existingCarpool.DepartureCity = updatedCarpool.DepartureCity;
            existingCarpool.Price = updatedCarpool.Price;
            existingCarpool.NumberOfPlaces = updatedCarpool.NumberOfPlaces;
            existingCarpool.IsOpen = updatedCarpool.IsOpen;
            existingCarpool.Description = updatedCarpool.Description;

            // Update other properties as needed

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarpoolAsync(Guid id)
        {
            var carpool =  _context.carpools.Include(c => c.Demands).Include(c => c.Owner).Where(c => c.Id == id).FirstOrDefault();
            if (carpool != null)
            {
                _context.carpools.Remove(carpool);
                await _context.SaveChangesAsync();
            }
        }

        public bool CarpoolExists(Guid id)
        {
            return _context.carpools.Include(c => c.Demands).Any(e => e.Id == id);
        }
    }

}
