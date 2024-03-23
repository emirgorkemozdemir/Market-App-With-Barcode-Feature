using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Market_Sistemi.Classes;
using AForge.Video.DirectShow;
using ZXing;

namespace Market_Sistemi
{
    public partial class AdminÜrünİşlemleri : Form
    {
        public AdminÜrünİşlemleri()
        {
            InitializeComponent();
        }

        FilterInfoCollection Cihazlar;
        VideoCaptureDevice kameram;

        private void btnCategory_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminKategoriİşlemleri categoryPage = new AdminKategoriİşlemleri();
            categoryPage.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminÜrünİşlemleri productPage = new AdminÜrünİşlemleri();
            productPage.Show();
        }

        public void LoadProducts()
        {
            SqlCommand commandList = new SqlCommand("Select ProductID,ProductName,ProductPrice,ProductBarcode,CategoryName from TableProduct inner join TableCategory on TableProduct.ProductCategory = TableCategory.CategoryID", SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            SqlDataAdapter da = new SqlDataAdapter(commandList);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgProduct.DataSource = dt;
        }

        public void LoadCategories()
        {
            SqlCommand commandLoadCategories = new SqlCommand("Select * from TableCategory",SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            cmbBoxCategory.DisplayMember = "CategoryName";

            cmbBoxCategory.ValueMember ="CategoryID";

            SqlDataAdapter daLoadCategories = new SqlDataAdapter(commandLoadCategories);

            DataTable dtLoadCategories = new DataTable();

            daLoadCategories.Fill(dtLoadCategories);

            cmbBoxCategory.DataSource = dtLoadCategories;
        }

        public void loadCategories_new()
        {
            SqlCommand commandLoadCategories = new SqlCommand("Select * from TableCategory", SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            SqlDataReader dr = commandLoadCategories.ExecuteReader();

            while (dr.Read())
            {
                cmbBoxCategory.Items.Add(dr["CategoryName"].ToString());
            }

            dr.Close();

            cmbBoxCategory.SelectedIndex = 0;
        }

        private void AdminÜrünİşlemleri_Load(object sender, EventArgs e)
        {
            
            LoadProducts();
            LoadCategories();

            Cihazlar = new FilterInfoCollection(FilterCategory.VideoInputDevice);


            foreach (FilterInfo cihaz in Cihazlar)
            {
                cmbKamera.Items.Add(cihaz.Name);
            }

            cmbKamera.SelectedIndex = 0;

            kameram = new VideoCaptureDevice(Cihazlar[cmbKamera.SelectedIndex].MonikerString);
            kameram.Start();
        }

        private void btnMW_Click(object sender, EventArgs e)
        {
            kameram.Stop();
            this.Hide();
            Form1 newForm = new Form1();
            newForm.Show();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlCommand commandAdd = new SqlCommand("Insert into TableProduct (ProductName,ProductCategory,ProductPrice,ProductBarcode) values (@pname,@pcategory,@pprice,@pbarcode)",SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            commandAdd.Parameters.AddWithValue("@pname", tboxProductName.Text);
            commandAdd.Parameters.AddWithValue("@pcategory", Convert.ToInt32(cmbBoxCategory.SelectedValue));
            commandAdd.Parameters.AddWithValue("@pprice", tboxProductPrice.Text);
            commandAdd.Parameters.AddWithValue("@pbarcode", tboxProductBarcode.Text);

            commandAdd.ExecuteNonQuery();

            IncreaseCategoryCount();
            LoadProducts();
            MessageBox.Show("Ürün başarıyla eklendi");

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlCommand commandDelete = new SqlCommand("Delete from TableProduct where ProductID=@pid",SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            commandDelete.Parameters.AddWithValue("@pid", Convert.ToInt32(Convert.ToInt32(SelectedID)));

            commandDelete.ExecuteNonQuery();

            DecreaseCategoryCount();
            LoadProducts();

            MessageBox.Show("Ürün başarıyla silindi");
        }

        string SelectedID;

        private void dgProduct_SelectionChanged(object sender, EventArgs e)
        {
            if (dgProduct.CurrentRow==null)
            {

            }
            else
            {
                SelectedID = dgProduct.CurrentRow.Cells["ProductID"].Value.ToString();
                lblSelectedID.Text = SelectedID;
            }
        
        }

        public void IncreaseCategoryCount()
        {
            SqlCommand commandIncrease = new SqlCommand("Update TableCategory set CategoryCount+=1 where CategoryID=@pid",SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            commandIncrease.Parameters.AddWithValue("@pid", Convert.ToInt32(cmbBoxCategory.SelectedValue));

            commandIncrease.ExecuteNonQuery();
        }

        public void DecreaseCategoryCount()
        {
            SqlCommand commandIncrease = new SqlCommand("Update TableCategory set CategoryCount-=1 where CategoryID=@pid", SqlConnectionClass.connect);

            SqlConnectionClass.CheckConnection(SqlConnectionClass.connect);

            commandIncrease.Parameters.AddWithValue("@pid", Convert.ToInt32(cmbBoxCategory.SelectedValue));

            commandIncrease.ExecuteNonQuery();
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {




            var newTask = Task.Factory.StartNew(() =>
            {
                kameram.NewFrame += VideoCaptureDevice_NewFrame;

            });



        }




        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap GoruntulenenBarkod = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader okuyucu = new BarcodeReader();
            var sonuc = okuyucu.Decode(GoruntulenenBarkod);

            if (sonuc != null)
            {
                tboxProductBarcode.Invoke(new MethodInvoker(delegate ()
                {
                    tboxProductBarcode.Text = sonuc.ToString();

                }
                ));
            }

            PictureBox1.Image = GoruntulenenBarkod;
        }

      
    }
}
