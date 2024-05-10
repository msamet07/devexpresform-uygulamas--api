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
using System.IO;

namespace apiproject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadXmlData();
        }

        public void LoadXmlData()
        {
            try
            {
                // XML dosyasının yolunu belirt
                string xmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "invoices.xml");

                // XML dosyasından verileri oku ve bir DataSet'e yükle
                DataSet xmlDataSet = new DataSet();
                xmlDataSet.ReadXml(xmlFilePath);


                // DataSet içindeki 'data' tablosunu DataGridView'a bağla
                ekran.DataSource = xmlDataSet.Tables["data"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("XML dosyası yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        
    }
}
