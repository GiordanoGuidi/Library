using Library.Models;
using Library.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Library.Controllers
{
    public class AuthController : Controller
    {
        private readonly MyDbContext _context;
        //DI del context
        public AuthController(MyDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            //Controllo se l'email gia esiste
            if (await _context.Users.AnyAsync(c => c.Email == registerUserDto.Email))
                return BadRequest("L'email è già registrata.");

            // Creazione dell'indirizzo
            var address = new Address
            {
                Street = registerUserDto.Street,
                City = registerUserDto.City,
                ZipCode = registerUserDto.ZipCode,
            };
            // Salvo l'indirizzo nel DB
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();


            //Creo l'utente
            var newUser = new User
            {
                Firstname= registerUserDto.Firstname,
                Lastname= registerUserDto.Lastname,
                Email = registerUserDto.Email,
                //Utilizzo BCrypt per l'hashing della password
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password),
                AddressId = address.Id,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("Registrazione completata con successo.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            //Recupero l'utente
            var user = await _context.Users.FirstOrDefaultAsync( c => c.Email == loginUserDto.Email);
            if (user == null) return BadRequest("Email o password errati");
            //Utilizzo la verifica di BCrypt per controllare la validità della password
            bool isValid = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password);
            if (!isValid) return BadRequest("Password errata");
            else return Ok("Accesso effettuato");

        }

        
    }
}
