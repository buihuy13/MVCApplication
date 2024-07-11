using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest.Models
{
    public class PostCategory
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category cate { get; set; }

        [ForeignKey("PostId")]
        public Post post { get; set; }
    }
}
