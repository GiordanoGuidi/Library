using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models.DTO;
using System.Linq;
using Library.PaginationUtils;
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Reflection.Metadata.BlobBuilder;


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
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] int page,[FromQuery] int pageSize)
        {
            Console.WriteLine(page);
            Console.WriteLine(pageSize);
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Evito richieste troppo grandi

            var query = _context.Books.AsQueryable();
            var books = await query
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b=> new Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate,
                    Stock = b.Stock,
                })
                .ToListAsync();

            // Calcolo il numero totale di record per determinare il numero totale di pagine
            var totalRecords = await query.CountAsync();

            // Calcolo il numero totale di pagine
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            //Creo l'oggetto paginationInfo
            var paginationInfo = new PaginationInfo(page, pageSize, totalRecords);
            //Restituisco i libri e le info sulla paginazione
            return Ok(new PagedResult<Book>(books, paginationInfo));
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
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBookByTitleAndNationality(string? title,string? nationality,int page,int pageSize)
        {

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Evito richieste troppo grandi

            var query = _context.Books
                .Include(b => b.Author)
                .AsQueryable();

            // Filtri dinamici
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(nationality))
            {
                query = query.Where(b => b.Author != null && b.Author.Nationality == nationality);
            }

            // Conta il numero totale di record prima dell'impaginazione
            var totalRecords = await query.CountAsync();

            // Applica paginazione con Skip e Take
            var books = await query
                .OrderBy(b=> b.Id)
                //Indico gli elementi da saltare
                .Skip((page - 1) * pageSize)
                //Prendo i successivi 10
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id= b.Id,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate,
                    Stock = b.Stock,
                    AuthorNationality = b.Author != null ? b.Author.Nationality : null
                })
                .ToListAsync();

            //Creo l'oggetto paginationInfo
            var paginationInfo = new PaginationInfo(page, pageSize, totalRecords);
            //Restituisco i libri e le info sulla paginazione
            return Ok(new PagedResult<BookDto>(books, paginationInfo));
        }

        //Azione per recuperare il libro in base al prezzo e allo stock
        [HttpGet("GetByPriceAndStock")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBookByPriceAndStock(decimal? price,int? stock,int page,int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Evito richieste troppo grandi

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

            // Calcolo il numero totale di record per determinare il numero totale di pagine
            var totalRecords = await query.CountAsync();

            // Calcolo il numero totale di pagine
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);


            //Creo l'oggetto BookDto
            var books = await query
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
            {
                Id= b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                Price = b.Price,
                PublishedDate = b.PublishedDate,
                Stock = b.Stock,
            }).ToListAsync();

            //Creo l'oggetto paginationInfo
            var paginationInfo = new PaginationInfo(page, pageSize,totalRecords);
            //Restituisco i libri e le info sulla paginazione
            return Ok(new PagedResult<BookDto>(books, paginationInfo));
        }
    }
}
