using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số")]
        public double Price { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int CartId { get; set; }
        public virtual Cart? Cart { get; set; }
    }
}
