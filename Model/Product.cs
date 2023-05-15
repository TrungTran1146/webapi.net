using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }


        public string? Description { get; set; }

        public int Quantity { get; set; }
        public int BrandId { get; set; }
        //public virtual Brand Brand { get; set; }

         public int TypeCarId { get; set; }
       // public virtual TypeCar TypeCar { get; set; }

        public virtual ICollection<ImportProduct> ImportProducts { get; } = new List<ImportProduct>();


    }
}
