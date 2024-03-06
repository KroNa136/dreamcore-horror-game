namespace Dreamcore_Horror_Game_API_Server
{
    public class TestClass
    {
        private string connectionString = "Driver={PostgreSQL ODBC Driver(UNICODE)}; Server=localhost;Port=5432;Database=dreamcore_horror_game;UID=postgres;PWD=root";
        private string scaffoldCommand = "Scaffold-DbContext \"Host=localhost;Port=5432;Database=dreamcore_horror_game;Username=postgres;Password=root\" (Provider: Npgsql.EntityFrameworkCore.PostgreSQL)";
    }
}
