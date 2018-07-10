using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MRPClasses;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml;
using System.Collections;
using MahApps.Metro.Controls;

namespace MRPTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string ApiId = "0b9de772-3959-4330-a518-f4a9e035489a";
        private const string ApiKey = "egwTWe7rCPq8h6okMsIcP34apJcdw5VFNPeOgvvsSUTHqEfNeEMdvVopElp1WtmNKYebYCTadLKaFnAKobwKw==";

        int firstRound = 1;
        CommonCode objCode = new CommonCode();
        BackgroundWorker worker = new BackgroundWorker();
        DateTime dt = new DateTime(2016, 6, 2, 1, 1, 1);

        DataTable dtOrderStatus;
        DataTable dtSupplier;
        DataTable dtProduct;

        StringBuilder orderStatusString;
        StringBuilder supplierString;
        StringBuilder productString;    

        public MainWindow()
        {
            InitializeComponent();
            var guid = Guid.NewGuid().ToString();
            TimeSpanUpcoming = MRPTool.Properties.Settings.Default.TimeSpanChange;
            UpcomingOrderSettings.Text = TimeSpanUpcoming + " days";
        }

        public static DataTable SavedRecord = new DataTable();
        public static string TimeSpanUpcoming = MRPTool.Properties.Settings.Default.TimeSpanChange;
        DataTable result = new DataTable();
        private void StockOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = objCode.ReteriveCSVFiles(Convert.ToInt32(TimeSpanUpcoming));
               // result.WriteToCsvFile("MRPOutPut.csv");
                if (result == null)
                {
                    System.Windows.MessageBox.Show("Files are missing,please provide all the files.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    CommonDataFill();

                    StackFilters.Visibility = Visibility.Visible;
                    System.Windows.MessageBox.Show("Data exported sucessfully,please check folder stock orders on your desktop.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                }

                Bind_DropDownLists();
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("Error in fetching the data. {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ScrollViewer1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void UnleashedData_RowEnter(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {

        }

        int firstSelectedRow = 0;
        private void UnleashedData_CellMouseClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            firstSelectedRow = e.RowIndex;
            // UnleashedData.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
        }
        
        DataTable table = new DataTable();
        private void SearchStockOrders_Click(object sender, RoutedEventArgs e)
        {
            if (SearchProduct.Text.Trim() != string.Empty)
            {
                if (SavedRecord.Rows.Count > 0)
                {
                    CommonDataFillSearch();

                     
                }
                else
                {
                    System.Windows.MessageBox.Show("No record available to filter. Please calculate stock order first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                CommonDataFill();

            }

        }

        System.Windows.Forms.DataGridViewCheckBoxColumn checkBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        private void CommonDataFill(DataTable dtFilter = null)
        {
            UnleashedData.DataSource = null;
            this.UnleashedData.Rows.Clear();
           

            
            if (dtFilter != null)
            {
              
                SavedRecord = null;
                SavedRecord = dtFilter;

                if(dtFilter.Rows.Count < 0)
                {
                    SavedRecord = result;
                }
            }
            else
            {
                SavedRecord = result;
            }
            UnleashedData.AutoGenerateColumns = false;
            UnleashedData.DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
            UnleashedData.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            //UnleashedData.DefaultCellStyle[3].NullValue = false;
            //Add Columns
            UnleashedData.ColumnCount = 15;
            UnleashedData.Columns[0].Name = "ProductCode";
            UnleashedData.Columns[0].HeaderText = "Product Code";
            UnleashedData.Columns[0].DataPropertyName = "ProductCode";

            //UnleashedData.Columns[0].Width = 200;
            UnleashedData.Columns[1].HeaderText = "Product Description";
            UnleashedData.Columns[1].Name = "ProductDescription";
            UnleashedData.Columns[1].DataPropertyName = "ProductDescription";

            // UnleashedData.Columns[1].Width = 200;
            UnleashedData.Columns[2].Name = "RequiredOrderStatus";
            UnleashedData.Columns[2].HeaderText = "Order Status";
            UnleashedData.Columns[2].DataPropertyName = "RequiredOrderStatus";
            UnleashedData.Columns[2].Width = 100;

            UnleashedData.Columns[3].Name = "RequiredOrderDate";
            UnleashedData.Columns[3].HeaderText = "Order Date";
            UnleashedData.Columns[3].DataPropertyName = "RequiredOrderDate";
            UnleashedData.Columns[3].Width = 80;

            if (firstRound == 1)
            {
                // UnleashedData.Columns.Add(chk);
                checkBoxColumn.HeaderText = "Create PO";
                checkBoxColumn.Width = 20;
                checkBoxColumn.DefaultCellStyle.NullValue = false;
                checkBoxColumn.Name = "checkBoxColumn";

                UnleashedData.Columns.Insert(4, checkBoxColumn);
                firstRound = 8;
            }

            UnleashedData.Columns[5].Name = "SupplierName";
            UnleashedData.Columns[5].HeaderText = "Supplier Name";
            UnleashedData.Columns[5].DataPropertyName = "SupplierName";
            // chk.Name = "chk";
            // UnleashedData.Columns[4].Width = 200;
            UnleashedData.Columns[6].Name = "SupplierCode";
            UnleashedData.Columns[6].HeaderText = "Supplier Code";
            UnleashedData.Columns[6].DataPropertyName = "SupplierCode";
            UnleashedData.Columns[6].Width = 90;

            UnleashedData.Columns[7].Name = "Quantity";
            UnleashedData.Columns[7].HeaderText = "Quantity";
            UnleashedData.Columns[7].DataPropertyName = "Quantity";
            UnleashedData.Columns[7].Width = 60;

            UnleashedData.Columns[8].Name = "Trigger";
            UnleashedData.Columns[8].HeaderText = "Trigger";
            UnleashedData.Columns[8].DataPropertyName = "Trigger";

            UnleashedData.Columns[9].Name = "StockOnHand";
            UnleashedData.Columns[9].HeaderText = "Stock On Hand";
            UnleashedData.Columns[9].DataPropertyName = "StockOnHand";
            UnleashedData.Columns[9].Width = 100;

            UnleashedData.Columns[10].Name = "StockOnPurchase";
            UnleashedData.Columns[10].HeaderText = "Stock On Purchase";
            UnleashedData.Columns[10].DataPropertyName = "StockOnPurchase";
            UnleashedData.Columns[10].Width = 120;

            UnleashedData.Columns[11].Name = "UpcomingOrderQuantities";
            UnleashedData.Columns[11].HeaderText = "Upcoming Order Quantities (" + TimeSpanUpcoming + " days)";
            UnleashedData.Columns[11].DataPropertyName = "UpcomingOrderQuantities";

            UnleashedData.Columns[12].Name = "UpcomingOrderQuantities2";
            UnleashedData.Columns[12].HeaderText = "Upcoming Order Quantities (later)";
            UnleashedData.Columns[12].DataPropertyName = "UpcomingOrderQuantities2";

            UnleashedData.Columns[13].Name = "MinStockQty";
            UnleashedData.Columns[13].HeaderText = "Minimum Stock Quantities";
            UnleashedData.Columns[13].DataPropertyName = "MinStockQty";

            UnleashedData.Columns[14].Name = "ProductGroup";
            UnleashedData.Columns[14].HeaderText = "Product Group";
            UnleashedData.Columns[14].DataPropertyName = "ProductGroup";

            UnleashedData.DataSource = SavedRecord;
        }

        int firstIndex = 0;
        int rowIndex = 0;
        int newIndex = 0;
        string SerchText = string.Empty;
        private void CommonDataFillSearch()
        {
            // Code to search the  alphanumneric Part Number (in Column1 header called "PART NUMBER") and highlihgt the row
            //foreach (System.Windows.Forms.DataGridViewRow row in UnleashedData.Rows)
            //{
            //    var cellValue = row.Cells["ProductCode"].Value;
            //    if (cellValue != null && cellValue.ToString() == SearchProduct.Text)
            //    {
            //        // UnleashedData.Rows[row.Index].DefaultCellStyle.BackColor = System.Drawing.Color.Yellow;
            //        UnleashedData.Rows[previousRow].Selected = false;
            //        UnleashedData.Rows[firstSelectedRow].Selected = false;
            //        UnleashedData.Rows[row.Index].Selected = true;
            //        previousRow = row.Index;
            //        UnleashedData.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Blue;
            //        //UnleashedData.FirstDisplayedScrollingRowIndex = UnleashedData.SelectedRows[0].Index;
            //        UnleashedData.CurrentCell = UnleashedData.Rows[row.Index].Cells[0];
            //    }

            //}


            foreach (System.Windows.Forms.DataGridViewRow row in UnleashedData.Rows)
            {
                if (row.Index == UnleashedData.Rows.Count - 1)
                {
                    System.Windows.MessageBox.Show("You has searched to the end of the list. Do you want to continue at the beginning?");
                    continue;
                }
                if (SerchText != SearchProduct.Text)
                {
                    UnleashedData.FirstDisplayedScrollingRowIndex = 0;
                    newIndex = 0;
                    //initialIndex = 0;
                }

                if (row.Index <= newIndex)
                    continue;
                else
                    newIndex = 0;
                var cellValue = row.Cells["ProductCode"].Value;
                var productDescription = row.Cells["ProductDescription"].Value;
                SerchText = SearchProduct.Text;

                if (cellValue != null && cellValue.ToString().ToLower().Contains(SearchProduct.Text.ToLower()) || productDescription != null && productDescription.ToString().ToLower().Contains(SearchProduct.Text.ToLower()))
                {
                    UnleashedData.Rows[previousRow].Selected = false;
                    UnleashedData.Rows[newIndex].Selected = false;
                    if (firstIndex == 0)
                    {
                        rowIndex = row.Index < newIndex ? newIndex + 1 : row.Index;
                        firstIndex = row.Index < newIndex ? newIndex + 1 : row.Index;
                    }
                    if (newIndex != row.Index)
                    {
                        UnleashedData.Rows[newIndex].Selected = false;
                        UnleashedData.Rows[row.Index < newIndex ? newIndex + 1 : row.Index].Selected = false;
                        UnleashedData.Rows[row.Index < newIndex ? newIndex + 1 : row.Index].Selected = true;
                        previousRow = row.Index < newIndex ? newIndex + 1 : row.Index;
                        UnleashedData.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Blue;
                        newIndex = row.Index < newIndex ? newIndex + 1 : row.Index;

                        //if (initialIndex == 0)
                        //    initialIndex = newIndex;

                        break;
                    }




                }
                else if (productDescription != null && productDescription.ToString().ToLower().Contains(SearchProduct.Text.ToLower()))
                {
                    break;
                }
            }
            if (newIndex == 0 && UnleashedData.FirstDisplayedScrollingRowIndex <= 0)
            {
                UnleashedData.Rows[previousRow].Selected = false;
                UnleashedData.Rows[newIndex].Selected = false;
                System.Windows.MessageBox.Show("Your search has found 0 results.");
            }
            else if (newIndex > 0)
            {
                UnleashedData.FirstDisplayedScrollingRowIndex = newIndex;
            }

            //if (newIndex == 0 && initialIndex > 0)
            //{
            //    newIndex = initialIndex;
            //    UnleashedData.Rows[previousRow].Selected = false;
            //    UnleashedData.Rows[newIndex].Selected = true;

            //    UnleashedData.FirstDisplayedScrollingRowIndex = newIndex;
            //}

        }

        private void SwitchWindow_Click(object sender, RoutedEventArgs e)
        {
            UnleashedWindow obj = new UnleashedWindow();
            obj.Show();
            this.Close();
        }
        
        int previousRow = 0;
        private void UnleashedData_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.DataGridView.HitTestInfo testInfo = UnleashedData.HitTest(e.X, e.Y);

            //Need to check to make sure that the row index is 0 or greater as the first
            //row is a zero and where there is no rows the row index is a -1
            if (testInfo.RowIndex >= 0 && testInfo.RowIndex != previousRow)
            {
                //UnleashedData.Rows[previousRow].Selected = false;
                //UnleashedData.Rows[testInfo.RowIndex].Selected = true;
                //previousRow = testInfo.RowIndex;
                //UnleashedData.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Blue;
            }
        }

        private void ChkUpcomingOrderWinodw_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UpcomingOrder();
            if (dialog.ShowDialog() == true)
            {

                if (dialog.ResponseText.Trim() != string.Empty)
                {
                    TimeSpanUpcoming = dialog.ResponseText.Trim();
                    MRPTool.Properties.Settings.Default.TimeSpanChange = TimeSpanUpcoming;
                    UpcomingOrderSettings.Text = TimeSpanUpcoming + " days";

                }
            }
            ChkUpcomingOrderWinodw.IsChecked = false;
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                //var result = CommonCode.GenerateTopPurchaseOrder(Guid.NewGuid().ToString());
                // var SplitResult = result.Split('-');
                // var SplitResultInt = Convert.ToInt64(SplitResult[1]) + 1;
                //var TotalZeros = "";
                //switch (SplitResultInt.ToString().Count())
                //{
                //    case 3:
                //        TotalZeros = "00000";
                //        break;
                //    case 4:
                //        TotalZeros = "0000";
                //        break;
                //    case 5:
                //        TotalZeros = "000";
                //        break;
                //    case 6:
                //        TotalZeros = "00";
                //        break;
                //    case 7:
                //        TotalZeros = "0";
                //        break;
                //    default:
                //        break;
                //}
                string message = string.Empty;
                string order = "<?xml version=\"1.0\"?> \r\n <PurchaseOrder xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns = \"http://api.unleashedsoftware.com/version/1\" >";
                string supplierCode = string.Empty;
                string currencyCode = string.Empty;
                double taxRate = 0.0;
                double subTotal = 0.0;
                double taxTotal = 0.0;
                double total = 0.0;
                Warehouses wh = new Warehouses();
                //Random generator = new Random();
                //var result = CommonCode.GenerateTopPurchaseOrder(Guid.NewGuid().ToString());

                //int r = generator.Next(0000000, 9999999);
                order += "\r\n<Guid>" + Guid.NewGuid().ToString() + "\r\n</Guid><OrderDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</OrderDate>";
                order += "<RequiredDate>" + DateTime.Now.AddDays(Convert.ToInt32(TimeSpanUpcoming)).ToString("yyyy-MM-dd") + "</RequiredDate>";
                order += " <Supplier><SupplierCode>{0}</SupplierCode></Supplier><OrderStatus>Parked</OrderStatus><ExchangeRate>0.478090</ExchangeRate><Currency><CurrencyCode>{1}</CurrencyCode></Currency>";
                order += "<PurchaseOrderLines>";
                int i = 0;
                SupplierItem supplier = null;
                foreach (System.Windows.Forms.DataGridViewRow row in UnleashedData.Rows)
                {
                    bool isSelected = Convert.ToBoolean(row.Cells["checkBoxColumn"].EditedFormattedValue);
                    if (isSelected)
                    {
                        i = i + 1;
                        string productCode = row.Cells["ProductCode"].Value.ToString();
                        DataRow[] dr = CommonCode.productDt.Select("[Product Code]='" + productCode + "'");
                        double defaultPurchasePrice = 0;
                        string units = string.Empty;
                        if (dr.Count() > 0)
                        {
                            defaultPurchasePrice = Convert.ToDouble(dr[0]["Default Purchase Price"]);
                            units = Convert.ToString(dr[0]["Units"]);

                        }

                        if (supplierCode == string.Empty)
                        {
                            supplierCode = row.Cells["SupplierCode"].Value.ToString();
                            var response = CommonCode.GetSupplierInformation(supplierCode);
                            if (response != null)
                            {
                                supplier = response.Items.Where(m => m.SupplierCode == supplierCode).FirstOrDefault();
                                if (supplier != null)
                                    currencyCode = supplier.Currency.CurrencyCode;

                            }
                        }
                        double linetax = 0;

                        //UpcomingOrderQuantities
                        string orderQuantity = row.Cells["Quantity"].Value.ToString();
                        if (orderQuantity == string.Empty)
                        {
                            orderQuantity = "0";
                        }
                        //objSupplier.SupplierCode = row.Cells["SupplierCode"].Value.ToString();
                        double lineTotal = defaultPurchasePrice * Convert.ToDouble(orderQuantity);
                        // double lineTotal = RoundUp(defaultPurchasePrice, 2) * Convert.ToDouble(orderQuantity);



                        subTotal += lineTotal;
                        if (supplier != null)
                        {
                            if (supplier.Taxable == true)
                            {
                                linetax = (lineTotal * 10) / 100;
                                //linetax = Math.Round(defaultPurchasePrice, 2)
                                taxRate = 10;
                                taxTotal += linetax;
                            }
                        }

                        order += "<PurchaseOrderLine><LineNumber> " + i + " </LineNumber><Product><ProductCode>" + productCode + "</ProductCode></Product>";
                        order += "<OrderQuantity> " + orderQuantity + " </OrderQuantity><UnitPrice>" + defaultPurchasePrice + "</UnitPrice><LineTotal>" + lineTotal + "</LineTotal><LineTax>" + linetax + "</LineTax>";
                        // order += "<BCUnitPrice>" + Math.Round(defaultPurchasePrice,2) + "</BCUnitPrice><BCSubTotal>" + Math.Round(lineTotal,2) + "</BCSubTotal>";
                        order += "<UnitOfMeasure><Name>" + units + "</Name></UnitOfMeasure>";
                        order += "</PurchaseOrderLine>";
                    }
                }
                total = subTotal + taxTotal;
                subTotal = subTotal;
                taxTotal = taxTotal;
                order += "</PurchaseOrderLines><TaxRate>" + Math.Round(taxRate / 100, 2) + "</TaxRate><SubTotal>" + subTotal + "</SubTotal><TaxTotal>" + taxTotal + "</TaxTotal><Total>" + total + "</Total>";

                //order += "<BCSubTotal>" + subTotal + "</BCSubTotal><BCTaxTotal>" + taxTotal + "</BCTaxTotal><BCTotal>" + total + "</BCTotal>";
                if (glbWarehouse != string.Empty)
                {
                    order += "<Warehouse><WarehouseCode>" + glbWarehouse + "</WarehouseCode></Warehouse>";
                    wh = CommonCode.GetWarehouseInformation(glbWarehouse);
                }
                order += "<DiscountRate>0.00</DiscountRate></PurchaseOrder>";
                string completeOrder = string.Format(order, supplierCode, currencyCode);

                if (supplierCode != string.Empty)
                {
                    try
                    {
                        var xmlstring = CommonCode.AddPurchaseOrder(completeOrder);
                        var firstIndex = xmlstring.IndexOf("<Guid>");
                        var lastIndex = xmlstring.IndexOf("</Guid>");
                        int indexStart = firstIndex + 6;
                        int indexLast = lastIndex - firstIndex - 6;
                        if (indexStart >= 0 && indexLast >= 0)
                        {
                            var remainingString = xmlstring.Substring(indexStart, indexLast);
                            var result222 = CommonCode.GeneratePurchaseOrder(remainingString);
                            //ConvertHTMLtoPDF(result222, wh);
                            Helpers.Converters.ConvertHTMLtoPDF(result222, wh);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(xmlstring);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                else
                {
                    System.Windows.MessageBox.Show("Supplier is not selected.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.InnerException + "," + ex.Message + "," + ex.StackTrace, "Error");
            }

        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        /* public void ConvertHTMLtoPDF(PurchaseOrdersItem response, Warehouses wh)
        {
            var readFile = AppDomain.CurrentDomain.BaseDirectory + @"Bill\Page.html";

            var html = System.IO.File.ReadAllText(readFile);
            var warehouse = wh.Items.Count > 0 ? wh.Items.SingleOrDefault() : new Warehouse();

            var logoPath = AppDomain.CurrentDomain.BaseDirectory + @"Bill\customhtml.html";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string requiredDate = string.Empty;
            string orderDate = string.Empty;
            string orderNumber = string.Empty;
            string supplierCode = string.Empty;
            string referenceNumber = string.Empty;
            //string subTotal = string.Empty;
            //string taxTotal = string.Empty;
            //string completeTotal = string.Empty;
            StringBuilder sBuilder = new StringBuilder();
            var item = response;

            requiredDate = item.RequiredDate.ToString("dd/MM/yyyy");
            orderDate = item.OrderDate.ToString("dd/MM/yyyy");
            orderNumber = item.OrderNumber;
            supplierCode = item.Supplier == null ? "" : item.Supplier.SupplierCode;
            referenceNumber = item.Guid;
            double subTotal = item.SubTotal ?? 0;
            double taxTotal = item.TaxTotal ?? 0;
            double completeTotal = item.Total ?? 0;


            html = html.Replace("##SupCode##", supplierCode);
            html = html.Replace("##DeliveryName##", warehouse == null ? "" : warehouse.StreetNo);
            html = html.Replace("##StreetAddress##", warehouse == null ? "" : warehouse.AddressLine1);
            html = html.Replace("##Suburb##", warehouse == null ? "" : warehouse.AddressLine2);
            html = html.Replace("##State##", warehouse == null ? "" : warehouse.Region);
            html = html.Replace("##PostalCode##", warehouse == null ? "" : warehouse.PostCode);
            html = html.Replace("##PhoneNumber##", warehouse == null ? "" : warehouse.PhoneNumber);

            if (item.PurchaseOrderLines != null)
            {

                foreach (var item1 in item.PurchaseOrderLines)
                {
                    var Product = CommonCode.GetProductInformation(item1.Product != null ? item1.Product.ProductCode : "");
                    var suppCode = (Product.Items.Count > 0 && Product.Items.FirstOrDefault().Supplier != null) ? Product.Items.FirstOrDefault().Supplier.SupplierProductCode : "";

                    string lineNumber = item1.LineNumber.ToString();
                    string productCode = item1.Product.ProductCode;
                    string productDescription = item1.Product.ProductDescription;
                    string quantity = item1.OrderQuantity.ToString();
                    string units = item1.Product != null && item1.Product.UnitOfMeasure != null ? item1.Product.UnitOfMeasure.Name : "";
                    double unitPrice = item1.UnitPrice ?? 0;
                    double amount = item1.LineTotal ?? 0;
                    double tax = item1.LineTax ?? 0;
                    string htmlFormat = string.Format("<tr style='border-bottom:2px solid #ddd;'><th style = 'padding: 5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal; font-size: 13px;' >{0}</p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'>{1}</p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {2} </p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {3}</p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {4} </p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {5} </p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {6} </p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {7} </p></th>"
                            + "<th style = 'padding:5px 0; border-bottom:1px solid #ddd;'><p style = 'margin:0; font-weight:normal;font-size: 13px;'> {8} </p></th></tr>", lineNumber, suppCode, productCode, productDescription, quantity, units, unitPrice.ToString("0.00"), amount.ToString("0.00"), tax.ToString("0.00")
                            );
                    sBuilder.Append(htmlFormat);
                }
            }



            html = html.Replace("##RequiredDate##", requiredDate);
            html = html.Replace("##OrderDate##", orderDate);
            html = html.Replace("##OrderNumber##", orderNumber);
            html = html.Replace("##SupplierCode##", supplierCode);
            html = html.Replace("##ReferenceNumber##", referenceNumber);
            html = html.Replace("##ReplaceBodyContent##", sBuilder.ToString());

            html = html.Replace("##SubTotal##", subTotal.ToString("0.00"));
            html = html.Replace("##TaxTotal##", taxTotal.ToString("0.00"));
            html = html.Replace("##CompleteTotal##", completeTotal.ToString("0.00"));
            html = html.Replace("##Instruments##", item.Supplier.SupplierName);
            html = html.Replace("##Comments##", item.Comments);
            System.IO.File.WriteAllText(logoPath, html);

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            converter.Options.MarginRight = 5;
            converter.Options.MarginLeft = 5;
            SelectPdf.PdfDocument doc = converter.ConvertUrl(logoPath);

            string WriteExcel = path + "//Purchase Orders";
            if (!Directory.Exists(WriteExcel))
            {
                Directory.CreateDirectory(WriteExcel);
            }
            string excelFile = WriteExcel + "//Purchase Order_" + item.OrderNumber + "_" + DateTime.Now.ToString("yyyy.dd.MM").Replace("/", ".") + "-" + item.Supplier.SupplierName.Replace("/", "_") + ".pdf";
            doc.Save(excelFile);
            doc.Close();
        } */

        private void UnleashedData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (UnleashedData.IsCurrentCellDirty)
            {
                UnleashedData.CommitEdit(System.Windows.Forms.DataGridViewDataErrorContexts.Commit);
            }
        }

        private void UnleashedData_DataError(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
        {

        }

        private void SearchProduct_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (SearchProduct.Text.Trim().ToLower() != string.Empty)
                {
                    if (SavedRecord.Rows.Count > 0)
                    {
                        CommonDataFillSearch();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("No record available to filter. Please calculate stock order first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    CommonDataFill();

                }
            }
        }

        private void UnleashedData_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            UnleashedData.CommitEdit(System.Windows.Forms.DataGridViewDataErrorContexts.Commit);
        }
        
        string glbSupplier = string.Empty;
        string glbProduct = string.Empty;
        string glbWarehouse = string.Empty;
        private void UnleashedData_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex != -1)
            {
                // Handle checkbox state change here
                if (UnleashedData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "True")
                {
                    //do something
                    string supplier = UnleashedData.Rows[e.RowIndex].Cells[6].Value.ToString();
                    string productgroup = UnleashedData.Rows[e.RowIndex].Cells[14].Value.ToString();
                    if (glbSupplier == string.Empty || glbSupplier == supplier)
                    {

                        glbSupplier = supplier;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Please choose same supplier.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        UnleashedData.Rows[e.RowIndex].Cells[4].Value = false;
                        UnleashedData.CancelEdit();
                    }

                    if (glbProduct == string.Empty || glbProduct == productgroup)
                    {
                        switch (productgroup)
                        {
                            case "Meters":
                                glbWarehouse = "MWF";
                                break;
                            case "Sensors":
                                glbWarehouse = "MWQ";
                                break;
                            case "Projects":
                                glbWarehouse = "Projects";
                                break;
                            default:
                                break;
                        }
                        glbProduct = productgroup;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Please choose same product group.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        UnleashedData.Rows[e.RowIndex].Cells[4].Value = false;
                        UnleashedData.CancelEdit();
                    }

                }
                else
                {
                    //do something
                    int encountercheck = 0;
                    foreach (System.Windows.Forms.DataGridViewRow row in UnleashedData.Rows)
                    {
                        System.Windows.Forms.DataGridViewCheckBoxCell chk = (System.Windows.Forms.DataGridViewCheckBoxCell)row.Cells[4];
                        if (chk.Value != null)
                        {
                            bool checkTrue = (bool)chk.Value; //because chk.Value is initialy null
                            if (checkTrue)
                            {
                                encountercheck = 1;
                            }
                        }

                    }
                    if (encountercheck == 0)
                    {
                        glbSupplier = string.Empty;
                        glbProduct = string.Empty;
                    }
                }
            }
        }
        
        List<Item> ItemsProducts = new List<Item>();
        private void ShowPopUp_CellMouseClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                string productCode = UnleashedData.Rows[e.RowIndex].Cells[0].Value.ToString();
                ProductDetailWindow productDetail = new MRPTool.ProductDetailWindow(productCode);

                productDetail.Show();
                //var responseProducts = UnLeashedMain.GetJson("Products/1", ApiId, ApiKey, "");
                //var dtProducts = JsonConvert.DeserializeObject<ULProducts>(responseProducts);
                ////ItemsProducts.AddRange(dtProducts.Items);
                //MessageBox.Show("Files are missing,please provide all the files.", "Warning");
            }
        }

        private void CmbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
           DropDownFilters();

        }

        private void DropDownFilters()
        {
            DataTable dt = new DataTable();
           
            dt = result;
            //var selectedTag = ((ComboBoxItem)cmbOrderStatus.SelectedItem).Tag.ToString();
            var selectedTag = cmbOrderStatus.SelectedItem;

            if (selectedTag != "--Order Status--" && selectedTag != "ALL" && cmbOrderStatus.Text != "")
            {
                DataView dv1 = dt.DefaultView;
                dv1.RowFilter = "RequiredOrderStatus= '" + selectedTag + "'";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }
            else if (selectedTag == "ALL")
            {
                DataView dv1 = dt.DefaultView;
                dv1.RowFilter = "";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }

            if (CmbSupplier.SelectedValue != "--- Supplier ---" && CmbSupplier.SelectedValue != "ALL" && CmbSupplier.Text != "" && CmbSupplier.SelectedValue != "0")
            {
                DataView dv1 = dt.DefaultView;
                //dv1.RowFilter = "SupplierCode = '" + CmbSupplier.SelectedValue + "'";
                dv1.RowFilter = "SupplierCode IN (1011, 1012)";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }
            else if (CmbSupplier.SelectedValue == "ALL")
            {
                DataView dv1 = dt.DefaultView;
                dv1.RowFilter = "";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }


            string SelectedProduct = cmbProduct.SelectedValue.ToString();

            if (SelectedProduct != "-- Product Group --" && SelectedProduct != "ALL" && cmbProduct.Text != "" && cmbProduct.SelectedValue != "-1")
            {
                DataView dv1 = dt.DefaultView;
                dv1.RowFilter = "ProductGroup= '" + cmbProduct.SelectedValue + "'";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }
            else if (SelectedProduct == "ALL")
            {
                DataView dv1 = dt.DefaultView;
                dv1.RowFilter = "";
                dt = dv1.ToTable();
                CommonDataFill(dt);
            }
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            DropDownFilters();
        }

        private void cmbOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            DropDownFilters();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

           
            //Bind_DropDownLists();

        }

        private void Bind_DropDownLists()
        {
            dtOrderStatus = new DataTable();
            dtSupplier = new DataTable();
            dtProduct = new DataTable();

            orderStatusString = new StringBuilder();
            supplierString = new StringBuilder();
            productString = new StringBuilder();

            // ComboBox for the Order Status
            dtOrderStatus.Columns.Add("Name", typeof(string));
            dtOrderStatus.Columns.Add("IsChecked", typeof(bool));

            //dtOrderStatus.Rows.Add("--Order Status--", false);
            //dtOrderStatus.Rows.Add("All", false);
            dtOrderStatus.Rows.Add("Order Now", false);
            dtOrderStatus.Rows.Add("Upcoming", false);
            dtOrderStatus.Rows.Add("No Order Required", false);

            cmbOrderStatus.DataContext = dtOrderStatus;

            // ComboBox for Suppliers
            var Supplierresponse = CommonCode.GetSuppliers();
            var distinctSuppliers = (from row in result.AsEnumerable()
                                     select new { 
                                         SupplierCode = row.Field<string>("SupplierCode"), 
                                         SupplierName = row.Field<string>("SupplierName") 
                                     }).Distinct().OrderBy(ee => ee.SupplierName);

            dtSupplier.Columns.Add("SupplierName", typeof(string));
            dtSupplier.Columns.Add("SupplierCode", typeof(string));
            dtSupplier.Columns.Add("IsChecked", typeof(bool));

            //dtSupplier.Rows.Add("All", CommonCode.GetSuppliers().Items.Count(), false);

            foreach (var supplier in distinctSuppliers)
            {
                if (!String.IsNullOrEmpty(supplier.SupplierName) && !String.IsNullOrWhiteSpace(supplier.SupplierName))
                    dtSupplier.Rows.Add(supplier.SupplierName, supplier.SupplierCode, false);
            }

            CmbSupplier.DataContext = dtSupplier;


            // Combox for the Products
            var ProductResponse = CommonCode.GetProductsGroupInfo();
            var Productenumerable = ProductResponse.Items;
            ProductGroup productGroupMeter = Productenumerable.Where(m => m.GroupName.ToLower() == "meters").FirstOrDefault();
            ProductGroup productGroupSensor = Productenumerable.Where(m => m.GroupName.ToLower() == "sensors").FirstOrDefault();
            int indexMeter = Productenumerable.IndexOf(productGroupMeter);
            int indexSensor = Productenumerable.IndexOf(productGroupSensor);
            Productenumerable.RemoveAt(indexMeter);
            Productenumerable.RemoveAt(indexSensor);
            Productenumerable.Insert(0, productGroupMeter);
            Productenumerable.Insert(1, productGroupSensor);

            dtProduct.Columns.Add("GroupName", typeof(string));
            dtProduct.Columns.Add("Value", typeof(string));
            dtProduct.Columns.Add("IsChecked", typeof(bool));

            //dtProduct.Rows.Add("All", CommonCode.GetProductsGroupInfo().Items.Count(), false);

            foreach (var product in Productenumerable)
            {
                dtProduct.Rows.Add(product.GroupName, product.GroupName, false);
            }

            cmbProduct.DataContext = dtProduct;
        }

        private void OrderStatusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var selectedContent = checkbox.Content;
            var getRow = dtOrderStatus.Select("Name='" + selectedContent +"'").FirstOrDefault();
            int index = dtOrderStatus.Rows.IndexOf(getRow);
            cmbOrderStatus.SelectedIndex = index;
            BindOrderStatusComboBox();
        }

        private void OrderStatusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cmbOrderStatus.SelectedIndex = this.dtOrderStatus.Rows.IndexOf(((IEnumerable<DataRow>)this.dtOrderStatus.Select("IsChecked=True")).FirstOrDefault<DataRow>());
            BindOrderStatusComboBox();
        }

        private void SupplierCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var selectedContent = checkbox.Content;
            var getRow = dtSupplier.Select("SupplierName='" + selectedContent + "'").FirstOrDefault();
            int index = dtSupplier.Rows.IndexOf(getRow);
            CmbSupplier.SelectedIndex = index;
            BindSupplierComboBox();
        }

        private void SupplierCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.CmbSupplier.SelectedIndex = this.dtSupplier.Rows.IndexOf(((IEnumerable<DataRow>)this.dtSupplier.Select("IsChecked=True")).FirstOrDefault<DataRow>());
            BindSupplierComboBox();
        }

        private void ProductCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var selectedContent = checkbox.Content;
            var getRow = dtProduct.Select("GroupName='" + selectedContent + "'").FirstOrDefault();
            int index = dtProduct.Rows.IndexOf(getRow);
            cmbProduct.SelectedIndex = index;
            BindProductComboBox();
        }

        private void ProductCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cmbProduct.SelectedIndex = this.dtProduct.Rows.IndexOf(((IEnumerable<DataRow>)this.dtProduct.Select("IsChecked=True")).FirstOrDefault<DataRow>());
            BindProductComboBox();
        }

        private void BindOrderStatusComboBox()
        {
            var query = from r in dtOrderStatus.AsEnumerable()
                        where r.Field<bool>("IsChecked") == true
                        select new
                        {
                            Name = r["Name"].ToString()
                        };

            orderStatusString = new StringBuilder();

            foreach (var item in query)
            {
                orderStatusString.Append("'" + item.Name + "',");
            }

            if (orderStatusString != null && !orderStatusString.ToString().Equals(""))
            {
                orderStatusString.Remove(orderStatusString.Length - 1, 1);
                // cmbOrderStatus.Text = orderStatusString.ToString();
            }
            
            DataTable dt = new DataTable();
            dt = result;
            dt = Helpers.PersistentFilters.GetFilteredData(dt.DefaultView, orderStatusString, supplierString, productString);

            CommonDataFill(dt);
            //empty combobox
            if(orderStatusString.ToString()==string.Empty)
            {
                cmbOrderStatus.SelectedIndex = -1;
            }
        }

        private void BindSupplierComboBox()
        {
            var query = from r in dtSupplier.AsEnumerable()
                        where r.Field<bool>("IsChecked") == true
                        select new
                        {
                            Name = r["SupplierCode"].ToString()
                        };

            supplierString = new StringBuilder();

            var count = query.Count();

            foreach (var item in query)
            {
                supplierString.Append("'" + item.Name + "',");
            }

            if (supplierString != null && !supplierString.ToString().Equals(""))
            {
                supplierString.Remove(supplierString.Length - 1, 1);
                // cmbOrderStatus.Text = supplierString.ToString();
            }

            DataTable dt = new DataTable();
            dt = result;
            dt = Helpers.PersistentFilters.GetFilteredData(dt.DefaultView, orderStatusString, supplierString, productString);

            CommonDataFill(dt);
            //empty combobox
            if (supplierString.ToString() == string.Empty)
            {
                CmbSupplier.SelectedIndex = -1;
            }
        }

        private void BindProductComboBox()
        {
            var query = from r in dtProduct.AsEnumerable()
                        where r.Field<bool>("IsChecked") == true
                        select new
                        {
                            Name = r["Value"].ToString()
                        };

            productString = new StringBuilder();
            var count = query.Count();

            foreach (var item in query)
            {
                productString.Append("'" + item.Name + "',");
            }

            if (productString != null && !productString.ToString().Equals(""))
            {
                productString.Remove(productString.Length - 1, 1);
                //cmbOrderStatus.Text = productString.ToString();
            }

            
            DataTable dt = new DataTable();
            dt = result;
            dt = Helpers.PersistentFilters.GetFilteredData(dt.DefaultView, orderStatusString, supplierString, productString);

            CommonDataFill(dt);
            //empty combobox
            if (productString.ToString() == string.Empty)
            {
                cmbProduct.SelectedIndex = -1;
            }
        }
    }
}