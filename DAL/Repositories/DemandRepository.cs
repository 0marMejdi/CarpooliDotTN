using CarpooliDotTN.DAL.IRepositories;
using CarpooliDotTN.Models;
using Microsoft.EntityFrameworkCore;

namespace CarpooliDotTN.DAL.Repositories
{
    public class DemandRepository : IDemandRepository
    {
        private readonly CarpooliDbContext _context;

        public DemandRepository(CarpooliDbContext context)
        {
            _context = context;
        }

        public Task<Demand> GetByIdAsync(Guid id)
        {
            return _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner).Where(c=> c.Id==id).FirstOrDefaultAsync();
        }

        public async Task<List<Demand>> GetAllAsync()
        {
            return await _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner).ToListAsync();
        }

        public async Task<List<Demand>> GetDemandsByCarpoolAsync(Guid carpoolId)
        {
            return await _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner)
                .Where(d => d.CarpoolId == carpoolId)
                .ToListAsync();
        }

        public async Task<List<Demand>> GetDemandsBySenderAsync(string senderId)
        {
            return await _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner)
                .Where(d => d.PassengerId == senderId)
                .ToListAsync();
        }

        public async Task AddAsync(Demand demand)
        {
            _context.demands.Add(demand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Demand demand)
        {
            _context.demands.Update(demand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            Demand demand = _context.demands.Find(id);
            if (demand != null)
            {
                _context.demands.Remove(demand);
                 _context.SaveChangesAsync();
            }
        }
        public async Task<Demand> GetDemandByPassengerAndCarpoolAsync(string passengerId, Guid carpoolId)
        {
            return await _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner)
                .FirstOrDefaultAsync(d => d.PassengerId == passengerId && d.CarpoolId == carpoolId);
        }
        public bool IsApplied(string userId, Guid carpoolId)
        {
            return _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner).Any(d => d.PassengerId == userId && d.CarpoolId == carpoolId);
        }
        public Task<List<Demand>> GetDemandsByOwnerIdAsync(string userId) { 
            return _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Include(d => d.Carpool.Owner)
                .Where(d => d.Carpool.OwnerId == userId)
                .ToListAsync();
        }
    }
}
