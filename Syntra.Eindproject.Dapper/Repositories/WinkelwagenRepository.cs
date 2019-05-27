using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syntra.Eindproject.BL;
using Syntra.Eindproject.BL.Models;

namespace Syntra.Eindproject.Dapper.Repositories
{
    public class WinkelwagenRepository : ProductRepository
    {

        // Winkelwagen nummer maken
        public void InsertWinkelwagen()
        {

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"INSERT INTO Winkelwagen (Totaalprijs, Aanmaakdatum)
                                        values(0, GetDate())");
            }
        }
        // Winkelwagen Lijn toevoegen
        public void InsertWinkelwagenLijnen(int productid, float aantal)
        {
                using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
                {
                    connection.Execute(@"insert into WinkelwagenLijnen(WinkelwagenId, ProductId, Aantal, Eenheid, Prijs, Korting,Bedrag)
                                        values((select top 1 Id from Winkelwagen order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
                                        (select Prijs  from Product where id = @productid), (select Product.Korting from Product where id = @productid),(select Cast(((100-Korting)/100.00) As Decimal(7,2)) As Korting From Product where id = @productid)*(select Prijs from Product where id = @productid))",
                        new
                        {
                            ProductId = productid,
                            Aantal = aantal,

                        });

                }
        }

        #region vorigeinser
       //public void InsertWinkelwagenLijnen(int productid, float aantal)
       //{
       //    using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
       //    {
       //        connection.Execute(@"insert into WinkelwagenLijnen(WinkelwagenId, ProductId, Aantal, Eenheid, Prijs, Korting)
       //                                values((select top 1 Id from Winkelwagen order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
       //                                (select Prijs  from Product where id = @productid), (select Product.Korting from Product where id = @productid))",
       //            new
       //            {
       //                ProductId = productid,
       //                Aantal = aantal,
       //
       //            });
       //
       //    }
       //}
       //

        #endregion

        #region LijnenId
        //public Winkelwagen GetWinkelwagenLijnenId()
        //{
        //    using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //        return connection.QueryFirst<Winkelwagen>(
        //            @"select top 1 WinkelwagenId from WinkelwagenLijnen order by ID desc ");
        //    }
        //
        //}
        //
        #endregion
        // IEnumerable die instaat voor de display voor de winkelwagen lijst
        public IEnumerable<Winkelwagen> GetWinkelwagenLijnen()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Winkelwagen>(
                    @"SELECT WinkelwagenId, WinkelwagenLijnen.ID, ProductId, Aantal, WinkelwagenLijnen.Prijs, WinkelwagenLijnen.Eenheid , Product.Naam, Product.Soort, Product.VervalDatum , (WinkelwagenLijnen.Prijs * WinkelwagenLijnen.Aantal) as SubTotaal , Product.Korting, WinkelwagenLijnen.Bedrag
                        FROM WinkelwagenLijnen INNER JOIN Product on Product.Id = WinkelwagenLijnen.ProductId
                        Where WinkelwagenId = (select  MAX(WinkelwagenLijnen.WinkelwagenId) from WinkelwagenLijnen) ");
            }

            #region tekst
            //using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            //{
            //    return connection.Query<Winkelwagen>(
            //        @"SELECT WinkelwagenId, WinkelwagenLijnen.ID, ProductId, Aantal, WinkelwagenLijnen.Prijs, WinkelwagenLijnen.Eenheid 
            //            FROM WinkelwagenLijnen 
            //            Where WinkelwagenId = (select  MAX(WinkelwagenLijnen.WinkelwagenId) from WinkelwagenLijnen) ");
            //}
            //
            #endregion

        }

        // IEnumerable die instaat voor de display voor de totaal te betalen som
        public IEnumerable<Winkelwagen> GetWinkelwagenLijnen2()
        {

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Winkelwagen>(
                    @"Select SUM(Bedrag*Aantal) as TotaalPrijsLijst from WinkelwagenLijnen 
                  where (WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc)) ");
            }

        }

        // winkelwagen lijn verwijderen
        public void DeleteWinkelwagenLijnen(int WinkelwagenLijnenid)
        {

            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"delete from WinkelwagenLijnen where (ID = @iD AND WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc))",
                                    new
                                    {
                                        ID = WinkelwagenLijnenid,
                                    });
            }

        }
        // Updaten van winkelwagen lijn
        public void UpdateWinkelwagenLijnen(int IDWinkelwagenLijn, float AantalProdWinkelwagen)
        {

            if (IDWinkelwagenLijn.ToString() == string.Empty || AantalProdWinkelwagen.ToString() == string.Empty)
            {
                throw new BusinessException("Niet alle velden zijn ingevuld!");
            }

            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"update WinkelwagenLijnen set  Aantal = @aantal 
                where WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc) AND ID = @iD",
                                     new
                                     {
                                         Aantal = AantalProdWinkelwagen,
                                         ID = IDWinkelwagenLijn,
                                     });

            }

            #region tekst
            // if (naam == string.Empty || soort == string.Empty || prijs.ToString() == string.Empty || eenheid == string.Empty)
            // {
            //     throw new BusinessException("Niet alle velden zijn ingevuld!");
            // }
            //
            // DateTime vervalDatum;
            //
            // if (!DateTime.TryParse(vervaldatum, out vervalDatum))
            // {
            //     throw new BusinessException("Geen geldige datum");
            // }
            //
            #endregion
        }

        #region continue
        public bool CompareAantal(float AantalWinkelwagen, float AantalProduct, int productId)
        {
            bool Isvalid = true;
       
            if (AantalWinkelwagen > AantalProduct)
            {
                Isvalid = false;
            }
            return Isvalid;
       
        }
       
        public float CompareAantal2(float AantalWinkelwagen, float AantalProduct, int productId)
        {
       
            if (AantalWinkelwagen > AantalProduct)
            {
                AantalWinkelwagen = AantalProduct;
            }
            return AantalWinkelwagen;
       
        }

        public IEnumerable<Winkelwagen> CountAantalWinkelwagenPerProductId(int WinkelwagenProducId)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
              return  connection.Query<Winkelwagen>(@"Select SUM(Aantal) as totaalAantalPerProduct from WinkelwagenLijnen 
                                    where (ProductId = @productId AND WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc))",
                                    new
                                    {
                                        ProductId = WinkelwagenProducId,
                                    });

            }

        }

        public IEnumerable<Winkelwagen>  CountAantalProductPerProductId(int WinkelwagenProducId)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
               return  connection.Query<Winkelwagen>(@"Select Product.Stock from Product where (id = @id )",
                                    new
                                    {
                                        id = WinkelwagenProducId,
                                    });
       
            }
        }
        #endregion

        //Controleren of Product Id in het magazijn te vinden is tijdens het scannen
        public IEnumerable<int?> CheckProductIdAvailable(int WinkelwagenProductId)
         {
             using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
             {
                 return connection.Query<int?>(@"Select id from Product where (id = @id )",
                                      new
                                      {
                                          id = WinkelwagenProductId,
                                      });
             }
         }

        //Controleren of Product Id in het magazijn te vinden is tijdens de update van de lijn
        public IEnumerable<float?> CheckProductIdAvailableUpdate(int WinkelwagenLijnID)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<float?>(@"Select distinct Product.Stock 
from Product inner join  WinkelwagenLijnen on Product.Id = WinkelwagenLijnen.ProductId
where (Product.id = (select  WinkelwagenLijnen.ProductId from WinkelwagenLijnen Where WinkelwagenLijnen.ID = @id) )",
                                     new
                                     {
                                         id = WinkelwagenLijnID,
                                     });
            }
        }

        // Controleren hoeveel items er zich in de winkelwagen bevinden van productId
        public IEnumerable<float?> CheckAantalProductIdAvailableWinkelwagenUpdate(int WinkelwagenLijnID)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<float?>(@"select SUM( WinkelwagenLijnen.Aantal) from WinkelwagenLijnen Where WinkelwagenLijnen.ID = @id ",
                                     new
                                     {
                                         id = WinkelwagenLijnID,
                                     });
            }
        }

        //Controleren hoeveel items van een bepaalde product Id in het Magazijn zijn
        public IEnumerable<float> CheckAantalInMagazijn(int WinkelwagenProductId)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<float>(@"Select Stock from Product where (id = @id )",
                                     new
                                     {
                                         id = WinkelwagenProductId,
                                     });
            }
        }

        //Controleren hoeveel items van een bepaalde product Id in de winkelwagen zijn
        public IEnumerable<float?> CheckAantalInWinkelwagen(int WinkelwagenProductId)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<float?>(@"Select SUM(Aantal) from WinkelwagenLijnen where (Productid = @id ) AND WinkelwagenId = (select  MAX(WinkelwagenLijnen.WinkelwagenId) from WinkelwagenLijnen)",
                                     new
                                     {
                                         id = WinkelwagenProductId,
                                     });
            }
        }

        #region CountAantalWinkelwagenPerProductId2
       // public Winkelwagen CountAantalWinkelwagenPerProductId2(int WinkelwagenProducId)
       // {
       //     using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
       //     {
       //         return connection.QueryFirst(@"Select SUM(Aantal) as totaalAantalPerProduct from WinkelwagenLijnen 
       //                             where (ProductId = @productId AND WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc))",
       //                               new
       //                               {
       //                                   ProductId = WinkelwagenProducId,
       //                               });
       //     }
       //
       // }
       //
        #endregion


        //public float CountAantalProductPerProductId2(int WinkelwagenProducId)
        //{
        //    using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //        return connection.QueryFirst(@"Select Product.Stock from Product where (id = @id )",
        //                             new
        //                             {
        //                                 id = WinkelwagenProducId,
        //                             });

        //    }
        //}


        #region tekst
        //  public void TotaalTeBetalen()
        //  {
        //      using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
        //      {
        //          connection.Execute(@"Select SUM(Prijs*Aantal)  from WinkelwagenLijnen 
        //          where (WinkelwagenId = (select top 1 WinkelwagenId from WinkelwagenLijnen order by WinkelwagenId desc))");
        //
        //      }
        //
        //  }
        //

        #endregion


    }
}
