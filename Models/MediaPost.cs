namespace Social_Media_Project.Models
{
    public class MediaPost
    {
        //Id
        public int? Id { get; set; }
        //hdnId
        public int? hdnId { get; set; }

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

        //AddPostPhoto
        public IFormFile? AddPostPhoto { get; set; }

        //Post Caption
        public string? PostCaption { get; set; }
    }
}
