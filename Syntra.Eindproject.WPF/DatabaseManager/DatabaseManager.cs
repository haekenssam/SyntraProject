using Syntra.Eindproject.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syntra.Eindproject.Dapper.Repositories;

namespace Syntra.Eindproject.WPF
{
    public class DatabaseManager
    {
        private static readonly DatabaseManager _manager = new DatabaseManager();

        public static DatabaseManager Instance => _manager;

        public ProductRepository ProductRepository => new ProductRepository();
        public GebruikerRepository GebruikerRepository => new GebruikerRepository();
        public BestellingRepository BestellingRepository => new BestellingRepository();
        public WinkelwagenRepository WinkelwagenRepository => new WinkelwagenRepository();

        private DatabaseManager()
        {

            //Connection.Instance.SetConnection("server=FILIP\\FILIPDECKERS; database=Syntra; User id= sa; password=Desultan0");
            Connection.Instance.SetConnection("server=Eindproject; database=Syntra; User id= Syntra; password=Syntra123!");
        }
    }
}

