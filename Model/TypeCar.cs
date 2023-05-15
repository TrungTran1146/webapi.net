using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    [Table("TypeCar")]
    public class TypeCar
    {
        [Key]
        public int Id { get; set; }
        public string NameType { get; set; }


        public string? Description { get; set; }
        public virtual ICollection<Product> Products { get; } = new List<Product>();
    }
}
