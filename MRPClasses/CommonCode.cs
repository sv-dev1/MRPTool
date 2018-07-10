using Microsoft.VisualBasic.FileIO;
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
    public class CommonCode
    {
        private bool bom = false;
        private bool products = false;
        //private bool purchaseOrders = false;
        private bool salesOrders = false;
        private bool sohlist = false;
        List<decimal> sad = new List<decimal>();
        List<clsStockOrder> objlistOrder = new List<clsStockOrder>();
        public static DataTable productDt = new DataTable();
        DataTable dtExceptionProducts = new DataTable();
        DataTable dtExceptionBOMItems = new DataTable();



        //private const string testApiId = "18059178-6dc1-4942-92d9-c9052d9911f6";
        //private const string testApiKey = "jFaajEzTzQROnLayTvAW4nliSHYSe0hsQnm4VuD8B6S2Q3mJwI90cT7Vj6oJYckvijjvi1HeBlkEOnsnw==";
        private const string ApiId = "0b9de772-3959-4330-a518-f4a9e035489a";
        private const string ApiKey = "egwTWe7rCPq8h6okMsIcP34apJcdw5VFNPeOgvvsSUTHqEfNeEMdvVopElp1WtmNKYebYCTadLKaFnAKobwKw==";

        public DataTable ReteriveCSVFiles(int sixWeekWindow)
        {
            sad = new List<decimal>();
            objlistOrder = new List<clsStockOrder>();
            DataTable dtBOM = new DataTable();
            DataTable dtProducts = new DataTable();
            DataTable dtPurchaseOrders = new DataTable();
            DataTable dtSalesOrders = new DataTable();
            DataTable dtSOHList = new DataTable();
            DataTable dtStockOrders = new DataTable();


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

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string completePath = path + @"\Example Files";
            DirectoryInfo di = new DirectoryInfo(completePath);
            FileInfo[] files = di.GetFiles("*.csv");

            if (files.Count() > 3)
            {
                foreach (string file in Directory.EnumerateFiles(completePath, "*.csv"))
                {
                    // string contents = File.ReadAllText(file);
                    if (file.ToLower().IndexOf("bom") > -1)
                    {
                        dtBOM = ConvertCSVtoDataTable(file, true);
                        bom = true;
                    }
                    else if (file.ToLower().IndexOf("products") > -1)
                    {
                        dtProducts = ConvertCSVtoDataTable(file, true);
                        productDt = dtProducts;
                        products = true;
                    }
                    //else if (file.ToLower().IndexOf("purchaseorders") > -1)
                    //{
                    //    dtPurchaseOrders = ConvertCSVtoDataTable(file, true);
                    //    purchaseOrders = true;
                    //}
                    else if (file.ToLower().IndexOf("salesorders") > -1)
                    {
                        dtSalesOrders = ConvertCSVtoDataTable(file, true);
                        salesOrders = true;
                    }
                    else if (file.ToLower().IndexOf("sohlist") > -1)
                    {
                        dtSOHList = ConvertCSVtoDataTable1(file, true);
                        sohlist = true;
                    }
                    else if (file.ToLower().IndexOf("exceptionassembledproduct") > -1)
                    {
                        dtExceptionBOMItems = ConvertCSVtoDataTable(file, true);
                    }
                    else if (file.ToLower().IndexOf("exceptionproductlist") > -1)
                    {
                        dtExceptionProducts = ConvertCSVtoDataTable(file, true);
                    }
                }
                //foreach (DataRow item in dtSalesOrders.Rows)
                //{
                //    try
                //    {
                //        DateTime.ParseExact(Convert.ToString(item["RequiredDate"]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    }
                //    catch (Exception ex)
                //    {

                //        var fdsfd = DateTime.ParseExact(Convert.ToString(item["RequiredDate"]), "d/M/yyyy", CultureInfo.InvariantCulture);
                //    }
                //}
                var listBOM = BillOfMaterial(dtBOM);

                List<string> exceptionBOMItemsList = GetExceptionItemsList(dtExceptionBOMItems);
                List<string> exceptionProductsList = GetExceptionProductsList(dtExceptionProducts);

                if (bom && products && salesOrders && sohlist)
                {
                    var results = (from tableProducts in dtProducts.AsEnumerable()

                                   join tableSOHList in dtSOHList.AsEnumerable() on tableProducts["Product Code"] equals tableSOHList["Product Code"] into temp1
                                   from SOHList in temp1.DefaultIfEmpty()

                                   join tableSalesOrders in dtSalesOrders.AsEnumerable() on tableProducts["Product Code"] equals tableSalesOrders["ProductCode"] into temp3
                                   from SalesOrders in temp3.DefaultIfEmpty()

                                   where Convert.ToString(SalesOrders == null ? "" : SalesOrders["OrderStatus"]) != "Completed" && Convert.ToString(SalesOrders == null ? "" : SalesOrders["OrderStatus"]) != "Deleted"
                                   select new clsStockOrder
                                   {
                                       ProductCode = Convert.ToString(tableProducts == null ? "" : tableProducts["Product Code"]),
                                       BarCode = Convert.ToString(tableProducts == null ? 35 : tableProducts["BarCode"]),//LeadTime
                                       MinStockAlertLevel = Convert.ToString(tableProducts == null ? "" : tableProducts["Min Stock Alert Level"]),//Minimum Stock level
                                       PackSize = Convert.ToString(tableProducts == null ? "" : tableProducts["Pack Size"]),
                                       SupplierCode = Convert.ToString(tableProducts == null ? "" : tableProducts["Supplier Code"]),//Suplier info
                                       SupplierName = Convert.ToString(tableProducts == null ? "" : tableProducts["Supplier Name"]),//Suplier info
                                       ProductDescription = Convert.ToString(tableProducts == null ? "" : tableProducts["Product Description"]).Replace(",", "").Replace("=", ""),
                                       OnPurchase = Convert.ToString(SOHList == null ? "0" : SOHList["On Purchase"]),// Stock on purchase
                                       QtyOnHand = Convert.ToString(SOHList == null ? "0" : SOHList["Qty On Hand"]),//Stock on Hand
                                       OrderStatus = Convert.ToString(SalesOrders == null ? "" : SalesOrders["OrderStatus"]).ToLower(),
                                       RequiredDate = SalesOrders == null ? DateTime.Now : DateTime.ParseExact(Convert.ToString(SalesOrders["RequiredDate"]), "d/M/yyyy", CultureInfo.InvariantCulture),
                                       OrderQuantity = Convert.ToDecimal(Convert.ToDecimal(SalesOrders == null ? "0" : Convert.ToString(SalesOrders["OrderQuantity"]) == "" ? "0" : SalesOrders["OrderQuantity"]).ToString("G")),//Order Quantity
                                       ProductGroup = Convert.ToString(tableProducts == null ? "" : tableProducts["Product Group"]),//Warehouse info
                                       Quantity = 0.0M,//Convert.ToDecimal(listBOM.Where(mm => mm.AssembledProductCode == Convert.ToString(tableProducts["Product Code"])).FirstOrDefault().Quantity),
                                   }).ToList();


                    // results = results.Where(m => m.ProductCode == "009-0044").ToList();
                    foreach (var item in results)
                    {
                        var resultFinal = listBOM.Where(mm => mm.AssembledProductCode == item.ProductCode).ToList();

                        if (resultFinal.Count > 0)
                        {
                            // decimal ComparisonResult = 0.0M;
                            foreach (var itemt in resultFinal)
                            {
                                clsStockOrder objModelOrder = new clsStockOrder();

                                var resultFinall = listBOM.Where(mm => mm.AssembledProductCode == itemt.ComponentProductCode).ToList();

                                objModelOrder.ProductCode = itemt.ComponentProductCode;
                                objModelOrder.Quantity = itemt.Quantity;
                                objModelOrder.RequiredDate = item.RequiredDate;
                                objModelOrder.OrderQuantity = itemt.Quantity * item.OrderQuantity;
                                objlistOrder.Add(objModelOrder);

                                if (resultFinall.Count > 0)
                                {
                                    load_categories(resultFinall, listBOM, itemt.Quantity * item.OrderQuantity, item.RequiredDate, itemt.ComponentProductCode);
                                }
                            }
                            // item.ComparisonResult += ComparisonResult;
                        }
                    }

                    objlistOrder = objlistOrder.Where(m => m.ProductCode != null).ToList();

                    //var fgfdg = results.Where(m => m.ProductCode == "500MMSOWAS2129D").ToList();
                    //var fdfd = objlistOrder.Where(m => m.ProductCode == "500MMSOWAS2129D").ToList();
                    //var dfsf = fgfdg;
                    //var fds = fdfd;


                    var detailResult = objlistOrder.Concat(results);//.Where(m=>m.ProductCode== "200MMBRAS2129DSS");

                    //Reassign packsize and suppliername and supplier code
                    foreach (var item in detailResult)
                    {
                        DataRow[] dr = dtProducts.Select("[Product Code]='" + item.ProductCode + "'");
                        if (dr.Count() > 0)
                        {
                            item.PackSize = Convert.ToString(dr[0]["Pack Size"]);
                            item.SupplierName = Convert.ToString(dr[0]["Supplier Name"]);//"Supplier Name";
                            item.SupplierCode = Convert.ToString(dr[0]["Supplier Code"]); //"Supplier Code";
                        }

                    }

                    var CombinedListIndividualProduct = detailResult
                        .GroupBy(x => new { x.ProductCode })
                        .Select(cl => new
                        {
                            ProductCode = cl.Key,
                            ProductList = cl.ToList()
                        }).ToList();

                    //var dfsdf = CombinedListIndividualProduct;

                    foreach (var item1 in CombinedListIndividualProduct)
                    {
                        var resultGreaterThan6Week = item1.ProductList.Where(m => m.RequiredDate > DateTime.Now.AddDays(sixWeekWindow)).ToList();
                        var resultLessThan6Week = item1.ProductList.Where(m => m.RequiredDate < DateTime.Now.AddDays(sixWeekWindow)).ToList();

                        //var fdsfe = resultLessThan6Week;

                        // The Total no. of records is for Range Less Than 6 Weeks & Range Greater than 6 Weeks > 0 
                        if (resultGreaterThan6Week.Count > 0 && resultLessThan6Week.Count > 0)
                        {
                            var lessthan6Weeks = resultLessThan6Week
                                .GroupBy(l => l.ProductCode)
                                .Select(cl => new clsStockOrder
                                {
                                    ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                                    BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "35" : cl.Where(m => m.BarCode != null).First().BarCode,
                                    MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? "" : cl.Where(m => m.MinStockAlertLevel != null).First().MinStockAlertLevel,
                                    PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? "" : cl.Where(m => m.PackSize != null).First().PackSize,
                                    OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OnPurchase != null).First().OnPurchase,
                                    QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? "" : cl.Where(m => m.QtyOnHand != null).First().QtyOnHand,
                                    OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).First().OrderStatus,
                                    RequiredDate = cl.Where(m => m.RequiredDate != null) == null ? DateTime.Now : cl.Where(m => m.RequiredDate != null).Min(m => m.RequiredDate),
                                    SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).First().SupplierCode,
                                    SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).First().SupplierName,
                                    ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).First().ProductDescription,
                                    ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).First().ProductGroup,
                                    OrderQuantity = cl.Sum(c => c.OrderQuantity),
                                    ComparisonResult = 0.00M,
                                    Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity,//BOM
                                }).ToList();

                            var greatethan6Weeks = resultGreaterThan6Week
                                .GroupBy(l => l.ProductCode)
                                .Select(cl => new clsStockOrder
                                {
                                    ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                                    BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "35" : cl.Where(m => m.BarCode != null).First().BarCode,
                                    MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? "" : cl.Where(m => m.MinStockAlertLevel != null).First().MinStockAlertLevel,
                                    PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? "" : cl.Where(m => m.PackSize != null).First().PackSize,
                                    OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OnPurchase != null).First().OnPurchase,
                                    QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? "" : cl.Where(m => m.QtyOnHand != null).First().QtyOnHand,
                                    OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).First().OrderStatus,
                                    RequiredDate = cl.Where(m => m.RequiredDate != null) == null ? DateTime.Now : cl.Where(m => m.RequiredDate != null).Min(m => m.RequiredDate),
                                    SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).First().SupplierCode,
                                    SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).First().SupplierName,
                                    ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).First().ProductDescription,
                                    ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).First().ProductGroup,
                                    OrderQuantity = cl.Sum(c => c.OrderQuantity),
                                    ComparisonResult = 0.00M,
                                    Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity, //BOM
                                }).ToList();

                            foreach (var item in lessthan6Weeks)
                            {
                                DataRow dr = dtStockOrders.NewRow();
                                string sBOMResult = "order now";

                                var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).ToList();

                                if (IfBOM.Count > 0)
                                {
                                    var flaggedBOMItems = exceptionBOMItemsList.Any(itemCode => itemCode == item.ProductCode);

                                    if (!flaggedBOMItems)
                                    {
                                        sBOMResult = "no order required";
                                    }
                                }

                                double value1;
                                double value2;
                                double value3;
                                double value4;
                                var result1 = double.TryParse(item.QtyOnHand == "" ? "0.00" : item.QtyOnHand, out value1);
                                var result2 = double.TryParse(item.OnPurchase == "" ? "0.00" : item.OnPurchase, out value2);
                                var result3 = double.TryParse(item.MinStockAlertLevel == "" ? "0.00" : item.MinStockAlertLevel, out value3);
                                var result4 = double.TryParse(item.PackSize == "" ? "0.00" : item.PackSize, out value4);

                                if (result1 && result2 && result3 && result4)
                                {
                                    double QtyOnHand = Convert.ToDouble(item.QtyOnHand == "" ? "0" : item.QtyOnHand);
                                    double OnPurchase = Convert.ToDouble(item.OnPurchase == "" ? "0" : item.OnPurchase);
                                    double MinStockAlertLevel = Convert.ToDouble(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel);
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
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 1 : 3;
                                        dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 1 : 3;
                                        dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                            dr[3] = GOrderQuanity.RequiredDate.AddDays(-Convert.ToInt32(42)).ToShortDateString();
                                            dr[4] = item.SupplierName;
                                            dr[5] = item.SupplierCode;
                                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                            dr[7] = sBOMResult == "order now" ? 2 : 3;
                                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                            dr[7] = sBOMResult == "order now" ? 2 : 3;
                                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                            dr[7] = sBOMResult == "order now" ? 2 : 3;
                                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                            dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = 3;
                                        dr[8] = "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", GOrderQuanity.OrderQuantity);
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
                                        dr[14] = item.ProductGroup;

                                        dtStockOrders.Rows.Add(dr);
                                    }
                                }
                            }

                        }
                        if (resultGreaterThan6Week.Count == 0 && resultLessThan6Week.Count > 0)
                        {
                            var result = resultLessThan6Week
                                .GroupBy(l => l.ProductCode)
                                .Select(cl => new clsStockOrder
                                {
                                    ProductCode = cl.FirstOrDefault() == null ? "" : cl.FirstOrDefault().ProductCode,
                                    BarCode = cl.Where(m => m.BarCode != null).FirstOrDefault() == null ? "35" : cl.Where(m => m.BarCode != null).First().BarCode,
                                    MinStockAlertLevel = cl.Where(m => m.MinStockAlertLevel != null).FirstOrDefault() == null ? "" : cl.Where(m => m.MinStockAlertLevel != null).First().MinStockAlertLevel,
                                    PackSize = cl.Where(m => m.PackSize != null).FirstOrDefault() == null ? "" : cl.Where(m => m.PackSize != null).First().PackSize,
                                    OnPurchase = cl.Where(m => m.OnPurchase != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OnPurchase != null).First().OnPurchase,
                                    QtyOnHand = cl.Where(m => m.QtyOnHand != null).FirstOrDefault() == null ? "" : cl.Where(m => m.QtyOnHand != null).First().QtyOnHand,
                                    OrderStatus = cl.Where(m => m.OrderStatus != null).FirstOrDefault() == null ? "" : cl.Where(m => m.OrderStatus != null).First().OrderStatus,
                                    RequiredDate = cl.Where(m => m.RequiredDate != null) == null ? DateTime.Now : cl.Where(m => m.RequiredDate != null).Min(m => m.RequiredDate),
                                    SupplierCode = cl.Where(m => m.SupplierCode != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierCode != null).First().SupplierCode,
                                    SupplierName = cl.Where(m => m.SupplierName != null).FirstOrDefault() == null ? "" : cl.Where(m => m.SupplierName != null).First().SupplierName,
                                    ProductDescription = cl.Where(m => m.ProductDescription != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductDescription != null).First().ProductDescription,
                                    ProductGroup = cl.Where(m => m.ProductGroup != null).FirstOrDefault() == null ? "" : cl.Where(m => m.ProductGroup != null).First().ProductGroup,
                                    OrderQuantity = cl.Sum(c => c.OrderQuantity),
                                    ComparisonResult = 0.00M,
                                    Quantity = cl.FirstOrDefault() == null ? 0 : cl.FirstOrDefault().Quantity,//BOM
                                }).ToList();

                            foreach (var item in result)
                            {
                                string sBOMResult = "order now";
                                var IfBOM = listBOM.Where(m => m.AssembledProductCode == item.ProductCode).ToList();

                                if (IfBOM.Count > 0)
                                {
                                    var flaggedBOMItems = exceptionBOMItemsList.Any(itemCode => itemCode == item.ProductCode);

                                    if (!flaggedBOMItems)
                                    {
                                        sBOMResult = "no order required";
                                    }
                                }

                                double value1;
                                double value2;
                                double value3;
                                double value4;
                                var result1 = double.TryParse(item.QtyOnHand == "" ? "0.00" : item.QtyOnHand, out value1);
                                var result2 = double.TryParse(item.OnPurchase == "" ? "0.00" : item.OnPurchase, out value2);
                                var result3 = double.TryParse(item.MinStockAlertLevel == "" ? "0.00" : item.MinStockAlertLevel, out value3);
                                var result4 = double.TryParse(item.PackSize == "" ? "0.00" : item.PackSize, out value4);

                                if (result1 && result2 && result3 && result4)
                                {
                                    //var sda = Convert.ToDouble(item.QtyOnHand == "" ? "0.00" : item.QtyOnHand) + Convert.ToDouble(item.OnPurchase == "" ? "0" : item.OnPurchase);
                                    //var sfdfda = Convert.ToDouble(item.MinStockAlertLevel == "" ? "0.00" : item.MinStockAlertLevel);
                                    DataRow dr = dtStockOrders.NewRow();
                                    double QtyOnHand = Convert.ToDouble(item.QtyOnHand == "" ? "0" : item.QtyOnHand);
                                    double OnPurchase = Convert.ToDouble(item.OnPurchase == "" ? "0" : item.OnPurchase);
                                    double MinStockAlertLevel = Convert.ToDouble(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel);

                                    if (QtyOnHand + OnPurchase < MinStockAlertLevel && (QtyOnHand + OnPurchase) - Convert.ToDouble(item.OrderQuantity) < MinStockAlertLevel)
                                    {
                                        //order now
                                        dr[0] = item.ProductCode;
                                        dr[1] = item.ProductDescription;
                                        dr[2] = sBOMResult;
                                        dr[3] = DateTime.Now.ToShortDateString();
                                        dr[4] = item.SupplierName;
                                        dr[5] = item.SupplierCode;
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 1 : 3;
                                        dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", "");
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = sBOMResult == "order now" ? 1 : 3;
                                        dr[8] = sBOMResult == "order now" ? "below minimum stock" : "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", "");
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                            dr[12] = string.Format("{0:0.##}", "");
                                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                                dr[2] = sBOMResult == "order now" ? "upcoming" : "no order required";
                                                dr[3] = resuldt.AddDays(-42).ToShortDateString();
                                                dr[4] = item.SupplierName;
                                                dr[5] = item.SupplierCode;
                                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                                dr[7] = sBOMResult == "order now" ? 2 : 3;
                                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                                dr[12] = string.Format("{0:0.##}", "");
                                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                                dr[7] = sBOMResult == "order now" ? 2 : 3;
                                                dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                                dr[12] = string.Format("{0:0.##}", "");
                                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                                    dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                                    dr[7] = sBOMResult == "order now" ? 2 : 3;
                                                    dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                                    dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                                    dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                                    dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                                    dr[12] = string.Format("{0:0.##}", "");
                                                    dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                                    dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                                    dr[7] = sBOMResult == "order now" ? 1 : 3;
                                                    dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                                    dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                                    dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                                    dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                                    dr[12] = string.Format("{0:0.##}", "");
                                                    dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                                dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                                dr[7] = sBOMResult == "order now" ? 1 : 3;
                                                dr[8] = sBOMResult == "order now" ? "lead time empty" : "";
                                                dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                                dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                                dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                                dr[12] = string.Format("{0:0.##}", "");
                                                dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                            dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                            dr[7] = sBOMResult == "order now" ? 1 : 3;
                                            dr[8] = sBOMResult == "order now" ? "upcoming orders" : "";
                                            dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                            dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                            dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                            dr[12] = string.Format("{0:0.##}", "");
                                            dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
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
                                        dr[6] = string.Format("{0:0.##}", Convert.ToDecimal(item.PackSize == "" ? "0" : item.PackSize));
                                        dr[7] = 3;
                                        dr[8] = "";
                                        dr[9] = string.Format("{0:0.##}", Convert.ToDecimal(item.QtyOnHand == "" ? "0" : item.QtyOnHand));
                                        dr[10] = string.Format("{0:0.##}", Convert.ToDecimal(item.OnPurchase == "" ? "0" : item.OnPurchase));
                                        dr[11] = string.Format("{0:0.##}", item.OrderQuantity);
                                        dr[12] = string.Format("{0:0.##}", "");
                                        dr[13] = string.Format("{0:0.##}", Convert.ToDecimal(item.MinStockAlertLevel == "" ? "0" : item.MinStockAlertLevel));
                                        dr[14] = item.ProductGroup;

                                        dtStockOrders.Rows.Add(dr);
                                    }
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
                    string CSVFile = WriteCSV + "//Stock Orders_" + DateTime.Now.ToShortDateString().Replace("/", ".") + "_" + DateTime.Now.ToShortTimeString().Replace(":", ".").Replace(" ", "") + "_Excel" + ".csv";

                    if (!System.IO.File.Exists(CSVFile))
                    {
                        System.IO.File.Create(CSVFile).Close();
                    }

                    DataView dv = dtStockOrders.DefaultView;
                    // dv.Sort = "ProductGroup desc";
                    // dv.Sort = "OrderbyStatus,SupplierName asc,Warehouse desc";
                    dv.Sort = "OrderbyStatus,SupplierName asc,ProductGroup desc";
                    DataTable sortedDT = dv.ToTable();
                    dtStockOrders = sortedDT;

                    dtStockOrders.WriteToCsvFile(CSVFile);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // MessageBox.Show("File count is" + files.Count() + ".please place required files in the folder.");
                return null;
            }
            System.GC.Collect();

            return dtStockOrders;
        }

        public static List<string> GetExceptionProductsList(DataTable dtExceptionProducts)
        {
            List<string> exceptionProductsList = new List<string>();

            foreach (DataRow item in dtExceptionProducts.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(item["ProductCode"])))
                {
                    string productCode = Convert.ToString(item["ProductCode"]);

                    exceptionProductsList.Add(productCode);
                }
            }

            return exceptionProductsList;
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

        public static DataTable ConvertCSVtoDataTable1(string strFilePath, bool isFirstRowHeader)
        {
            // You can also read from a file

            TextFieldParser parser = new TextFieldParser(strFilePath);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] headers;
            headers = parser.ReadFields();
            foreach (string item in headers)
            {
                if (item == string.Empty)
                {
                    headers = parser.ReadFields();
                    break;
                }
            }

            //string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header.Trim('"').Trim('*'));
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

        public static List<clsBillOfMaterial> BillOfMaterial(DataTable dt)
        {
            List<clsBillOfMaterial> listBOM = new List<clsBillOfMaterial>();
            string AssembledProductCode = string.Empty;
            foreach (DataRow item in dt.Rows)
            {

                clsBillOfMaterial objBOM = new clsBillOfMaterial();

                if (!string.IsNullOrEmpty(Convert.ToString(item["Assembled Product Code"])))
                {
                    objBOM.AssembledProductCode = Convert.ToString(item["Assembled Product Code"]);
                    objBOM.ComponentProductCode = Convert.ToString(item["Component Product Code"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(item["Quantity"])))
                    {
                        objBOM.Quantity = Convert.ToDecimal(item["Quantity"]);
                    }
                    else
                    {
                        objBOM.Quantity = 0.00M;
                    }
                    AssembledProductCode = objBOM.AssembledProductCode;
                }
                else
                {
                    objBOM.AssembledProductCode = AssembledProductCode;
                    objBOM.ComponentProductCode = Convert.ToString(item["Component Product Code"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(item["Quantity"])))
                    {
                        objBOM.Quantity = Convert.ToDecimal(item["Quantity"]);
                    }
                    else
                    {
                        objBOM.Quantity = 0.00M;
                    }
                }

                listBOM.Add(objBOM);
                objBOM = null;

            }

            var rejectList = listBOM.Where(m => m.ComponentProductCode.Contains("OH") || m.ComponentProductCode.Contains("LABOUR")).ToList();
            listBOM = listBOM.Except(rejectList).ToList();
            //listBOM = listBOM.Where(m => m.AssembledProductCode == "500MMSOWAS2129D").ToList();
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

        // decimal Quantity = 0.0M;
        // decimal ComparisonResult = 0.0M;
        protected void load_categories(List<clsBillOfMaterial> list, List<clsBillOfMaterial> listBOM, decimal OrderQuantity, DateTime RequiredDate, string fail)
        {
            if (list != null)
            {
                foreach (clsBillOfMaterial cat in list)
                {
                    //if (cat.ComponentProductCode == "571-0040")
                    //{
                    //    int i = 1;
                    //    int j = i;
                    //}
                    clsStockOrder objModelOrder = new clsStockOrder();
                    var resultFinall = listBOM.Where(mm => mm.AssembledProductCode == cat.ComponentProductCode).ToList();

                    objModelOrder.ProductCode = cat.ComponentProductCode;
                    objModelOrder.Quantity = cat.Quantity;
                    objModelOrder.RequiredDate = RequiredDate;
                    objModelOrder.OrderQuantity = cat.Quantity * OrderQuantity;

                    if (resultFinall.Count > 0)
                    {
                        //  clsBOMOrders objModelOrder = new clsBOMOrders();
                        objlistOrder.Add(objModelOrder);

                        load_categories(resultFinall, listBOM, cat.Quantity * OrderQuantity, RequiredDate, fail);
                    }
                    else
                    {
                        //sad.Add(cat.Quantity);
                        //Quantity = Convert.ToDecimal(cat.Quantity);
                        // ComparisonResult += OrderQuantity * cat.Quantity;

                        objlistOrder.Add(objModelOrder);
                    }
                }
            }


            // return ComparisonResult;
        }

        //private const string testApiId = "18059178-6dc1-4942-92d9-c9052d9911f6";
        //private const string testApiKey = "jFaajEzTzQROnLayTvAW4nliSHYSe0hsQnm4VuD8B6S2Q3mJwI90cT7Vj6oJYckvijjvi1HeBlkEOnsnw==";

        //private const string ApiId = "0b9de772-3959-4330-a518-f4a9e035489a";
        //private const string ApiKey = "egwTWe7rCPq8h6okMsIcP34apJcdw5VFNPeOgvvsSUTHqEfNeEMdvVopElp1WtmNKYebYCTadLKaFnAKobwKw==";

        public static PurchaseOrdersItem GeneratePurchaseOrder(string GUID)
        {
            string responseProducts = UnLeashedMain.GetPurchaseOrderJson("PurchaseOrders", ApiId, ApiKey, GUID);
            var dtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrdersItem>(responseProducts);
            return dtBOMa;
        }

        public static string GenerateTopPurchaseOrder(string GUID)
        {
            string responseProducts = UnLeashedMain.CheckPurchaseOrderPageNumber("PurchaseOrders", ApiId, ApiKey, "");
            var dtBOMa1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ULPurchaseOrders>(responseProducts);

            string lastRecord = UnLeashedMain.GetLastPagePurchaseOrders("PurchaseOrders/" + dtBOMa1.Pagination.NumberOfPages.ToString(), ApiId, ApiKey, "", "1");

            var orderNumber = Newtonsoft.Json.JsonConvert.DeserializeObject<ULPurchaseOrders>(lastRecord).Items.FirstOrDefault().OrderNumber;
            return orderNumber;// dtBOMa1;
        }

        public static SupplierInfo GetSupplierInformation(string supplierCode)
        {
            var supplierList = UnLeashedMain.GetSupplierJson("Suppliers", ApiId, ApiKey, "", supplierCode);
            var TempdtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<SupplierInfo>(supplierList);
            return TempdtBOMa;
        }

        public static SupplierInfo GetSuppliers()
        {
            var supplierList = UnLeashedMain.GetSupplierJson("Suppliers", ApiId, ApiKey, "");

            var TempdtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<SupplierInfo>(supplierList);
            return TempdtBOMa;

        }

        public static Warehouses GetWarehouseInformation(string WarehouseCode)
        {
            var WarehouseList = UnLeashedMain.GetWarehouseCodeJson("Warehouses", ApiId, ApiKey, "", WarehouseCode);
            var TempdtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<Warehouses>(WarehouseList);
            return TempdtBOMa;
        }

        public static ProductInfo GetProductsGroupInfo()
        {
            var supplierList = UnLeashedMain.GetProductGroupsJson("ProductGroups", ApiId, ApiKey, "");
            var TempdtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductInfo>(supplierList);
            return TempdtBOMa;
        }

        public static ProductItems GetProductInformation(string productCode)
        {
            var supplierList = UnLeashedMain.GetProductJson("Products/1", ApiId, ApiKey, "", productCode);
            var TempdtBOMa = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductItems>(supplierList);
            return TempdtBOMa;
        }

        public static string AddPurchaseOrder(string order)
        {
            //var Order = Newtonsoft.Json.JsonConvert.SerializeObject(objOrder);           
            var datastring = UnLeashedMain.PostXml("PurchaseOrders", ApiId, ApiKey, Guid.NewGuid().ToString(), order);
            return datastring;
        }
    }

    public static class DataTableExtensions
    {
        public static void WriteToCsvFile(this DataTable dataTable, string filePath)
        {
            dataTable.Columns.Remove("OrderbyStatus");
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);



            foreach (DataRow dr in dataTable.Rows)
            {

                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString().Replace(",", "").Replace("=", "").Replace("\"", "") + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            System.IO.File.WriteAllText(filePath, fileContent.ToString());

        }


    }

    public class clsBillOfMaterial
    {
        public string AssembledProductCode { get; set; }
        public string ComponentProductCode { get; set; }
        public decimal Quantity { get; set; }
    }
    public class clsStockOrder
    {
        public string ProductCode { get; set; }
        public string BarCode { get; set; }
        public string MinStockAlertLevel { get; set; }
        public string PackSize { get; set; }
        public string OnPurchase { get; set; }
        public string QtyOnHand { get; set; }
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

    public class clsBOMOrders
    {
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal OrderQuantity { get; set; }
        public string RequiredDate { get; set; }
    }

    //public class ExceptionItems
    //{
    //    public string ItemCode { get; set; }
    //    public string Description { get; set; }
    //}
}
