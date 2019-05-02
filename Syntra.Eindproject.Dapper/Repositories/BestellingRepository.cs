using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syntra.Eindproject.BL;

namespace Syntra.Eindproject.Dapper.Repositories
{
    public class BestellingRepository
    {
        public void InsertBestelling()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"INSERT INTO Bestelling (Totaal, Datum)
                                        values(0, GetDate())");
            }
        }

        public void InsertBestellingLijn(int productid, float aantal)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into BestellingLijnen(BestellingId, ProductId, Aantal, Eenheid, Prijs, Bedrag)
                                        values((select top 1 Id from Bestelling order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
                                        (select Prijs  from Product where id = @productid),(@aantal)*(select Prijs  from Product where id = @productid))",
                    new
                    {
                        ProductId = productid,
                        Aantal = aantal,

                    });
            }
        }

        public IEnumerable<Bestelling> GetBestelling()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Bestelling>(
                    @"select prod.Naam, bestl.Aantal, Prod.Eenheid, prod.Prijs, (prod.Prijs * bestl.aantal) as Totaal from Bestelling best
                                        left join BestellingLijnen bestl on bestl.BestellingId = best.Id
                                        left join Product prod on prod.id = bestl.ProductId
                                        where best.Id = (select top 1 Id from Bestelling order by id desc)");
            }
        }

        public IEnumerable<Bestelling> GetBestellingLijnen()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Bestelling>(@"SELECT BestellingLijnen.BestellingId, Product.Naam, BestellingLijnen.ProductId, 
	                                                    BestellingLijnen.Aantal, Product.Prijs, Product.Eenheid, BestellingLijnen.Bedrag 
	                                                    FROM BestellingLijnen 
	                                                    INNER JOIN Product 
	                                                    on Product.Id = BestellingLijnen.ProductId
	                                                    Where BestellingId = (select top 1 Id from Bestelling order by id desc)");

            }
        }

        public IEnumerable<string> GetBestellingId()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<string>(
                    @"select top 1 Id from Bestelling order by id desc ");
            }


        }

        public IEnumerable<double> GetTeBetalenBedrag()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<double>(
                    @"select Sum(Bedrag) As Totaal
                      from BestellingLijnen
                       Where BestellingId = (select top 1 BestellingId from BestellingLijnen order by ID desc) ");
            }


        }
    }
}
