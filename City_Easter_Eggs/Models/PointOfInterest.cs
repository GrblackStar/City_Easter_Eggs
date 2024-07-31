#region Using

using City_Easter_Eggs.QuadTree;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Security.Policy;
using System.Text.Json.Serialization;

#endregion

namespace City_Easter_Eggs.Models
{
    [PrimaryKey("PointId")]
    public class PointOfInterest : IQuadTreeObject
    {
        public string PointId { get; set; }

        public string Name { get; set; } = "Untitled";
        public string Description { get; set; } = "No Description";
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public long TimeStamp { get; set; }

        [JsonIgnore]
        public User Creator { get; set; }
        public int Likes { get; set; }

        [NotMapped]
        public string PointCreatorId { get => Creator?.UserId; }

        [JsonIgnore]
        public List<LikedPoints> LikedPoints { get; set; } = new List<LikedPoints>();

        [JsonIgnore]
        public List<FavouritePoints> FavoritedPoints { get; set; } = new List<FavouritePoints>();

        public PointOfInterest()
        {
            PointId = Guid.NewGuid().ToString();
            TimeStamp = (long) DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            Likes = 0;
        }

		public Vector2 GetPosition()
		{
            return new Vector2((float)Latitude, (float)Longitude);
		}

        public Rectangle GetBounds()
        {
            Vector2 objectPos = GetPosition();
            return new Rectangle(objectPos, new Vector2(MathHelper.MetersToEarthRadiusRadian(1)));
        }
    }
}