using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syntra.Eindproject.BL;
using Dapper;

namespace Syntra.Eindproject.Dapper
{
    public class ProductRepository
    {
        public IEnumerable<Product> GetProducts()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Product>(@"
                        SELECT Id, Soort, Naam, Oorsprong, Prijs, Eenheid FROM PRODUCT ");
            }
        }

        public void InsertProduct(Product product)
        {
            using(SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into product (Naam, Soort, Oorsprong)
                                    values ( @Naam, @Soort, @Oorsprong) ",
                                    new
                                    {
                                        Naam = product.Naam,
                                        Soort = product.Soort,
                                        Oorsprong = product.Oorsprong
                                    });
                    
            }
        }
    }
}
