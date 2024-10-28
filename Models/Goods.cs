using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Goods
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Giá nhập không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Import price must be a positive number")]
        public double ImportPrice { get; set; }

        [Required(ErrorMessage = "Tổng giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive number")]
        public double TotalPrice { get; set; }
        // Khóa ngoại Product
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        // Khóa ngoại Shipment
        public int ShipmentId { get; set; }
        public virtual Shipment? Shipment { get; set; }
    }
}
