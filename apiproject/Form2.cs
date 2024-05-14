using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apiproject
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private async Task ShowPdfAsync()
        {
            // API adresi
            string apiUrl = "https://edocumentapi.mysoft.com.tr/api/InvoiceInbox/getInvoiceInboxPdfAsZip";

            // Parametre
            string invoiceETTN = "03383a0f-f90f-461a-99f5-5e8cf150b320";

            // Query string oluştur
            string queryString = $"?invoiceETTN={invoiceETTN}";

            // HTTP isteği oluştur
            var httpClient = new HttpClient();

            // Bearer token ekleyin
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "AhSIEd1Jh6KrFaEu3Lzs2eFMIjO3ru5Cqd5HsUj9UpydvS2JR2IM0UKCDHOQPPVT7oTLX85025W7TnTpBXvcB05HdqZxbYi06vPaQkxEl2PqhVraHEHPoIdZVg7ODU5ERaJDpyBfanuVfKc7mGcFqa7FeUCcMY9KANcsly9ADXnouuXDusgC_Lqk-BBJ9xKRLrxaojcW5s5boeHRzuFDOxk2OvTRUTlCVa9elk6wQIaLUHDe9I76PGaUBw0sgzb4fd-AbM6MxzfTaKEP6Xug8d_LsHQO8Y0bZqL56GUmMrmyUYfxh-b9WDRPkcNJAw17Ac8fFWIiPN9oJjC60RKn76PB-l4X6Zj7K6lhK8RWmL18JrOC-2iA8EADu4iwce30x9GPZ6DdmSv6ZCsyHjA2RTycHyE0t10LICJLyokF5T71vacZQykdL_2nI148Pbnxo-3bN74glJaDzEjoaSu9kuZq368gs3AZdeKH0YKAUdge_vfw6QsVj88xBdZxZ73ELfo9xrlNA6wZ9p0vCRkrrigyFvZyoynpk9AXVKdVObINsjfpYAqwIB8hLd9RMBNCAFAbyasfPuDx2c-54oaMhcJ5ZbtIpuncLZrdm3r1YqTD74jaUpSSb47UaAPiOk24Vx17z_bnL6kwVuygI3UgZDP5UdnkR0G767HPjDCIVtfi1w1q30f-wPmseMknUvdvPRZ727lZaCGDAZqX1A68vEEUW7pqhx6Emaz3gzV0Jv8pmXC3ds4QN50UW_UPQRBk5tUpmgIopkIaxbLs_ZaYUCxoM9Cok6E5EqYXeMBfcEM");

            // HTTP isteğini gönder ve yanıtı al
            var response = await httpClient.GetAsync(apiUrl + queryString);

            // Yanıtın içeriğini al
            PdfResponse pdfResponse = JsonConvert.DeserializeObject<PdfResponse>(await response.Content.ReadAsStringAsync());

            // Base64 stringini byte dizisine dönüştür
            //// Base64 stringini byte dizisine dönüştür
            //byte[] pdfBytes = Convert.FromBase64String(pdfResponse.data);

            // Base64 stringini byte dizisine dönüştür
            byte[] zipBytes = Convert.FromBase64String(pdfResponse.data);

            
           
            // Byte dizisini MemoryStream'e aktar
            using (MemoryStream zipStream = new MemoryStream(zipBytes))
            {
                // Zip dosyasını çözümle
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    // Zip dosyasındaki her bir giriş için işlem yap
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        // Girişin ismi
                        string entryName = entry.FullName;

                        // Girişin içeriğini bayt dizisine yükle
                        using (MemoryStream entryMemoryStream = new MemoryStream())
                        {
                            using (Stream entryStream = entry.Open())
                            {
                                entryStream.CopyTo(entryMemoryStream);
                            }

                            // Bayt dizisini PDFViewer kontrolüne yükle
                            entryMemoryStream.Seek(0, SeekOrigin.Begin); // Bellek akışını başa sıfırla
                            pdfViewer1.LoadDocument(entryMemoryStream);
                        }
                    }
                }
            }




        }




        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            await ShowPdfAsync();
        }
    }
}
