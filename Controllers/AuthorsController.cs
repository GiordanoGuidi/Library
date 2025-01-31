using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly MyDbContext _context;
        //DI del context
        public AuthorsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/<AuthorsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        }

        // GET api/<AuthorsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        // POST api/<AuthorsController>
        [HttpPost]
        public async Task<ActionResult<Author>> Post(Author author) 
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthor), new {id=author.Id},author);
        }

        // PUT api/<AuthorsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }
            _context.Entry(author).State = EntityState.Modified;//Informo che l'author è stato modificato
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<AuthorsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
