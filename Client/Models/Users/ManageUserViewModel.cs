using Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Client.Models.Users
{
    public class ManageUserViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "Username must be in range between 4 and 16 characters")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "First name")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "First name must be in range between 2 and 32 characters")]
        public string? FirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Last name must be in range between 2 and 32 characters")]
        public string? LastName { get; set; } = string.Empty;

        [Range(18, 99, ErrorMessage = "Age must be in range between 18 and 99")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Incorrect email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Incorrect phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Incorrect password")]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "Password must be in range between 4 and 32 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password, ErrorMessage = "Incorrect confirmation password")]
        [Compare("Password", ErrorMessage = "Passwords does not much")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = RoleType.User.ToString();

        public List<Product> Basket { get; set; } = new();

        public ManageUserViewModel(long id, string username, string? firstName, string? lastName, int? age, string email, string phone, string password, string confirmPassword, string role)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
            Phone = phone;
            Password = password;
            ConfirmPassword = confirmPassword;
            Role = role;
        }

        public ManageUserViewModel() { }
    }
}
