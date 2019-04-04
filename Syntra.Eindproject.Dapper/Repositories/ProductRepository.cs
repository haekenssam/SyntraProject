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
                        SELECT Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock FROM PRODUCT ");
            }
        }

        public IEnumerable<Product> GetBadProduct()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Product>(@"select Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock FROM PRODUCT where VervalDatum < Getdate() ");
            }
        }

        public void InsertProduct(Product product)
        {
            using(SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into product (Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock)
                                    values (@Id, @Naam, @Soort, @Oorsprong, @Prijs, @Eenheid, @AanmaakDatum, @VervalDatum, @Stock) ",
                                    new
                                    {
                                        Id = product.Id,
                                        Naam = product.Naam,
                                        Soort = product.Soort,
                                        Oorsprong = product.Oorsprong,
                                        Prijs = product.Prijs,
                                        Eenheid = product.Eenheid,
                                        AanmaakDatum = product.AanmaakDatum,
                                        VervalDatum = product.VervalDatum,
                                        Stock = product.Stock
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
                                     Eenheid = @Eenheid, AanmaakDatum = @AanmaakDatum, VervalDatum = @vervaldatum, Stock = @stock where Id = @id ",
                                     new
                                     {
                                         Id = product.Id,
                                         Naam = product.Naam,
                                         Soort = product.Soort,
                                         Oorsprong = product.Oorsprong,
                                         Prijs = product.Prijs,
                                         Eenheid = product.Eenheid,
                                         AanmaakDatum = product.AanmaakDatum,
                                         VervalDatum = product.VervalDatum,
                                         Stock = product.Stock
                                     });
            }
        }
    }
}
