using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AuctionDTOs.Custom_Validations;

namespace AuctionDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        [AllowedFileExtensions("jpg,jpeg,png,gif", ErrorMessage = "Please select a valid file type. (.jpg, .jpeg, .png or .gif)")]
        public AvatarImageIdDTO AvatarImage { get; set; }
    }

    public class UserDetailDTO : UserDTO
    {
        [Required(ErrorMessage = "An e-mail address is required.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+‌​)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "An address is required.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public List<ListingDTO> Listings { get; set; }
        public List<BidDTO> Bids { get; set; }
    }      

    public class AuthenticatedUserDTO : UserDetailDTO
    {
        //Implementation on the backlog; will be used for HTTPS.
    }

    public class UserLoginDTO
    {
        [Required(ErrorMessage = "An e-mail address is required.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+‌​)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; }

        [Required]       
        [DataType(DataType.Password)]
        [NotEqualTo("EmailAddress", ErrorMessage = "The password must not match the e-mail.")]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 24 characters.")]
        public string Password { get; set; }
    }


    public class UserRegisterDTO : UserLoginDTO
    {
        [Required(ErrorMessage = "A username is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "An address is required.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please retype your password.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 24 characters.")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please retype your e-mail address.")]
        [Display(Name = "Confirm E-mail")]
        [DataType(DataType.EmailAddress)]
        [Compare("EmailAddress", ErrorMessage = "The e-mail and confirmation do not match.")]
        public string ConfirmEmail { get; set; }
    }

    public class UserEditDTO : UserDTO
    {
        [Required(ErrorMessage = "An e-mail address is required.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+‌​)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "An address is required.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        [AllowedFileExtensions("jpg,jpeg,png,gif", ErrorMessage = "Please select a valid file type. (.jpg, .jpeg, .png or .gif)")]
        public new AvatarImageDTO AvatarImage { get; set; }
    }

    public class UserEditPasswordDTO : UserDTO
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 24 characters.")]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [NotEqualTo("OldPassword", ErrorMessage = "The new password must not match the old password.")]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 24 characters.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please retype your password.")]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]        
        [Compare("NewPassword", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
