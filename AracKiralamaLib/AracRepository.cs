using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace AracKiralamaLib
{
    public class AracRepository : IAracRepository
    {
        private readonly List<Arac> _aracListesi = new List<Arac>();

        private readonly string _connectionString;

        public AracRepository()
        {
            try
            {
                // Bağlantı dizesini ConfigurationManager üzerinden alıyoruz
                string _connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

                // Bağlantıyı kuruyoruz
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Veritabanına bağlantıyı açıyoruz
                    con.Open();
                    Console.WriteLine("Veritabanına başarıyla bağlanıldı.");
                }
            }
            catch (SqlException sqlEx)
            {
                // SQL hatası oluşursa burası çalışır
                Console.WriteLine("Veritabanı bağlantısı hatası: " + sqlEx.Message);
            }
            catch (ConfigurationErrorsException configEx)
            {
                // Config hatası oluşursa (bağlantı dizesi hatalı vb.) burası çalışır
                Console.WriteLine("Bağlantı dizesi hatası: " + configEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer genel hatalar için
                Console.WriteLine("Genel hata: " + ex.Message);
            }

        }

        public void AracEkle(Arac arac)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Araclar (Plaka,Marka, Model, Yil, Fiyat, KiradaMi) VALUES (@Plaka,@Marka, @Model, @Yil, @Fiyat, @KiradaMi)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Plaka", arac.Plaka);
                cmd.Parameters.AddWithValue("@Marka", arac.Marka);
                cmd.Parameters.AddWithValue("@Model", arac.Model);
                cmd.Parameters.AddWithValue("@Yil", arac.Yil);
                cmd.Parameters.AddWithValue("@Fiyat", arac.Fiyat);
                cmd.Parameters.AddWithValue("@KiradaMi", arac.KiradaMi);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        
        public void AracGuncelle(Arac arac)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Araclar 
                SET Plaka = @Plaka,
                    Marka = @Marka,
                    Model = @Model,
                    Yil = @Yil,
                    Fiyat = @Fiyat,
                    KiradaMi = @KiradaMi
                WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parametreleri ekleyin
                cmd.Parameters.AddWithValue("@Plaka", arac.Plaka);
                cmd.Parameters.AddWithValue("@Marka", arac.Marka);
                cmd.Parameters.AddWithValue("@Model", arac.Model);
                cmd.Parameters.AddWithValue("@Yil", arac.Yil);
                cmd.Parameters.AddWithValue("@Fiyat", arac.Fiyat);
                cmd.Parameters.AddWithValue("@KiradaMi", arac.KiradaMi);
                cmd.Parameters.AddWithValue("@Id", arac.Id); // Güncellenecek aracın Id'si

                // Bağlantıyı aç ve sorguyu çalıştır
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Araç başarıyla güncellendi.");
                }
                else
                {
                    Console.WriteLine("Güncelleme işlemi başarısız. Belirtilen Id ile bir kayıt bulunamadı.");
                }
            }

        }

        public void AracSil(int aracId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Silme işlemi için SQL sorgusu
                string query = "DELETE FROM Araclar WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parametre ekleme
                cmd.Parameters.AddWithValue("@Id", aracId);  // Silinecek aracın ID'si

                // Bağlantıyı aç
                con.Open();

                // Sorguyu çalıştır
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Araç başarıyla silindi.");
                }
                else
                {
                    Console.WriteLine("Araç silme işlemi başarısız. Belirtilen Id ile araç bulunamadı.");
                }
            }

        }

        public Arac AracGetir(int aracId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Araclar WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", aracId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Arac
                        {
                            Id = reader.GetInt32(0), // Id
                            Plaka = reader.GetString(1), // Plaka
                            Marka = reader.GetString(2), // Marka
                            Model = reader.GetString(3), // Model
                            Yil = reader.GetInt32(4), // Yil
                            Fiyat = reader.GetInt32(5), // Fiyat
                            KiradaMi = reader.GetBoolean(6) // KiradaMi
                        };
                    }
                }
            }

            // Eğer araç bulunamazsa null döner
            return null;
        }


        public List<Arac> TumAraclariGetir()
        {
            List<Arac> aracListesi = new List<Arac>();

            // Bağlantı dizesi (app.config veya web.config'den alınmalı)
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            // SQL sorgusu
            string query = "SELECT * FROM Araclar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Arac arac = new Arac
                        {
                            Id = reader.GetInt32(0),         // Id sütunu
                            Plaka = reader.GetString(1),    // Plaka sütunu
                            Marka = reader.GetString(2),    // Marka sütunu
                            Model = reader.GetString(3),    // Model sütunu
                            Yil = reader.GetInt32(4),        // Yıl sütunu
                            Fiyat = reader.GetInt32(5),        // Yıl sütunu
                            KiradaMi = reader.GetBoolean(6),        // Yıl sütunu
                        };

                        aracListesi.Add(arac);
                    }
                }
                catch (Exception ex)
                {
                    // Hata günlüğü veya özel hata işlemi
                    Console.WriteLine($"Hata: {ex.Message}");
                }
            }

            return aracListesi;
        }

    }
}

