//******************************************************
//*
//* Copyright (c) 2015, Branko Pedisic // Ardento Pty Ltd
//* 
//* All rights reserved.
//* 
//* Redistribution and use in source and binary forms, with or without modification, 
//* are permitted provided that the following conditions are met:
//* 
//* 
//* 1. Redistributions of source code must retain the above copyright notice, 
//*    this list of conditions and the following disclaimer.
//* 
//* 2. Redistributions in binary form must reproduce the above copyright notice,
//*    this list of conditions and the following disclaimer in the documentation 
//*    and/or other materials provided with the distribution.
//* 
//* 
//* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//* ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//* IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
//* INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
//* NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
//* PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
//* ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
//* OF SUCH DAMAGE. 
//* 
//******************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xero.Api.Core.Model;
using Xero.Api.Infrastructure.Exceptions;
using XeroIntegrationSample.Models;

namespace XeroIntegrationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            UploadAPInvoicesToXero();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");

            while (!Console.KeyAvailable)
            {
                // wait for the "any" key
                // before exiting.
            }
        }

        private static void UploadAPInvoicesToXero()
        {
            try
            {
                var invoices = ReadData();

                if (invoices.Count == 0)
                {
                    Console.WriteLine("No invoices found.");
                    return;
                }

                var api = new Xero.Applications.Private.Core();

                List<Invoice> xeroInvoiceBatch = new List<Invoice>();

                foreach (var invoice in invoices)
                {
                    var xeroInvoice = new Invoice();
                    xeroInvoice.Type = global::Xero.Api.Core.Model.Types.InvoiceType.AccountsPayable;
                    xeroInvoice.Contact = new Contact() { Name = invoice.CustomerName };
                    xeroInvoice.Number = invoice.InvoiceNumber;
                    xeroInvoice.Date = invoice.InvoiceDate;
                    xeroInvoice.DueDate = invoice.DueDate;
                    xeroInvoice.Reference = invoice.Reference;

                    foreach (var line in invoice.Lines)
                    {
                        var xeroInvoiceLine = new LineItem();
                        xeroInvoiceLine.ItemCode = line.ItemCode;
                        xeroInvoiceLine.Description = line.Description;
                        xeroInvoiceLine.Quantity = line.Quantity;
                        xeroInvoiceLine.UnitAmount = line.UnitAmount;
                        xeroInvoiceLine.AccountCode = line.AccountCode;
                        xeroInvoiceLine.TaxType = line.TaxType;

                        xeroInvoice.Items = new List<LineItem>();
                        xeroInvoice.Items.Add(xeroInvoiceLine);
                    }

                    xeroInvoiceBatch.Add(xeroInvoice);

                    Console.WriteLine("Invoice \"" + invoice.InvoiceNumber
                        + "\" added to the batch.");
                }

                if (xeroInvoiceBatch.Count == 0)
                {
                    Console.WriteLine("No invoices were added to the batch. "
                        + "Nothing to send.");
                }
                else
                {
                    Console.WriteLine("Sending batch to Xero...");
                    
                    var items = api.Create(xeroInvoiceBatch);

                    Console.WriteLine(xeroInvoiceBatch.Count
                        + " invoices were successfully sent to Xero.");
                }
            }
            catch (ValidationException vex)
            {
                string error = vex.Message + " (" + vex.ErrorNumber + ")";

                foreach (var valError in vex.ValidationErrors)
                {
                    error += "\r\n"
                        + valError.Message;
                }

                Console.WriteLine(error);
            }
            catch (XeroApiException apiEx)
            {
                string error = apiEx.Message + " (" + apiEx.Code + ")";

                if (apiEx.InnerException != null)
                    error += "\r\n" + apiEx.InnerException.Message;

                Console.WriteLine(error);
            }
            catch (Exception ex)
            {
                string error = ex.Message;

                if (ex.InnerException != null)
                    error += "\r\n" + ex.InnerException.Message;

                Console.WriteLine(error);
            }
        }

        private static List<InvoiceModel> ReadData()
        {
            var invoices = new List<InvoiceModel>();

            try
            {
                XDocument doc = XDocument.Load(@"..\..\Resources\Data\TestData.xml");

                invoices = doc.Root
                    .Elements("Invoice")
                    .Select(x => new InvoiceModel
                    {
                        CustomerName = (string)x.Element("CustomerName"),
                        InvoiceNumber = (string)x.Element("InvoiceNumber"),
                        InvoiceDate = DateTime.Parse((string)x.Element("InvoiceDate")),
                        DueDate = DateTime.Parse((string)x.Element("DueDate")),
                        Reference = (string)x.Element("Reference"),
                        Lines = x.Descendants("Line")
                          .Select(d => new InvoiceLineModel
                          {
                              ItemCode = (string)d.Element("ItemCode"),
                              Description = (string)d.Element("Description"),
                              Quantity = decimal.Parse((string)d.Element("Quantity")),
                              UnitAmount = decimal.Parse((string)d.Element("UnitAmount")),
                              AccountCode = (string)d.Element("AccountCode"),
                              TaxType = (string)d.Element("TaxType")
                          }).ToList()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in read data. " + ex.Message);
            }

            return invoices;
        }
    }
}
