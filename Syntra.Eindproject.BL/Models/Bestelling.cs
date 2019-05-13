using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.BL
{
    public class Bestelling : Product
    {
        public int ID { get; set; }
        public float Totaal { get; set; }
        public DateTime Datum { get; set; }
        public int BestellingId { get; set; }
        //public int BestellingId { get; set; }
        public int ProductId { get; set; }
        //public string ProductId2 { get; set; }
        //public int ProductId2 { get; set; }
        public float Aantal { get; set; }
        //public int Aantal { get; set; }
        //public string Aantal2 { get; set; }
        //public int Aantal2 { get; set; }
        public float Bedrag { get; set; }
        public float TotaalTeBetalen { get; set; }
        public float Betaald { get; set; }
        public float TerugBetalen { get; set; }

        public Bestelling( int inputID, float inputAantal)
        {
            ProductId = inputID;
            Aantal = inputAantal;
        }

        public string ShowBestellingInput()
        {
            string BestellingInput =  "  productID:         " + ProductId + "  Aantal: " + Aantal;
            return BestellingInput;
        }

    }
}
