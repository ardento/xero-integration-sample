using System;
using System.Collections.Generic;

namespace XeroIntegrationSample.Models
{
    public class InvoiceModel
    {
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Reference { get; set; }

        public List<InvoiceLineModel> Lines { get; set; }
    }
}