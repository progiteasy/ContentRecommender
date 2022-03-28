namespace ContentRecommender.ViewModels.Users
{
    public class UserViewModel
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RegistrationDate { get; set; }
        public string LastLoginDate { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
    }
}
