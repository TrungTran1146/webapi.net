using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Model
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        //  public virtual User User { get; set; }
        public string Name { get; set; }

        public int Phone { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }

        public int TotalOder { get; set; }
    }
}
