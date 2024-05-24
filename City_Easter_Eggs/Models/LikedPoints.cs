namespace City_Easter_Eggs.Models
{
    public class LikedPoints
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string PointId { get; set; }
        public PointOfInterest PointOfInterest { get; set; }
    }
}
