using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string? dbConnectionString = builder.Configuration.GetConnectionString("Default");

            if (dbConnectionString != null)
                builder.Services.AddDbContext<DreamcoreHorrorGameContext>(options => options.UseNpgsql(dbConnectionString));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}