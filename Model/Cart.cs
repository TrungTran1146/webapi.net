using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
       // public virtual Order Order { get; set; }
         public int ProductId { get; set; }

       // public virtual Product Product { get; set; }

         public int UserId { get; set; }

       // public virtual User User { get; set; }
        public string? Status { get; set; }
        public int Quantity { get; set; }

    }
}
