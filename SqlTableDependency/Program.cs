using System;
using System.Configuration;
using System.Collections.Generic;
using TableDependency;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using TableDependency.Enums;
using System.Data.SqlClient;

namespace SqlTableDependency
{
    static class Program
    {
        static string _con = ConfigurationManager.ConnectionStrings["connectionString"].ToString();
        static void Main(string[] args)

        {
            var mapper = new ModelToTableMapper<Products>();
            mapper.AddMapping(c => c.Id, "Id");
            mapper.AddMapping(c => c.Name, "Name");

            using (var dep = new SqlTableDependency<Products>(_con, "Products", mapper))
            {
                dep.OnChanged += Changed;
                dep.Start();


                Console.ReadKey();

                dep.Stop();
            }
        }

        private static void Changed(object sender, RecordChangedEventArgs<Products> e)
        {
            if (e.ChangeType != ChangeType.None)
            {

                Console.WriteLine("\n================Events=============================\n");
                var changedEntity = e.Entity;
                Console.WriteLine("DML operation: " + e.ChangeType);
                Console.WriteLine("ID: " + changedEntity.Id);
                Console.WriteLine("Name: " + changedEntity.Name);
                Console.WriteLine("\n===================Result==========================\n");

                List<Products> lst = GetAllStocks();
                foreach (var item in lst)
                {
                    Console.WriteLine("----------------------------------------------\n");
                    Console.WriteLine("Id: " + item.Id);
                    Console.WriteLine("First Name: " + item.Name);
                    Console.WriteLine("Last Name: " + item.Price);

                }

                Console.WriteLine("\n Press a key to exit");
            }
        }

        public static List<Products> GetAllStocks()
        {
            List<Products> lstCustomer = new List<Products>();
            var connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM Products";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Id"));
                            var name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"));
                            var Surname = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Price"));

                            lstCustomer.Add(new Products { Id = Id, Name = name, Price = Surname });
                        }
                    }
                }
            }

            return lstCustomer;
        }
    }
}
