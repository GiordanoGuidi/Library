using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Seed
{
    public class DataSeeder
    {
        private readonly MyDbContext _context;
        //Eseguo la DI del Context
        public DataSeeder(MyDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Controlla se ci sono già dati nella tabella Authors
            if (!_context.Authors.Any())
            {
                var authors = new Faker<Author>()
                    .RuleFor(a => a.Firstname, f => f.Name.FirstName())
                    .RuleFor(a => a.Lastname, f => f.Name.LastName())
                    .RuleFor(a => a.Nationality, f => f.Address.CountryCode())
                    .Generate(1000);
                _context.Authors.AddRange(authors);
                _context.SaveChanges();
            }

            // Controlla se ci sono già dati nella tabella Addresses
            if (!_context.Addresses.Any())
            {
                var addresses = new Faker<Address>()
                    .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                    .RuleFor(a => a.City, f => f.Address.City())
                    .RuleFor(a => a.ZipCode, f => f.Address.ZipCode("#####"))
                    .Generate(1000);
                _context.Addresses.AddRange(addresses);
                _context.SaveChanges();
            }

            // Carica gli autori e gli indirizzi da DbContext dopo che sono stati salvati
            var authorsList = _context.Authors.ToList();
            var addressesList = _context.Addresses.ToList();

            // Controlla se ci sono già dati nella tabella Books
            if (!_context.Books.Any())
            {
                var books = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                    .RuleFor(b => b.AuthorId, f => f.PickRandom(authorsList).Id)  // Usa la lista caricata di autori
                    .RuleFor(b => b.Price, f => Math.Round(f.Random.Decimal(10, 100), 2))
                    .RuleFor(b => b.PublishedDate, f => DateOnly.FromDateTime(f.Date.Past(10)))
                    .RuleFor(b => b.Stock, f => f.Random.Int(0, 100))
                    .Generate(1000);
                _context.Books.AddRange(books);
                _context.SaveChanges();
            }

            // Controlla se ci sono già dati nella tabella Users
            if (!_context.Users.Any())
            {
                var users = new Faker<User>()
                    .RuleFor(u => u.Firstname, f => f.Name.FirstName())
                    .RuleFor(u => u.Lastname, f => f.Name.LastName())
                    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Firstname, u.Lastname))
                    .RuleFor(u => u.Password, f => f.Internet.Password())
                    .RuleFor(u => u.AddressId, f => f.PickRandom(addressesList).Id)  // Usa la lista caricata di indirizzi
                    .RuleFor(u => u.CreatedAt, f => DateOnly.FromDateTime(f.Date.Past(3)))
                    .Generate(1000);
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
        }
    }
    }
