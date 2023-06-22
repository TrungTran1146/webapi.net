using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Model
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }

    }
}
