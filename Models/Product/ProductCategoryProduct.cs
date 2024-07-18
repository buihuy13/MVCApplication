using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest.Models.Product
{
    public class ProductCategoryProduct
    {
        [Required]
        public int CategoryProductId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("CategoryProductId")]
        public CategoryProduct categoryproduct { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel product { get; set; }
    }
}
