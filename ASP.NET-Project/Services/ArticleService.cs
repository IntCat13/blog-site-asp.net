using ASP.NETProject.Data;
using ASP.NETProject.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETProject.Services
{
    // Service for articles
    // Contains methods for creating, reading, updating and deleting articles
    public class ArticleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        
        public ArticleService(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // Method that returns all articles
        public async Task<List<Article>> GetArticles()
        {
            // Get all articles from database
            return await _context.Article.ToListAsync();
        }

        // Method that returns details for a specific article
        public async Task<Article> GetArticle(int? id)
        {
            // Get article from database
            return await _context.Article.FirstOrDefaultAsync(m => m.Id == id);
        }
        
        public async Task<ArticleViewModel> GetArticleViewModel(int? id)
        {
            ArticleViewModel newModel = new ArticleViewModel();
            newModel.currentArticle = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            var comments = _context.Comment
                .Where(x => x.ArticleID == newModel.currentArticle.Id);
            
            newModel.comments = comments;
            
            if (newModel.currentArticle != null)
            {
                newModel.currentArticle.Views = newModel.currentArticle.Views + 1;
                _context.Update(newModel.currentArticle);
                await _context.SaveChangesAsync();
            }
            
            // Get article from database
            return newModel;
        }

        // Method that returns all articles created by a specific user
        public async Task<Article> GetArticleByCommentId(int commentId)
        {
            // Get comment from database
            var comment = await _context.Comment.FindAsync(commentId);
            return await _context.Article.FindAsync(comment.ArticleID);
        }

        // Method that returns all articles created by a specific user
        public async Task CreateArticle(Article article, ClaimsPrincipal user)
        {
            // Upload image to server
            // If no image is uploaded, use default image
            if (article.ImageFile != null)
            {
                // Load image to server
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                string extension = Path.GetExtension(article.ImageFile.FileName);
                article.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploaded", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await article.ImageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                // Use default image
                article.ImagePath = "noimage.jpg";
            }

            // Set all other properties
            var currentUserId = user.Identity?.GetUserName();

            article.ReleaseDate = DateTime.Today;
            article.CreatorId = currentUserId;

            // Save changes to database
            _context.Add(article);
            await _context.SaveChangesAsync();
        }

        // Method that updates an article
        public async Task UpdateArticle(Article article)
        {
            // Get article from database
            var articlePreEdited = await _context.Article.FindAsync(article.Id);

            article.CreatorId = articlePreEdited.CreatorId;
            article.ReleaseDate = articlePreEdited.ReleaseDate;
            article.Views = articlePreEdited.Views;

            // Upload image to server if a new image is uploaded
            if (article.ImageFile != null)
            {
                // Load image to server
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                string extension = Path.GetExtension(article.ImageFile.FileName);
                article.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploaded", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await article.ImageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                article.ImagePath = articlePreEdited.ImagePath;
            }

            // Detach article from context
            _context.Entry(articlePreEdited).State = EntityState.Detached;
            _context.Update(article);
            
            // Save changes to database
            await _context.SaveChangesAsync();
        }

        // Method that deletes an article
        public async Task DeleteArticle(int id)
        {
            // Get article from database
            var article = await _context.Article.FindAsync(id);

            if (article != null)
            {
                // Remove article from database
                _context.Article.Remove(article);
            }

            // Save changes to database
            await _context.SaveChangesAsync();
        }
    }
}