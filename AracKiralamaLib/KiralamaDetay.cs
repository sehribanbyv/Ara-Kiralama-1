using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracKiralamaLib
{
    public class KiralamaDetay
    {
        public string AracPlaka { get; set; }
        public string AracMarka { get; set; }
        public string AracModel { get; set; }
        public string Musteri { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int KiralamaId { get; set; }
        public int MusteriId { get; set; }
        public int AracId { get; set; }
    }

}
