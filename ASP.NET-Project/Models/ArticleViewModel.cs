using NuGet.DependencyResolver;

namespace ASP.NETProject.Models
{
    public class ArticleViewModel
{
        public Article currentArticle { get; set; }
        public IEnumerable<Comment> comments { get; set; }
        public Comment newComment { get; set; }
    }
}
