using Microsoft.AspNetCore.Mvc;
using Titube.Data;
using Titube.Entities;
using Titube.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Titube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly ApplicationDbContext _context;

        public MediaController(IStorageService storageService, ApplicationDbContext context)
        {
            _storageService = storageService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Media>>> GetAll()
        {
            var media = await _context.Media.ToListAsync();
            return Ok(media);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> Get(int id)
        {
            var media = await _context.Media.FindAsync(id);
            if (media == null)
                return NotFound();
            return media;
        }

        [HttpGet("file/{id}")]
        public async Task<ActionResult> GetFile(int id)
        {
            var media = await _context.Media.FindAsync(id);
            if (media == null)
                return NotFound();
            
            var fileBytes = await _storageService.GetFileAsync(media.FilePath);
            return File(fileBytes, "application/octet-stream", media.FileName);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<Media>> Upload(IFormFile file)
        {
            string folder = file.ContentType.StartsWith("image/") ? "images" : "videos";
            string filePath = await _storageService.UploadFileAsync(file, folder);

            var media = new Media
            {
                UserId = 1,
                FileName = file.FileName,
                FilePath = filePath,
                FileDescription = "",
                FileSize = file.Length.ToString(),
                Type = file.ContentType.StartsWith("image/") ? MediaType.Image : MediaType.Video,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Media.Add(media);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("Get", new { id = media.Id }, media);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var media = await _context.Media.FindAsync(id);
            if (media == null)
                return NotFound();
            
            await _storageService.DeleteFileAsync(media.FilePath);
            
            _context.Media.Remove(media);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}