using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly MyDbContext _context;
        //DI del context
        public BooksController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/<BooksControllerr>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetUsers()
        {
            return await _context.Books.ToListAsync();
        }

        // GET api/<BooksControllerr>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        // POST api/<BooksControllerr>
        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT api/<BooksControllerr>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<BooksControllerr>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Azione per recuperare il libro in base al titolo
        [HttpGet("GetByTitle")]
        public async Task<ActionResult<Book>> GetBookByTitle(string title)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                return NotFound("Libro non trovato");
            }

            return Ok(book);
        }
    }
}
