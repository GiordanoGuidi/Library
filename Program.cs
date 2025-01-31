
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Models.Seed;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

           


            // Configuro i servizi qui (add db context e altre dipendenze)
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MainSqlConnection")));
            builder.Services.AddTransient<DataSeeder>(); // Aggiungo il DataSeeder ai servizi


            //Cors Policy
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy",
                    builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            
            var app = builder.Build();

            app.UseCors("CorsPolicy");


            //// Eseguo il seeding all'avvio
            //using (var scope = app.Services.CreateScope())
            //{
            //    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            //    seeder.Seed(); // Chiamata al metodo di seeding per popolare il database
            //}

            // Configura il middleware della pipeline per la richiesta HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();  
                app.UseSwaggerUI(); 
            }

         
            app.UseHttpsRedirection();
            app.UseAuthorization();
            // Attiva il middleware per i log degli errori
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            app.MapControllers();
            app.Run();
        }
    }
}
