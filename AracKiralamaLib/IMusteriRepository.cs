using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracKiralamaLib
{
    public interface IMusteriRepository
    {
        void MusteriEkle(Musteri musteri);
        void MusteriGuncelle(Musteri musteri);
        void MusteriSil(int musteriId);
        Musteri MusteriGetir(int musteriId);
        List<Musteri> TumMusterileriGetir();
    }
}

