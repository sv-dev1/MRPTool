using MahApps.Metro.Controls;
using MRPClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MRPTool
{
    /// <summary>
    /// Interaction logic for ProductDetailWindow.xaml
    /// </summary>
    public partial class ProductDetailWindow : MetroWindow
    {
        public ProductDetailWindow()
        {
            InitializeComponent();
        }
        string productCode = string.Empty;
        public ProductDetailWindow(string productCode)
        {
            InitializeComponent();
            this.productCode = productCode;
            var response = CommonCode.GetProductInformation(productCode);
            if (response != null)
            {
                var product = response.Items.Where(m => m.ProductCode == productCode).FirstOrDefault();
                if (product != null)
                {

                    txtblockProductName.Text = product.ProductDescription;
                    txtblockProductCode.Text = product.ProductCode;
                    txtblockBarCode.Text = product.Barcode != null ? product.Barcode.ToString() : "";
                    txtblockUnitOfMeasure.Text = product.UnitOfMeasure != null ? product.UnitOfMeasure.Name : "";
                    txtblockProductGroup.Text = product.ProductGroup != null ? product.ProductGroup.GroupName : "";
                    txtblockSalesAccount.Text = product.XeroSalesAccount != null ? product.XeroSalesAccount.ToString() : "";
                    txtblockCostOfGoodsSoldAccount.Text = product.XeroCostOfGoodsAccount != null ? product.XeroCostOfGoodsAccount.ToString() : "";
                    txtpackSize.Text = product.PackSize!=null? product.PackSize.ToString():"";
                    txtMinSlot.Text = product.MinStockAlertLevel!=null? product.MinStockAlertLevel.ToString():"";
                    txtMaxSlot.Text = product.MaxStockAlertLevel != null ? product.MaxStockAlertLevel.ToString() : "";
                    txtAverageLanded.Text = product.AverageLandPrice != null ? product.AverageLandPrice.ToString() : "";

                    if (product.Supplier != null)
                    {
                        txtblockPurchasePrice.Text = product.Supplier.SupplierProductPrice != null ? product.Supplier.SupplierProductPrice.ToString() : "";
                        txtblockSupplierCode.Text = product.Supplier.SupplierCode;
                        txtblockSupplierName.Text = product.Supplier.SupplierName;
                        var responseSupplier = CommonCode.GetSupplierInformation(product.Supplier.SupplierCode);
                        if (responseSupplier!=null)
                        {
                            var supplier = responseSupplier.Items.Where(c => c.SupplierCode == product.Supplier.SupplierCode).FirstOrDefault();
                            if (supplier != null)
                                txtCurrency.Text = supplier.Currency.CurrencyCode != null ? supplier.Currency.CurrencyCode.ToString() : "";
                        }
                    }

                    SellPriceTier1.Text = product.SellPriceTier1.Value;
                    SellPriceTier2.Text = product.SellPriceTier2.Value;
                    SellPriceTier3.Text = product.SellPriceTier3.Value;
                    SellPriceTier4.Text = product.SellPriceTier4.Value;
                    SellPriceTier5.Text = product.SellPriceTier5.Value;
                    SellPriceTier6.Text = product.SellPriceTier6.Value;
                    SellPriceTier7.Text = product.SellPriceTier7.Value;
                    SellPriceTier8.Text = product.SellPriceTier8.Value;
                    SellPriceTier9.Text = product.SellPriceTier9.Value;
                    SellPriceTier10.Text = product.SellPriceTier10.Value;
                }
            }
        }
    }
}
