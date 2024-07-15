using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

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
        public void CategoryChildrenIds(ICollection<Category> categoryChildren = null, List<int> lists = null)
        {
            if (categoryChildren == null)
            {
                categoryChildren = this.ChildrenCategories;
            }

            if (lists == null)
            {
                throw new Exception("Wrong");
            }

            foreach (var cate in categoryChildren)
            {
                lists.Add(cate.Id);
                CategoryChildrenIds(cate.ChildrenCategories, lists);
            }
        }
        public List<int> ParentCategoryIds(Category category,List<int> lists)
        {
            if (category != null)
            {
                lists.Add(category.Id);
                return ParentCategoryIds(category.ParentCategory, lists);
            }
            return lists;
        }
    }
}
