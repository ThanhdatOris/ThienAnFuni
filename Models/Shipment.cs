using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Shipment // Phiếu nhập lô hàng
    {
        [Display(Name = "Mã NH")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ngày nhập không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhập")]
        public DateTime ReceiptDate { get; set; }
        [Required(ErrorMessage = "Tổng giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng giá phải là số")]
        [Display(Name = "Tổng tiền")]
        public double TotalPrice { get; set; }

        [Required(ErrorMessage = "Tổng số lượng không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng số lượng phải là số")]
        [Display(Name = "Tổng số lượng")]
        public int TotalQuantity { get; set; }
        [Display(Name = "Ghi chú")]

        public string? Note { get; set; }
        [Display(Name = "Mã NCC")]

        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public string ManagerId { get; set; }
        public virtual Manager? Manager { get; set; }
        public virtual ICollection<Goods> Goods { get; set; } = new List<Goods>();


    }
}
