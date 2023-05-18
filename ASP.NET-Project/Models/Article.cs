using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NETProject.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string? CreatorId { get; set; }
        [Column(TypeName = "nvarchar(100)")]

        public string Title { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Header { get; set; }

        [Column(TypeName = "nvarchar(4000)")]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public int Views { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image of article")]
        public string? ImagePath { get; set; }

        [NotMapped]
        [DisplayName("Upload file")]
        public IFormFile? ImageFile { get; set; }
    }
}
