namespace City_Easter_Eggs.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Likes { get; set; }
        public string[] LikedPointsIds { get; set; }
    }
}
