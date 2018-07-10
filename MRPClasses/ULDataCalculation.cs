using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPClasses
{
    public class ULDataCalculation
    {
        ////////Client Credentials
        private const string ApiId = "0b9de772-3959-4330-a518-f4a9e035489a";
        private const string ApiKey = "egwTWe7rCPq8h6okMsIcP34apJcdw5VFNPeOgvvsSUTHqEfNeEMdvVopElp1WtmNKYebYCTadLKaFnAKobwKw==";
        ////Test Credentials
        //private const string ApiId = "18059178-6dc1-4942-92d9-c9052d9911f6";
        // private const string ApiKey = "jFaajEzTzQROnLayTvAW4nliSHYSe0hsQnm4VuD8B6S2Q3mJwI90cT7Vj6oJYckvijjvi1HeBlkEOnsnw==";        
        List<clsStockOrderU> objlistOrder = new List<clsStockOrderU>();
        public static List<Item> productList = new List<Item>();

        public DataTable StockOrders(int sixWeekWindow)
        {
            DataTable dtStockOrders = new DataTable();
            DataTable dtBOM = new DataTable();
            DataTable dtExceptionBOMItems = new DataTable();
            DataTable dtExceptionProducts = new DataTable();

            objlistOrder = new List<clsStockOrderU>();
            dtStockOrders.Columns.Add("ProductCode");
            dtStockOrders.Columns.Add("ProductDescription");
            dtStockOrders.Columns.Add("RequiredOrderStatus");
            dtStockOrders.Columns.Add("RequiredOrderDate");
            dtStockOrders.Columns.Add("SupplierName");
            dtStockOrders.Columns.Add("SupplierCode");
            dtStockOrders.Columns.Add("Quantity");//PackSize
            dtStockOrders.Columns.Add("OrderbyStatus");
            dtStockOrders.Columns.Add("Trigger");
            dtStockOrders.Columns.Add("StockOnHand");
            dtStockOrders.Columns.Add("StockOnPurchase");
            dtStockOrders.Columns.Add("UpcomingOrderQuantities");
            dtStockOrders.Columns.Add("UpcomingOrderQuantities2");
            dtStockOrders.Columns.Add("MinStockQty");
            dtStockOrders.Columns.Add("ProductGroup");
            List<Item> ItemsProducts = new List<Item>();
            List<StockOnHandItem> StockOnHand = new List<StockOnHandItem>(); ;

            /*try
            {
                StockOnHand = new List<StockOnHandItem>();
            }
            catch (Exception ex)
            {
                var sdsd = ex.Message;
            }*/

            string responseProducts = UnLeashedMain.GetJson("Products/1", ApiId, ApiKey, "");
            string responseBillOfMaterials = UnLeashedMain.GetJsonForBillOfMaterials("BillOfMaterials/1", ApiId, ApiKey, "");
            var dtBOMa = JsonConvert.DeserializeObject<ULBillOfMaterials>(responseBillOfMaterials);
            List<BillOfMaterialsItem> ItemsBOMs = new List<BillOfMaterialsItem>();
            ItemsBOMs.AddRange(dtBOMa.Items);

            for (int i = 2; i <= dtBOMa.Pagination.NumberOfPages; i++)
            {
                try
                {
                    responseBillOfMaterials = UnLeashedMain.GetJsonForBillOfMaterials("BillOfMaterials/" + i, ApiId, ApiKey, "");
                    var TempdtBOMa = JsonConvert.DeserializeObject<ULBillOfMaterials>(responseBillOfMaterials);
                    ItemsBOMs.AddRange(TempdtBOMa.Items);
                }
                catch (Exception ex)
                {

                }
            }

            // var listBOM1 = BillOfMaterialApi(ItemsBOMs);

            string responseSalesOrders = UnLeashedMain.GetJson("SalesOrders/1", ApiId, ApiKey, "");
            var dtSalesOrders = JsonConvert.DeserializeObject<ULSalesOrder>(responseSalesOrders);
            //  var fdfdfd = dtBOMa;
            ExtendSalesOrder(dtSalesOrders.Items);
            //  var fdsf = listBOM1.Where(m => m.Quantity > 500).ToList();
            //  var fdfd = fdsf;

            var dtProducts = JsonConvert.DeserializeObject<ULProducts>(responseProducts);
            ItemsProducts.AddRange(dtProducts.Items);
            for (int i = 2; i <= dtProducts.Pagination.NumberOfPages; i++)
            {
                try
                {
                    responseProducts = UnLeashedMain.GetJson("Products/" + i, ApiId, ApiKey, "");
                    dtProducts = JsonConvert.DeserializeObject<ULProducts>(responseProducts);
                    ItemsProducts.AddRange(dtProducts.Items);
                }
                catch (Exception)
                {


                }

            }
            ItemsProducts = ItemsProducts.Where(m => m.Obsolete == false).ToList();
            productList = ItemsProducts;
            //var fsdf = ItemsProducts.Where(m => m.ProductCode == "090-0130").ToList();
            //var dfsf = fsdf;
            //var ffdsdf = listSalesOrderLine.Where(m => m.ProductCode == "090-0130").ToList();
            //var dfsdsf = ffdsdf;
            ////string responsePurchaseOrders = UnLeashedMain.GetJson("PurchaseOrders", ApiId, ApiKey, "");
            string responseStockOnHand = UnLeashedMain.GetJson("StockOnHand/1", ApiId, ApiKey, "");

            // var dtPurchaseOrders = JsonConvert.DeserializeObject<ULPurchaseOrders>(responsePurchaseOrders);

            var dtSOHList = JsonConvert.DeserializeObject<ULStockOnHand>(responseStockOnHand);
            StockOnHand.AddRange(dtSOHList.Items);
            for (int i = 2; i <= dtSOHList.Pagination.NumberOfPages; i++)
            {
                try
                {
                    responseStockOnHand = UnLeashedMain.GetJson("StockOnHand/" + i, ApiId, ApiKey, "");
                    dtSOHList = JsonConvert.DeserializeObject<ULStockOnHand>(responseStockOnHand);
                    StockOnHand.AddRange(dtSOHList.Items);
                }
                catch (Exception)
                {


                }
            }



            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


            #region "Reading the Exception Files"
            string completePath = path + @"\Example Files";

            DirectoryInfo di = new DirectoryInfo(completePath);
            FileInfo[] files = di.GetFiles("*.csv");

            foreach (string file in Directory.EnumerateFiles(completePath, "*.csv"))
            {
                // string contents = File.ReadAllText(file);
                if (file.ToLower().IndexOf("exceptionassembledproduct") > -1)
                {
                    dtExceptionBOMItems = ConvertCSVtoDataTable(file, true);
                }
                else if (file.ToLower().IndexOf("exceptionproductlist") > -1)
                {
                    dtExceptionProducts = ConvertCSVtoDataTable(file, true);
                }
            }
            #endregion


            List<string> exceptionItemsList = GetExceptionItemsList(dtExceptionBOMItems);
            List<string> exceptionProductsList = GetExceptionProductsList(dtExceptionProducts);

            var listBOM = BillOfMaterialApi(ItemsBOMs);
            //var ggfd = listBOM.Where(m => m.ComponentProductCode == "090-0130").ToList();
            //var dffdgsf = ggfd;

            // Stock ON Hand without null values
            var updatedStockOnHand = StockOnHand.Where(r => r != null).Select(r => r).ToList();
            var updatedSalesOrderLine = listSalesOrderLine.Where(r => r != null).Select(r => r).ToList();
            var updatedItemsProducts = ItemsProducts.Where(r => r != null).Select(r => r).ToList();
            var results = (from tableProducts in updatedItemsProducts
                           join tableSOHList in updatedStockOnHand on tableProducts.ProductCode equals tableSOHList.ProductCode into soh
                           from SOHList in soh.DefaultIfEmpty()
                           join tableSalesOrders in updatedSalesOrderLine on tableProducts.ProductCode equals tableSalesOrders.ProductCode into so
                           from SalesOrders in so.DefaultIfEmpty()
                               // where Convert.ToString(SalesOrders == null ? "" : SalesOrders.OrderStatus) != "Completed" && Convert.ToString(SalesOrders == null ? "" : SalesOrders.OrderStatus) != "Deleted"
                           select new clsStockOrderU
                           {
                               ProductCode = Convert.ToString(tableProducts == null ? "" : tableProducts.ProductCode),
                               //ProductCode = String.IsNullOrEmpty(tableProducts.ProductCode) ? String.Empty : tableProducts.ProductCode,
                               BarCode = tableProducts.Barcode,//LeadTime
                               MinStockAlertLevel = tableProducts.MinStockAlertLevel,//Minimum Stock level
                               PackSize = tableProducts.PackSize,
                               SupplierCode = Convert.ToString(tableProducts == null ? "" : tableProducts.Supplier == null ? "" : tableProducts.Supplier.SupplierCode),//Suplier info
                               SupplierName = Convert.ToString(tableProducts == null ? "" : tableProducts.Supplier == null ? "" : tableProducts.Supplier.SupplierName),//Suplier info
                               ProductDescription = Convert.ToString(tableProducts.ProductDescription).Replace(",", "").Replace("=", ""),
                               OnPurchase = SOHList == null ? 0 : SOHList.OnPurchase.HasValue ? SOHList.OnPurchase.Value : 0,// Stock on purchase
                               QtyOnHand = SOHList == null ? 0 : SOHList.QtyOnHand.HasValue ? SOHList.QtyOnHand.Value : 0,//Stock on Hand
                               OrderStatus = Convert.ToString(SalesOrders == null ? "" : SalesOrders.OrderStatus).ToLower(),
                               RequiredDate = (SalesOrders == null ? DateTime.Now : SalesOrders.RequiredDate),
                               ProductGroup = Convert.ToString(tableProducts == null ? "" : tableProducts.ProductGroup == null ? "" : tableProducts.ProductGroup.GroupName),//group info
                               OrderQuantity = Convert.ToDecimal(Convert.ToDecimal(SalesOrders == null ? "0" : Convert.ToString(SalesOrders.OrderQuantity)).ToString("G")),//Order Quantity
                               //AssembledProductCode = Convert.ToString(r == null ? "" : r["Assembled Product Code"]),
                               Quantity = 0.0M,//Convert.ToDecimal(listBOM.Where(mm => mm.AssembledProductCode == Convert.ToString(tableProducts["Product Code"])).FirstOrDefault().Quantity),
                           }).ToList();



            foreach (var item in results)
            {
                var resultFinal = listBOM.Where(mm => mm.AssembledProductCode == item.ProductCode).ToList();
                if (resultFinal.Count > 0)
                {
                    // decimal ComparisonResult = 0.0M;
                    foreach (var itemt in resultFinal)
                    {
                        var resultFinall = listBOM.Where(mm => mm.AssembledProductCode == itemt.ComponentProductCode).ToList();

                        if (resultFinall.Count > 0)
                        {
                            clsStockOrderU objModelOrder = new clsStockOrderU();
                            objModelOrder.ProductCode = itemt.ComponentProductCode;
                            objModelOrder.Quantity = itemt.Quantity;
                            objModelOrder.RequiredDate = item.RequiredDate;
                            objModelOrder.OrderQuantity = itemt.Quantity * item.OrderQuantity;
                            objlistOrder.Add(objModelOrder);
                            objModelOrder = null;
                            load_categories(resultFinall, listBOM, itemt.Quantity * item.OrderQuantity, item.RequiredDate);
                        }
                        else
                        {

                            clsStockOrderU objModelOrder = new clsStockOrderU();
                            objModelOrder.ProductCode = itemt.ComponentProductCode;
                            objModelOrder.Quantity = itemt.Quantity;
                            objModelOrder.RequiredDate = item.RequiredDate;
                            objModelOrder.OrderQuantity = itemt.Quantity * item.OrderQuantity;
                            objlistOrder.Add(objModelOrder);
                            objModelOrder = null;
                        }
                    }
                }
            }
            var detailResult = objlistOrder.Concat(results);//.Where(m => m.ProductCode == "M5033");
            foreach (var item in detailResult)
            {
                //DataRow[] dr = dtProducts.Select("[Product Code]='" + item.ProductCode + "'");
                //if (dr.Count() > 0)
                //{
                //    item.PackSize = Convert.ToString(dr[0]["Pack Size"]);
                //    item.SupplierName = Convert.ToString(dr[0]["Supplier Name"]);//"Supplier Name";
                //    item.SupplierCode = Convert.ToString(dr[0]["Supplier Code"]); //"Supplier Code";
                //}
                var product = ItemsProducts.Where(m => m.ProductCode == item.ProductCode).FirstOrDefault();
                if (product != null)
                {
                    item.PackSize = product.PackSize;
                    item.SupplierName = product.Supplier == null ? "" : product.Supplier.SupplierName;
                    item.SupplierCode = product.Supplier == null ? "" : product.Supplier.SupplierCode;
                }
            }

            var CombinedListIndividualProduct = detailResult
                .GroupBy(x => new { x.ProductCode })
                .Select(cl => new
                {
                    ProductCode = cl.Key,
                    ProductList = cl.ToList()
                }).ToList();

            foreach (var item1 in CombinedListIndividualProduct)
            {
                var result11 = item1.ProductList.Where(m => m.RequiredDate > DateTime.Now.AddDays(sixWeekWindow)).ToList();
                var result21 = item1.ProductList.Where(m => m.RequiredDate < DateTime.Now.AddDays(sixWeekWindow)).ToList();

                //var fdsfe = result21;

                if (result11.Count > 0 && result21.Count > 0)
                {
                    var lessthan6Weeks = result21
                        .GroupBy(l => l.ProductCode)
                        .Select(cl => new clsStockOrderU
                        {
                            ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                            BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "35" : cl.Where(m => m.BarCode != null).FirstOrDefault().BarCode,
                            MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault().MinStockAlertLevel,
                            PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.PackSize != null).FirstOrDefault().PackSize,
                            OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.OnPurchase != null).Max(m => m.OnPurchase),
                            QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.QtyOnHand != null).Max(m => m.QtyOnHand),
                            OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).FirstOrDefault().OrderStatus,
                            RequiredDate = cl.Where(m => m.RequiredDate != null) == null ? DateTime.Now : cl.Where(m => m.RequiredDate != null).Min(m => m.RequiredDate),
                            SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).FirstOrDefault().SupplierCode,
                            SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).FirstOrDefault().SupplierName,
                            ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).FirstOrDefault().ProductDescription,
                            ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).FirstOrDefault().ProductGroup,
                            OrderQuantity = cl.Sum(c => c.OrderQuantity),
                            ComparisonResult = 0.00M,
                            Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity,//BOM
                        }).ToList();

                    var greatethan6Weeks = result11
                        .GroupBy(l => l.ProductCode)
                        .Select(cl => new clsStockOrderU
                        {
                            ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                            BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "35" : cl.Where(m => m.BarCode != null).FirstOrDefault().BarCode,
                            MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault().MinStockAlertLevel,
                            PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.PackSize != null).FirstOrDefault().PackSize,
                            OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.OnPurchase != null).Max(m => m.OnPurchase),
                            QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.QtyOnHand != null).Max(m => m.QtyOnHand),
                            OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).FirstOrDefault().OrderStatus,
                            RequiredDate = cl.Where(m => m.RequiredDate != null) == null ? DateTime.Now : cl.Where(m => m.RequiredDate != null).Min(m => m.RequiredDate),
                            SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).FirstOrDefault().SupplierCode,
                            SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).FirstOrDefault().SupplierName,
                            ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).FirstOrDefault().ProductDescription,
                            ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).FirstOrDefault().ProductGroup,
                            OrderQuantity = cl.Sum(c => c.OrderQuantity),
                            ComparisonResult = 0.00M,
                            Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity,//BOM
                        }).ToList();

                    foreach (var item in lessthan6Weeks)
                    {
                        DataRow dr = dtStockOrders.NewRow();
                        string sBOMResult = "order now";
                        var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).ToList();
                        //var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).FirstOrDefault();

                        //if (IfBOM.Count > 0)
                        //{
                        //    sBOMResult = "no order required";
                        //}

                        if (IfBOM.Count > 0)
                        {
                            // Check if the BOM item is in the BOM Exception List
                            var flaggedBOMItems = exceptionItemsList.Any(itemCode => itemCode == item.ProductCode);

                            if (!flaggedBOMItems)
                            {
                                sBOMResult = "no order required";
                            }
                        }

                        double QtyOnHand = item.QtyOnHand;
                        double OnPurchase = item.OnPurchase;
                        double MinStockAlertLevel = Convert.ToDouble(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel);
                        var GOrderQuanity = greatethan6Weeks.Where(m => m.ProductCode == item.ProductCode).FirstOrDefault();

                        if (QtyOnHand + OnPurchase < MinStockAlertLevel)
                        {
                            //order now
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = sBOMResult;
                            dr[3] = DateTime.Now.ToShortDateString();
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                            dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                        else if (QtyOnHand + OnPurchase - Convert.ToDouble(item.OrderQuantity) < MinStockAlertLevel)
                        {
                            //order now
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = sBOMResult;
                            dr[3] = DateTime.Now.ToShortDateString();
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                        else if (QtyOnHand + OnPurchase - Convert.ToDouble(item.OrderQuantity) - (GOrderQuanity != null ? Convert.ToDouble(GOrderQuanity.OrderQuantity) : 0) < MinStockAlertLevel)
                        {

                            if (item.BarCode != "" && Convert.ToDouble(item.BarCode) <= 35)
                            {
                                //order now
                                dr[0] = item.ProductCode;
                                dr[1] = item.ProductDescription;
                                dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                dr[3] = GOrderQuanity.RequiredDate.AddDays(-42).ToShortDateString();
                                dr[4] = item.SupplierName;
                                dr[5] = item.SupplierCode;
                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                dr[7] = sBOMResult == "order now" ? 2 : 3;
                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                dr[14] = item.ProductGroup;

                                dtStockOrders.Rows.Add(dr);
                            }
                            else if (item.BarCode != "" && Convert.ToDouble(item.BarCode) > 35)
                            {
                                dr[0] = item.ProductCode;
                                dr[1] = item.ProductDescription;
                                dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                dr[3] = GOrderQuanity.RequiredDate.AddDays(-Convert.ToInt32(item.BarCode) - 14).ToShortDateString(); //DateTime.Now.AddDays(sixWeekWindow).ToShortDateString();
                                dr[4] = item.SupplierName;
                                dr[5] = item.SupplierCode;
                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                dr[7] = sBOMResult == "order now" ? 2 : 3;
                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                dr[14] = item.ProductGroup;

                                dtStockOrders.Rows.Add(dr);
                            }
                            else
                            {
                                dr[0] = item.ProductCode;
                                dr[1] = item.ProductDescription;
                                dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                dr[3] = GOrderQuanity.RequiredDate.AddDays(-42).ToShortDateString(); //DateTime.Now.AddDays(sixWeekWindow).ToShortDateString();
                                dr[4] = item.SupplierName;
                                dr[5] = item.SupplierCode;
                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                dr[7] = sBOMResult == "order now" ? 2 : 3;
                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                dr[14] = item.ProductGroup;

                                dtStockOrders.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = "no order required";
                            dr[3] = "";
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = 3;
                            dr[8] = "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                    }
                }
                if (result11.Count == 0 && result21.Count > 0)
                {
                    var result = result21
                        .GroupBy(l => l.ProductCode)
                        .Select(cl => new clsStockOrderU
                        {
                            ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                            BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.BarCode != null).FirstOrDefault().BarCode,
                            MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault().MinStockAlertLevel,
                            PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.PackSize != null).FirstOrDefault().PackSize,
                            OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.OnPurchase != null).Max(m => m.OnPurchase),
                            QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? 0 : cl.Where(m => m.QtyOnHand != null).Max(m => m.QtyOnHand),
                            OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).FirstOrDefault().OrderStatus,
                            RequiredDate = cl.FirstOrDefault() == null ? DateTime.Now : cl.FirstOrDefault().RequiredDate,
                            SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).FirstOrDefault().SupplierCode,
                            SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).FirstOrDefault().SupplierName,
                            ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).FirstOrDefault().ProductDescription,
                            ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).FirstOrDefault().ProductGroup,
                            OrderQuantity = cl.Sum(c => c.OrderQuantity),
                            ComparisonResult = 0.00M,
                            Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity,//BOM
                        }).ToList();

                    foreach (var item in result)
                    {
                        string sBOMResult = "order now";
                        var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).ToList();
                        //var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).FirstOrDefault();

                        //if (IfBOM.Count > 0)
                        //{
                        //    sBOMResult = "no order required";
                        //}

                        if (IfBOM.Count > 0)
                        {
                            // Check if the BOM item is in the BOM Exception List
                            var flaggedBOMItems = exceptionItemsList.Any(itemCode => itemCode == item.ProductCode);

                            if (!flaggedBOMItems)
                            {
                                sBOMResult = "no order required";
                            }
                        }

                        //var sda = item.QtyOnHand + item.OnPurchase;
                        //var sfdfda = Convert.ToDouble(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel);

                        DataRow dr = dtStockOrders.NewRow();
                        double QtyOnHand = Convert.ToDouble(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand);
                        double OnPurchase = Convert.ToDouble(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase);
                        double MinStockAlertLevel = Convert.ToDouble(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel);

                        if (QtyOnHand + OnPurchase < MinStockAlertLevel && (QtyOnHand + OnPurchase) - Convert.ToDouble(item.OrderQuantity) < MinStockAlertLevel)
                        {
                            //order now
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = sBOMResult;
                            dr[3] = DateTime.Now.ToShortDateString();
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                            dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", "");
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                        else if (QtyOnHand + OnPurchase < MinStockAlertLevel)
                        {
                            //order now
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = sBOMResult;
                            dr[3] = DateTime.Now.ToShortDateString();
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                            dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", "");
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                        else if ((QtyOnHand + OnPurchase) - Convert.ToDouble(item.OrderQuantity) < MinStockAlertLevel)
                        {
                            DateTime resuldt = item.RequiredDate;

                            if (Convert.ToDateTime(resuldt) < DateTime.Now.AddDays(sixWeekWindow))
                            {
                                //order now
                                dr[0] = item.ProductCode;
                                dr[1] = item.ProductDescription;
                                dr[2] = sBOMResult;
                                dr[3] = DateTime.Now.ToShortDateString();
                                dr[4] = item.SupplierName;
                                dr[5] = item.SupplierCode;
                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                dr[7] = sBOMResult == "order now" ? 1 : 3;
                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                dr[12] = string.Format("{0:0.##}", "");
                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                dr[14] = item.ProductGroup;

                                dtStockOrders.Rows.Add(dr);
                            }
                            else
                            {
                                if (Convert.ToDouble(item.BarCode) <= 35)
                                {
                                    //order now
                                    dr[0] = item.ProductCode;
                                    dr[1] = item.ProductDescription;
                                    dr[2] = sBOMResult;
                                    dr[3] = resuldt.AddDays(-42).ToShortDateString();
                                    dr[4] = item.SupplierName;
                                    dr[5] = item.SupplierCode;
                                    dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                    dr[7] = sBOMResult == "order now" ? 1 : 3;
                                    dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                    dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                    dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                    dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                    dr[12] = string.Format("{0:0.##}", "");
                                    dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                    dr[14] = item.ProductGroup;

                                    dtStockOrders.Rows.Add(dr);
                                }
                                else
                                {
                                    dr[0] = item.ProductCode;
                                    dr[1] = item.ProductDescription;
                                    dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                    dr[3] = item.RequiredDate.AddDays(-Convert.ToInt32(item.BarCode) - 14).ToShortDateString(); //DateTime.Now.AddDays(sixWeekWindow).ToShortDateString();
                                    dr[4] = item.SupplierName;
                                    dr[5] = item.SupplierCode;
                                    dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                    dr[7] = sBOMResult == "order now" ? 2 : 3;
                                    dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                    dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                    dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                    dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                    dr[12] = string.Format("{0:0.##}", "");
                                    dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                    dr[14] = item.ProductGroup;

                                    dtStockOrders.Rows.Add(dr);
                                }
                            }
                        }
                        else if (QtyOnHand + OnPurchase < Convert.ToDouble(item.OrderQuantity))
                        {
                            DateTime resuldt = item.RequiredDate;

                            if (Convert.ToDateTime(resuldt) > DateTime.Now.AddDays(sixWeekWindow))
                            {
                                //order now

                                if (!string.IsNullOrEmpty(item.BarCode))
                                {
                                    if (Convert.ToDouble(item.BarCode) > sixWeekWindow)
                                    {
                                        dr[0] = item.ProductCode;
                                        dr[1] = item.ProductDescription;
                                        dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                        dr[3] = resuldt.AddDays(-42).ToShortDateString();
                                        dr[4] = item.SupplierName;
                                        dr[5] = item.SupplierCode;
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 2 : 3;
                                        dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", "");
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                        dr[14] = item.ProductGroup;

                                        dtStockOrders.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        dr[0] = item.ProductCode;
                                        dr[1] = item.ProductDescription;
                                        dr[2] = sBOMResult;
                                        dr[3] = DateTime.Now.ToShortDateString();
                                        dr[4] = item.SupplierName;
                                        dr[5] = item.SupplierCode;
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 1 : 3;
                                        dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", "");
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                        dr[14] = item.ProductGroup;

                                        dtStockOrders.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    dr[0] = item.ProductCode;
                                    dr[1] = item.ProductDescription;
                                    dr[2] = sBOMResult;
                                    dr[3] = DateTime.Now.ToShortDateString();
                                    dr[4] = item.SupplierName;
                                    dr[5] = item.SupplierCode;
                                    dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                    dr[7] = sBOMResult == "order now" ? 1 : 3;
                                    dr[8] = sBOMResult == "order now" ? "lead time empty" : "";
                                    dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                    dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                    dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                    dr[12] = string.Format("{0:0.##}", "");
                                    dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                    dr[14] = item.ProductGroup;

                                    dtStockOrders.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                //order now
                                dr[0] = item.ProductCode;
                                dr[1] = item.ProductDescription;
                                dr[2] = sBOMResult;
                                dr[3] = DateTime.Now.ToShortDateString();
                                dr[4] = item.SupplierName;
                                dr[5] = item.SupplierCode;
                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                                dr[7] = sBOMResult == "order now" ? 1 : 3;
                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                dr[12] = string.Format("{0:0.##}", "");
                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                                dr[14] = item.ProductGroup;

                                dtStockOrders.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            dr[0] = item.ProductCode;
                            dr[1] = item.ProductDescription;
                            dr[2] = "no order required";
                            dr[3] = "";
                            dr[4] = item.SupplierName;
                            dr[5] = item.SupplierCode;
                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.PackSize) == "" ? 0 : item.PackSize));
                            dr[7] = 3;
                            dr[8] = "";
                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.QtyOnHand) == "" ? 0 : item.QtyOnHand));
                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.OnPurchase) == "" ? 0 : item.OnPurchase));
                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                            dr[12] = string.Format("{0:0.##}", "");
                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(Convert.ToString(item.MinStockAlertLevel) == "" ? 0 : item.MinStockAlertLevel));
                            dr[14] = item.ProductGroup;

                            dtStockOrders.Rows.Add(dr);
                        }
                    }
                }
            }

            // Get the list of rows to be removed
            var rowsToDelete = (from row in dtStockOrders.AsEnumerable()
                                where exceptionProductsList.Contains(row.Field<string>("ProductCode"))
                                select row).ToList();

            // Remove the rows from the dataTable
            rowsToDelete.ForEach(row => dtStockOrders.Rows.Remove(row));


            string WriteCSV = path + "//Stock Orders";
            if (!Directory.Exists(WriteCSV))
            {
                Directory.CreateDirectory(WriteCSV);
            }
            string CSVFile = WriteCSV + "//Stock Orders_" + DateTime.Now.ToShortDateString().Replace("/", ".") + "_" + DateTime.Now.ToShortTimeString().Replace(":", ".").Replace(" ", "") + "_Unleashed" + ".csv";
            if (!System.IO.File.Exists(CSVFile))
            {
                System.IO.File.Create(CSVFile).Close();

            }
            DataView dv = dtStockOrders.DefaultView;
            //dv.Sort = "ProductGroup desc";
            dv.Sort = "OrderbyStatus,SupplierName asc,ProductGroup desc";
            DataTable sortedDT = dv.ToTable();
            dtStockOrders = sortedDT;

            dtStockOrders.WriteToCsvFile(CSVFile);
            System.GC.Collect();
            return dtStockOrders;
        }

        private static List<string> GetExceptionProductsList(DataTable dtExceptionProducts)
        {
            List<string> exceptionProductsList = new List<string>();

            foreach (DataRow item in dtExceptionProducts.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(item["ProductCode"])))
                {
                    string itemCode = Convert.ToString(item["ProductCode"]);

                    exceptionProductsList.Add(itemCode);
                }
            }

            return exceptionProductsList;
        }

        List<clsSalesOrderExtend> listSalesOrderLine = null;
        public void ExtendSalesOrder(List<SalesItem> salesItem)
        {
            listSalesOrderLine = new List<clsSalesOrderExtend>();
            foreach (var item in salesItem)
            {
                foreach (var itemP in item.SalesOrderLines)
                {
                    clsSalesOrderExtend orderline = new clsSalesOrderExtend();
                    orderline.ProductCode = itemP.Product.ProductCode;
                    orderline.ProductDescription = itemP.Product.ProductDescription;
                    orderline.OrderQuantity = itemP.OrderQuantity ?? 0;
                    orderline.OrderStatus = item.OrderStatus;
                    orderline.RequiredDate = item.RequiredDate;
                    orderline.WarehouseCode = item.Warehouse == null ? "" : item.Warehouse.WarehouseCode;
                    listSalesOrderLine.Add(orderline);
                }

            }
            // var sdfds = listSalesOrderLine.Where(m => m.OrderQuantity > 500).ToList();
            // var fdsf = sdfds;
            listSalesOrderLine = listSalesOrderLine.Where(m => m.OrderStatus != "Completed" && m.OrderStatus != "Deleted").ToList();

        }
        public static DataTable ConvertCSVtoDataTable(string strFilePath, bool isFirstRowHeader)
        {
            // You can also read from a file
            TextFieldParser parser = new TextFieldParser(strFilePath);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] headers;
            headers = parser.ReadFields();
            //string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header.Trim('"').Trim('*').Replace("*", ""));
            }
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = fields[i].Trim('"').Trim('=').Trim('"').Trim('"').Trim('*');
                }
                dt.Rows.Add(dr);
            }

            parser.Close();
            return dt;
        }

        //decimal Quantity = 0.0M;
       // decimal ComparisonResult = 0.0M;
        protected void load_categories(List<clsBillOfMaterial> list, List<clsBillOfMaterial> listBOM, decimal OrderQuantity, DateTime RequiredDate)
        {

            foreach (clsBillOfMaterial cat in list)
            {
                //if (cat.ComponentProductCode == "571-0040")
                //{
                //    int i = 1;
                //    int j = i;
                //}

                var resultFinall = listBOM.Where(mm => mm.AssembledProductCode == cat.ComponentProductCode).ToList();
                if (resultFinall.Count > 0)
                {
                    clsStockOrderU objModelOrder = new clsStockOrderU();
                    //  clsBOMOrders objModelOrder = new clsBOMOrders();0000000000000000............000000000
                    objModelOrder.ProductCode = cat.ComponentProductCode;
                    objModelOrder.Quantity = cat.Quantity;
                    objModelOrder.RequiredDate = RequiredDate;
                    objModelOrder.OrderQuantity = cat.Quantity * OrderQuantity;
                    objlistOrder.Add(objModelOrder);
                    objModelOrder = null;
                    load_categories(resultFinall, listBOM, cat.Quantity * OrderQuantity, RequiredDate);
                }
                else
                {
                    clsStockOrderU objModelOrder = new clsStockOrderU();

                    objModelOrder.ProductCode = cat.ComponentProductCode;
                    objModelOrder.Quantity = cat.Quantity;
                    objModelOrder.RequiredDate = RequiredDate;
                    objModelOrder.OrderQuantity = cat.Quantity * OrderQuantity;
                    objlistOrder.Add(objModelOrder);
                    objModelOrder = null;
                }


            }

            // return ComparisonResult;
        }

        public static List<clsBillOfMaterial> BillOfMaterialApi(List<BillOfMaterialsItem> dt)
        {
            List<clsBillOfMaterial> listBOM = new List<clsBillOfMaterial>();
            string AssembledProductCode = string.Empty;
            foreach (var item in dt)
            {
                foreach (var BillOfMaterialsLine in item.BillOfMaterialsLines)
                {
                    clsBillOfMaterial objBOM = new clsBillOfMaterial();

                    objBOM.ComponentProductCode = BillOfMaterialsLine.Product.ProductCode;
                    objBOM.AssembledProductCode = item.Product == null ? "" : item.Product.ProductCode;//Convert.ToString(item["Component Product Code"]);
                    objBOM.Quantity = Convert.ToDecimal(BillOfMaterialsLine.Quantity);

                    listBOM.Add(objBOM);
                }
            }

            var rejectList = listBOM.Where(m => m.ComponentProductCode.Contains("OH") || m.ComponentProductCode.Contains("LABOUR")).ToList();
            listBOM = listBOM.Except(rejectList).ToList();
            return listBOM;
        }

        // Get the List of items in the ExceptionList.csv file
        public static List<string> GetExceptionItemsList(DataTable dt)
        {
            List<string> exceptionListItems = new List<string>();

            foreach (DataRow item in dt.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(item["AssembledProductException"])))
                {
                    string itemCode = Convert.ToString(item["AssembledProductException"]);

                    exceptionListItems.Add(itemCode);
                }
            }

            return exceptionListItems;
        }
    }



    public class clsSalesOrderExtend
    {
        public string OrderStatus { get; set; }
        public DateTime RequiredDate { get; set; }
        public double OrderQuantity { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string WarehouseCode { get; set; }
    }
    public class clsBOMOrderss
    {
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal OrderQuantity { get; set; }
        public DateTime RequiredDate { get; set; }
    }
    public class clsStockOrderU
    {
        public string ProductCode { get; set; }
        public string BarCode { get; set; }
        public double? MinStockAlertLevel { get; set; }
        public double? PackSize { get; set; }
        public double OnPurchase { get; set; }
        public double QtyOnHand { get; set; }
        public string OrderStatus { get; set; }
        public DateTime RequiredDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ProductDescription { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal ComparisonResult { get; set; }
        public string ProductGroup { get; set; }
    }
}
