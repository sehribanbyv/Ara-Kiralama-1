using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AracKiralamaLib
{
    public class MusteriRepository : IMusteriRepository
    {
        private readonly List<Musteri> _musteriListesi = new List<Musteri>();

        private readonly string _connectionString;

        public MusteriRepository()
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


        public void MusteriEkle(Musteri musteri)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Musteriler (Tc,Isim, Soyisim, Telefon) VALUES (@Tc,@Isim, @Soyisim, @Telefon)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Tc", musteri.Tc);
                cmd.Parameters.AddWithValue("@Isim", musteri.Isim);
                cmd.Parameters.AddWithValue("@Soyisim", musteri.Soyisim);
                cmd.Parameters.AddWithValue("@Telefon", musteri.Telefon);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MusteriGuncelle(Musteri musteri)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Musteriler 
                SET Tc = @Tc,
                    Isim = @Isim,
                    Soyisim = @Soyisim,
                    Telefon = @Telefon
                WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parametreleri ekleyin
                cmd.Parameters.AddWithValue("@Tc", musteri.Tc);
                cmd.Parameters.AddWithValue("@Isim", musteri.Isim);
                cmd.Parameters.AddWithValue("@Soyisim", musteri.Soyisim);
                cmd.Parameters.AddWithValue("@Telefon", musteri.Telefon);
                cmd.Parameters.AddWithValue("@Id", musteri.Id); // Güncellenecek aracın Id'si

                // Bağlantıyı aç ve sorguyu çalıştır
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Müşteri başarıyla güncellendi.");
                }
                else
                {
                    Console.WriteLine("Güncelleme işlemi başarısız. Belirtilen Id ile bir kayıt bulunamadı.");
                }
            }

        }


        public void MusteriSil(int musteriId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString özelliği başlatılmamış. 'AracKiralamaDB' için bağlantı dizesini kontrol edin.");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Silme işlemi için SQL sorgusu
                string query = "DELETE FROM Musteriler WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parametre ekleme
                cmd.Parameters.AddWithValue("@Id", musteriId);  // Silinecek aracın ID'si

                // Bağlantıyı aç
                con.Open();

                // Sorguyu çalıştır
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Müşteri başarıyla silindi.");
                }
                else
                {
                    Console.WriteLine("Müşteri silme işlemi başarısız. Belirtilen Id ile araç bulunamadı.");
                }
            }
        }

        public Musteri MusteriGetir(int musteriId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Musteriler WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", musteriId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Musteri
                        {
                            Id = reader.GetInt32(0),         // Id sütunu
                            Tc = reader.GetString(1),         // Id sütunu
                            Isim = reader.GetString(2),    // Plaka sütunu
                            Soyisim = reader.GetString(3),    // Marka sütunu
                            Telefon = reader.GetString(4),    // Model sütunu
                        };
                    }
                }
            }

            // Eğer araç bulunamazsa null döner
            return null;
        }

        public List<Musteri> TumMusterileriGetir()
        {
            List<Musteri> musteriListesi = new List<Musteri>();

            // Bağlantı dizesi (app.config veya web.config'den alınmalı)
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            // SQL sorgusu
            string query = "SELECT * FROM Musteriler";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Musteri musteri = new Musteri
                        {
                            Id = reader.GetInt32(0),         // Id sütunu
                            Tc = reader.GetString(1),         // Id sütunu
                            Isim = reader.GetString(2),    // Plaka sütunu
                            Soyisim = reader.GetString(3),    // Marka sütunu
                            Telefon = reader.GetString(4),    // Model sütunu
                       
                        };

                        musteriListesi.Add(musteri);
                    }
                }
                catch (Exception ex)
                {
                    // Hata günlüğü veya özel hata işlemi
                    Console.WriteLine($"Hata: {ex.Message}");
                }
            }
            
            return musteriListesi;
        }
    }
}

