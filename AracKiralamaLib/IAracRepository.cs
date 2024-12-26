﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracKiralamaLib
{
    public interface IAracRepository
    {
        void AracEkle(Arac arac);
        void AracGuncelle(Arac arac);
        void AracSil(int aracId);
        Arac AracGetir(int aracId);
        List<Arac> TumAraclariGetir();
    }
}