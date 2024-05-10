using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace apiproject
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static async Task Main(string[] args)
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
                new KeyValuePair<string, string>("grant_type", grantType)
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
                    // JSON verisini dinamik olarak oluştur
                    JObject jsonContent = new JObject();
                    jsonContent["startDate"] = "2024-05-01T00:00:00";
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
                        var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

                        // PostAsync metodu ile isteği gönder ve cevabı al
                        var response = await client.PostAsync("", content);

                        // Sunucudan gelen yanıtı al
                        string responseJson = await response.Content.ReadAsStringAsync();

                        // JSON verisini XML'e dönüştür
                        JObject jsonObject = JObject.Parse(responseJson);
                        XDocument xmlData = JsonConvert.DeserializeXNode(responseJson, "Root");


                        // Konsolda XML verisini göster
                        // Console.WriteLine(xmlData.ToString());

                        // XML verisini faturalar listesine dönüştür
                        List<Invoice> invoices = InvoiceHelper.ParseInvoices(xmlData);

                        // Faturaları işle veya görüntüle
                        foreach (var invoice in invoices)
                        {
                            Console.WriteLine($"Fatura No: {invoice.DocNo}");
                            Console.WriteLine($"Profil: {invoice.Profile}");
                            Console.WriteLine($"Fatura Durumu: {invoice.InvoiceStatusText}");
                            Console.WriteLine($"Fatura Tipi: {invoice.InvoiceType}");
                            Console.WriteLine($"ETTN: {invoice.Ettn}");
                            Console.WriteLine($"Fatura Tarihi: {invoice.DocDate}");
                            Console.WriteLine($"PK Alias: {invoice.PkAlias}");
                            Console.WriteLine($"GB Alias: {invoice.GbAlias}");
                            Console.WriteLine($"VKN/TCKN: {invoice.VknTckn}");
                            Console.WriteLine($"Hesap Adı: {invoice.AccountName}");
                            Console.WriteLine($"Satır Uzunluğu Tutarı: {invoice.LineExtensionAmount}");
                            Console.WriteLine($"Vergi Hariç Tutar: {invoice.TaxExclusiveAmount}");
                            Console.WriteLine($"Vergi Dahil Tutar: {invoice.TaxInclusiveAmount}");
                            Console.WriteLine($"Ödenecek Yuvarlama Tutarı: {invoice.PayableRoundingAmount}");
                            Console.WriteLine($"Ödenecek Tutar: {invoice.PayableAmount}");
                            Console.WriteLine($"Toplam İndirim Tutarı: {invoice.AllowanceTotalAmount}");
                            Console.WriteLine($"Toplam Vergi: {invoice.TaxTotalTra}");
                            Console.WriteLine($"Para Birimi Kodu: {invoice.CurrencyCode}");
                            Console.WriteLine($"Para Birimi Kuru: {invoice.CurrencyRate}");
                            Console.WriteLine($"Oluşturulma Tarihi: {invoice.CreateDate}");
                            Console.WriteLine($"Referans Anahtarı: {invoice.ReferenceKey}");
                            Console.WriteLine();
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Token alınamadı.");
                }
            }
        }
    
    public static class InvoiceHelper
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
    }
}
