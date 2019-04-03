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
                        SELECT Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum FROM PRODUCT ");
            }
        }

        public void InsertProduct(Product product)
        {
            using(SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into product (Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum)
                                    values (@Id, @Naam, @Soort, @Oorsprong, @Prijs, @Eenheid, @AanmaakDatum, @VervalDatum) ",
                                    new
                                    {
                                        Id = product.Id,
                                        Naam = product.Naam,
                                        Soort = product.Soort,
                                        Oorsprong = product.Oorsprong,
                                        Prijs = product.Prijs,
                                        Eenheid = product.Eenheid,
                                        AanmaakDatum = product.AanmaakDatum,
                                        VervalDatum = product.VervalDatum
                                    });
    
            }
        }

        public void DeleteProduct(int id, string naam)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"delete from product where Id = @id or Naam = @naam ",
                                    new
                                    {
                                        Id = id,
                                        Naam = naam
                                    });
            }
        }

        public void UpdateProduct(Product product)
        {
            using(SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"update product set Id = @id, Naam = @naam, Soort = @soort, Oorsprong = @oorsprong, Prijs = @prijs, 
                                     Eenheid = @Eenheid, AanmaakDatum = @AanmaakDatum, VervalDatum = @vervaldatum where Id = @id ",
                                     new
                                     {
                                         Id = product.Id,
                                         Naam = product.Naam,
                                         Soort = product.Soort,
                                         Oorsprong = product.Oorsprong,
                                         Prijs = product.Prijs,
                                         Eenheid = product.Eenheid,
                                         AanmaakDatum = product.AanmaakDatum,
                                         VervalDatum = product.VervalDatum
                                     });
            }
        }
    }
}
