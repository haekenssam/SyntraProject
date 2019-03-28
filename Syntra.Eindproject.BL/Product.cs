using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.BL
{
    public class Product
    {
        public double Id { get; set; }
        public string Soort { get; set; }
        public string Naam { get; set; }
        public string Oorsprong { get; set; }
        public string Eenheid { get; set; }
        public double Prijs { get; set; }
        public DateTime AanmaakDatum { get; set; }
        public DateTime VervalDatum { get; set; }

        //constructor
        public Product()
        { }
    }
}
