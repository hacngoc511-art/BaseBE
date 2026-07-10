using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseBE.Infrastructure.Data;
using BaseBE.Domain.Entities;

namespace BaseBE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _context.Authors
                .Include(x => x.Books)
                .ToListAsync();

            return Ok(authors);
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(x => x.Books)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
                return NotFound();

            return Ok(author);
        }

        // POST: api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return Ok(author);
        }

        // PUT: api/authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            if (id != author.Id)
                return BadRequest();

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(author);
        }

        // DELETE: api/authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return Ok("Author deleted successfully.");
        }
    }
}