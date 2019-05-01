﻿using System;
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

        public void InsertBestellingLijn(int productid, int aantal)
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                connection.Execute(@"insert into BestellingLijnen(BestellingId, ProductId, Aantal, Eenheid, Prijs)
                                        values((select top 1 Id from Bestelling order by id desc), @productid, @aantal,(select Eenheid from Product where id = @productid),
                                        (select Prijs  from Product where id = @productid))",
                    new
                    {
                        ProductId = productid,
                        Aantal = aantal

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
    }
}
