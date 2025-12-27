namespace QuickPass.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        // Datos de Account 
        public string Email { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public bool OtherMed { get; set; } = false;
        // Datos de Users
        public string NameUser { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? UrlPic { get; set; }

    }
}
