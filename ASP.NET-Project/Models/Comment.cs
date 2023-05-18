using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NETProject.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string? AuthorName { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int ArticleID { get; set; }
    }
}
