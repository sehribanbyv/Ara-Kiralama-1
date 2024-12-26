using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AracKiralamaLib
{
    public class KiralamaServisi : IKiralamaServisi
    {
        private readonly IAracRepository _aracRepository;
        private readonly IMusteriRepository _musteriRepository;

        public KiralamaServisi(IAracRepository aracRepository, IMusteriRepository musteriRepository)
        {
            _aracRepository = aracRepository;
            _musteriRepository = musteriRepository;
        }

        public List<KiralamaDetay> KiralikAracMusteriListesi()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;
            var kiralikListe = new List<KiralamaDetay>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                k.Id AS KiralamaId,
                k.MusteriId,
                k.AracId,
                a.Plaka AS AracPlaka,
                a.Marka AS AracMarka,
                a.Model AS AracModel,
                m.Isim AS MusteriIsim,
                m.Soyisim AS MusteriSoyisim,
                k.Baslangic,
                k.Bitis
            FROM 
                Kiralamalar k
            INNER JOIN 
                Araclar a ON k.AracId = a.Id
            INNER JOIN 
                Musteriler m ON k.MusteriId = m.Id
            WHERE 
                a.KiradaMi = 1";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kiralikListe.Add(new KiralamaDetay
                        {
                            KiralamaId = reader.GetInt32(reader.GetOrdinal("KiralamaId")),
                            MusteriId = reader.GetInt32(reader.GetOrdinal("MusteriId")),
                            AracId = reader.GetInt32(reader.GetOrdinal("AracId")),
                            AracPlaka = reader.GetString(reader.GetOrdinal("AracPlaka")),
                            AracMarka = reader.GetString(reader.GetOrdinal("AracMarka")),
                            AracModel = reader.GetString(reader.GetOrdinal("AracModel")),
                            Musteri = reader.GetString(reader.GetOrdinal("MusteriIsim")) + " " + reader.GetString(reader.GetOrdinal("MusteriSoyisim")),
                            BaslangicTarihi = reader.GetDateTime(reader.GetOrdinal("Baslangic")),
                            BitisTarihi = reader.GetDateTime(reader.GetOrdinal("Bitis"))
                        });
                    }
                }
            }

            return kiralikListe;
        }





        public void AracKirala(int musteriId, int aracId, DateTime baslangic, DateTime bitis)
        {
            var musteri = _musteriRepository.MusteriGetir(musteriId);
            
            var arac = _aracRepository.AracGetir(aracId);
            Console.WriteLine(arac.Marka);
            if (musteri == null)
            {
                throw new Exception("Müşteri bulunamadı.");
            }

            if (arac == null)
            {
                throw new Exception("Araç bulunamadı.");
            }

            if (arac.KiradaMi)
            {
                throw new Exception("Araç zaten kirada.");
            }

            // Araç kirada olarak işaretlenir
            arac.KiradaMi = true;
            _aracRepository.AracGuncelle(arac);

            // Kiralama bilgileri veritabanına kaydedilir
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString))
            {
                string query = "INSERT INTO Kiralamalar (MusteriId, AracId, Baslangic, Bitis) VALUES (@MusteriId, @AracId, @Baslangic, @Bitis)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MusteriId", musteriId);
                cmd.Parameters.AddWithValue("@AracId", aracId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Araç {arac.Marka} {arac.Model} başarıyla {musteri.Isim} {musteri.Soyisim} adlı müşteriye kiralandı.");
        }

        public void KiralamaIptal(int kiralamaId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString))
            {
                // Kiralama bilgisi kontrol edilir
                string selectQuery = "SELECT AracId FROM Kiralamalar WHERE Id = @KiralamaId";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@KiralamaId", kiralamaId);

                con.Open();
                object aracIdObj = selectCmd.ExecuteScalar();

                if (aracIdObj == null)
                {
                    throw new Exception("Kiralama bilgisi bulunamadı.");
                }

                int aracId = Convert.ToInt32(aracIdObj);

                // Araç bilgisi alınır ve kiradan çıkarılır
                var arac = _aracRepository.AracGetir(aracId);
                if (arac == null || !arac.KiradaMi)
                {
                    throw new Exception("Araç bulunamadı veya kirada değil.");
                }

                arac.KiradaMi = false;
                _aracRepository.AracGuncelle(arac);

                // Kiralama kaydı veritabanından silinir
                string deleteQuery = "DELETE FROM Kiralamalar WHERE Id = @KiralamaId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                deleteCmd.Parameters.AddWithValue("@KiralamaId", kiralamaId);

                deleteCmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Araç kiralama kaydı başarıyla iptal edildi.");
        }
    }

}

