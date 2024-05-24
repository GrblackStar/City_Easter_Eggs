#region Using

using Microsoft.EntityFrameworkCore;

#endregion

namespace City_Easter_Eggs.Models
{
    [PrimaryKey("UserId")]
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public int LikesObtained { get; set; }
        public ICollection<PointOfInterest> PlacedPoints { get; set; }
        public List<LikedPoints> LikedPoints { get; set; }
        public List<FavouritePoints> FavoritedPoints { get; set; }

        #region Identity

        public string Username { get; set; }
        public string UsernameNormalized { get; set; }
        public string? PasswordHash { get; set; }

        #endregion

        public User(string username)
        {
            Username = username;
            UsernameNormalized = username.ToUpperInvariant();
            Name = username; // todo

            UserId = Guid.NewGuid().ToString();
            PlacedPoints = new HashSet<PointOfInterest>();
        }
    }
}