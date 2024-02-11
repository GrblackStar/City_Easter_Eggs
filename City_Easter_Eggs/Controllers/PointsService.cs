#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;

#endregion

namespace City_Easter_Eggs.Controllers
{
    public class PointsService
    {
        private readonly ApplicationDbContext _context;

        public PointsService(ILogger<PointsController> logger, ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PointOfInterest> GetPoints()
        {
            return _context.POIs;
        }

        public async Task CreatePointAsync(string name, string description, double longitude, double latitude)
        {
            Console.WriteLine(longitude);
            Console.WriteLine(latitude);
            Console.WriteLine();
            // Add logic to save the point to the database using _context
            _context.POIs.Add(new PointOfInterest { Name = name, Description = description, Latitude = latitude, Longitude = longitude });
            await _context.SaveChangesAsync();
        }
    }
}