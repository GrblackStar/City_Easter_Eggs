#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;
using City_Easter_Eggs.QuadTree;
using Microsoft.AspNetCore.Rewrite;
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

        private static QuadTree<PointOfInterest> _quadTree;

        public PointsService(ILogger<PointsService> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            
            // otherwise it will be created and every time for each user
            if(_quadTree == null)
            {
                _quadTree = new QuadTree<PointOfInterest>(new Rectangle(-85, -180, 85 * 2, 180 * 2));

                var points = _context.POIs.Include(x => x.Creator);
                foreach (var point in points)
                {
                    _quadTree.AddObject(point);
                }
            }
        }

        public async Task<IEnumerable<PointOfInterestFrontend>> GetPoints(float Longitude, float Latitude, bool ShowInRadius)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            var points = new List<PointOfInterest>();
            if (ShowInRadius)
            {
                float radLat = 0.04496608029593653f;
                float radLng = radLat / MathF.Cos((float)Latitude * MathF.PI / 180);

                var ellipse = new Ellipse(Latitude, Longitude, radLat, radLng);
                _quadTree.GetObjectsIntersectingShape(points, ellipse);
            }
            else
            {
                _quadTree.GetAllObjects(points);
            }
            return points.Select(p => new PointOfInterestFrontend(p, currentUser));
        }

        public async Task<IEnumerable<string>> GetLikedByUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);
            if (currentUser == null) return null;

            return currentUser.LikedPoints.Select(x => x.PointId);
        }

        public async Task<PointOfInterestFrontend?> LikePoint(string markerId)
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

        public async Task<PointOfInterestFrontend?> FavoriteAddPoint(string markerId)
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
            currentUser?.FavoritedPoints.Add(like);

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

        public async Task<PointOfInterestFrontend> FavoriteRemovePoint(string markerId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            PointOfInterest? point = _context.POIs.FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return null;

            FavouritePoints? like = _context.FavouritePoints.FirstOrDefault(l => l.User == currentUser && l.Point == point);

            if (like == null) return null;

            point.FavoritedPoints.Remove(like);
            currentUser.FavoritedPoints.Remove(like);

            _context.FavouritePoints.Remove(like);

            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
        }


        public async Task CreatePointAsync(string name, string description, double longitude, double latitude)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);

            if (description == null) description = "No Description";

            var point = (new PointOfInterest
			{
				Name = name,
				Description = description,
				Latitude = latitude,
				Longitude = longitude,
				Creator = currentUser
			});

            _context.POIs.Add(point);
            await _context.SaveChangesAsync();

            _quadTree.AddObject(point);
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