using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
                        SELECT Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock, Korting FROM PRODUCT ");
            }
        }

        public IEnumerable<Product> GetBadProduct()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Product>(@"select Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock, Korting FROM PRODUCT where VervalDatum < Getdate() ");
            }
        }

        public void InsertProduct(int id, string naam, string soort, string oorsprong, double prijs, string eenheid, string vervaldatum, double stock, int korting)
        {
            if (naam == string.Empty || soort == string.Empty || prijs.ToString() == string.Empty || eenheid == string.Empty)
            {
                throw new BusinessException("Niet alle velden zijn ingevuld!");
            }

            DateTime vervalDatum;

            if (!DateTime.TryParse(vervaldatum, out vervalDatum))
            {
                throw new BusinessException("Geen geldige datum");
            }

            if (!IsValidProduct(id))
            {
                throw new BusinessException("Product ID bestaat al");
            }
            
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into product (Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock, Korting)
                                    values (@Id, @Naam, @Soort, @Oorsprong, @Prijs, @Eenheid, GetDate(), @vervalDatum, @Stock, @Korting) ",
                                    new
                                    {
                                        Id = id,
                                        Naam = naam,
                                        Soort = soort,
                                        Oorsprong = oorsprong,
                                        Prijs = prijs,
                                        Eenheid = eenheid,
                                        VervalDatum = vervalDatum,
                                        Stock = stock,
                                        Korting = korting
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

        public void UpdateProduct(int id, string naam, string soort, string oorsprong, double prijs, string eenheid, string vervaldatum, double stock, int korting)
        {
            if (naam == string.Empty || soort == string.Empty || prijs.ToString() == string.Empty || eenheid == string.Empty )
            {
                throw new BusinessException("Niet alle velden zijn ingevuld!");
            }

            DateTime vervalDatum;

            if (!DateTime.TryParse(vervaldatum, out vervalDatum))
            {
                throw new BusinessException("Geen geldige datum");
            }

            if (!IsValidProduct(id))
            {
                throw new BusinessException("Product ID bestaat al");
            }
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"update product set Id = @id, Naam = @naam, Soort = @soort, Oorsprong = @oorsprong, Prijs = @prijs, 
                                     Eenheid = @Eenheid, VervalDatum = @vervalDatum, Stock = @stock, Korting = @korting where Id = @id ",
                                     new
                                     {
                                         Id = id,
                                         Naam = naam,
                                         Soort = soort,
                                         Oorsprong = oorsprong,
                                         Prijs = prijs,
                                         Eenheid = eenheid,
                                         VervalDatum = vervalDatum,
                                         Stock = stock,
                                         Korting = korting
                                     });
                
            }
        }

        public bool IsValidProduct(int id)
        {
           bool IsValidProduct = true;

           List<Product> products = GetProducts().ToList();

           bool t = products.Any(x => x.Id == id);

           if (t == true)
           {
               IsValidProduct = false;
           }

           return IsValidProduct;
        }

        public void UpdateStockProduct()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"UPDATE Product 
                                     SET  Product.Stock = (Product.Stock - BestellingLijnen.Aantal)
                                     FROM Product 
                                     INNER JOIN BestellingLijnen
                                     ON BestellingLijnen.ProductId = Product.id
                                     WHERE Product.id = BestellingLijnen.ProductId 
                                     AND (BestellingLijnen.BestellingId = (select top 1 id from Bestelling Order by id desc))");
            }
        }
    }
}
