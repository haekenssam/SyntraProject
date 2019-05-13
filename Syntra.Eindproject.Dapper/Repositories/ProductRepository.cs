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

        public void InsertProduct(int id, string naam, string soort, string oorsprong, double prijs, string eenheid, string vervaldatum, double stock)
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

            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into product (Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock)
                                    values (@Id, @Naam, @Soort, @Oorsprong, @Prijs, @Eenheid, GetDate(), @vervalDatum, @Stock) ",
                                    new
                                    {
                                        Id = id,
                                        Naam = naam,
                                        Soort = soort,
                                        Oorsprong = oorsprong,
                                        Prijs = prijs,
                                        Eenheid = eenheid,
                                        VervalDatum = vervalDatum,
                                        Stock = stock
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

        public void UpdateProduct(int id, string naam, string soort, string oorsprong, double prijs, string eenheid, string vervaldatum, double stock)
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

            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"update product set Id = @id, Naam = @naam, Soort = @soort, Oorsprong = @oorsprong, Prijs = @prijs, 
                                     Eenheid = @Eenheid, VervalDatum = @vervalDatum, Stock = @stock where Id = @id ",
                                     new
                                     {
                                         Id = id,
                                         Naam = naam,
                                         Soort = soort,
                                         Oorsprong = oorsprong,
                                         Prijs = prijs,
                                         Eenheid = eenheid,
                                         VervalDatum = vervalDatum,
                                         Stock = stock
                                     });
                
            }
        }



        //public void GetProductBestelling(int id)
        //{
        //    DateTime vervalDatum;

        //    if (!DateTime.TryParse(vervaldatum, out vervalDatum))
        //    {
        //        throw new BusinessException("Geen geldige datum");
        //    }

        //    using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //        connection.Execute(@"insert into product (Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock)
        //                            values (@Id, @Naam, @Soort, @Oorsprong, @Prijs, @Eenheid, GetDate(), @vervalDatum, @Stock) ",
        //                            new
        //                            {
        //                                Id = id,
        //                                Naam = naam,
        //                                Soort = soort,
        //                                Oorsprong = oorsprong,
        //                                Prijs = prijs,
        //                                Eenheid = eenheid,
        //                                VervalDatum = vervalDatum,
        //                                Stock = stock
        //                            });
        //        connection.Execute(@"update product set Id = @id, Naam = @naam, Soort = @soort, Oorsprong = @oorsprong, Prijs = @prijs, 
        //                             Eenheid = @Eenheid, VervalDatum = @vervalDatum, Stock = @stock where Id = @id ",
        //             new
        //             {
        //                 Id = id,
        //                 Naam = naam,
        //                 Soort = soort,
        //                 Oorsprong = oorsprong,
        //                 Prijs = prijs,
        //                 Eenheid = eenheid,
        //                 VervalDatum = vervalDatum,
        //                 Stock = stock
        //             });

        //        return connection.Query<Product>(@"select Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock FROM PRODUCT where VervalDatum < Getdate() AND Id = @id ");
        //        return connection.Query<Product>(@"select Id, Naam, Soort, Oorsprong, Prijs, Eenheid, AanmaakDatum, VervalDatum, Stock FROM PRODUCT where VervalDatum < Getdate() AND Id = @id ");

        //public void InsertBetaling(int id )
        //{
        //    using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //        connection.Execute(@"insert into Betalingen (BestellingId, Totaal, Betaald, Terug)
        //                                values((select top 1 Id from Bestelling order by id desc))",
        //            new
        //            {
        //                Id = totaalTeBetalen
        //            });
        //    }
        //}
        //
        //    }
        //}

    }
}
