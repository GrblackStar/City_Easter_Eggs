#region Using

using City_Easter_Eggs.Models;
using City_Easter_Eggs.Pages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;

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

        public Task<IEnumerable<PointOfInterestFrontend>> GetPoints()
        {
            return _service.GetPoints();
        }

        public async Task<IActionResult> GetPointDetails(UpdatePointInputModel marker)
        {
            var pointDetails = await _service.GetPointDetailsAsync(marker.MarkerId);

            if (pointDetails == null)
            {
                return NotFound();
            }

            return Ok(pointDetails);
        }

        [HttpPost]
        public async Task<IActionResult> LikePoint(UpdatePointInputModel marker)
        {
            PointOfInterestFrontend pointLiked = await _service.LikePoint(marker.MarkerId);
            return Ok(pointLiked);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoint(CreatePointInputModel input)
        {
            await _service.CreatePointAsync(input.Name, input.Description, input.UserLocationLongitude, input.UserLocationLatitude);

            //return RedirectToPage("/Index");
            return Ok();
        }

        public class PointOfInterestFrontend
        {
            public PointOfInterest Point { get; set; }
            public bool LikedByCurrentUser { get; set; }

            public PointOfInterestFrontend(PointOfInterest point, User? loggedInUser)
            {
                Point = point;
                LikedByCurrentUser = loggedInUser != null && loggedInUser.LikedPoints.Any(x => x.PointId == point.PointId);
            }
        }

        public class CreatePointInputModel
        {
            [Required(ErrorMessage = "Name is required")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
            public string Name { get; set; }

            [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
            public string? Description { get; set; }

            public double UserLocationLongitude { get; set; }
            public double UserLocationLatitude { get; set; }
        }

        public class UpdatePointInputModel
        {
            public string MarkerId { get; set; }
        }
    }
}