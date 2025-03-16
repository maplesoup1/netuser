using System.ComponentModel.DataAnnotations.Schema;

namespace Titube.Entities
{
    public enum MediaType
    {
        Image = 0,
        Video = 1
    }
    public class Media
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileDescription { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}