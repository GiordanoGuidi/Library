using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models.DTO;
using System.Linq;

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
        [HttpGet("GetByTitleAndNationality")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBookByTitleAndNationality(string? title,string? nationality)
        {
            var query = _context.Books
           .Include(b => b.Author) 
           //trasformo dinamicamente la query
           .AsQueryable();

            //Controllo se il titolo è inserito
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }
            //Controllo se la nazionalità è inserita
            if (!string.IsNullOrEmpty(nationality))
            {
                query = query.Where(b => b.Author != null && b.Author.Nationality == nationality);
            }

            //Creo l'oggetto BookDto
            var books = await query.Select(b => new BookDto
            {
                Title = b.Title,
                AuthorId = b.AuthorId,
                Price = b.Price,
                PublishedDate = b.PublishedDate,
                Stock = b.Stock,
                AuthorNationality = b.Author != null ? b.Author.Nationality : null 
            }).ToListAsync();

            return Ok(books);
        }

        //Azione per recuperare il libro in base al prezzo e allo stock
        [HttpGet("GetByPriceAndStock")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBookByPriceAndStock(decimal? price,int? stock)
        {
            var query = _context.Books.AsQueryable();

            //Controllo se il prezzo è inserito
            if(price != null)
            {
                query = query.Where(b => b.Price >=  price);
            }
            //Controllo se lo stock è inserito
            if(stock != null)
            {
                query = query.Where(b => b.Stock >= stock);
            }
            //Se entrambi presenti ordino per prezzo e poi per stock crescente
            if (price != null && stock != null)
            {
                query = query.OrderBy(b => b.Price).ThenBy(b => b.Stock);
            }
            // Se solo il prezzo è presente, ordino solo per prezzo crescente
            else if (price != null)
            {
                query = query.OrderBy(b => b.Price);
            }
            // Se solo lo stock è presente, ordino solo per stock crescente
            else if (stock != null)
            {
                query = query.OrderBy(b => b.Stock);
            }


            //Creo l'oggetto BookDto
            var books = await query.Select(b => new BookDto
            {
                Title = b.Title,
                AuthorId = b.AuthorId,
                Price = b.Price,
                PublishedDate = b.PublishedDate,
                Stock = b.Stock,
            }).ToListAsync();

            return Ok(books);
        }
    }
}
