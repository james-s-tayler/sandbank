using System;

namespace Database
{
    public class DatabaseConnection
    {
        public string Password { get; set; }
        public string Dbname { get; set; }
        public string Engine { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }

        public string GetConnectionString()
        {
            switch (Engine)
            {
                case "postgres": return $"Host={Host};Port={Port};Database={Dbname};Username={Username};Password={Password}";
            }
            throw new NotImplementedException($"Only RDS Engine type 'postgres' supported, but was '{Engine}'");
        }
    }
}