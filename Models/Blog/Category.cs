using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(200)]
        public string? Content { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [DisplayName("Current Url")]
        public string Slug { get; set; }

        //Các category con
        public ICollection<Category>? ChildrenCategories { get; set;}

        [DisplayName("Parent Category Id")]
        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        [DisplayName("Parent Category")]
        public Category? ParentCategory { get; set; }
    }
}
