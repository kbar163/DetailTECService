using Microsoft.Data.SqlClient;

namespace DetailTECService.Coms
{
    public class Builder
    {
        public static string ConnectionString()
        {
            var connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = @"DESKTOP-0CULEUE\DETAILDB";
            connectionString.InitialCatalog = "DetailTECDB";
            connectionString.UserID = "testuser";
            connectionString.Password = "testpassword";
            Console.WriteLine(connectionString.ToString());
            return connectionString.ToString();
        }
    }
}
