#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;

#endregion

namespace City_Easter_Eggs.Controllers
{
    public class PointsService
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        private UserService _userService;

        public PointsService(ILogger<PointsService> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public IEnumerable<PointOfInterest> GetPoints()
        {
            return _context.POIs;
        }

        public async Task<int> LikePoint(string markerId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);

            var point = _context.POIs.FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return 0;

            var like = new LikedPoints
            {
                UserId = currentUser.UserId,
                PointId = point.PointId
            };

            point.LikedPoints.Add(like);
            point.Likes++;

            currentUser.LikedPoints.Add(like);
            point.Creator.LikesObtained++;

            await _context.SaveChangesAsync();
            return point.Likes;
        }

        public async Task CreatePointAsync(string name, string description, double longitude, double latitude)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);

            if (description == null) description = "No Description";
            
            _context.POIs.Add(new PointOfInterest
            {
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                Creator = currentUser
            });

            await _context.SaveChangesAsync();
        }
    }
}