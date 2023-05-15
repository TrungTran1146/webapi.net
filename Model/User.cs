using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(250)]
        public string Password { get; set; }

        [MaxLength(250)]
        public string FullName { get; set; }

        public string Email { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }
        public bool IsAdmin { get; set; }

        
    }
}
