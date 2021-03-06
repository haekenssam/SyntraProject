﻿using System;
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

            //if (!IsValidProduct(id))
            //{
            //    throw new BusinessException("Product ID bestaat al");
            //}

            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"update product set Naam = @naam, Soort = @soort, Oorsprong = @oorsprong, Prijs = @prijs, 
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
        //Controle of ProductId al bestaat 
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
        //Update van de stock in winkelwagen en kassierster
        public void UpdateStockProduct(double Aantal)
        {

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"UPDATE Product 
                                     SET  Product.Stock = (Product.Stock - @Aantal)
                                     FROM Product 
                                     INNER JOIN BestellingLijnen
                                     ON BestellingLijnen.ProductId = Product.id
                                     WHERE Product.id = BestellingLijnen.ProductId 
                                     AND (BestellingLijnen.BestellingId = (select top 1 id from Bestelling Order by id desc))",
                                     new
                                     {
                                         Aantal = Aantal
                                     });
            }
        }
        //Controle van de actuele stock 
        public double ControleStock(int ProductId, Double Aantal)
        {
            List<Product> products = GetProducts().ToList();


            if (ProductId.ToString() == string.Empty || ProductId == 0)
            {
                throw new BusinessException("Ongeldig product");
            }
            if (Aantal == 0)
            {
                throw new BusinessException("Geef een aantal in!");
            }
            if (Aantal < 0)
            {
                throw new BusinessException("Aantal kan niet negatief zijn.");
            }
            else
            {
                var stock = (from i in products
                             where i.Id == ProductId
                             select i.Stock).Single();
                if (stock == 0)
                {
                    throw new BusinessException("Geen stock meer");
                }
                if (Aantal > stock)
                {
                    Aantal = stock;
                }
            }

            return Aantal;
        }
        //Als lijnen verwijderd worden wordt de stock terug aangepast
        public void AddStock(int ProductId, double Aantal)
        {
            using(var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"UPDATE Product
                                     SET Product.Stock = (Product.Stock + @Aantal)
                                     WHERE Product.Id = @ProductId",
                                     new
                                     {
                                         Aantal = Aantal,
                                         ProductId = ProductId
                                     });
            }
        }
    }
}
