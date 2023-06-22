using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Model
{
    [Table("ImportProduct")]
    public class ImportProduct
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }


    }
}
