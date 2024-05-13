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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace apiproject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //LoadXmlData();
            
        }
        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Token almak için kullanıcı bilgileri
                var tokenUsername = "efatura@etfbilisim.com";
                var tokenPassword = "RejrCwB3";
                var grantType = "password";

                // Token almak için istek verisi
                var tokenData = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("username", tokenUsername),
            new KeyValuePair<string, string>("password", tokenPassword),
            new KeyValuePair<string, string>("grant_type", grantType),
          //  new KeyValuePair<string, string>("invoiceETTN", "03383a0f-f90f-461a-99f5-5e8cf150b320")

        });

                // HttpClient oluştur
                using (var tokenClient = new HttpClient())
                {
                    // Token alacağımız API adresini belirt
                    tokenClient.BaseAddress = new Uri("https://edocumentapi.mysoft.com.tr/oauth/token");

                    // Token alma isteği gönder
                    var tokenResponse = await tokenClient.PostAsync("", tokenData);

                    // Yanıtı JSON olarak al
                    string tokenJson = await tokenResponse.Content.ReadAsStringAsync();

                    // JSON yanıtını dinamik nesneye çevir
                    dynamic tokenObject = JObject.Parse(tokenJson);

                    // Token'ı al
                    string accessToken = tokenObject.access_token;

                    // Token oluşturulduysa devam et
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // XML verilerini yükle
                        LoadXmlDataAsync(accessToken);
                    }
                    else
                    {
                        MessageBox.Show("Token alınamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken bir hata oluştu: " + ex.Message);
            }

        }


        public async Task LoadXmlDataAsync(string accessToken)
        {
            try
            {
                // JSON verisini dinamik olarak oluştur
                JObject jsonContent = new JObject();
                jsonContent["startDate"] = "2024-01-01T00:00:00";
                jsonContent["endDate"] = "2024-05-05T00:00:00";
                jsonContent["pkAlias"] = "";
                jsonContent["isUseDocDate"] = true;
                jsonContent["cessionStatus"] = 0;
                jsonContent["afterValue"] = 0;
                jsonContent["limit"] = 100;
                jsonContent["tenantIdentifierNumber"] = "";

                // JSON verisini string'e dönüştür
                string jsonString = jsonContent.ToString();

                // HttpClient oluştur
                using (var client = new HttpClient())
                {
                    // istek atacağım API adresini belirt
                    client.BaseAddress = new Uri("https://edocumentapi.mysoft.com.tr/api/InvoiceInbox/getInvoiceInboxWithHeaderInfoListForPeriod");
                   

                    // Bearer token'ı ekle
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue
                        ("Bearer", accessToken);

                    // JSON içeriğini HTTP içeriğine dönüştür
                    var content = new StringContent(jsonString,Encoding.UTF8, "application/json");

                    // PostAsync metodu ile isteği gönder ve cevabı al
                    var response = await client.PostAsync("", content);

                    // Sunucudan gelen yanıtı al
                    string responseJson = await response.Content.ReadAsStringAsync();


                    // JSON verisini XML'e dönüştür
                    JObject jsonObject = JObject.Parse(responseJson);
                    XDocument xmlData = JsonConvert.DeserializeXNode(responseJson, "Root");

                    // XML verisini DataSet'e aktar
                    DataSet dataSet = new DataSet();
                    using (var reader = xmlData.CreateReader())
                    {
                        dataSet.ReadXml(reader);
                    }

                    // DataSet içindeki 'data' tablosunu DataGridView'a bağla
                    ekran.DataSource = dataSet.Tables["data"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("XML dosyası yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        static class InvoiceHelper
        {

            public static List<Invoice> ParseInvoices(XDocument xmlData)
            {

                List<Invoice> invoices = new List<Invoice>();

                foreach (var dataElement in xmlData.Root.Elements("data"))
                {
                    Invoice invoice = new Invoice
                    {
                        Id = dataElement.Element("id")?.Value,
                        Profile = dataElement.Element("profile")?.Value,
                        InvoiceStatusText = dataElement.Element("invoiceStatusText")?.Value,
                        InvoiceType = dataElement.Element("invoiceType")?.Value,
                        Ettn = dataElement.Element("ettn")?.Value,
                        DocNo = dataElement.Element("docNo")?.Value,
                        DocDate = DateTime.Parse(dataElement.Element("docDate")?.Value),
                        PkAlias = dataElement.Element("pkAlias")?.Value,
                        GbAlias = dataElement.Element("gbAlias")?.Value,
                        VknTckn = dataElement.Element("vknTckn")?.Value,
                        AccountName = dataElement.Element("accountName")?.Value,
                        LineExtensionAmount = decimal.Parse(dataElement.Element("lineExtensionAmount")?.Value),
                        TaxExclusiveAmount = decimal.Parse(dataElement.Element("taxExclusiveAmount")?.Value),
                        TaxInclusiveAmount = decimal.Parse(dataElement.Element("taxInclusiveAmount")?.Value),
                        PayableRoundingAmount = decimal.Parse(dataElement.Element("payableRoundingAmount")?.Value),
                        PayableAmount = decimal.Parse(dataElement.Element("payableAmount")?.Value),
                        AllowanceTotalAmount = decimal.Parse(dataElement.Element("allowanceTotalAmount")?.Value),
                        TaxTotalTra = decimal.Parse(dataElement.Element("taxTotalTra")?.Value),
                        CurrencyCode = dataElement.Element("currencyCode")?.Value,
                        CurrencyRate = decimal.Parse(dataElement.Element("currencyRate")?.Value),
                        CreateDate = DateTime.Parse(dataElement.Element("createDate")?.Value),
                        ReferenceKey = dataElement.Element("referanceKey")?.Value
                    };

                    invoices.Add(invoice);
                }

                return invoices;
            }
        }

        private void ekran_Click(object sender, EventArgs e)
        {


        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Form2 fr2 = new Form2();
            fr2.ShowDialog();
        }
    }
}
