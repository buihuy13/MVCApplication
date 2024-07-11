using MVCTest.Models;

namespace MVCApplication.Areas.Blog.Models.Posts
{
    public class CreatePostsModel : Post
    {
        public int[] CategoryIds { get; set; }
    }
}
