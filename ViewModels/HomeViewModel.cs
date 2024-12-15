using ThienAnFuni.Models;
using System.Collections.Generic;


namespace ThienAnFuni.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }

        public List<Product> FeaturedProducts { get; set; }
        public List<Product> NewProducts { get; set; }
        public List<Product> BestSellerProducts { get; set; }
    }
}
