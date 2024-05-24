#region Using

using Microsoft.EntityFrameworkCore;

#endregion

namespace City_Easter_Eggs.Models
{
    [PrimaryKey("PointId")]
    public class PointOfInterest
    {
        public string PointId { get; set; }

        public string Name { get; set; } = "Untitled";
        public string Description { get; set; } = "No Description";
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public long TimeStamp { get; set; }

        public User Creator { get; set; }
        public int Likes { get; set; }
        public List<LikedPoints> LikedPoints { get; set; }
        public List<FavouritePoints> FavoritedPoints { get; set; }

        public PointOfInterest()
        {
            PointId = Guid.NewGuid().ToString();
            TimeStamp = (long) DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            Likes = 0;
        }
    }
}