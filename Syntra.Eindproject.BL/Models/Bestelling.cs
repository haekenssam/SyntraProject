using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.BL
{
    public class Bestelling : Product
    {
        public int Id { get; set; }
        public float Totaal { get; set; }
        public DateTime Datum { get; set; }
        public int BestellingId { get; set; }
        public int ProductId { get; set; }
        public float Aantal { get; set; }
        public float Bedrag { get; set; }
        public float TotaalTeBetalen { get; set; }
        public float Betaald { get; set; }
        public float TerugBetalen { get; set; }
        public int BestellingLijnenId { get; set; }
        public float Terug { get; set; }
        public string Som { get; set; }
        public string Hoevl { get; set; }
        public string EenheidsPrijs { get; set; }
        public string KortingPercentage { get; set; }
        public int LijnId { get; set; }
    }
}
