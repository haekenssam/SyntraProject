using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syntra.Eindproject.BL.Models;

namespace Syntra.Eindproject.Dapper.Repositories
{
    public class GebruikerRepository
    {
        public IEnumerable<User>  GetGebruikers()
        {
            using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
            {
                return connection.Query<User>(@"SELECT ID, Gebruiker, Paswoord FROM Gebruikers");
            }
        }

        public bool IsUserValid(string gebruiker, string paswoord)
        {
            List<User> users = GetGebruikers().ToList();
            bool test = true;

            var q = from u in users
                where u.Gebruiker == gebruiker && u.Paswoord == paswoord
                select u;

            if (q.Any())
            {
                test = true;

            }
            else
            {
                test = false;
            }

            return test;

        }
        //public bool IsValid(string gebruiker, string paswoord)
        //{
        //    using (var connection = new SqlConnection(Connection.Instance.ConnectionString))
        //    {
        //       bool IsValid = true;

        //       int Count = connection.QuerySingle<int>(
        //            @"SELECT COUNT(ID) FROM Gebruikers where Gebruiker = @gebruiker and Paswoord = @paswoord",
        //            new
        //            {
        //                Gebruiker = gebruiker,
        //                Paswoord = paswoord
        //            });

        //       if (Count != 1)
        //       {
        //           IsValid = false;
        //       }

        //       return IsValid;
        //    }
        //} 
    }
}
