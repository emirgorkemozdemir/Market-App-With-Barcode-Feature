using AForge.Video.DirectShow;
using Market_Sistemi.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZXing;

namespace Market_Sistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection Cihazlar;
        VideoCaptureDevice kameram;

        private void btnCategory_Click(object sender, EventArgs e)
        {
            kameram.Stop();
            this.Hide();
            AdminKategoriİşlemleri categoryPage = new AdminKategoriİşlemleri();
            categoryPage.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kameram.Stop();
            this.Hide();
            AdminÜrünİşlemleri productPage = new AdminÜrünİşlemleri();
            productPage.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            Cihazlar = new FilterInfoCollection(FilterCategory.VideoInputDevice);


            foreach (FilterInfo cihaz in Cihazlar)
            {
                cmbKamera.Items.Add(cihaz.Name);
            }

            cmbKamera.SelectedIndex = 0;

            kameram = new VideoCaptureDevice(Cihazlar[cmbKamera.SelectedIndex].MonikerString);
            kameram.Start();
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {




            var newTask = Task.Factory.StartNew(() =>
            {
                kameram.NewFrame += VideoCaptureDevice_NewFrame;

            });



        }

        public static double FindProductPrice(string barcode)
        {
            SqlCommand commandFindProductPrice = new SqlCommand("Select ProductPrice from TableProduct where ProductBarcode=@p1", SqlConnectionClass.connect);
            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            commandFindProductPrice.Parameters.AddWithValue("@p1", barcode);

            SqlDataReader dr = commandFindProductPrice.ExecuteReader();
            double price = 0;
            while (dr.Read())
            {
                price = Convert.ToDouble(dr[0]);
            }

            dr.Close();

            return price;
        }

        double sum = 0;
        double price = 0;
        string temp_barcode = " ";

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap GoruntulenenBarkod = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader okuyucu = new BarcodeReader();
            var sonuc = okuyucu.Decode(GoruntulenenBarkod);

            if (sonuc != null)
            {
                txtBarcode.Invoke(new MethodInvoker(delegate ()
                {


                    if (temp_barcode == sonuc.ToString())
                    {

                    }
                    else
                    {
                        temp_barcode = sonuc.ToString();
                        txtBarcode.Text = sonuc.ToString();
                        price = FindProductPrice(txtBarcode.Text);

                        if (price == 0)
                        {
                           
                        }
                        else
                        {
                            sum += price;
                            richTextBox1.Text = sum.ToString();
                        }
                       
                    }


                }
                ));
            }

            PictureBox1.Image = GoruntulenenBarkod;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            sum = 0;
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            sum += price;
            richTextBox1.Text = sum.ToString();
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            sum -= price;
            richTextBox1.Text = sum.ToString();
        }
    }
}
