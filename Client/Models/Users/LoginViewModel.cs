using Library.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Client.Models.Users
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "Username must be in range between 4 and 16 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Incorrect password")]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "Password must be in range between 4 and 32 characters")]
        public string Password { get; set; } = string.Empty;

        public LoginViewModel(string username, string password)
        {
            Username = username.Trim().ToLower();
            Password = password;
        }

        public LoginViewModel() { }
    }
}
