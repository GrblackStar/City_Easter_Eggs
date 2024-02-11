#region Using

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
        private readonly PointsService _service;

        public PointsController(ILogger<PointsController> logger, PointsService service)
        {
            _logger = logger;
            _service = service;
        }

        public IEnumerable<PointOfInterest> GetPoints()
        {
            return _service.GetPoints();
        }
    }
}