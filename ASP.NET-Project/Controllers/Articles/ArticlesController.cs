using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NETProject.Data;
using ASP.NETProject.Models;
using Microsoft.AspNet.Identity;
using ASP.NETProject.Models;
using ASP.NET_Project.Areas.Permissons;

namespace ASP.NETProject.Controllers
{

    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ArticlesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
              return _context.Article != null ? 
                          View(await _context.Article.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Article'  is null.");
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

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

            if (newModel.currentArticle == null)
            {
                return NotFound();
            }

            return View(newModel);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatorId,Title,Header,Content,ReleaseDate,Views,ImageFile")] Article article)
        {
            if (ModelState.IsValid)
            {
                if (article.ImageFile != null)
                {
                    //Zapisujemy zdjecie w plik wwwroot/uploaded
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                    string extension = Path.GetExtension(article.ImageFile.FileName);
                    article.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/uploaded", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await article.ImageFile.CopyToAsync(fileStream);
                    }
                } else
                {
                    article.ImagePath = "noimage.jpg";
                }

                var currentUserId = User.Identity?.GetUserName();

                article.ReleaseDate = DateTime.Today;
                article.CreatorId = currentUserId;
  
                
                //Wpisujemy record do bazy danyc
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task CreateComment([Bind("Id,AuthorName,Title,Header,Content,ReleaseDate,ArticleId")] Comment newComment)
        {
            Comment commentRecord = new Comment();
            if (ModelState.IsValid)
            {
                //Zapisujemy zdjecie w plik wwwroot/uploaded        
                var currentUserId = User.Identity?.GetUserName();

                commentRecord.Content = newComment.Content;
                commentRecord.ReleaseDate = DateTime.Now;
                commentRecord.AuthorName = currentUserId;
                commentRecord.ArticleID = newComment.Id;


                //Wpisujemy record do bazy danyc
                _context.Comment.Add(commentRecord);
                await _context.SaveChangesAsync();
            }

            Response.Redirect("Details/"+commentRecord.ArticleID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task DeleteComment(int id)
        {
            var currentUserId = User.Identity?.GetUserName();

            var comment = await _context.Comment.FindAsync(id);
            var currentArticle = await _context.Article.FindAsync(comment.ArticleID);

            if (User.IsHasPermissons() ||
                User.Identity?.GetUserName() == comment.AuthorName)
            {
                if (comment != null)
                {
                    _context.Comment.Remove(comment);
                }

                await _context.SaveChangesAsync();
            }

            Response.Redirect("Details/" + currentArticle.Id);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatorId,Title,Header,Content,ReleaseDate,Views,ImageFile")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            var articlePreEdited = await _context.Article.FindAsync(id);

            article.CreatorId = articlePreEdited.CreatorId;
            article.ReleaseDate = articlePreEdited.ReleaseDate;
            article.Views = articlePreEdited.Views;

            //article.CreatorId = "raman.kuzmich@pollub.edu.pl";
            //article.ReleaseDate = DateTime.Today;
            //article.Views = 13;

            if (article.ImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                string extension = Path.GetExtension(article.ImageFile.FileName);
                article.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploaded", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await article.ImageFile.CopyToAsync(fileStream);
                }
            } else
            {
                article.ImagePath = articlePreEdited.ImagePath;
            }

            _context.Entry(articlePreEdited).State = EntityState.Detached;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Article == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Article'  is null.");
            }
            var article = await _context.Article.FindAsync(id);



            if (article != null)
            {
                _context.Article.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.Article?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
