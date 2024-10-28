namespace ThienAnFuni.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImgURL { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
