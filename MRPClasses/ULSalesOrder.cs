using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{
   

    public class SalesOrderProduct
    {
        public string Guid { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
    }

    public class SalesOrderLine
    {
        public int LineNumber { get; set; }
        public string LineType { get; set; }
        public SalesOrderProduct Product { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public double? OrderQuantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public double? LineTotal { get; set; }
        public object Volume { get; set; }
        public object Weight { get; set; }
        public object Comments { get; set; }
        public double? AverageLandedPriceAtTimeOfSale { get; set; }
        public double? TaxRate { get; set; }
        public double? LineTax { get; set; }
        public string XeroTaxCode { get; set; }
        public double? BCUnitPrice { get; set; }
        public double? BCLineTotal { get; set; }
        public double? BCLineTax { get; set; }
        public object LineTaxCode { get; set; }
        public object XeroSalesAccount { get; set; }
        public string Guid { get; set; }
        public Nullable<DateTime> LastModifiedOn { get; set; }
        public object SerialNumbers { get; set; }
        public object BatchNumbers { get; set; }
    }

    public class Customer
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int CurrencyId { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
    public  class Warehouses
    {
        public List<Warehouse> Items { get; set; }

    }
    public class Warehouse
    {
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public bool IsDefault { get; set; }
        public string StreetNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public object City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public object FaxNumber { get; set; }
        public string MobileNumber { get; set; }
        public object DDINumber { get; set; }
        public string ContactName { get; set; }
        public bool Obsolete { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class Currency
    {
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class Tax
    {

        public string TaxCode { get; set; }
        public object Description { get; set; }
        public double? TaxRate { get; set; }
        public bool CanApplyToExpenses { get; set; }
        public bool CanApplyToRevenue { get; set; }
        public bool Obsolete { get; set; }
        public string Guid { get; set; }
        public object LastModifiedOn { get; set; }
    }
    public class SalesPerson
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool Obsolete { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
    public class SalesItem
    {
        public List<SalesOrderLine> SalesOrderLines { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public string OrderStatus { get; set; }
        public Customer Customer { get; set; }
        public string CustomerRef { get; set; }
        public string Comments { get; set; }
        public Warehouse Warehouse { get; set; }
        public object ReceivedDate { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryStreetAddress { get; set; }
        public string DeliveryStreetAddress2 { get; set; }
        public string DeliverySuburb { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryRegion { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPostCode { get; set; }
        public Currency Currency { get; set; }
        public double ExchangeRate { get; set; }
        public double DiscountRate { get; set; }
        public Tax Tax { get; set; }
        public double? TaxRate { get; set; }
        public string XeroTaxCode { get; set; }
        public double SubTotal { get; set; }
        public double TaxTotal { get; set; }
        public double Total { get; set; }
        public double? TotalVolume { get; set; }
        public double? TotalWeight { get; set; }
        public double? BCSubTotal { get; set; }
        public double? BCTaxTotal { get; set; }
        public double? BCTotal { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public bool AllocateProduct { get; set; }
        public object SalesOrderGroup { get; set; }
        public string DeliveryMethod { get; set; }
        public SalesPerson SalesPerson { get; set; }
        public bool SendAccountingJournalOnly { get; set; }
        public object SourceId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string Guid { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class ULSalesOrder
    {
        public Pagination Pagination { get; set; }
        public List<SalesItem> Items { get; set; }
    }
}
