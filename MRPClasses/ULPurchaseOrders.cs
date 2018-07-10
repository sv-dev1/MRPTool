using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{


    //public class Supplier
    //{
    //    public string Guid { get; set; }
    //    public string SupplierCode { get; set; }
    //    public string SupplierName { get; set; }
    //}



    public class Product
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public object Barcode { get; set; }
        public object PackSize { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public object Depth { get; set; }
        public object Weight { get; set; }
        public double? MinStockAlertLevel { get; set; }
        public double? MaxStockAlertLevel { get; set; }
        public object ReOrderPoint { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public bool NeverDiminishing { get; set; }
        public double? LastCost { get; set; }
        public double? DefaultPurchasePrice { get; set; }
        public double? DefaultSellPrice { get; set; }
        public object AverageLandPrice { get; set; }
        public bool Obsolete { get; set; }
        public object Notes { get; set; }
        public PriceTier SellPriceTier1 { get; set; }
        public PriceTier SellPriceTier2 { get; set; }
        public PriceTier SellPriceTier3 { get; set; }
        public PriceTier SellPriceTier4 { get; set; }
        public PriceTier SellPriceTier5 { get; set; }
        public PriceTier SellPriceTier6 { get; set; }
        public PriceTier SellPriceTier7 { get; set; }
        public PriceTier SellPriceTier8 { get; set; }
        public PriceTier SellPriceTier9 { get; set; }
        public PriceTier SellPriceTier10 { get; set; }
        public object XeroTaxCode { get; set; }
        public object XeroTaxRate { get; set; }
        public bool TaxablePurchase { get; set; }
        public bool TaxableSales { get; set; }
        public object XeroSalesTaxCode { get; set; }
        public object XeroSalesTaxRate { get; set; }
        public bool IsComponent { get; set; }
        public bool IsAssembledProduct { get; set; }
        public bool CanAutoAssemble { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public object XeroSalesAccount { get; set; }
        public object XeroCostOfGoodsAccount { get; set; }
        public object BinLocation { get; set; }
        public Supplier Supplier { get; set; }
        public object SourceId { get; set; }
        public string CreatedBy { get; set; }
        public object SourceVariantParentId { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class PriceTier
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }




    public class PurchaseOrdersItem
    {
        public string Guid { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public object CompletedDate { get; set; }
        public Supplier Supplier { get; set; }
        public string SupplierRef { get; set; }
        public string Comments { get; set; }
        public object Printed { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryStreetAddress { get; set; }
        public string DeliverySuburb { get; set; }
        public string DeliveryRegion { get; set; }
        public object DeliveryCity { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPostCode { get; set; }
        public Currency Currency { get; set; }
        public double ExchangeRate { get; set; }
        public Tax Tax { get; set; }
        public double TaxRate { get; set; }
        public string XeroTaxCode { get; set; }
        public double? SubTotal { get; set; }
        public double? TaxTotal { get; set; }
        public double? Total { get; set; }
        public double? TotalVolume { get; set; }
        public double? TotalWeight { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public double? BCSubTotal { get; set; }
        public double? BCTaxTotal { get; set; }
        public double? BCTotal { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public Warehouse Warehouse { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public double DiscountRate { get; set; }
    }
    public class PurchaseOrderLine
    {
        public string Guid { get; set; }
        public int LineNumber { get; set; }
        public Product Product { get; set; }
        public DateTime DueDate { get; set; }
        public double? OrderQuantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? LineTotal { get; set; }
        public object Volume { get; set; }
        public object Weight { get; set; }
        public object Comments { get; set; }
        public object ReceiptQuantity { get; set; }
        public double? BCUnitPrice { get; set; }
        public double? BCSubTotal { get; set; }
        public Tax Tax { get; set; }
        public double? LineTax { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public double? DiscountedUnitPrice { get; set; }
        public double? DiscountRate { get; set; }
    }
    public class ULPurchaseOrders
    {
        public Pagination Pagination { get; set; }
        public List<PurchaseOrdersItem> Items { get; set; }
    }

    public class ProductItems
    {
        public Pagination Pagination { get; set; }
        public List<Product> Items { get; set; }
    }
}
