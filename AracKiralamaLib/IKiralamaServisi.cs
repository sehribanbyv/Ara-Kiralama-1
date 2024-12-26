using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracKiralamaLib
{
    public interface IKiralamaServisi
    {
        void AracKirala(int musteriId, int aracId, DateTime baslangic, DateTime bitis);
        void KiralamaIptal(int kiralamaId);
        List<KiralamaDetay> KiralikAracMusteriListesi();
    }
}

