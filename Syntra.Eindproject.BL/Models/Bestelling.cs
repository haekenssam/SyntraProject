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
        public int ProductId { get; set; }
        public float Aantal { get; set; }
        public float Bedrag { get; set; }
       
    }
}
