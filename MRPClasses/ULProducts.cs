using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{

    public class Pagination
    {
        public int NumberOfItems { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }

    public class UnitOfMeasure
    {
        public string Guid { get; set; }
        public string Name { get; set; }
    }

    public class SellPriceTier1
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier2
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier3
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier4
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier5
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier6
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier7
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier8
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier9
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class SellPriceTier10
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ProductGroup
    {
        public string GroupName { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class ProductInfo
    {
        public List<ProductGroup> Items { get; set; }
    }

    public class Supplier
    {
        public string SupplierProductCode { get; set; }
        public object SupplierProductDescription { get; set; }
        public double? SupplierProductPrice { get; set; }
        public string Guid { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class Item
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string Barcode { get; set; }
        public double? PackSize { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public object Depth { get; set; }
        public object Weight { get; set; }
        public double? MinStockAlertLevel { get; set; }
        public double? MaxStockAlertLevel { get; set; }
        public object ReOrderPoint { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public bool? NeverDiminishing { get; set; }
        public double? LastCost { get; set; }
        public double? DefaultPurchasePrice { get; set; }
        public double? DefaultSellPrice { get; set; }
        public object CustomerSellPrice { get; set; }
        public double AverageLandPrice { get; set; }
        public bool? Obsolete { get; set; }
        public string Notes { get; set; }
        public object Images { get; set; }
        public object ImageUrl { get; set; }
        public SellPriceTier1 SellPriceTier1 { get; set; }
        public SellPriceTier2 SellPriceTier2 { get; set; }
        public SellPriceTier3 SellPriceTier3 { get; set; }
        public SellPriceTier4 SellPriceTier4 { get; set; }
        public SellPriceTier5 SellPriceTier5 { get; set; }
        public SellPriceTier6 SellPriceTier6 { get; set; }
        public SellPriceTier7 SellPriceTier7 { get; set; }
        public SellPriceTier8 SellPriceTier8 { get; set; }
        public SellPriceTier9 SellPriceTier9 { get; set; }
        public SellPriceTier10 SellPriceTier10 { get; set; }
        public string XeroTaxCode { get; set; }
        public double? XeroTaxRate { get; set; }
        public bool? TaxablePurchase { get; set; }
        public bool? TaxableSales { get; set; }
        public string XeroSalesTaxCode { get; set; }
        public double? XeroSalesTaxRate { get; set; }
        public bool? IsComponent { get; set; }
        public bool? IsAssembledProduct { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public string XeroSalesAccount { get; set; }
        public string XeroCostOfGoodsAccount { get; set; }
        public object PurchaseAccount { get; set; }
        public string BinLocation { get; set; }
        public Supplier Supplier { get; set; }
        public object AttributeSet { get; set; }
        public object SourceId { get; set; }
        public object SourceVariantParentId { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? IsBatchTracked { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string Guid { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ULProducts
    {
        public Pagination Pagination { get; set; }
        public List<Item> Items { get; set; }
    }



}
public class PriceTier
{
    public string Key { get; set; }
    public string Value { get; set; }
}