﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kutuphane_uygulamasi
{

    public partial class Form1 : Form
    {
        //SQL kullanıcıların olduğu veritabanı bağlantısı
        SqlConnection kullanicilarbaglanti = new SqlConnection("Data Source=DESKTOP-TSDL2U5\\MSSQLSERVER3;Initial Catalog=Kutuphane_Kullanicilar;Integrated Security=True");

        public Form1()
        {
            InitializeComponent();
            Nick_Hatirla();
        }

        //private void Txt_Box_Parola_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    Txt_Box_Parola.PasswordChar = '\0'; //Yazılan şifreyi gizliyorum.
        //}

        private void Chc_Box_Parola_OnChange(object sender, EventArgs e)
        {
            if (Chc_Box_Parola.Checked == false) //if sorgusu yapıp check boxın durumunu kontrol ediyorum.if koşulu sağlıyorsa Parolam gösteriliyor
            {
                Txt_Box_Parola.isPassword = true;
                Chc_Box_Parola.Text = "Parolamı Göster";
                PictureBox_Lock_Unlock.Image = Image.FromFile("C:\\Users\\alime\\Desktop\\proje\\ikonlar\\\\password.png");
            }
            else
            {
                Txt_Box_Parola.isPassword = false;
                PictureBox_Lock_Unlock.Image = Image.FromFile("C:\\Users\\alime\\Desktop\\proje\\ikonlar\\\\unlock.png");
            }
        }

        private void BTN_Parola_Click(object sender, EventArgs e)
        {
            //parola unuttum butonuna tıklandığında parola değiştirme sayfasına geçiyor
            Form2 form2 = new Form2();
            form2.Show();


        }

        private void BTN_Yeni_uye_Click(object sender, EventArgs e)
        {
            //Yeni üyelik butonuna basıldığında üyelik sayfasına geçiyor
            YeniUyelik yeniUyelik = new YeniUyelik();
            yeniUyelik.Show();
        }

        private void Btn_Giris_Yap_Click(object sender, EventArgs e)
        {
            //Personel girişi check boxını kontrol ediyor
            if (chcbx_Personel.Checked == true)     //check box tikli ise kullanıcı adı veri tabanında personel olarak kayıtlı mı diye kontrol ediyor
            {
                kullanicilarbaglanti.Open();  //SQL baglantısı açılıyor
                string girilennick = Txt_Box_Kullanici_ism.Text;  //textboxın içindeki veriyi değişkene atıyor
                //girilen kullanıcı adında personeller tablosunda kaç tane veri oldugunu döndüren sql sorgu komutu
                SqlCommand personelnicksorgukomut = new SqlCommand("SELECT Count(Personel_Nick) FROM Kutuphane_Personeller WHERE Personel_Nick = '" + girilennick + "'"
                        , kullanicilarbaglanti);
                personelnicksorgukomut.ExecuteNonQuery();   //komut calıştırılıyor
                int varmi = (int)personelnicksorgukomut.ExecuteScalar();    //sorgudan dönen değer bir değişkene atılıyor.
                kullanicilarbaglanti.Close();

                if (varmi == 1)      //bu nickte personel varsa veritabanından şifresini çekiyor
                {
                    string personelnick = Txt_Box_Kullanici_ism.Text;
                    string personelsifre = Txt_Box_Parola.Text;
                    kullanicilarbaglanti.Open();
                    //girilen kullanıcı adındaki şifreyi alır
                    SqlCommand personelsifrealkomut = new SqlCommand("Select Personel_Sifre From Kutuphane_Personeller Where Personel_Nick = '" + personelnick + "'"
                            , kullanicilarbaglanti);
                    personelsifrealkomut.ExecuteNonQuery();

                    SqlDataReader sifreal = personelsifrealkomut.ExecuteReader();
                    sifreal.Read(); // veri tabanından veri okunuyor
                    string alinansifre = sifreal["Personel_Sifre"].ToString();  //alınan veri bir değişkene atılıyor
                    kullanicilarbaglanti.Close();
                    if (personelsifre != alinansifre)   //şifre yanlışsa messagebox ile uyarı veriyor
                    {
                        string message = "Lütfen şifrenizi kontrol edin!";
                        string title = "UYARI";
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);  // yanlış şifre girilirse
                    }
                    else    //Personel nicki var ve şifre doğru ise personel sayfasına atıyor
                    {
                        //Personel sayfasına atar
                        FormPersonel personelsayfasi = new FormPersonel();
                        personelsayfasi.lbl_personel_nick.Text = Txt_Box_Kullanici_ism.Text;
                        personelsayfasi.Show();

                        // son girilen nicki hafızaya kaydediyor
                        Son_Nick_Kaydet();

                    }
                    kullanicilarbaglanti.Close();


                }
                else    //girilen nickte veritabanında personel yoksa uyarı veriyor
                {
                    string message = "Bu kullanıcı adında bir personel yok, kullanıcı adınızı kontrol edin!";
                    string title = "UYARI";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);  // personel olmayan nick
                }

            }
            else    //Personel check boxı tikli değilse kullanıcı giriş yapıyor demektir
            {
                // girilen nick ve şifreyi değişkene atıyor
                string kullanicinick = Txt_Box_Kullanici_ism.Text;
                string kullanicisifre = Txt_Box_Parola.Text;
                kullanicilarbaglanti.Open();
                //girilen nickte veritabanında eşleşen kaç veri oldugunu gösteren sorgu
                SqlCommand kullanicinicksorgukomut = new SqlCommand("SELECT Count(kullanici_nick) FROM Kutuphane_Kullanicilar WHERE kullanici_nick = '" + kullanicinick + "'"
                        , kullanicilarbaglanti);
                kullanicinicksorgukomut.ExecuteNonQuery();

                int varmi = (int)kullanicinicksorgukomut.ExecuteScalar();   //döndürülen değer degiskene atanıyor
                kullanicilarbaglanti.Close();
                if (varmi == 1)     //girilen nick bir kullaniciya ait ise
                {
                    //girilen kullanıcı adındaki verinin şifresini dödüren sorgu
                    SqlCommand girisyapkomut = new SqlCommand("Select kullanici_sifre From Kutuphane_Kullanicilar Where kullanici_nick = '" + kullanicinick + "'"
                        , kullanicilarbaglanti);
                    kullanicilarbaglanti.Open();
                    girisyapkomut.ExecuteNonQuery();

                    //girilen kullanıcı adındaki şifreyi alır
                    SqlDataReader sifreal = girisyapkomut.ExecuteReader();
                    sifreal.Read();
                    string alinansifre = sifreal["kullanici_sifre"].ToString(); //alinan şifre bir değişkene atanıyor
                    kullanicilarbaglanti.Close();
                    if (kullanicisifre != alinansifre)  //girilen şifre ile veri tabanındaki şifre aynı mı diye bakıyor AYNI DEĞİLSE;
                    {
                        string message = "Lütfen kullanıcı adı veya şifrenizi kontrol edin!";
                        string title = "UYARI";
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);  // yanlış şifre girilirse
                    }
                    else    //girilen şifre doğruysa kitap kiralama formuna atıyor
                    {
                        FormKitapKiralama formac = new FormKitapKiralama();
                        formac.txt_giris_metni.Text = Txt_Box_Kullanici_ism.Text;  //girilen nicki kitap kiralama formundki labela yazıyor
                        formac.Show();
                        Son_Nick_Kaydet();
                    }
                    kullanicilarbaglanti.Close();
                }
                else    //girilen nickte kullanıcı yoksa
                {
                    string message = "Bu kullanıcı adında bir kullanıcı yok, kullanıcı adınızı kontrol edin!";
                    string title = "UYARI";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);  // kullanıcı olmayan nick
                }
            }

        }

        //Projenin ayarlar kısmına son girlen nick ve beni hatırla tuşunun verileri kaydediliyor
        private void Nick_Hatirla()
        {
            if (Properties.Settings.Default.KullaniciNick != String.Empty)
            {
                if (Properties.Settings.Default.NickHatirla == true)
                {
                    Txt_Box_Kullanici_ism.Text = Properties.Settings.Default.KullaniciNick;
                    Chc_Box_Ad_Hatirla.Checked = true;
                }
                else
                {
                    Txt_Box_Kullanici_ism.Text = Properties.Settings.Default.KullaniciNick;
                }
            }
        }

        //Projenin ayarlar kısmından girilen nick ve beni hatırla kayıtlı ise veriler çekilip ekrana yazdırılıyor
        private void Son_Nick_Kaydet()
        {
            if (Chc_Box_Ad_Hatirla.Checked == true)
            {
                Properties.Settings.Default.KullaniciNick = Txt_Box_Kullanici_ism.Text;
                Properties.Settings.Default.NickHatirla = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.KullaniciNick = "";
                Properties.Settings.Default.NickHatirla = false;
                Properties.Settings.Default.Save();
            }
        }

        private void Img_Button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();     //ekranın sağ üstündeki çarpı görseline tıklandığında uygulamanın kapatılmasını sağlayan kod
        }



        private void Txt_Box_Kullanici_ism_Enter(object sender, EventArgs e)
        {
            //kullanıcı adının girildiği texboxın içine girildiğinde görsel değişiyor
            Picture_box_Giris.Image = Image.FromFile("C:\\Users\\alime\\Desktop\\proje\\ikonlar\\giris yaparken\\ikinci.jpg");
        }



        private void chcbx_Personel_Click(object sender, EventArgs e)
        {
            //personelim chcboxına tıklandığında görsel değişiyor
            Picture_box_Giris.Image = Image.FromFile("C:\\Users\\alime\\Desktop\\proje\\ikonlar\\giris yaparken\\worker.png");
        }
        int k = 1; //  saheser 
        public void Txt_Box_Parola_Enter(object sender, EventArgs e)
        {
            //parola girme textboxına girildiğinde görsel değişiyor
            Picture_box_Giris.Image = Image.FromFile("C:\\Users\\alime\\Desktop\\proje\\ikonlar\\giris yaparken\\ucuncu.jpg");

            //girilen metnin parola oldugu atanıyor ve parolanızı giriniz yazısı siliniyor. 
            if (k == 1)
            {
                Txt_Box_Parola.isPassword = true;
                Txt_Box_Parola.Text = "";
                k--;
            }


        }
    }
}
