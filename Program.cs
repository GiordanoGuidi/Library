
using Microsoft.EntityFrameworkCore;
using Library.Models;

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
