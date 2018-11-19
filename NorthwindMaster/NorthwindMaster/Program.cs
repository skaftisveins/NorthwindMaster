using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace NorthwindMaster
{
    class Program
    {
        private static readonly string _connectionString = "Data Source=(local);Database=Northwind;Integrated Security=True";

        static void Main(string[] args)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            { 
                ListProducts(conn);

                Console.WriteLine("Select ProductId ");
                int productId = int.Parse(Console.ReadLine());
                ListProduct(conn, productId);

                Console.WriteLine("New Price:");
                double unitPrice = double.Parse(Console.ReadLine().Trim().Replace(',', '.'));
                UpdatePrice(conn, productId, unitPrice);
                ListProduct(conn, productId);
                Console.WriteLine("Price History:");
                PriceHistory(conn, productId);
            }
            Console.ReadKey();
        }

        private static void PriceHistory(SqlConnection conn, int productId)
        {
            SqlCommand cmd = new SqlCommand($"SELECT * FROM PriceHistory WHERE ProductID = {productId}", conn);
            UsingCMD(cmd, () =>
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["whenChanged"]} - {reader["oldUnitPrice"]} - {reader["newUnitPrice"]}");
                    }
                }
            });
        }

        private static void UpdatePrice(SqlConnection conn, int productId, double unitPrice)
        {
            SqlCommand cmd = new SqlCommand($"UPDATE Products SET UnitPrice={unitPrice} WHERE ProductId={productId}", conn);
            UsingCMD(cmd, () =>
            {
                int writer = cmd.ExecuteNonQuery();
                Console.WriteLine(writer);
            });
        }

        private static void ListProducts(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Products", conn);
            UsingCMD(cmd, () =>
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["productId"]} - {reader["productName"]} - {reader["unitPrice"]}");
                    }
                }
            });
        }

        private static void ListProduct(SqlConnection conn, int ProductId)
        {
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Products WHERE productId = {ProductId}", conn);
            UsingCMD(cmd, () =>
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["productId"]} - {reader["productName"]} - {reader["unitPrice"]}");
                    }
                }
            });
        }

        public static void UsingCMD(SqlCommand cmd, Action action)
        {
            try
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }

                cmd.Connection.Open();
                action.Invoke();
                cmd.Connection.Close();
            }
            finally
            {
                cmd?.Dispose();
            }
        }
    }
}
