using System.Configuration;
using System.Data.SqlClient;

namespace SendMailChecador
{
    public static class DatabaseConnection
    {
        public static SqlConnection Create()
        {
            var cs = ConfigurationManager.ConnectionStrings["ConnectionString"]?.ConnectionString;
            return new SqlConnection(cs);
        }
    }
}