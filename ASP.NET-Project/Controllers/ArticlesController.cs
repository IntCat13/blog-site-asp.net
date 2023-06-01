using System.Threading.Tasks;
using ASP.NETProject.Models;
using Microsoft.AspNetCore.Mvc;
using ASP.NETProject.Services;
using ASP.NET_Project.Areas.Permissons;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using ASP.NETProject.Data;

namespace ASP.NETProject.Controllers
{
    // Controller for articles
    // Contains methods for creating, reading, updating and deleting articles
    public class ArticlesController : Controller
    {
        // Dependency injection of services
        private readonly ArticleService _articleService;
        private readonly CommentService _commentService;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ArticlesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;

            _articleService = new ArticleService(context, hostEnvironment);
            _commentService = new CommentService(context);
        }

        // GET: Articles
        // Method that returns all articles
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetArticles();
            return View(articles);
        }

        // GET: Articles/Details/5
        // Method that returns details for a specific article
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var article = await _articleService.GetArticleViewModel(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        // GET: Articles/Create
        // Method that returns the view for creating a new article
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // Method that creates a new article
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatorId,Title,Header,Content,ReleaseDate,Views,ImageFile")] Article article)
        {
            if (ModelState.IsValid)
            {
                await _articleService.CreateArticle(article, User);
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        // Method that creates a new comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CreateComment([Bind("Id,AuthorName,Title,Header,Content,ReleaseDate,ArticleId")] Comment newComment)
        {
            if (ModelState.IsValid)
            {
                await _commentService.CreateComment(newComment, User);
                Response.Redirect("Details/" + newComment.ArticleID);
            }
        }

        // Method that deletes a comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task DeleteComment(int id)
{
            var currentArticle = await _articleService.GetArticleByCommentId(id);

            await _commentService.DeleteComment(id, User);
            Response.Redirect("Details/" + currentArticle.Id);
        }

        // GET: Articles/Edit/5
        // Method that returns the view for editing a specific article
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var article = await _articleService.GetArticle(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        // POST: Articles/Edit/5
        // Method that edits a specific article
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatorId,Title,Header,Content,ReleaseDate,Views,ImageFile")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _articleService.UpdateArticle(article);
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        // GET: Articles/Delete/5
        // Method that returns the view for deleting a specific article
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _articleService.GetArticle(id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        // Method that deletes a specific article
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _articleService.DeleteArticle(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
