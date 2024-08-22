namespace Social_Media_Project.Models
{
    public class NotificationSend
    {
        public string? Token { get; set; } = "";
        public string? Title { get; set; } = "";
        public string? Body { get; set; } = "";

        public string? RecipientUserId { get; set; }
        public string? MessageContent { get; set; }
    }
}
