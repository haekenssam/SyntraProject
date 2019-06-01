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
    public class BestellingRepository : ProductRepository
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
                    @"insert into BestellingLijnen(BestellingId, ProductId, Aantal, Eenheid, Prijs, Bedrag)
                                        values((select top 1 Id from Bestelling order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
                                        (select Prijs  from Product where id = @productid), (select Cast(((100-Korting)/100.00) As Decimal(7,2)) As Korting From Product where id = @productid) * (@aantal)*(select Prijs from Product where id = @productid))",
                    new
                    {
                        ProductId = productid,
                        Aantal = aantal,

                    });
            }
        }

        //Controle productid --> wordt bij Insertbestellinglijn() opgeroepen.
        //Deze moet naar ProductRepository!!!!
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
                return connection.Query<Bestelling>(
                    @"SELECT BestellingLijnen.ID AS LijnId, Product.Naam, Product.Korting, BestellingLijnen.ProductId, 
	                                                    BestellingLijnen.Aantal, Product.Prijs, Product.Eenheid, BestellingLijnen.Bedrag 
	                                                    FROM BestellingLijnen 
	                                                    INNER JOIN Product 
	                                                    on Product.Id = BestellingLijnen.ProductId
	                                                    Where BestellingId = (select top 1 Id from Bestelling order by id desc)");

            }
        }

        public Bestelling GetBestellingId()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                //QueryFirstOrDefault
                return connection.QueryFirst<Bestelling>(
                    @"select top 1 Id from Bestelling order by id desc ");
            }

        }

        public Bestelling GetTotaalTeBetalen()
        {
            //IEnumerable<Bestelling> getBestellingId = GetBestellingId();
            //IEnumerable<Bestelling> getBestellingLijnenId = GetBestellingLijnenId();

            //getBestellingId.First().ToString();
            //getBestellingLijnenId.First().ToString();

            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Bestelling>(
                    @"select Round(Sum(Bedrag),2)  As Totaal
                        from BestellingLijnen
                        Where BestellingId = (select top 1 Id from Bestelling order by id desc) ");
            }


        }

        public Bestelling GetBestellingLijnenId()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Bestelling>(
                    @"select top 1 BestellingId from BestellingLijnen order by id desc ");
            }

        }

        public void DeleteBestellingLijn(int bestellingLijnenId, int productId, float aantal)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(
                    @"delete from BestellingLijnen where (ID = @bestellingLijnenId AND @ProductId = @productid AND Aantal = @aantal) ",
                    new
                    {
                        BestellingLijnenId = bestellingLijnenId,
                        ProductId = productId,
                        Aantal = aantal

                    });
            }
        }

        public void InsertBetaling(float totaalTeBetalen, float betaald, float terugBetalen) //ID van bestelling toevoegen als parameter?
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into Betalingen (BestellingId, Totaal, Betaald, Terug)
                                        values((select top 1 Id from Bestelling order by id desc), @totaalTeBetalen, @betaald, @terugBetalen)",
                    new
                    {
                        TotaalTeBetalen = totaalTeBetalen,
                        Betaald = betaald,
                        TerugBetalen = terugBetalen

                    });
            }
        }

        public IEnumerable<Bestelling> GetBestellingLijnenKassaTicket(int bestellingid)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<Bestelling>(
                    @"select BestellingLijnen.ProductId, CONCAT(Sum(BestellingLijnen.Aantal),' ', Product.Eenheid) AS Hoevl, CONCAT(BestellingLijnen.Prijs,' €/',Product.Eenheid) AS EenheidsPrijs,
                                                        Product.Naam, CONCAT(Sum(BestellingLijnen.Bedrag),' €')  AS Som, CONCAT (Product.Korting, ' %') AS KortingPercentage
                                                        From BestellingLijnen 
                                                        Inner Join Product On Product.id = BestellingLijnen.ProductId
                                                        Where BestellingId = @bestellingid
	                                                    Group by BestellingLijnen.ProductId, BestellingLijnen.Prijs, Product.Naam, Product.Eenheid, Product.Korting ",
                    new
                    {
                        BestellingId = bestellingid
                    }
                );

            }
        }

        public Bestelling GetBetalingenTotaalTeBetalen()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Bestelling>(
                    @"select Totaal  From Betalingen
                    Where BestellingId = (select top 1 BestellingId from Betalingen order by BestellingId desc) ");
            }

        }

        public Bestelling GetBetalingenBetaald()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Bestelling>(
                    @"select Betaald From Betalingen 
                    Where BestellingId = (select top 1 BestellingId from Betalingen order by BestellingId desc) ");
            }

        }

        public Bestelling GetBetalingenTerug()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.QueryFirst<Bestelling>(
                    @"select Terug  From Betalingen
                    Where BestellingId = (select top 1 BestellingId from Betalingen order by BestellingId desc) ");
            }

        }

        public float TerugBetalenBedrag(float betaald, float totaalTeBetalen)
        {
            float bedrag = 0;
            if (betaald < totaalTeBetalen)
            {
                throw new BusinessException("Betaald bedrag kan niet kleiner zijn dan het te betalen bedrag");
            }

            if (betaald > totaalTeBetalen || betaald == totaalTeBetalen)
            {
                bedrag = betaald - totaalTeBetalen;
            }
            Math.Round(bedrag, 2);
            return bedrag;

        }

    }
}
