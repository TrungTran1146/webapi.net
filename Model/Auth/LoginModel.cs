using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(250)]
        public string? Password { get; set; }
    }
}
