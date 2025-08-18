namespace DataCommunication
{
    public class EnvironmentVariable
    {
        public static string environment = "Dev";
        public static string url()
        {
            if (environment.Contains("Dev"))
                return "https://localhost:44316/";
            else if (environment == "Test")
                return "https://localhost:44316/";
            else if (environment == "Prod")
                return "";
            else
                return "";
        }

        public static string DBConnection()
        {
            if (environment.Contains("Dev"))
                return "Server=192.168.7.41\\sql2012;Initial Catalog=delete3;User ID=atul;Password=cis1234;TrustServerCertificate=True;command Timeout=600;";
            else if (environment == "Test")
                return "Server=192.168.7.41\\sql2012;Initial Catalog=delete3;User ID=atul;Password=cis1234;TrustServerCertificate=True;command Timeout=600;";
            else if (environment == "Prod")
                return "";
            else
                return "";
        }

        public static string MongoConnection()
        {
            if (environment.Contains("Dev"))
                return "mongodb+srv://vineet:XC6VbZl9w9ftSljm@mediastorage.pm45wuj.mongodb.net/";
            else if (environment == "Test")
                return "mongodb+srv://vineet:XC6VbZl9w9ftSljm@mediastorage.pm45wuj.mongodb.net/";
            else if (environment == "Prod")
                return "";
            else
                return "";
        }
    }
}
