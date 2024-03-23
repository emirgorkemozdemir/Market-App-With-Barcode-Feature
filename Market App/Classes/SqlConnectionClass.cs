using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Market_Sistemi.Classes
{
   public class SqlConnectionClass
    {
        public static SqlConnection connect = new SqlConnection("Data Source=.;Initial Catalog=MarketDb;Integrated Security=True");

        public static void CheckConnection(SqlConnection tempConnection)
        {
            if (tempConnection.State==System.Data.ConnectionState.Closed)
            {
                tempConnection.Open();
            }
            else
            {

            }
        }
    }
}
