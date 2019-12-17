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

namespace BoyScoutWreathTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string wreathPath = "C:\\Boy Scouts\\wreath.xml";
        const string scoutPath = "C:\\Boy Scouts\\scout.xml";
        const string inventoryPath = "C:\\Boy Scouts\\inventory.xml";
        const string paymentsPath = "C:\\Boy Scouts\\payments.xml";

        private static readonly Regex _regexInteger = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static readonly Regex _regexDecimal = new Regex("[^0-9.]+"); //regex that matches disallowed text

        SalesData wreathData = new SalesData();
        public MainWindow()
        {
            InitializeComponent();

            LoadXMLFiles(wreathData);

            CreateScouts(wreathData);
            CreateWreaths(wreathData);

            enterDate.Text = DateTime.Now.ToShortDateString();

            scoutCombo.ItemsSource = wreathData.Scout.DefaultView;
            scoutCombo.SelectedIndex = 0;
            wreathCombo.ItemsSource = wreathData.Wreath.DefaultView;

            inventoryDataGrid.ItemsSource = wreathData.Inventory;
            paymentsDataGrid.ItemsSource = wreathData.Payments;

            UpdateAllForFilters();
        }

        private void LoadXMLFiles(SalesData wreathData)
        {
            //wreathData.Wreath.ReadXml(wreathPath);
            //wreathData.Scout.ReadXml(scoutPath);
            wreathData.Inventory.ReadXml(inventoryPath);
            wreathData.Payments.ReadXml(paymentsPath);
        }

        private void SaveXMLFiles(SalesData wreathData)
        {
            wreathData.Wreath.WriteXml(wreathPath);
            wreathData.Scout.WriteXml(scoutPath);
            wreathData.Inventory.WriteXml(inventoryPath);
            wreathData.Payments.WriteXml(paymentsPath);
        }

        private void CreateScouts(SalesData wreathData)
        {
            List<Scout> scoutList = new List<Scout>
            {
                new Scout("Harrison Boardman"),
                new Scout("Harrison Andropolis"),
                new Scout("Nathan Todzy"),
                new Scout("Ross Erstad"),
                new Scout("Max "),
                new Scout("Nicholas "),
                new Scout("Matthew "),
                new Scout("Ernie "),
                new Scout("Mason "),
                new Scout("Ryan Gofford"),
                new Scout("Nico "),
                new Scout("Elijha "),
                new Scout("Jaden "),
                new Scout("Pat McCormic"),
                new Scout("Johnathan"),
                new Scout("Alex"),
                new Scout("David"),
                new Scout("Cole")
            };

            ScoutFillRows(wreathData, scoutList);
        }

        private void CreateWreaths(SalesData wreathData)
        {
            List<Wreath> wreathList = new List<Wreath>
            {
                new Wreath("24 Inch Wreath", 24, 20.00m),
                new Wreath("30 Inch Wreath", 30, 25.00m),
                new Wreath("36 Inch Wreath", 36, 32.00m),
                new Wreath("48 Inch Wreath", 48, 50.00m),
                new Wreath("60 Inch Wreath", 60, 60.00m),
                new Wreath("72 Inch Wreath", 72, 75.00m),
                new Wreath("26 Inch Door Swag", 26, 20.00m),
                new Wreath("30 Inch Candy Cane", 30, 25.00m),
                new Wreath("40 Inch Star", 40, 50.00m),
                new Wreath("26 Inch Cross", 26, 20.00m),
                new Wreath("27 Inch Easle (for Cross)", 27, 5.00m),
                new Wreath("30 Inch Tree", 30, 32.00m),
                new Wreath("25 Feet Roping", 25, 30.00m),
                new Wreath("50 Feet Roping", 50, 60.00m),
                new Wreath("100 Feet Roping", 100, 110.00m)
            };

            //List<Wreath> sortedWreathList = wreathList.Sort();
            //wreathFillRows(wreathData, sortedWreathList);  
            WreathFillRows(wreathData, wreathList);
        }

        private void ScoutFillRows(SalesData wreathData, List<Scout> scoutList)
        {
            foreach (var scout in scoutList)
            {
                SalesData.ScoutRow newScoutRow = wreathData.Scout.NewScoutRow();
                newScoutRow.Name = scout.Name;
                wreathData.Scout.Rows.Add(newScoutRow);
            }
        }

        private void WreathFillRows(SalesData wreathData, List<Wreath> wreathList)
        {
            foreach (var wreath in wreathList)
            {
                SalesData.WreathRow newWreathRow = wreathData.Wreath.NewWreathRow();
                newWreathRow.Name = wreath.Name;
                newWreathRow.Price = wreath.Price;
                newWreathRow.Size = wreath.Size;
                wreathData.Wreath.Rows.Add(newWreathRow);
            }
        }

        private void WreathCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            priceLabel.Content = FindPriceFromWreathTable(wreathCombo.SelectedValue.ToString());
        }

        private decimal FindPriceFromWreathTable(string wreath_Name)
        {
            decimal price = 0m;
            DataTable wreathDataTable = wreathData.Wreath;

            if (wreath_Name != "")
            {
                var results = from myRow in wreathDataTable.AsEnumerable()
                              where myRow.Field<string>("Name") == wreath_Name
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
                //filterInventoryDataGrid(dateFilterCheckBox.IsChecked.Value);
                UpdateAllForFilters();
                ClearInventoryObjects();
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void AddPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = IsDataValidForPayments();
            if (errorMessage is null)
            {
                Payments payments = ReadPaymentsDataFromForm();
                AddPaymentsItem(payments);
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
            wreathCombo.SelectedValue = "";
            priceLabel.Content = "";
            wreathQuanityTextBox.Text = "";
            notesTextBox.Text = "";
        }

        private void ClearPaymentObjects()
        {
            cashPaymentBox.Text = "";
            checkPaymentBox.Text = "";
        }
        private void AddInventoryItem(Inventory inventory)
        {

            SalesData.InventoryRow newInventoryRow = wreathData.Inventory.NewInventoryRow();
            newInventoryRow.Entered_Date = inventory.Entered_Date;
            newInventoryRow.Scout_Name = inventory.Scout_Name;
            newInventoryRow.Wreath_Name = inventory.Wreath_Name;
            newInventoryRow.Price = inventory.Price;
            newInventoryRow.Quantity = inventory.Quantity;
            newInventoryRow.Total_Price = inventory.Total_Price;
            newInventoryRow.Notes = inventory.Notes;
            newInventoryRow.Delete_Row = inventory.Delete_Row;
            try
            {
                wreathData.Inventory.AddInventoryRow(newInventoryRow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void AddPaymentsItem(Payments payments)
        {
            SalesData.PaymentsRow newPaymentsRow = wreathData.Payments.NewPaymentsRow();
            newPaymentsRow.Scout_Name = payments.Scout_Name;
            newPaymentsRow.Cash_Payment = payments.Cash_Payment;
            newPaymentsRow.Check_Payment = payments.Check_Payment;
            newPaymentsRow.Entered_Date = payments.Entered_Date;
            newPaymentsRow.Delete_Row = payments.Delete_Row;
            try
            {
                wreathData.Payments.AddPaymentsRow(newPaymentsRow);
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
            string wreath_Name;
            decimal price;
            int quantity;
            string notes;

            try
            {
                entered_Date = System.Convert.ToDateTime(enterDate.Text.ToString());
                scout_Name = scoutCombo.SelectedValue.ToString();
                wreath_Name = wreathCombo.SelectedValue.ToString();
                price = FindPriceFromWreathTable(wreath_Name);
                quantity = System.Convert.ToInt32(wreathQuanityTextBox.Text.ToString());
                notes = notesTextBox.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            Inventory inventory = new Inventory(scout_Name, wreath_Name, entered_Date, price, quantity,
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
            if (wreathCombo.SelectedValue == null || wreathCombo.SelectedValue.ToString() == "")
                return "Please Select Valid Wreath.";
            if (wreathQuanityTextBox.Text == null || wreathQuanityTextBox.Text =="" || _regexInteger.IsMatch(wreathQuanityTextBox.Text.ToString()))
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
            foreach (SalesData.InventoryRow dataRowView in inventoryDataGrid.ItemsSource)
            {
                string wreathName = dataRowView.Wreath_Name;
                decimal price = FindPriceFromWreathTable(wreathName);
                int quantity = dataRowView.Quantity;
                totalDue = totalDue + price * quantity;
            }

            return totalDue;
        }

        private decimal FindTotalPaid()
        {
            decimal totalPaid = 0m;
            foreach (SalesData.PaymentsRow dataRowView in paymentsDataGrid.ItemsSource)
            {
                decimal cashPayment = dataRowView.Cash_Payment;
                decimal checkPayment = dataRowView.Check_Payment;
                totalPaid = totalPaid + cashPayment + checkPayment;
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
            SaveXMLFiles(wreathData);
        }

        private void ScoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAllForFilters();
        }

        private void FilterInventoryDataGrid(bool isDateFiltered)
        {
            if (scoutCombo.SelectedValue.ToString() != "")
            {
                string selectScout = "[Scout_Name] = '" + scoutCombo.SelectedValue.ToString() + "'";
                if (isDateFiltered)
                {
                    selectScout = selectScout + " and [Entered_Date] = '" + enterDate.Text.ToString() + "'";
                }
                inventoryDataGrid.ItemsSource = wreathData.Inventory.Select(selectScout);
            }
        }

        private void FilterPaymentsDataGrid(bool isDateFiltered)
        {
            if (scoutCombo.SelectedValue.ToString() != "")
            {
                string selectScout = "[Scout_Name] = '" + scoutCombo.SelectedValue.ToString() + "'";
                if (isDateFiltered)
                {
                    selectScout = selectScout + " and [Entered_Date] = '" + enterDate.Text.ToString() + "'";
                }
                paymentsDataGrid.ItemsSource = wreathData.Payments.Select(selectScout);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveXMLFiles(wreathData);
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
                foreach (SalesData.PaymentsRow dataRowView in paymentsDataGrid.ItemsSource)
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

                DataTable inventoryDataTable = FilterDataTableByScoutName(wreathData.Inventory, scoutName);
                wb.Worksheets.Add(inventoryDataTable, "Inventory");
                DataTable paymentsDataTable = FilterDataTableByScoutName(wreathData.Payments, scoutName);
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

            DataTable inventoryDataTable = wreathData.Inventory;
            wb.Worksheets.Add(inventoryDataTable, "Inventory");
            DataTable paymentsDataTable = wreathData.Payments;
            wb.Worksheets.Add(paymentsDataTable, "Payments");
            string fileName = directoryInfo.FullName + "_" + dateTime + ".xlsx";
            wb.SaveAs(fileName);
        }
    }
}
