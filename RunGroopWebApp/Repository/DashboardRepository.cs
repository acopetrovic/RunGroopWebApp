using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        //prva verzija
        //public async Task<List<Club>> GetAllUserClubs()
        //{
        //    var curUser = _httpContextAccessor.HttpContext?.User.GetuserId();
        //    var userClubs = _context.Clubs.Where(r => r.AppUser.Id == curUser.ToString());
        //    return userClubs.ToList();
        //}
        //druga verzija unapredjenja
        public async Task<List<Club>> GetAllUserClubs()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetuserId();
            return await _context.Clubs
                                 .Where(r => r.AppUser.Id == curUser)
                                 .ToListAsync();
        }

        public async Task<List<Race>> GeyAllUserRaces()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetuserId();
            var userRaces = _context.Races.Where(r => r.AppUser.Id == curUser.ToString());
            return userRaces.ToList();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetByIdNoTracking(string id)
        {
            return await _context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
