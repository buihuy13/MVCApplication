using MVCTest.Models;

namespace MVCApplication.Areas.Blog.Models.Posts
{
    public class PostsListModel
    {
        public int totalPosts { get; set; }
        public int countPages { get; set; }
        public int ITEMS_PER_PAGE { get; set; } = 10;
        public int currentPage { get; set; }
        public List<Post> posts { get; set; }
    }
}
