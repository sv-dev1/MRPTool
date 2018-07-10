using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{
   
    //public class Currency
    //{
    //    public string CurrencyCode { get; set; }
    //    public string Description { get; set; }
    //    public string Guid { get; set; }
    //    public DateTime LastModifiedOn { get; set; }
    //}

    public class SupplierItem
    {
        public string Guid { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string GSTVATNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccount { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MobileNumber { get; set; }
        public object DDINumber { get; set; }
        public string TollFreeNumber { get; set; }
        public string Email { get; set; }
        public Currency Currency { get; set; }
        public string Notes { get; set; }
        public bool Taxable { get; set; }
        public string XeroContactId { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class SupplierInfo
    {
        public List<SupplierItem> Items { get; set; }
    }
}
