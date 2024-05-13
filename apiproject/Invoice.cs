using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apiproject
{

    public class Invoice
    {
        public string Id { get; set; }
        public string Profile { get; set; }
        public string InvoiceStatusText { get; set; }
        public string InvoiceType { get; set; }
        public string Ettn { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string PkAlias { get; set; }
        public string GbAlias { get; set; }
        public string VknTckn { get; set; }
        public string AccountName { get; set; }
        public decimal LineExtensionAmount { get; set; }
        public decimal TaxExclusiveAmount { get; set; }
        public decimal TaxInclusiveAmount { get; set; }
        public decimal PayableRoundingAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal AllowanceTotalAmount { get; set; }
        public decimal TaxTotalTra { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime CreateDate { get; set; }
        public string ReferenceKey { get; set; }
    }
}