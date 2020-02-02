using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static BoyScoutWreathTracker.SalesData;

namespace BoyScoutWreathTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string MainDirectory = @"C:\Boy Scouts\";
        const string ItemPath = MainDirectory + "item.xml";
        const string ScoutPath = MainDirectory + "scout.xml";
        const string InventoryPath = MainDirectory + "inventory.xml";
        const string PaymentPath = MainDirectory + "payment.xml";

        private ItemDataTable dtItem = new ItemDataTable();
        private ScoutDataTable dtScout = new ScoutDataTable();
        private InventoryDataTable dtInventory = new InventoryDataTable();
        private PaymentDataTable dtPayment = new PaymentDataTable();

        private static readonly Regex _regexInteger = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static readonly Regex _regexDecimal = new Regex("[^0-9.]+"); //regex that matches disallowed text

        public MainWindow()
        {
            InitializeComponent();
            LoadXMLFiles();

            enterDate.Text = DateTime.Now.ToShortDateString();

            scoutCombo.ItemsSource = dtScout;
            scoutCombo.SelectedIndex = 0;
            itemCombo.ItemsSource = dtItem;

            inventoryDataGrid.ItemsSource = dtInventory;
            paymentDataGrid.ItemsSource = dtPayment;

            UpdateAllForFilters();
        }

        private void LoadXMLFiles()
        {
            LoadItems();
            LoadScouts();
            LoadInventory();
            LoadPayments();
        }

        private void LoadItems()
        {
            if (File.Exists(ItemPath))
            {
                dtItem.ReadXml(ItemPath);
            }
            else
            {
                CreateDirectory(MainDirectory);
                CreateItems();
                dtItem.WriteXml(ItemPath);
            }
        }

        private void LoadScouts()
        {
            if (File.Exists(ScoutPath))
            {
                dtScout.ReadXml(ScoutPath);
            }
            else
            {
                CreateDirectory(MainDirectory);
                CreateScouts();
                dtScout.WriteXml(ScoutPath);
            }
        }

        private void LoadInventory()
        {
            if (File.Exists(InventoryPath))
            {
                dtInventory.ReadXml(InventoryPath);
            }
            else
            {
                CreateDirectory(MainDirectory);
                dtInventory.WriteXml(InventoryPath);
                dtInventory.ReadXml(InventoryPath);
            }
        }

        private void LoadPayments()
        {
            if (File.Exists(PaymentPath))
            {
                dtPayment.ReadXml(PaymentPath);
            }
            else
            {
                CreateDirectory(MainDirectory);
                dtPayment.WriteXml(PaymentPath);
                dtPayment.ReadXml(PaymentPath);
            }
        }

        private void SaveXMLFiles()
        {
            dtItem.WriteXml(ItemPath);
            dtScout.WriteXml(ScoutPath);
            dtInventory.WriteXml(InventoryPath);
            dtPayment.WriteXml(PaymentPath);
        }

        private void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CreateScouts()
        {
            List<Scout> scoutList = new List<Scout>
            {
                new Scout("ScoutName")
            };

            ScoutFillRows(scoutList);
        }

        private void CreateItems()
        {
            List<Item> itemList = new List<Item>
            {
                new Item("Item Name", "Description", 20.00m)
            };

            ItemFillRows(itemList);
        }

        private void ScoutFillRows(List<Scout> scoutList)
        {
            foreach (var scout in scoutList)
            {
                ScoutRow newScoutRow = dtScout.NewScoutRow();
                newScoutRow.Name = scout.Name;
                dtScout.Rows.Add(newScoutRow);
            }
        }

        private void ItemFillRows(List<Item> itemList)
        {
            foreach (var item in itemList)
            {
                ItemRow newItemRow = dtItem.NewItemRow();
                newItemRow.Name = item.Name;
                newItemRow.Price = item.Price;
                newItemRow.Description = item.Description;
                dtItem.Rows.Add(newItemRow);
            }
        }

        private void ItemCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            priceLabel.Content = FindPriceFromItemTable(itemCombo.SelectedValue.ToString());
        }

        private decimal FindPriceFromItemTable(string item_Name)
        {
            decimal price = 0m;
            DataTable itemDataTable = dtItem;

            if (item_Name != "")
            {
                var results = from myRow in itemDataTable.AsEnumerable()
                              where myRow.Field<string>("Name") == item_Name
                              select myRow.Field<decimal>("Price");
                price = results.First();
            }

            return price;
        }

        private void AddInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = IsDataValidForInventory();
            if (errorMessage is null)
            {
                Inventory inventory = ReadInventoryDataFromForm();
                AddInventoryItem(inventory);
                UpdateAllForFilters();
                ClearInventoryObjects();
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void AddPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = IsDataValidForPayments();
            if (errorMessage is null)
            {
                Payments payments = ReadPaymentsDataFromForm();
                AddPaymentItem(payments);
                FilterPaymentsDataGrid(dateFilterCheckBox.IsChecked.Value);
                totalPaidLabel.Content = FindTotalPaid();
                totalDueLabel.Content = FindTotalDue();
                ClearPaymentObjects();
            } else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void ClearInventoryObjects()
        {
            itemCombo.SelectedValue = "";
            priceLabel.Content = "";
            itemQuanityTextBox.Text = "";
            notesTextBox.Text = "";
        }

        private void ClearPaymentObjects()
        {
            cashPaymentBox.Text = "";
            checkPaymentBox.Text = "";
        }
        private void AddInventoryItem(Inventory inventory)
        {

            InventoryRow newInventoryRow = dtInventory.NewInventoryRow();
            newInventoryRow.Entered_Date = inventory.Entered_Date;
            newInventoryRow.Scout_Name = inventory.Scout_Name;
            newInventoryRow.Item_Name = inventory.Item_Name;
            newInventoryRow.Price = inventory.Price;
            newInventoryRow.Quantity = inventory.Quantity;
            newInventoryRow.Total_Price = inventory.Total_Price;
            newInventoryRow.Notes = inventory.Notes;
            newInventoryRow.Delete_Row = inventory.Delete_Row;
            try
            {
                dtInventory.AddInventoryRow(newInventoryRow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void AddPaymentItem(Payments payments)
        {
            PaymentRow newPaymentRow = dtPayment.NewPaymentRow();
            newPaymentRow.Scout_Name = payments.Scout_Name;
            newPaymentRow.Cash_Payment = payments.Cash_Payment;
            newPaymentRow.Check_Payment = payments.Check_Payment;
            newPaymentRow.Entered_Date = payments.Entered_Date;
            newPaymentRow.Delete_Row = payments.Delete_Row;
            try
            {
                dtPayment.AddPaymentRow(newPaymentRow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Inventory ReadInventoryDataFromForm()
        {
            DateTime entered_Date;
            string scout_Name;
            string item_Name;
            decimal price;
            int quantity;
            string notes;

            try
            {
                entered_Date = System.Convert.ToDateTime(enterDate.Text.ToString());
                scout_Name = scoutCombo.SelectedValue.ToString();
                item_Name = itemCombo.SelectedValue.ToString();
                price = FindPriceFromItemTable(item_Name);
                quantity = System.Convert.ToInt32(itemQuanityTextBox.Text.ToString());
                notes = notesTextBox.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            Inventory inventory = new Inventory(scout_Name, item_Name, entered_Date, price, quantity,
            notes);

            return inventory;
        }

        private Payments ReadPaymentsDataFromForm()
        {
            Payments payments = new Payments();
            try
            {
                payments.Scout_Name = scoutCombo.SelectedValue.ToString();
                payments.Cash_Payment = System.Convert.ToDecimal(cashPaymentBox.Text.ToString());
                payments.Check_Payment = System.Convert.ToDecimal(checkPaymentBox.Text.ToString());
                payments.Entered_Date = System.Convert.ToDateTime(enterDate.Text.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            return payments;
        }

        private string IsDataValidForInventory()
        {
            if (scoutCombo.SelectedValue == null || scoutCombo.SelectedValue.ToString() == "")
                return "Please Select Valid Scout Name.";
            if (itemCombo.SelectedValue == null || itemCombo.SelectedValue.ToString() == "")
                return "Please Select Valid Item.";
            if (itemQuanityTextBox.Text == null || itemQuanityTextBox.Text =="" || _regexInteger.IsMatch(itemQuanityTextBox.Text.ToString()))
                return "Please Enter Valid Wreath Quantity.";
            if (enterDate.Text == null || enterDate.Text == "")
                return "Please Enter Valid Enter Date.";

            return null;
        }

        private string   IsDataValidForPayments()
        {
            if (scoutCombo.SelectedValue == null || scoutCombo.SelectedValue.ToString() == "")
                return "Please Select Valid Scout Name.";
            if (cashPaymentBox.Text == null || _regexDecimal.IsMatch(cashPaymentBox.Text.ToString()))
                return "Please Enter Valid Cash Amount.";
            if (checkPaymentBox.Text == null || _regexDecimal.IsMatch(checkPaymentBox.Text.ToString()))
                return "Please Enter Valid Check Amount.";
            if (enterDate.Text == null || enterDate.Text == "")
                return "Please Enter Valid Enter Date.";

            return null;
        }

        private decimal FindTotalSales()
        {
            decimal totalDue = 0m;
            if (inventoryDataGrid.ItemsSource != null)
            {
                foreach (SalesData.InventoryRow dataRowView in inventoryDataGrid.ItemsSource)
                {
                    string itemName = dataRowView.Item_Name;
                    decimal price = FindPriceFromItemTable(itemName);
                    int quantity = dataRowView.Quantity;
                    totalDue = totalDue + price * quantity;
                }
            }
            return totalDue;
        }

        private decimal FindTotalPaid()
        {
            decimal totalPaid = 0m;
            if (paymentDataGrid.ItemsSource != null)
            {
                foreach (PaymentRow dataRowView in paymentDataGrid.ItemsSource)
                {
                    decimal cashPayment = dataRowView.Cash_Payment;
                    decimal checkPayment = dataRowView.Check_Payment;
                    totalPaid = totalPaid + cashPayment + checkPayment;
                }
            }
            return totalPaid;
        }

        private string FindTotalDue()
        {
            decimal totalSales = 0m;
            decimal totalPaid = 0m;

            if (totalSalesLabel.Content.ToString() != "")
            {
                totalSales = System.Convert.ToDecimal(totalSalesLabel.Content);
                if (totalPaidLabel.Content.ToString() != "")
                {
                    totalPaid = System.Convert.ToDecimal(totalPaidLabel.Content);
                    return (totalSales - totalPaid).ToString();
                }
            }
            return null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveXMLFiles();
        }

        private void ScoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAllForFilters();
        }

        private void FilterInventoryDataGrid(bool isDateFiltered)
        {
            if (scoutCombo.SelectedValue != null && scoutCombo.SelectedValue.ToString() != "")
            {
                string selectScout = "[Scout_Name] = '" + scoutCombo.SelectedValue.ToString() + "'";
                if (isDateFiltered)
                {
                    selectScout = selectScout + " and [Entered_Date] = '" + enterDate.Text.ToString() + "'";
                }
                inventoryDataGrid.ItemsSource = dtInventory.Select(selectScout);
            }
        }

        private void FilterPaymentsDataGrid(bool isDateFiltered)
        {
            if (scoutCombo.SelectedValue != null && scoutCombo.SelectedValue.ToString() != "")
            {
                string selectScout = "[Scout_Name] = '" + scoutCombo.SelectedValue.ToString() + "'";
                if (isDateFiltered)
                {
                    selectScout = selectScout + " and [Entered_Date] = '" + enterDate.Text.ToString() + "'";
                }
                paymentDataGrid.ItemsSource = dtPayment.Select(selectScout);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveXMLFiles();
            Close();
        }

        private void DateFilterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (scoutCombo.SelectedValue != null)
            {
                UpdateAllForFilters();
            }
        }

        private void UpdateAllForFilters()
        {
            FilterInventoryDataGrid(dateFilterCheckBox.IsChecked.Value);
            FilterPaymentsDataGrid(dateFilterCheckBox.IsChecked.Value);
            totalSalesLabel.Content = FindTotalSales();
            totalPaidLabel.Content = FindTotalPaid();
            totalDueLabel.Content = FindTotalDue();
        }

        private void DeleteInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteSelectedInventory())
            {
                MessageBox.Show("Rows Deleted From Inventory Grid.");
            }
        }

        private bool DeleteSelectedInventory()
        {
            bool isDeleted = false;
            try
            {
                foreach (SalesData.InventoryRow dataRowView in inventoryDataGrid.ItemsSource)
                {
                    if (dataRowView.Delete_Row)
                    {
                        dataRowView.Delete();
                        UpdateAllForFilters();
                    }
                }
                isDeleted = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return isDeleted;
        }

        private bool DeleteSelectedPayments()
        {
            bool isDeleted = false;
            try
            {
                foreach (PaymentRow dataRowView in paymentDataGrid.ItemsSource)
                {
                    if (dataRowView.Delete_Row)
                    {
                        dataRowView.Delete();
                        UpdateAllForFilters();
                    }
                }
                isDeleted = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return isDeleted;
        }

        private void DeletePaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            if(DeleteSelectedPayments())
            {
                MessageBox.Show("Rows Deleted From Payments Grid.");
            }
        }

        private void ExportScoutToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            string exportPath = ExportScoutDataToExcel();
            if (exportPath != "")
            {
                MessageBox.Show("Exported to : " + exportPath);
            }
        }

        private string ExportScoutDataToExcel()
        {
            string fileName = "";
            string scoutName = scoutCombo.SelectedValue.ToString();

            if (scoutName != "")
            {
                scoutName.Replace(" ", "_");
                string dateTime = DateTime.Now.ToShortDateString().Replace("/","_");
                DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory("C:/Boy Scouts/");
                XLWorkbook wb = new XLWorkbook();

                DataTable inventoryDataTable = FilterDataTableByScoutName(dtInventory,scoutName);
                wb.Worksheets.Add(inventoryDataTable, "Inventory");
                DataTable paymentsDataTable = FilterDataTableByScoutName(dtPayment,scoutName);
                wb.Worksheets.Add(paymentsDataTable, "Payments");
                fileName = directoryInfo.FullName + scoutName + "_" + dateTime + ".xlsx";
                wb.SaveAs(fileName);
            }

            return fileName;
        }

        private DataTable FilterDataTableByScoutName(DataTable unfilteredDataTable, string scoutName)
        {
            DataTable inventoryFilteredDataTable = new DataTable();
                
            var rows = unfilteredDataTable.AsEnumerable().Where(row => row.Field<String>("Scout_Name") == scoutName);

            if (rows.Any())
            {
                inventoryFilteredDataTable = rows.CopyToDataTable();
            }

            return inventoryFilteredDataTable;
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            string dateTime = DateTime.Now.ToShortDateString().Replace("/", "_");
            DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory("C:/Boy Scouts/");
            XLWorkbook wb = new XLWorkbook();

            DataTable inventoryDataTable = dtInventory;
            wb.Worksheets.Add(inventoryDataTable, "Inventory");
            DataTable paymentsDataTable = dtPayment;
            wb.Worksheets.Add(paymentsDataTable, "Payment");
            string fileName = directoryInfo.FullName + "_" + dateTime + ".xlsx";
            wb.SaveAs(fileName);
        }
    }
}
