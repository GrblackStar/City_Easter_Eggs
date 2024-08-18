#region Using

using City_Easter_Eggs.Data;
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
        private IWebHostEnvironment _hostingEnvironment;

        public PointsController(ILogger<PointsController> logger, PointsService service, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public Task<IEnumerable<PointOfInterestFrontend>> GetPoints([FromQuery] UserRadiusInputModel userRadius)
        {
            return _service.GetPoints(userRadius.Longitude, userRadius.Latitude, userRadius.ShowInRadius);
        }

        [HttpPost]
        public async Task<IActionResult> LikePoint(UpdatePointInputModel marker)
        {
            PointOfInterestFrontend pointLiked = await _service.LikePoint(marker.MarkerId);
            return Ok(pointLiked);
        }

        [HttpPost]
        public async Task<IActionResult> FavoriteAddPoint(UpdatePointInputModel marker)
        {
            PointOfInterestFrontend pointLiked = await _service.FavoriteAddPoint(marker.MarkerId);
            return Ok(pointLiked);
        }

        [HttpPost]
        public async Task<IActionResult> UnLikePoint(UpdatePointInputModel marker)
        {
            PointOfInterestFrontend pointLiked = await _service.UnLikePoint(marker.MarkerId);
            return Ok(pointLiked);
        }

        [HttpPost]
        public async Task<IActionResult> FavoriteRemovePoint(UpdatePointInputModel marker)
        {
            PointOfInterestFrontend pointLiked = await _service.FavoriteRemovePoint(marker.MarkerId);
            return Ok(pointLiked);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoint(CreatePointInputModel input)
        {
            string imageId = null;
            if (!string.IsNullOrEmpty(input.Image))
            {
                const string imageFormatHeader = "data:image/";
                const string base64Header = "base64,";

                var base64Data = input.Image;
                var imageFormatIdx = base64Data.IndexOf(imageFormatHeader);
                var imageFormatEnd = base64Data.IndexOf(";");
                var base64HeaderEnd = base64Data.IndexOf(base64Header);
                string imageFormat = null;
                if (imageFormatIdx != -1 && imageFormatEnd != -1 && base64HeaderEnd != -1)
                {
                    imageFormat = base64Data.Substring(imageFormatIdx + imageFormatHeader.Length, imageFormatEnd - imageFormatIdx - imageFormatHeader.Length);
                    base64HeaderEnd += base64Header.Length;
                }

                if (imageFormat != null)
                {
                    imageId = Guid.NewGuid().ToString() + $".{imageFormat}";
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(filePath);
                    byte[] bytes = Convert.FromBase64String(input.Image.Substring(base64HeaderEnd));
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(filePath, imageId), bytes);
                }
            }

            await _service.CreatePointAsync(input.Name, input.Description, input.UserLocationLongitude, input.UserLocationLatitude, imageId);

            //return RedirectToPage("/Index");
            return Ok();
        }

        public class PointOfInterestFrontend
        {
            public PointOfInterest Point { get; set; }
            public bool LikedByCurrentUser { get; set; }
            public bool FavoriteByCurrentUser { get; set; }

            public bool IsTopPercentile { get; set; }

            public PointOfInterestFrontend(PointOfInterest point, User? loggedInUser)
            {
                Point = point;
                LikedByCurrentUser = loggedInUser != null && loggedInUser.LikedPoints.Any(x => x.PointId == point.PointId);
                FavoriteByCurrentUser = loggedInUser != null && loggedInUser.FavoritedPoints.Any(x => x.PointId == point.PointId);
                IsTopPercentile = point.Likes >= PointsService.TopPercentileLikes;
            }
        }

        public class CreatePointInputModel
        {
            [Required(ErrorMessage = "Името е задължително")]
            [StringLength(50, ErrorMessage = "Името трябва да е между {2} и {1} символа.", MinimumLength = 3)]
            public string Name { get; set; }

            [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
            public string? Description { get; set; }

            public string? Image { get; set; }

            public double UserLocationLongitude { get; set; }
            public double UserLocationLatitude { get; set; }
        }

        public class UpdatePointInputModel
        {
            public string MarkerId { get; set; }
        }

        public class UserRadiusInputModel
        {
            public float Longitude { get; set; }
            public float Latitude { get; set; }
            public bool ShowInRadius { get; set; }
        }
    }
}