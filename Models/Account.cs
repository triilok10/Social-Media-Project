namespace Social_Media_Project.Models
{
    public class Account
    {   //Id
        public int Id { get; set; }
        //Username
        public string? Username { get; set; }
        //Password
        public string? Password { get; set; }
        //Email
        public string? Email { get; set; }
        //Phone
        public string? Phone { get; set; }
        //Fullname
        public string? Fullname { get; set; }
        //ConfirmPassword
        public string? ConfirmPassword { get; set; }
        //DateOfBirth
        public DateOnly? DateOfBirth { get; set; }
        //ProfilePhoto
        public IFormFile? ProfilePhoto { get; set; }
        //ProfilePhotoPath
        public string? ProfilePhotoPath { get; set}
        //Bio
        public string? Bio { get; set; }
        //Hidden Id
        public int? hdnId { get; set; }
    }
}
