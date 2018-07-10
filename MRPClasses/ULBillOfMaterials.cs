using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{
    

    public class ProductBOM
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
    public object AverageLandPrice { get; set; }
    public bool? Obsolete { get; set; }
    public string Notes { get; set; }
    public object Images { get; set; }
    public object ImageUrl { get; set; }
    public object SellPriceTier1 { get; set; }
    public object SellPriceTier2 { get; set; }
    public object SellPriceTier3 { get; set; }
    public object SellPriceTier4 { get; set; }
    public object SellPriceTier5 { get; set; }
    public object SellPriceTier6 { get; set; }
    public object SellPriceTier7 { get; set; }
    public object SellPriceTier8 { get; set; }
    public object SellPriceTier9 { get; set; }
    public object SellPriceTier10 { get; set; }
    public object XeroTaxCode { get; set; }
    public object XeroTaxRate { get; set; }
    public bool? TaxablePurchase { get; set; }
    public bool? TaxableSales { get; set; }
    public object XeroSalesTaxCode { get; set; }
    public object XeroSalesTaxRate { get; set; }
    public bool? IsComponent { get; set; }
    public bool? IsAssembledProduct { get; set; }
    public ProductGroup ProductGroup { get; set; }
    public string XeroSalesAccount { get; set; }
    public string XeroCostOfGoodsAccount { get; set; }
    public object PurchaseAccount { get; set; }
    public object BinLocation { get; set; }
    public object Supplier { get; set; }
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

public class UnitOfMeasure2
    {
        public string Guid { get; set; }
        public string Name { get; set; }
    }

    public class ProductGroup2
    {
        public string GroupName { get; set; }
        public string Guid { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }


    public class Product2
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public object Barcode { get; set; }
        public double? PackSize { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public object Depth { get; set; }
        public object Weight { get; set; }
        public double? MinStockAlertLevel { get; set; }
        public double? MaxStockAlertLevel { get; set; }
        public object ReOrderPoint { get; set; }
        public UnitOfMeasure2 UnitOfMeasure { get; set; }
        public bool? NeverDiminishing { get; set; }
        public double? LastCost { get; set; }
        public double? DefaultPurchasePrice { get; set; }
        public double? DefaultSellPrice { get; set; }
        public object CustomerSellPrice { get; set; }
        public object AverageLandPrice { get; set; }
        public bool? Obsolete { get; set; }
        public string Notes { get; set; }
        public object Images { get; set; }
        public object ImageUrl { get; set; }
        public object SellPriceTier1 { get; set; }
        public object SellPriceTier2 { get; set; }
        public object SellPriceTier3 { get; set; }
        public object SellPriceTier4 { get; set; }
        public object SellPriceTier5 { get; set; }
        public object SellPriceTier6 { get; set; }
        public object SellPriceTier7 { get; set; }
        public object SellPriceTier8 { get; set; }
        public object SellPriceTier9 { get; set; }
        public object SellPriceTier10 { get; set; }
        public string XeroTaxCode { get; set; }
        public double? XeroTaxRate { get; set; }
        public bool? TaxablePurchase { get; set; }
        public bool? TaxableSales { get; set; }
        public object XeroSalesTaxCode { get; set; }
        public object XeroSalesTaxRate { get; set; }
        public bool? IsComponent { get; set; }
        public bool? IsAssembledProduct { get; set; }
        public ProductGroup2 ProductGroup { get; set; }
        public string XeroSalesAccount { get; set; }
        public string XeroCostOfGoodsAccount { get; set; }
        public object PurchaseAccount { get; set; }
        public object BinLocation { get; set; }
        public object Supplier { get; set; }
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



        public class BillOfMaterialsLine
    {
        public string Guid { get; set; }
        public int? LineNumber { get; set; }
        public Product2 Product { get; set; }
        public double? Quantity { get; set; }
        public double? WastageQuantity { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public double? LineTotalCost { get; set; }
    }

    public class BillOfMaterialsItem
    {
        public string Guid { get; set; }
        public ProductBOM Product { get; set; }
        public List<BillOfMaterialsLine> BillOfMaterialsLines { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public double? TotalCost { get; set; }
        public bool? Obsolete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
    public class ULBillOfMaterials
    {
        public Pagination Pagination { get; set; }
        public List<BillOfMaterialsItem> Items { get; set; }
    }
}
