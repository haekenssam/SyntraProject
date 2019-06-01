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
                                     SET Totaalprijs = @totaaltebetalen where ID = @id
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
    }
}