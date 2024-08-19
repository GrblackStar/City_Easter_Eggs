#region Using

using City_Easter_Eggs.Data;
using City_Easter_Eggs.Models;
using City_Easter_Eggs.QuadTree;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Globalization;
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

        public static int TopPercentileLikes;
        private static List<PointOfInterest> _reusableList = new List<PointOfInterest>();

        public PointsService(ILogger<PointsService> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;

            // otherwise it will be created and every time for each user
            if (_quadTree == null)
            {
                CreateSystemPoints();

                _quadTree = new QuadTree<PointOfInterest>(new Rectangle(-85, -180, 85 * 2, 180 * 2));

                var points = _context.POIs.Include(x => x.Creator);
                foreach (var point in points)
                {
                    _quadTree.AddObject(point);
                }
                UpdateTopPercentileLikes();
            }
        }

        private void UpdateTopPercentileLikes()
        {
            _reusableList.Clear();
            _quadTree.GetAllObjects(_reusableList);
            if (_reusableList.Count == 0) return;

            _reusableList.Sort((a, b) => MathF.Sign(a.Likes - b.Likes));
            int top = (int)MathF.Floor(_reusableList.Count * 0.9f);
            TopPercentileLikes = _reusableList[top].Likes;
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

            PointOfInterest? point = _context.POIs.Include(x => x.Creator).FirstOrDefault(p => p.PointId == markerId);

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

            _quadTree.UpdateObject(point);
            UpdateTopPercentileLikes();

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

            _quadTree.UpdateObject(point);
            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
        }

        public async Task<PointOfInterestFrontend> UnLikePoint(string markerId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            User? currentUser = await _userService.GetUserFromPrincipal(user);
            currentUser = _context.Users.Where(x => x == currentUser).Include(x => x.FavoritedPoints).Include(x => x.LikedPoints).FirstOrDefault();

            PointOfInterest? point = _context.POIs.Include(x => x.Creator).FirstOrDefault(p => p.PointId == markerId);

            if (point == null) return null;

            LikedPoints? like = _context.LikedPoints.FirstOrDefault(l => l.User == currentUser && l.Point == point);

            if (like == null) return null;

            point.Likes--;
            point.LikedPoints.Remove(like);
            currentUser.LikedPoints.Remove(like);
            point.Creator.LikesObtained--;

            _context.LikedPoints.Remove(like);

            _quadTree.UpdateObject(point);
            UpdateTopPercentileLikes();

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

            _quadTree.UpdateObject(point);
            await _context.SaveChangesAsync();
            return new PointOfInterestFrontend(point, currentUser);
        }

        public async Task CreatePointAsync(string name, string description, double longitude, double latitude, string? imageId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var currentUser = await _userService.GetUserFromPrincipal(user);

            if (description == null) description = "No Description";

            var point = new PointOfInterest
            {
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                Creator = currentUser,
                ImageId = imageId ?? string.Empty
            };

            _context.POIs.Add(point);
            await _context.SaveChangesAsync();

            _quadTree.AddObject(point);
        }


        private static List<PointOfInterest> _systemPoints = new List<PointOfInterest>()
        {
            new PointOfInterest()
            {
                Name = "Metrostation Serdika",
                Latitude = 42.6978605,
                Longitude = 23.3211527,
            },
            new PointOfInterest()
            {
                Name = "Metrostation NDK",
                Latitude = 42.6887654,
                Longitude = 23.3195801,
            },

            // Line M1
            new PointOfInterest()
            {
                Name = "Metrostation Slivnitsa",
                Latitude = 42.72642,
                Longitude = 23.2618503,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Lyulin",
                Latitude = 42.718397,
                Longitude = 23.2572352,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Zapaden Park",
                Latitude = 42.7113692,
                Longitude = 23.2705883,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Vardar",
                Latitude = 42.7059275,
                Longitude = 23.2848925,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Konstantin Velichkov",
                Latitude = 42.701834,
                Longitude = 23.2986114,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Opalchenska",
                Latitude = 42.6993922,
                Longitude = 23.3120976,
            },
            new PointOfInterest()
            {
                Name = "Metrostation St. Kliment Ohridski",
                Latitude = 42.6921761,
                Longitude = 23.3347454,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Vasil Levski Stadium",
                Latitude = 42.6860267,
                Longitude = 23.3320521,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Joliot Curie",
                Latitude = 42.6713838,
                Longitude = 23.350927,
            },
            new PointOfInterest()
            {
                Name = "Metrostation G.M. Dimitrov",
                Latitude = 42.6628765,
                Longitude = 23.3579076,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Musagenitsa",
                Latitude = 42.6541842,
                Longitude = 23.3719218,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Musagenitsa",
                Latitude = 42.6541842,
                Longitude = 23.3719218,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Mladost 1",
                Latitude = 42.6586002,
                Longitude = 23.3643255,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Aleksandar Malinov",
                Latitude = 42.6480469,
                Longitude = 23.3766183,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Acad. Al. Teodorov Balan",
                Latitude = 42.6415298,
                Longitude = 23.3727482,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Business Park Sofia",
                Latitude = 42.6298904,
                Longitude = 23.3731506,
            },

            // Line M4
            new PointOfInterest()
            {
                Name = "Metrostation Mladost 3",
                Latitude = 42.6464681,
                Longitude = 23.3840013,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Inter Expo Center - Tsarigradsko shose",
                Latitude = 42.6498534,
                Longitude = 23.3938374,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Druzhba",
                Latitude = 42.6588931,
                Longitude = 23.3946398,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Iskarsko Shose",
                Latitude = 42.6685054,
                Longitude = 23.4020805,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Sofiyska Sveta gora",
                Latitude = 42.6764457,
                Longitude = 23.4101895,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Sofia Airport Station",
                Latitude = 42.6880665,
                Longitude = 23.4161139,
            },

            // Line M2
            new PointOfInterest()
            {
                Name = "Metrostation Obelya",
                Latitude = 42.7410248,
                Longitude = 23.276511,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Lomsko shose",
                Latitude = 42.7404604,
                Longitude = 23.2873268,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Beli Dunav",
                Latitude = 42.7356167,
                Longitude = 23.292641,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Nadezhda",
                Latitude = 42.728167,
                Longitude = 23.3009072,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Han Kubrat",
                Latitude = 42.7229508,
                Longitude = 23.3065694,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Maria Luiza",
                Latitude = 42.7139402,
                Longitude = 23.3138236,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Central Railway Station",
                Latitude = 42.7107586,
                Longitude = 23.3200577,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Luvov Most",
                Latitude = 42.7054721,
                Longitude = 23.3238614,
            },
            new PointOfInterest()
            {
                Name = "Metrostation European Union",
                Latitude = 42.6786937,
                Longitude = 23.3217363,
            },
            new PointOfInterest()
            {
                Name = "Metrostation James Bourchier",
                Latitude = 42.6710893,
                Longitude = 23.3206841,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Vitosha",
                Latitude = 42.6592117,
                Longitude = 23.3165073,
            },

            // Line M3
            new PointOfInterest()
            {
                Name = "Metrostation Gorna Banya",
                Latitude = 42.682251,
                Longitude = 23.2407451,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Ovcha Kupel II",
                Latitude = 42.6847939,
                Longitude = 23.2475848,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Moesia",
                Latitude = 42.6839744,
                Longitude = 23.2560981,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Ovcha Kupel",
                Latitude = 42.6827867,
                Longitude = 23.2706196,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Krasno Selo",
                Latitude = 42.6788904,
                Longitude = 23.2844841,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Bulgaria",
                Latitude = 42.67938,
                Longitude = 23.3020658,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Medical University",
                Latitude = 42.6869351,
                Longitude = 23.3099973,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Sv. Patriarh Etimiy",
                Latitude = 42.6881496,
                Longitude = 23.3283651,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Orlov Most",
                Latitude = 42.6905825,
                Longitude = 23.3367175,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Teatralna",
                Latitude = 42.6965164,
                Longitude = 23.346647,
            },
            new PointOfInterest()
            {
                Name = "Metrostation Hadzi Dimitar",
                Latitude = 42.7029288,
                Longitude = 23.3531728,
            },
        };

        public void CreateSystemPoints()
        {
            User? adminUser = _context.Users.FirstOrDefault(x => x.Username == "Admin");
            if (adminUser == null)
            {
                adminUser = new User("Admin")
                {
                    Name = "System",
                    PasswordHash = "NoLogin"
                };
                _context.Users.Add(adminUser);
                _context.SaveChanges();
            }

            for (int i = 0; i < _systemPoints.Count; i++)
            {
                PointOfInterest systemPoint = _systemPoints[i];
                systemPoint.PointId = $"SystemPoint-{i}";
                systemPoint.Creator = adminUser;

                if (!_context.POIs.Any(x => x.PointId == systemPoint.PointId))
                {
                    _context.POIs.Add(systemPoint);
                }
            }
            _context.SaveChanges();
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