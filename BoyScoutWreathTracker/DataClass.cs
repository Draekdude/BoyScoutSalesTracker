using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoyScoutWreathTracker
{
    class Wreath
    {
        private string name;
        private int size;
        private decimal price;

        public Wreath( string name, int size, decimal price)
        {
            Name = name;
            Size = size;
            Price = price;
        }
        public string Name { get => name; set => name = value; }
        public int Size { get => size; set => size = value; }
        public decimal Price { get => price; set => price = value; }
    }
    class Scout
    {
        private string name;

        public Scout(string name)
        {
            Name = name;
        }
        public string Name { get => name; set => name = value; }
    }
    class Payments
    {
        private string scout_Name;
        private DateTime entered_Date;
        private decimal cash_Payment;
        private decimal check_Payment;
        private bool delete_Row;

        public Payments()
        {

        }

        public Payments(string scout_Name, DateTime entered_Date, decimal cash_Payment, decimal check_Payment)
        {
            Scout_Name = scout_Name;
            Entered_Date = entered_Date;
            Cash_Payment = cash_Payment;
            Check_Payment = check_Payment;
            Delete_Row = false;
        }

        public string Scout_Name { get => scout_Name; set => scout_Name = value; }
        public DateTime Entered_Date { get => entered_Date; set => entered_Date = value; }
        public decimal Cash_Payment { get => cash_Payment; set => cash_Payment = value; }
        public decimal Check_Payment { get => check_Payment; set => check_Payment = value; }
        public bool Delete_Row { get => delete_Row; set => delete_Row = value; }
    }

    class Inventory
    {
        private string scout_Name;
        private string wreath_Name;
        private DateTime entered_Date;
        private decimal price;
        private int quantity;
        private decimal total_Price;
        private string notes;
        private bool delete_Row;

        public Inventory()
        {
            
        }

        public Inventory(string scout_Name, string wreath_Name, DateTime entered_Date, decimal price, int quantity,
            string notes)
        {
            Scout_Name = scout_Name;
            Wreath_Name = wreath_Name;
            Entered_Date = entered_Date;
            Price = price;
            Quantity = quantity;
            Total_Price = price * quantity;
            Notes = notes;
            Delete_Row = false;
        }

        public string Scout_Name { get => scout_Name; set => scout_Name = value; }
        public string Wreath_Name { get => wreath_Name; set => wreath_Name = value; }
        public DateTime Entered_Date { get => entered_Date; set => entered_Date = value; }
        public decimal Price { get => price; set => price = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public decimal Total_Price { get => total_Price; set => total_Price = value; }
        public string Notes { get => notes; set => notes = value; }
        public bool Delete_Row { get => delete_Row; set => delete_Row = value; }

    }
}
