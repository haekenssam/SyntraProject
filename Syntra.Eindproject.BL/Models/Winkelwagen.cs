using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.BL.Models
{
    public class Winkelwagen
    {

        public int Id { get; set; }
        public int WinkelwagenId { get; set; }
        public int ProductId { get; set; }
        public float Aantal { get; set; }
        public float Prijs { get; set; }
        public string Eenheid { get; set; }
        public string Naam { get; set; }
        public string Soort { get; set; }
        public string VervalDatum { get; set; }
        public float Subtotaal { get; set; }
        public float Totaal { get; set; }
        public decimal Korting { get; set; }
        public decimal Bedrag { get; set; }
        public int WinkelwagenLijnenId { get; set; }
        public int WinkelwagenNr { get; set; }
        public int LijnId { get; set; }
        public float TotaalTeBetalen { get; set; }

        //constructor
        public Winkelwagen()
        { }

    }
}
