using MRPClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPTool.Helpers
{
    public static class Converters
    {
        public static void ConvertHTMLtoPDF(PurchaseOrdersItem purchaseOrderItem, Warehouses warehouses)
        {
            var file = AppDomain.CurrentDomain.BaseDirectory + @"Bill\Page.html";
            var tempHtmlTemplate = AppDomain.CurrentDomain.BaseDirectory + @"Bill\customhtml.html";
            var htmlTemplate = System.IO.File.ReadAllText(file);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Warehouse warehouse = warehouses.Items.Count > 0 ? warehouses.Items.SingleOrDefault() : new Warehouse();

            StringBuilder sb = new StringBuilder();

            string orderDate = String.Empty;
            string orderNumber = String.Empty;
            string requiredDate = String.Empty;
            string supplierCode = String.Empty;
            string referenceNumber = String.Empty;

            orderDate = purchaseOrderItem.OrderDate.ToString("dd/MM/yyyy");
            requiredDate = purchaseOrderItem.RequiredDate.ToString("dd/MM/yyyy");
            orderNumber = purchaseOrderItem.OrderNumber;
            supplierCode = purchaseOrderItem.Supplier == null ? "" : purchaseOrderItem.Supplier.SupplierCode;
            referenceNumber = purchaseOrderItem.Guid;

            double subTotal = purchaseOrderItem.SubTotal ?? 0;
            double taxTotal = purchaseOrderItem.TaxTotal ?? 0;
            double completeTotal = purchaseOrderItem.Total ?? 0;

            htmlTemplate = htmlTemplate.Replace("##DeliveryName##", warehouse == null ? "" : warehouse.StreetNo);
            htmlTemplate = htmlTemplate.Replace("##StreetAddress##", warehouse == null ? "" : warehouse.AddressLine1);
            htmlTemplate = htmlTemplate.Replace("##Suburb##", warehouse == null ? "" : warehouse.AddressLine2);
            htmlTemplate = htmlTemplate.Replace("##State##", warehouse == null ? "" : warehouse.Region);
            htmlTemplate = htmlTemplate.Replace("##PostalCode##", warehouse == null ? "" : warehouse.PostCode);
            htmlTemplate = htmlTemplate.Replace("##SupCode##", supplierCode);
            htmlTemplate = htmlTemplate.Replace("##PhoneNumber##", warehouse == null ? "" : warehouse.PhoneNumber);

            // Items in the Purchase Order
            foreach(var lineItem in purchaseOrderItem.PurchaseOrderLines)
            {
                var lineItemProduct = CommonCode.GetProductInformation(lineItem.Product != null ? lineItem.Product.ProductCode : "");
                string lineItemSupplierCode = (lineItemProduct.Items.Count > 0 && lineItemProduct.Items.FirstOrDefault().Supplier != null) ? lineItemProduct.Items.FirstOrDefault().Supplier.SupplierProductCode : "";
                string lineItemNumber = lineItem.LineNumber.ToString();
                string lineItemProductCode = lineItem.Product.ProductCode;
                string lineItemDescription = lineItem.Product.ProductDescription;
                string lineItemQuantity = lineItem.OrderQuantity.ToString();
                string lineItemUnits = lineItem.Product != null && lineItem.Product.UnitOfMeasure != null ? lineItem.Product.UnitOfMeasure.Name : "";
                double lineItemUnitPrice = lineItem.UnitPrice ?? 0;
                double lineItemTotal = lineItem.LineTotal ?? 0;
                double lineItemTax = lineItem.LineTax ?? 0;

                string htmlFormat = string.Format(
                    "<tr style='border-bottom:2px solid #ddd;'><th style = 'padding: 5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal; font-size: 13px;' >{0}</p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'>{1}</p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {2} </p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {3}</p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {4} </p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {5} </p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {6} </p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {7} </p></th>"
                    + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {8} </p></th></tr>",
                    lineItemNumber, lineItemSupplierCode, lineItemProductCode, lineItemDescription, lineItemQuantity, lineItemUnits, lineItemUnitPrice.ToString("0.00"), lineItemTotal.ToString("0.00"), lineItemTax.ToString("0.00")
                );

                sb.Append(htmlFormat);
            }

            htmlTemplate = htmlTemplate.Replace("##RequiredDate##", requiredDate);
            htmlTemplate = htmlTemplate.Replace("##OrderDate##", orderDate);
            htmlTemplate = htmlTemplate.Replace("##OrderNumber##", orderNumber);
            htmlTemplate = htmlTemplate.Replace("##SupplierCode##", supplierCode);
            htmlTemplate = htmlTemplate.Replace("##ReferenceNumber##", referenceNumber);
            htmlTemplate = htmlTemplate.Replace("##ReplaceBodyContent##", sb.ToString());

            htmlTemplate = htmlTemplate.Replace("##SubTotal##", subTotal.ToString("0.00"));
            htmlTemplate = htmlTemplate.Replace("##TaxTotal##", taxTotal.ToString("0.00"));
            htmlTemplate = htmlTemplate.Replace("##CompleteTotal##", completeTotal.ToString("0.00"));
            htmlTemplate = htmlTemplate.Replace("##Instruments##", purchaseOrderItem.Supplier.SupplierName);
            htmlTemplate = htmlTemplate.Replace("##Comments##", purchaseOrderItem.Comments);

            System.IO.File.WriteAllText(tempHtmlTemplate, htmlTemplate);

            SelectPdf.HtmlToPdf pdfFile = new SelectPdf.HtmlToPdf();
            pdfFile.Options.MarginRight = 5;
            pdfFile.Options.MarginRight = 5;

            SelectPdf.PdfDocument document = pdfFile.ConvertUrl(tempHtmlTemplate);

            string directory = path + "//Purchase Orders";

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            string pdfDocument = directory + "//Purchase Order_" + purchaseOrderItem.OrderNumber + "_" + DateTime.Now.ToString("yyyy.dd.MM").Replace("/", ".") + "-" + purchaseOrderItem.Supplier.SupplierName.Replace("/", "_") + ".pdf";
            
            document.Save(pdfDocument);
            document.Close();
        }
    }
}
