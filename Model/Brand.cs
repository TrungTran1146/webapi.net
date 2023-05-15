using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace WebAPI.Model
{
    [Table("Brand")]
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        public string BrandName { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; } = new List<Product>();


    }
}
