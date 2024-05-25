using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace City_Easter_Eggs.Models
{
    [PrimaryKey("FavoriteId")]
    public class FavouritePoints
    {
        public string FavoriteId;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? PointId { get; set; }

        [ForeignKey("PointId")]
        public PointOfInterest? Point { get; set; }

        public FavouritePoints()
        {
            FavoriteId = Guid.NewGuid().ToString();
        }
    }
}
