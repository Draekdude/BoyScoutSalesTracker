using System;
using System.Collections.Generic;
using System.Data;
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

        WreathData wreathData = new WreathData();
        public MainWindow()
        {
            InitializeComponent();

            loadXMLFiles(wreathData);

            createScouts(wreathData);
            createWreaths(wreathData);

            enterDate.Text = DateTime.Now.ToShortDateString();

            scoutCombo.ItemsSource = wreathData.Scout.DefaultView;
            wreathCombo.ItemsSource = wreathData.Wreath.DefaultView;

            //inventoryDataGrid.ItemsSource = wreathData.Inventory;
            //paymentsDataGrid.ItemsSource = wreathData.Payments;
        }

        private void loadXMLFiles(WreathData wreathData)
        {
            //wreathData.Wreath.ReadXml(wreathPath);
            //wreathData.Scout.ReadXml(scoutPath);
            wreathData.Inventory.ReadXml(inventoryPath);
            wreathData.Payments.ReadXml(paymentsPath);
        }

        private void saveXMLFiles(WreathData wreathData)
        {
            wreathData.Wreath.WriteXml(wreathPath);
            wreathData.Scout.WriteXml(scoutPath);
            wreathData.Inventory.WriteXml(inventoryPath);
            wreathData.Payments.WriteXml(paymentsPath);
        }

        private void createScouts(WreathData wreathData)
        {
            List<Scout> scoutList = new List<Scout>();
            scoutList.Add(new Scout("Harrison Boardman"));
            scoutList.Add(new Scout("Nathan Todzy"));

            scoutFillRows(wreathData, scoutList);
        }

        private void createWreaths(WreathData wreathData)
        {
            List<Wreath> wreathList = new List<Wreath>();
            wreathList.Add(new Wreath("20 Inch Wreath", 20, 16.45m));
            wreathList.Add(new Wreath("24 Inch Wreath", 24, 99.45m));

            wreathFillRows(wreathData, wreathList);
        }

        private void scoutFillRows(WreathData wreathData, List<Scout> scoutList)
        {
            foreach (var scout in scoutList)
            {
                WreathData.ScoutRow newScoutRow = wreathData.Scout.NewScoutRow();
                newScoutRow.Name = scout.Name;
                wreathData.Scout.Rows.Add(newScoutRow);
            }
        }

        private void wreathFillRows(WreathData wreathData, List<Wreath> wreathList)
        {
            foreach (var wreath in wreathList)
            {
                WreathData.WreathRow newWreathRow = wreathData.Wreath.NewWreathRow();
                newWreathRow.Name = wreath.Name;
                newWreathRow.Price = wreath.Price;
                newWreathRow.Size = wreath.Size;
                wreathData.Wreath.Rows.Add(newWreathRow);
            }
        }

        private void wreathCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            priceLabel.Content = findPriceFromWreathTable(wreathCombo.SelectedValue.ToString());
        }

        private decimal findPriceFromWreathTable(string wreath_Name)
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

        private void addInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = isDataValidForInventory();
            if (errorMessage is null)
            {
                Inventory inventory = readInventoryDataFromForm();
                AddInventoryItem(inventory);
                filterInventoryDataGrid(dateFilterCheckBox.IsChecked.Value);
                totalSalesLabel.Content = findTotalSales();
                clearInventoryObjects();
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void addPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = isDataValidForPayments();
            if (errorMessage is null)
            {
                Payments payments = readPaymentsDataFromForm();
                AddPaymentsItem(payments);
                filterPaymentsDataGrid(dateFilterCheckBox.IsChecked.Value);
                totalPaidLabel.Content = findTotalPaid();
                totalDueLabel.Content = findTotalDue();
                clearPaymentObjects();
            } else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void clearInventoryObjects()
        {
            wreathCombo.SelectedValue = "";
            priceLabel.Content = "";
            wreathQuanityTextBox.Text = "";
            notesTextBox.Text = "";
        }

        private void clearPaymentObjects()
        {
            cashPaymentBox.Text = "";
            checkPaymentBox.Text = "";
        }
        private void AddInventoryItem(Inventory inventory)
        {

            WreathData.InventoryRow newInventoryRow = wreathData.Inventory.NewInventoryRow();
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
            WreathData.PaymentsRow newPaymentsRow = wreathData.Payments.NewPaymentsRow();
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

        private Inventory readInventoryDataFromForm()
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
                price = findPriceFromWreathTable(wreath_Name);
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

        private Payments readPaymentsDataFromForm()
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

        private string isDataValidForInventory()
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

        private string isDataValidForPayments()
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

        private decimal findTotalSales()
        {
            decimal totalDue = 0m;
            foreach (WreathData.InventoryRow dataRowView in inventoryDataGrid.ItemsSource)
            {
                string wreathName = dataRowView.Wreath_Name;
                decimal price = findPriceFromWreathTable(wreathName);
                int quantity = dataRowView.Quantity;
                totalDue = totalDue + price * quantity;
            }

            return totalDue;
        }

        private decimal findTotalPaid()
        {
            decimal totalPaid = 0m;
            foreach (WreathData.PaymentsRow dataRowView in paymentsDataGrid.ItemsSource)
            {
                decimal cashPayment = dataRowView.Cash_Payment;
                decimal checkPayment = dataRowView.Check_Payment;
                totalPaid = totalPaid + cashPayment + checkPayment;
            }

            return totalPaid;
        }

        private string findTotalDue()
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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveXMLFiles(wreathData);
        }

        private void scoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateAllForFilters();
        }

        private void filterInventoryDataGrid(bool isDateFiltered)
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

        private void filterPaymentsDataGrid(bool isDateFiltered)
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

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            saveXMLFiles(wreathData);
            Close();
        }

        private void dateFilterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (scoutCombo.SelectedValue != null)
            {
                updateAllForFilters();
            }
        }

        private void updateAllForFilters()
        {
            filterInventoryDataGrid(dateFilterCheckBox.IsChecked.Value);
            filterPaymentsDataGrid(dateFilterCheckBox.IsChecked.Value);
            totalSalesLabel.Content = findTotalSales();
            totalPaidLabel.Content = findTotalPaid();
            totalDueLabel.Content = findTotalDue();
        }

        private void deleteInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (deleteSelectedInventory())
            {
                MessageBox.Show("Rows Deleted From Inventory Grid.");
            }
        }

        private bool deleteSelectedInventory()
        {
            bool isDeleted = false;
            try
            {
                foreach (WreathData.InventoryRow dataRowView in inventoryDataGrid.ItemsSource)
                {
                    if (dataRowView.Delete_Row)
                    {
                        dataRowView.Delete();
                        updateAllForFilters();
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

        private bool deleteSelectedPayments()
        {
            bool isDeleted = false;
            try
            {
                foreach (WreathData.PaymentsRow dataRowView in paymentsDataGrid.ItemsSource)
                {
                    if (dataRowView.Delete_Row)
                    {
                        dataRowView.Delete();
                        updateAllForFilters();
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

        private void deletePaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            if(deleteSelectedPayments())
            {
                MessageBox.Show("Rows Deleted From Payments Grid.");
            }
        }
    }
}
