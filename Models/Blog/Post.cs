using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string? Description { get; set; }

        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [DisplayName("Current URL")]
        [StringLength(160, MinimumLength = 5)]
        public string? Slug { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string? Content { get; set; }
        public bool? Published { get; set; }
        public string? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public AppUser? Author { get; set; }

        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }

        [DisplayName("Date Updated")]
        public DateTime? DateUpdated { get; set; }
        public List<PostCategory>? PostCategories { get; set; }
    }
}
