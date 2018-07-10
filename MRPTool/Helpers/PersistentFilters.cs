using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPTool.Helpers
{
    public static class PersistentFilters
    {
        public static DataTable GetFilteredData(DataView dv, StringBuilder orderStatus, StringBuilder supplier, StringBuilder product)
        {
            if (orderStatus.Length != 0 && supplier.Length != 0 && product.Length != 0)
            {
                dv.RowFilter = String.Format("RequiredOrderStatus IN ({0}) AND SupplierCode IN ({1}) AND ProductGroup IN ({2})",
                    orderStatus.ToString(), supplier.ToString(), product.ToString());
            }
            else if (orderStatus.Length != 0 && supplier.Length != 0 && product.Length == 0)
            {
                dv.RowFilter = String.Format("RequiredOrderStatus IN ({0}) AND SupplierCode IN ({1})",
                    orderStatus.ToString(), supplier.ToString());
            }
            else if (orderStatus.Length != 0 && supplier.Length == 0 && product.Length != 0)
            {
                dv.RowFilter = String.Format("RequiredOrderStatus IN ({0}) AND ProductGroup IN ({1})",
                    orderStatus.ToString(), product.ToString());
            }
            else if (orderStatus.Length != 0 && supplier.Length == 0 && product.Length == 0)
            {
                dv.RowFilter = String.Format("RequiredOrderStatus IN ({0})",
                    orderStatus.ToString());
            }
            else if (orderStatus.Length == 0 && supplier.Length != 0 && product.Length != 0)
            {
                dv.RowFilter = String.Format("SupplierCode IN ({0}) AND ProductGroup IN ({1})",
                    supplier.ToString(), product.ToString());
            }
            else if (orderStatus.Length == 0 && supplier.Length != 0 && product.Length == 0)
            {
                dv.RowFilter = String.Format("SupplierCode IN ({0})",
                    supplier.ToString());
            }
            else if (orderStatus.Length == 0 && supplier.Length == 0 && product.Length != 0)
            {
                dv.RowFilter = String.Format("ProductGroup IN ({0})",
                    product.ToString());
            }
            else
            {
                dv.RowFilter = "";
            }

            return dv.ToTable();
        }
    }
}
