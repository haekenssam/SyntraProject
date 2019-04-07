using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.BL
{
    public class Product
    {
        public int Id { get; set; }
        public string Soort { get; set; }
        public string Naam { get; set; }
        public string Oorsprong { get; set; }
        public string Eenheid { get; set; }
        public double Prijs { get; set; }
        public DateTime AanmaakDatum { get; set; }
        public DateTime VervalDatum { get; set; }

        public double Stock { get; set; }

        public Product(int id, string naam, string soort, string oorsprong, double prijs, string eenheid, DateTime vervaldatum, double stock)
        {
            Id = id;
            Naam = naam;
            Soort = soort;
            Oorsprong = oorsprong;
            Prijs = prijs;
            Eenheid = eenheid;
            AanmaakDatum = aanmaakdatum;
            VervalDatum = vervaldatum;
            Stock = stock;

        }

        //constructor
        public Product()
        { }
    }
}
