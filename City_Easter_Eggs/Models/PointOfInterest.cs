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
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public long TimeStamp { get; set; }

        public User Creator { get; set; }
        public int Likes { get; set; }

        public PointOfInterest()
        {
            PointId = new Guid().ToString();
            TimeStamp = (long) DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }
    }
}