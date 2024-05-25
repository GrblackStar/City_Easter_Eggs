using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace City_Easter_Eggs.Models
{
    [PrimaryKey("LikedId")]
    public class LikedPoints
    {
        public string LikedId;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? PointId { get; set; }

        [ForeignKey("PointId")]
        public PointOfInterest? Point { get; set; }

        public LikedPoints()
        {
            LikedId = Guid.NewGuid().ToString();
        }
    }
}
