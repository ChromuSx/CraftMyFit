namespace CraftMyFit.Models
{
    public partial class User
    {
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
    }
}
