namespace Social_Media_Project.Models
{
    public class MediaPost
    {
        //Id
        public int? Id { get; set; }
        //UserId
        public int? UserId { get; set; }
        //hdnId
        public int? hdnId { get; set; }

        //DateOfBirth
        public DateTime? DateOfBirth { get; set; }

        //ProfilePhotoPath
        public string? ProfilePhotoPath { get; set; }

        //Username
        public string? Username { get; set; }

        //Bio
        public string? Bio { get; set; }
        //Posts
        public string? Posts { get; set; }
        //Fullname
        public string? Fullname { get; set; }
        //PhotoPath
        public string? PhotoPath { get; set; }
        //ProfilePhoto
        public IFormFile? ProfilePhoto { get; set; }
        //AddPostPhoto
        public IFormFile? AddPostPhoto { get; set; }

        //Post Caption
        public string? PostCaption { get; set; }
        //Mobile
        public string? Mobile { get; set; }
        //Email
        public string? Email { get; set; }

        //HdnUsername
        public string? HdnUsername { get; set; }
        //Follower 
        public string? Follower { get; set; }
        //Following
        public string? Following { get; set; }

        //PostCount
        public string? PostCount { get; set; }
    }
}
