using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    [Table("ImportProduct")]
    public class ImportProduct
    {
        [Key]
        public int Id { get; set; }

          public int ProductId { get; set; }
      //  public virtual Product Product { get; set; }

        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        

    }
}
