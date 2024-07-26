namespace Social_Media_Project.Models
{
    public class MediaPost
    {
        //DateOfBirth
        public DateTime? DateOfBirth { get; set; }
        //ProfilePhoto
        public IFormFile? ProfilePhoto { get; set; }
        //ProfilePhotoPath
        public string? ProfilePhotoPath { get; set; }
        //Bio
        public string? Bio { get; set; }
        //Posts
        public string? Posts { get; set; }
        //Fullname
        public string? Fullname { get; set; }
        //PhotoPath
        public string? PhotoPath { get; set; }
    }
}
