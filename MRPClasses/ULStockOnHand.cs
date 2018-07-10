using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{



    public class StockOnHandItem
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductGuid { get; set; }
      //  public object ProductSourceId { get; set; }
        public string ProductGroupName { get; set; }
        public string WarehouseId { get; set; }
        public string Warehouse { get; set; }
        public string WarehouseCode { get; set; }
      //  public object DaysSinceLastSale { get; set; }
        public Nullable<double> OnPurchase { get; set; }
        public Nullable<double> AllocatedQty { get; set; }
        public Nullable<double> AvailableQty { get; set; }
        public Nullable<double> QtyOnHand { get; set; }
        public Nullable<double> AvgCost { get; set; }
        public Nullable<double> TotalCost { get; set; }
        public string Guid { get; set; }
        public Nullable<DateTime> LastModifiedOn { get; set; }
    }

    public class ULStockOnHand
    {
        public Pagination Pagination { get; set; }
        public List<StockOnHandItem> Items { get; set; }
    }
}
