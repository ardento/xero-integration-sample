
namespace XeroIntegrationSample.Models
{
    public class InvoiceLineModel
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public string AccountCode { get; set; }
        public string TaxType { get; set; }
    }
}
