using ASP.NETProject.Data;
using ASP.NETProject.Models;

using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Project.Areas.Permissons;

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASP.NETProject.Services
{
    // Service for comments
    // Contains methods for creating, reading, updating and deleting comments
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method that returns all comments
        public async Task<Comment> GetComment(int id)
        {
            // Get comment from database
            return await _context.Comment.FindAsync(id);
        }

        // Method that returns all comments for a specific article
        public async Task CreateComment(Comment newComment, ClaimsPrincipal user)
        {
            Comment commentRecord = new Comment();

            // Get current user
            var currentUserId = user.Identity?.GetUserName();

            // Create new comment
            // Set values for new comment
            commentRecord.Content = newComment.Content;
            commentRecord.ReleaseDate = DateTime.Now;
            commentRecord.AuthorName = currentUserId;
            commentRecord.ArticleID = newComment.Id;

            // Add comment to database
            _context.Comment.Add(commentRecord);
            await _context.SaveChangesAsync();
        }

        // Method that returns all comments for a specific article
        public async Task DeleteComment(int id, ClaimsPrincipal user)
        {
            // Get current user
            var currentUserId = user.Identity?.GetUserName();

            // Get comment from database
            var comment = await _context.Comment.FindAsync(id);
            var currentArticle = await _context.Article.FindAsync(comment.ArticleID);

            // Check if user has permissions to delete comment
            if (user.IsHasPermissons() || user.Identity?.GetUserName() == comment.AuthorName)
            {
                if (comment != null)
                {
                    // Delete comment from database
                    _context.Comment.Remove(comment);
                }

                // Update article
                await _context.SaveChangesAsync();
            }
        }
    }
}