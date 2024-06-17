using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCalculator_Shreyansh
{
    internal class Program
    {
        private class Items
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public bool IsImported { get; set; }
            public int Quantity { get; set; }

            public Items(string name, decimal price, bool isImported, int quantity)
            {
                Name = name;
                Price = price;
                IsImported = isImported;
                Quantity = quantity;
            }
        }
        static string[] NonTaxables = {"pills", "book", "chocolate" };
        static void Main(string[] args)
        {
            var items = new List<Items>();
            Console.WriteLine("Enter the items (Type 'done' to finish):");

            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToLower() == "done") break;
                Items item = ParseItem(input);
                if (item != null) items.Add(item);
            }

            PrintReceipt(items);
            Console.ReadLine();
        }
        /// <summary>
        /// This is to map the string input to the Items class.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Items class object if input was in correct format, else returns null. </returns>
        static Items ParseItem(string input)
        {
            // Input format: "<Quantity> <item name> at <price>"
            // Example input: "1 book at 124.99"
            var parts = input.Split(new[] { " at " }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid input format. Try again.");
                return null;
            }

            var details = parts[0].Split(' ');
            if (details.Length < 2)
            {
                Console.WriteLine("Invalid input format. Try again.");
                return null;
            }

            int quantity;
            if (!int.TryParse(details[0], out quantity))
            {
                Console.WriteLine("Invalid quantity. Try again.");
                return null;
            }

            string name = string.Join(" ", details, 1, details.Length - 1);
            decimal price;
            if (!decimal.TryParse(parts[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price))
            {
                Console.WriteLine("Invalid price. Try again.");
                return null;
            }

            bool isImported = name.Contains("imported");

            return new Items(name, price, isImported, quantity);
        }
        /// <summary>
        /// Takes List of Shopping Cart items and 
        /// Print the Total tax and the cost of the item
        /// </summary>
        /// <param name="items"></param>
        static void PrintReceipt(List<Items> items)
        {
            decimal totalTax = 0;
            decimal totalCost = 0;

            foreach (var item in items)
            {
                decimal itemTax = CalculateTax(item);
                decimal itemTotal = item.Price + itemTax;
                totalTax += itemTax * item.Quantity;
                totalCost += itemTotal * item.Quantity;

                Console.WriteLine($"{item.Quantity} {item.Name}: {itemTotal:F2}");
            }

            Console.WriteLine($"Tax: {totalTax:F2}");
            Console.WriteLine($"Total: {totalCost:F2}");
        }
        /// <summary>
        /// Calculate the tax of each item independently
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The tax calculated rounding up to nearest 5 paisa</returns>
        static decimal CalculateTax(Items item)
        {
            decimal tax = 0;

            bool nonTaxableItem = false;

            for(int i = 0; i< NonTaxables.Length; i++)
            {
                if (item.Name.Contains(NonTaxables[i]))
                {
                    nonTaxableItem = true;
                    break;
                }
            }

            // Check if the item is non Taxable
            if (!nonTaxableItem)
            {
                tax += item.Price * 0.10m; // 10% of the Price
            }

            // Check if the item is imported
            if (item.IsImported)
            {
                tax += item.Price * 0.05m; // 5% import duty
            }

            // Round up to 5 paise (100 paisa = 1 rupee so 20 * 5 paisa = 1 rupee)
            tax = Math.Ceiling(tax * 20) / 20;
            return tax;
        }
    }
}