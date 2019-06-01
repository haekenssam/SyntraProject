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

        // Winkelwagennummer aanmaken (OK)
        public void InsertWinkelwagen()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"INSERT INTO Winkelwagen (Totaalprijs, Aanmaakdatum)
                                        values(0, GetDate())");
            }
        }
        
        //WinkelwagenId halen (OK)
        public Winkelwagen GetWinkelWagenId()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                //QueryFirstOrDefault
                return connection.QueryFirst<Winkelwagen>(
                    @"select top 1 ID from Winkelwagen order by ID desc ");
            }

        }

        //Een nieuwe WinkelwagenLijn toevoegen (OK)
        public void InsertWinkelwagenLijnen(int productid, float aantal)
        {
            if (string.IsNullOrEmpty(productid.ToString()))
            {
                throw new BusinessException("Ongeldig product");
            }

            if (!IsValidProduct(productid))
            {
                throw new BusinessException("Ongeldig product");
            }

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(
                    @"insert into WinkelwagenLijnen(WinkelwagenId, ProductId, Aantal, Eenheid, Prijs, Bedrag)
                                        values((select top 1 Id from Winkelwagen order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
                                        (select Prijs  from Product where id = @productid), (select Cast(((100-Korting)/100.00) As Decimal(7,2)) As Korting From Product where id = @productid) * (@aantal)*(select Prijs from Product where id = @productid))",
                    new
                    {
                        ProductId = productid,
                        Aantal = aantal,

                    });
            }
        }

        //WinkelwagenLijnen (OK) 
        public IEnumerable<Winkelwagen> GetWinkelwagenLijnen()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Winkelwagen>(
                    @"SELECT WinkelwagenLijnen.ID As LijnId, Product.Naam, Product.Korting, WinkelwagenLijnen.ProductId, 
	                                                    WinkelwagenLijnen.Aantal, Product.Prijs, Product.Eenheid, WinkelwagenLijnen.Bedrag 
	                                                    FROM WinkelwagenLijnen 
	                                                    INNER JOIN Product 
	                                                    on Product.Id = WinkelwagenLijnen.ProductId
	                                                    Where WinkelwagenLijnen.WinkelwagenId = (select top 1 ID from Winkelwagen order by ID desc)");

            }
        }

        //WinkelwagenLijnen2 (OK) 
        public IEnumerable<Winkelwagen> GetWinkelwagenLijnen2(int winkelwagenNr)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Winkelwagen>(
                    @"SELECT WinkelwagenLijnen.ID As LijnId,  Product.Naam, Product.Korting, WinkelwagenLijnen.ProductId, 
	                                                    WinkelwagenLijnen.Aantal, Product.Prijs, Product.Eenheid, WinkelwagenLijnen.Bedrag 
	                                                    FROM WinkelwagenLijnen 
	                                                    INNER JOIN Product 
	                                                    on Product.Id = WinkelwagenLijnen.ProductId
	                                                    Where WinkelwagenLijnen.WinkelwagenId = @winkelwagenNr",
                    new
                    {
                        WinkelwagenNr = winkelwagenNr
                    }
                        );

            }
        }

        //Totaal te betalen (OK)
        public Winkelwagen GetTotaalTeBetalen()
        {

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Winkelwagen>(
                    @"select Round(Sum(Bedrag),2) As Totaal
                        from WinkelwagenLijnen
                        Where WinkelwagenId = (select top 1 ID from Winkelwagen order by id desc) ");
            }


        }

        //Delete WinkelwagenLijnen (OK)
        public void DeletewinkelwagenLijn(int winkelwagenLijnenId, int productId, float aantal)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(
                    @"delete from WinkelwagenLijnen where (ID = @winkelwagenLijnenId AND @ProductId = @productid AND Aantal = @aantal) ",
                    new
                    {
                        WinkelwagenLijnenId = winkelwagenLijnenId,
                        ProductId = productId,
                        Aantal = aantal

                    });
            }
        }

        //MOET NAAR Product Repository?
        public bool IsValidProduct(int productid)
        {
            List<Product> products = GetProducts().ToList();
            bool isValid = true;

            var q = from p in products
                    where p.Id == productid
                    select p;

            if (q.Any())
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        //TotaalTeBetalen (OK)
        public Winkelwagen GetTotaalTeBetalen2()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Winkelwagen>(
                    @"select Round(Sum(Bedrag),2)  As Totaal
                        from WinkelwagenLijnen
                        Where WinkelwagenId = (select top 1 Id from Winkelwagen order by id desc) ");
            }


        }

        //Insert totaal te betalen bedrag in Totaalprijs in tabel Winkelwagen (OK)
        public void UpdateTotaalWinkelWagen(int id, float totaalTeBetalen)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"UPDATE Winkelwagen
                                     SET Totaalprijs
 = @totaaltebetalen where ID = @id
                                     ",
                                 new
                                 {
                                     TotaalTeBetalen = totaalTeBetalen,
                                     Id = id
                                 }
                                     );
            }
        }

        // (OK)
        public void DeleteEmptyWinkelwagenLijnen()
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"DELETE WinkelwagenLijnen FROM WinkelwagenLijnen 
                                     INNER JOIN Winkelwagen 
                                     ON WinkelwagenLijnen.WinkelwagenId = Winkelwagen.ID
                                     WHERE Winkelwagen.Totaalprijs = 0 ");
            }
        }

        // (OK)
        public void DeleteEmptyWinkelwagen()
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"DELETE Winkelwagen FROM Winkelwagen WHERE Totaalprijs = 0");
            }
        }


        //******************************************************************************************************************************

        //Controleren of Product Id in het magazijn te vinden is tijdens het scannen
        #region CheckProductIdAvailable
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
        #endregion
        //Controleren of Product Id in het magazijn te vinden is tijdens de update van de lijn
        #region CheckProductIdAvailableUpdate
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
        #endregion
        // Controleren hoeveel items er zich in de winkelwagen bevinden van productId
        #region CheckAantalProductIdAvailableWinkelwagenUpdate
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
        #endregion
        //Controleren hoeveel items van een bepaalde product Id in het Magazijn zijn
        #region CheckAantalInMagazijn

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
        #endregion
        //Controleren hoeveel items van een bepaalde product Id in de winkelwagen zijn
        #region CheckAantalInWinkelwagen
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

        #endregion

        // Controleren of er voldoende stock is bij de Update van een Lijn
        public bool EnoughStockUpdate(int winkelwagenLijnId, float aantalInWinkelwagen)
        {
            List<float?> checkAvailableAantalMagazijn = CheckProductIdAvailableUpdate(winkelwagenLijnId).ToList();
            List<float?> checkAvailableAantalWinkelwagenLijnen = CheckAantalProductIdAvailableWinkelwagenUpdate(winkelwagenLijnId).ToList();
            float? aantalMagazijn = checkAvailableAantalMagazijn[0];
            float? aantalWinkelwagenLijnen = checkAvailableAantalWinkelwagenLijnen[0];
            float? difference = aantalMagazijn - aantalWinkelwagenLijnen - aantalInWinkelwagen;
            if (difference < 0)
            {
                return false;
            }
            else { return true; }
        }
        // Controleren of er voldoende stock is bij de scan
        public bool EnoughStockScan(int productId, float aantalInWinkelwagen)
        {
            List<float> checkAvailableAantalMagazijn = CheckAantalInMagazijn(productId).ToList();
            List<float?> checkAvailableAantalWinkelwagenLijnen = CheckAantalInWinkelwagen(productId).ToList();
            float? aantalMagazijn = checkAvailableAantalMagazijn[0];
            float? aantalWinkelwagenLijnen = checkAvailableAantalWinkelwagenLijnen[0];
            float? difference = aantalMagazijn - aantalWinkelwagenLijnen - aantalInWinkelwagen;
            if (difference < 0)
            {
                return false;
            }
            else { return true; }
        }
        // Winkelwagen Lijn toevoegen
        //public void InsertWinkelwagenLijnen(int productid, float aantal)
        //{
        //    List<int?> checkAvailableProductId = CheckProductIdAvailable(productid).ToList();


        //    // Controleren of ingegeven productId in de lijst is
        //    if (checkAvailableProductId.Count == 0)
        //    {
        //        //MessageBox.Show("Gekozen product is niet beschikbaar");
        //        throw new BusinessException("Gekozen product is niet beschikbaar");
        //    }
        //    // Controleren of ingegeven productid of aantal 0 is
        //    if (productid.ToString() == (string.Empty) || aantal.ToString() == string.Empty)
        //    {
        //        throw new BusinessException("Niet alle velden zijn ingevuld!");
        //    }
        //    // Controleren of ingegeven hoeveeheid 0 is
        //    if (aantal == 0)
        //    {
        //        throw new BusinessException("'Aantal' mag niet 0 zijn");
        //    }
        //    // Controleren of ingegeven hoeveeheid negatief is
        //    if (aantal < 0)
        //    {
        //        throw new BusinessException("'Aantal' is niet positief");
        //    }

        //    //Controleren of er nog voldoende stock is van een bepaald product
        //        if (EnoughStockScan( productid,  aantal) == false)
        //        {
        //            // MessageBox.Show("Te weinig in voorraad");
        //            throw new BusinessException("Niet voldoende in voorraad");
        //        }

        //    using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //        connection.Execute(@"insert into WinkelwagenLijnen(WinkelwagenId, ProductId, Aantal, Eenheid, Prijs, Korting,Bedrag)
        //                                values((select top 1 Id from Winkelwagen order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
        //                                (select Prijs  from Product where id = @productid), (select Product.Korting from Product where id = @productid),(select Cast(((100-Korting)/100.00) As Decimal(7,2)) As Korting From Product where id = @productid)*(select Prijs from Product where id = @productid))",
        //            new
        //            {
        //                ProductId = productid,
        //                Aantal = aantal,

        //            });

        //    }
        //}

        public void UpdateWinkelwagenLijnen(int IDWinkelwagenLijn, float AantalProdWinkelwagen)
        {

            //Controleren of de velden zijn ingevuld
            if (IDWinkelwagenLijn.ToString() == string.Empty || AantalProdWinkelwagen.ToString() == string.Empty)
            {
                throw new BusinessException("Niet alle velden zijn ingevuld!");
            }
            // Controleren of aantal niet negatief is
            if (AantalProdWinkelwagen < 0)
            {
                throw new BusinessException("'Aantal' mag niet negatief zijn");
                //MessageBox.Show("Inhoud niet correct ingevuld");
            }
            //Controleren of aantal niet 0 is
            if (AantalProdWinkelwagen == 0)
            {
                throw new BusinessException("'Aantal' mag niet 0 zijn");
            }
            // Controleren of er genoeg stock is
            if (EnoughStockUpdate(IDWinkelwagenLijn, AantalProdWinkelwagen) == false)
            {
                //MessageBox.Show("Niet voldoende voorraad");
                throw new BusinessException("Niet voldoende voorraad");
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
        }
    }
}