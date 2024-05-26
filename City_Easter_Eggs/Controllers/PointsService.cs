#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;
using static City_Easter_Eggs.Controllers.PointsController;

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

        public async Task<IEnumerable<PointOfInterestFrontend>> GetPoints()
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);

            var points = _context.POIs.Include(x => x.Creator);
            return points.Select(p => new PointOfInterestFrontend(p, currentUser));
        }

        public async Task<PointDetailsDto> GetPointDetailsAsync(string markerId)
        {
            //var user = _httpContextAccessor.HttpContext?.User;
            //var currentUser = await _userService.GetUserFromPrincipal(user);

            //var point = await _context.POIs
            //    .Include(p => p.LikedPoints)
            //    .Include(p => p.FavoritedPoints)
            //    .FirstOrDefaultAsync(p => p.PointId == markerId);

            //if (point == null) return null;

            //var isLiked = point.LikedPoints.Any(lp => lp.User.UserId == currentUser.UserId);
            //var isFavorite = point.FavoritedPoints.Any(fp => fp.User.UserId == currentUser.UserId);

            //return new PointDetailsDto
            //{
            //    Name = point.Name,
            //    Description = point.Description,
            //    Likes = point.Likes,
            //    IsLiked = isLiked,
            //    IsFavorite = isFavorite
            //};
            return null;
        }

        public async Task<IEnumerable<string>> GetLikedByUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);
            if (currentUser == null) return null;

            return currentUser.LikedPoints.Select(x => x.PointId);
        }

        public async Task<PointOfInterestFrontend> LikePoint(string markerId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            PointOfInterest? point = _context.POIs.FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return null;

            var like = new LikedPoints
            {
                User = currentUser,
                Point = point
            };

            point.Likes++;
            point.LikedPoints.Add(like);
            currentUser.LikedPoints.Add(like);
            point.Creator.LikesObtained++;

            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
        }

        public async Task<PointOfInterestFrontend> FavoriteAddPoint(string markerId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            PointOfInterest? point = _context.POIs.FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return null;

            var like = new FavouritePoints
            {
                User = currentUser,
                Point = point
            };

            point.FavoritedPoints.Add(like);
            currentUser.FavoritedPoints.Add(like);

            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
        }

        public async Task<PointOfInterestFrontend> UnLikePoint(string markerId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            PointOfInterest? point = _context.POIs.FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return null;

            LikedPoints? like = _context.LikedPoints.FirstOrDefault(l => l.User == currentUser && l.Point == point);

            if (like == null) return null;

            point.Likes--;
            point.LikedPoints.Remove(like);
            currentUser.LikedPoints.Remove(like);
            point.Creator.LikesObtained--;

            _context.LikedPoints.Remove(like);

            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
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

    public class PointDetailsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
        public bool IsFavorite { get; set; }
    }
}