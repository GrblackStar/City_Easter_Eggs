#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace City_Easter_Eggs.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PointsController : ControllerBase
    {
        private readonly ILogger<PointsController> _logger;
        private readonly ApplicationDbContext _context;

        public PointsController(ILogger<PointsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IEnumerable<PointOfInterest> GetPoints()
        {
            return _context.POIs;
        }
    }
}