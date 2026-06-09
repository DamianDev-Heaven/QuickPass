namespace QuickPass.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string NameUser { get; set; } = string.Empty;
        public string? Description { get; set; }  
        public string? UrlPic { get; set; }
        public Guid AccId { get; set; }
        public virtual Account? Account { get; set; }
    }
}
