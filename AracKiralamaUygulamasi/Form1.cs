using AracKiralamaLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace AracKiralamaUygulamasi
{
    public partial class Form1 : Form
    {
        private readonly IAracRepository _aracRepository;
        private readonly IMusteriRepository _musteriRepository;
        private readonly IKiralamaServisi _kiralamaServisi;

        private void AraclariYukle()
        {
            try
            {
                // Araç listesini al
                List<Arac> araclar = _aracRepository.TumAraclariGetir();


                // DataGridView'e bağla
                aractablo.DataSource = araclar;
                aractablo.ReadOnly = true;
                // aractablo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //aractablo.Columns["KiradaMi"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private void MusterileriYukle()
        {
            try
            {
                // Araç listesini al
                List<Musteri> musteriler = _musteriRepository.TumMusterileriGetir();


                // DataGridView'e bağla
                musteritablo.DataSource = musteriler;
                musteritablo.ReadOnly = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }


        private void KiralikAracYukle()
        {
            try
            {
                // Araç listesini al
                List<KiralamaDetay> kiralamaDetays = _kiralamaServisi.KiralikAracMusteriListesi();


                // DataGridView'e bağla
                kiralatablo.DataSource = kiralamaDetays;
                kiralatablo.ReadOnly = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private void MusteriComboboxDoldur()
        {
            // Bağlantı dizesini al
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    // SQL sorgusu: Müşteri ID ve Adını al
                    string query = "SELECT Id, Isim + ' ' + Soyisim AS MusteriAdSoyad FROM Musteriler";

                    // Komut oluştur
                    SqlCommand cmd = new SqlCommand(query, con);

                    // Bağlantıyı aç
                    con.Open();

                    // Veriyi oku
                    SqlDataReader reader = cmd.ExecuteReader();

                    // ComboBox için DataTable oluştur
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Yeni bir satır oluştur ve "Müşteri Seç" seçeneği ekle
                    DataRow newRow = dt.NewRow();
                    newRow["Id"] = 0; // ID'yi 0 veya başka bir varsayılan değer yapabilirsiniz
                    newRow["MusteriAdSoyad"] = "Müşteri Seç";
                    dt.Rows.InsertAt(newRow, 0); // İlk sıraya ekle


                    // ComboBox'a bağla
                    txtKiralamaMusteriId.DataSource = dt;
                    txtKiralamaMusteriId.DisplayMember = "MusteriAdSoyad"; // Görünen metin
                    txtKiralamaMusteriId.ValueMember = "Id"; // Seçilen değerin ID'si
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        } 
        private void AracComboboxDoldur()
        {
            // Bağlantı dizesini al
            string connectionString = ConfigurationManager.ConnectionStrings["AracKiralamaDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    // SQL sorgusu: Müşteri ID ve Adını al
                    string query = "SELECT Id, Model FROM Araclar";

                    // Komut oluştur
                    SqlCommand cmd = new SqlCommand(query, con);

                    // Bağlantıyı aç
                    con.Open();

                    // Veriyi oku
                    SqlDataReader reader = cmd.ExecuteReader();

                    // ComboBox için DataTable oluştur
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Yeni bir satır oluştur ve "Müşteri Seç" seçeneği ekle
                    DataRow newRow = dt.NewRow();
                    newRow["Id"] = 0; // ID'yi 0 veya başka bir varsayılan değer yapabilirsiniz
                    newRow["Model"] = "Araç Seç";
                    dt.Rows.InsertAt(newRow, 0); // İlk sıraya ekle

                    // ComboBox'a bağla
                    txtKiralamaAracId.DataSource = dt;
                    txtKiralamaAracId.DisplayMember = "Model"; // Görünen metin
                    txtKiralamaAracId.ValueMember = "Id"; // Seçilen değerin ID'si
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void temizle()
        {
            txtAracId.Text = string.Empty;
            txtModel.Text = string.Empty;
            txtMarka.Text = string.Empty;
            txtYil.Text = string.Empty;
            txtFiyat.Text = string.Empty;

            txtMusteriId.Text=string.Empty;
            txtIsim.Text=string.Empty;
            txtSoyisim.Text=string.Empty;
            txtTelefon.Text=string.Empty;

        }

        public Form1()
        {
            InitializeComponent();

            // Repository ve servislerin örneklenmesi
            _aracRepository = new AracRepository();
            _musteriRepository = new MusteriRepository();
            _kiralamaServisi = new KiralamaServisi(_aracRepository, _musteriRepository);
        }

        private void btnAracEkle_Click(object sender, EventArgs e)
        {
            // Araç ekleme işlemleri
            var arac = new Arac
            {
                Id = 0,
                Plaka = txtAracId.Text,
                Marka = txtMarka.Text,
                Model = txtModel.Text,
                Yil = int.Parse(txtYil.Text),
                Fiyat = int.Parse(txtFiyat.Text),
                KiradaMi = false
            };

            _aracRepository.AracEkle(arac);
            AraclariYukle();
            temizle();
            MessageBox.Show("Araç başarıyla eklendi.");
        }

        private void btnMusteriEkle_Click(object sender, EventArgs e)
        {
            // Müşteri ekleme işlemleri
            var musteri = new Musteri
            {
                Tc = txtMusteriId.Text,
                Isim = txtIsim.Text,
                Soyisim = txtSoyisim.Text,
                Telefon = txtTelefon.Text
            };

            _musteriRepository.MusteriEkle(musteri);
            MusterileriYukle();
            temizle();
            MessageBox.Show("Müşteri başarıyla eklendi.");
        }

        private void btnAracKirala_Click(object sender, EventArgs e)
        {
            // Araç kiralama işlemleri
            int musteriId = int.Parse(txtKiralamaMusteriId.SelectedValue?.ToString());
            int aracId = int.Parse(txtKiralamaAracId.SelectedValue?.ToString());
            DateTime baslangic = dtpBaslangic.Value;
            DateTime bitis = dtpBitis.Value;

            try
            {
                _kiralamaServisi.AracKirala(musteriId, aracId, baslangic, bitis);
                KiralikAracYukle();
                temizle();
                MessageBox.Show("Araç başarıyla kiralandı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AraclariYukle();
            MusterileriYukle();
            MusteriComboboxDoldur();
            AracComboboxDoldur();
            KiralikAracYukle();
        }

        private void aractabloselect(object sender, DataGridViewCellEventArgs e)
        {
           
            txtAracId.Text = aractablo.CurrentRow.Cells[1].Value.ToString();
            txtMarka.Text = aractablo.CurrentRow.Cells[2].Value.ToString();
            txtModel.Text = aractablo.CurrentRow.Cells[3].Value.ToString();
            txtYil.Text = aractablo.CurrentRow.Cells[4].Value.ToString();
            txtFiyat.Text = aractablo.CurrentRow.Cells[5].Value.ToString();
            
            MessageBox.Show(aractablo.CurrentRow.Cells[5].Value.ToString());
           
        }

        private void btnAracGuncelle_Click(object sender, EventArgs e)
        {
            // Araç güncelleme işlemleri
            var arac = new Arac
            {
                Id = Convert.ToInt32(aractablo.CurrentRow.Cells["Id"].Value), // "Id" sütunun adını kullan
                Plaka = txtAracId.Text,
                Marka = txtMarka.Text,
                Model = txtModel.Text,
                Yil = int.Parse(txtYil.Text),
                Fiyat = int.Parse(txtFiyat.Text),
                KiradaMi = false
            };

            _aracRepository.AracGuncelle(arac);
            AraclariYukle();
            temizle();
            MessageBox.Show("Araç başarıyla güncellendi.");
        }

        private void btnAracSil_Click(object sender, EventArgs e)
        {
            _aracRepository.AracSil(Convert.ToInt32(aractablo.CurrentRow.Cells["Id"].Value));
            AraclariYukle();
            temizle();
            MessageBox.Show("Araç başarıyla silindi.");
        }

        private void btnMusteriGuncelle_Click(object sender, EventArgs e)
        {
            // Araç güncelleme işlemleri
            var musteri = new Musteri
            {
                Id = Convert.ToInt32(musteritablo.CurrentRow.Cells["Id"].Value), // "Id" sütunun adını kullan
                Tc = txtMusteriId.Text,
                Isim = txtIsim.Text,
                Soyisim = txtSoyisim.Text,
                Telefon = txtTelefon.Text
            };

            _musteriRepository.MusteriGuncelle(musteri);
            MusterileriYukle();
            temizle();
            MessageBox.Show("Müşteri başarıyla güncellendi.");
        }

        private void btnMusteriSil_Click(object sender, EventArgs e)
        {
            _musteriRepository.MusteriSil(Convert.ToInt32(aractablo.CurrentRow.Cells["Id"].Value));
            MusterileriYukle();
            temizle();
            MessageBox.Show("Müşteri başarıyla silindi.");
        }

        private void musteritabloselect(object sender, DataGridViewCellEventArgs e)
        {
            txtMusteriId.Text = musteritablo.CurrentRow.Cells[1].Value.ToString();
            txtIsim.Text = musteritablo.CurrentRow.Cells[2].Value.ToString();
            txtSoyisim.Text = musteritablo.CurrentRow.Cells[3].Value.ToString();
            txtTelefon.Text = musteritablo.CurrentRow.Cells[4].Value.ToString();
           
        }

        private void kiralamatabloselect(object sender, DataGridViewCellEventArgs e)
        {
            if (kiralatablo.CurrentRow != null)
            {
                // Müşteri ComboBox için SelectedValue atanıyor
                if (kiralatablo.CurrentRow.Cells[7].Value != null)
                {
                    txtKiralamaMusteriId.SelectedValue = kiralatablo.CurrentRow.Cells[7].Value;
                }

                // Araç ComboBox için SelectedValue atanıyor
                if (kiralatablo.CurrentRow.Cells[8].Value != null)
                {
                    txtKiralamaAracId.SelectedValue = kiralatablo.CurrentRow.Cells[8].Value;
                }

                // Tarih alanları için değerler atanıyor
                 if (kiralatablo.CurrentRow.Cells[4].Value != null)
                 {
                     dtpBaslangic.Value = Convert.ToDateTime(kiralatablo.CurrentRow.Cells[4].Value);
                 }

                 if (kiralatablo.CurrentRow.Cells[5].Value != null)
                 {
                     dtpBitis.Value = Convert.ToDateTime(kiralatablo.CurrentRow.Cells[5].Value);
                 }
               
            }
        }

        private void btnAracKiralaCancel_Click(object sender, EventArgs e)
        {
            _kiralamaServisi.KiralamaIptal(Convert.ToInt32(kiralatablo.CurrentRow.Cells["KiralamaId"].Value));
            KiralikAracYukle();
            temizle();
            MessageBox.Show("Araç Kiralama başarıyla silindi.");
        }

      
    }
}

