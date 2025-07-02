/*To compile: dotnet run*/
using System;
using System.Threading.Tasks;
using System.Linq;
using InventoryApp.Data;
using InventoryApp.Services;

namespace InventoryApp
{
    class Program
    {
        static async Task Main()
        {
            // Set up DbContext & service
            using var db = new AppDbContext();
            var      srv = new InventoryService(db);

            // Bulk import from JSON on startup
            await srv.ImportFromJsonAsync("import.json");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Inventory Manager ===\n");

                var items = await srv.GetAllAsync();

                // Low-stock alerts
                const int LOW_STOCK_THRESHOLD = 5;
                var lowStock = items.Where(i => i.Quantity < LOW_STOCK_THRESHOLD).ToList();
                if (lowStock.Any())
                {
                    Console.WriteLine("⚠️ Low-stock items:");
                    lowStock.ForEach(i => 
                        Console.WriteLine($"  * {i.Name} (qty: {i.Quantity})")
                    );
                    Console.WriteLine();
                }

                // List all items
                if (items.Count == 0)
                {
                    Console.WriteLine("  (no items)\n");
                }
                else
                {
                    items.ForEach(i =>
                        Console.WriteLine($"{i.Id}. {i.Name} - qty: {i.Quantity}, price: {i.Price:C}")
                    );
                }

                // Total inventory value
                var totalValue = items.Sum(i => i.Quantity * i.Price);
                Console.WriteLine($"\nTotal inventory value: {totalValue:C}\n");

                // Menu options
                Console.WriteLine("[A]dd  [U]pdate Qty  [D]elete  [S]earch  [E]xport CSV  [Q]uit");
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.A:
                        Console.Write("Name: ");
                        var n = Console.ReadLine() ?? "";
                        Console.Write("Qty:  ");
                        int.TryParse(Console.ReadLine(), out var q);
                        Console.Write("Price: ");
                        decimal.TryParse(Console.ReadLine(), out var p);
                        await srv.AddAsync(n, q, p);
                        break;

                    case ConsoleKey.U:
                        Console.Write("ID to update: ");
                        int.TryParse(Console.ReadLine(), out var uid);
                        Console.Write("New qty: ");
                        int.TryParse(Console.ReadLine(), out var nq);
                        await srv.UpdateQtyAsync(uid, nq);
                        break;

                    case ConsoleKey.D:
                        Console.Write("ID to delete: ");
                        int.TryParse(Console.ReadLine(), out var did);
                        await srv.DeleteAsync(did);
                        break;

                    case ConsoleKey.S:
                        Console.Write("Search term: ");
                        var term = Console.ReadLine() ?? "";
                        var results = srv.SearchByName(term);
                        Console.WriteLine($"\nMatches for \"{term}\":");
                        results.ForEach(i =>
                            Console.WriteLine($"{i.Id}. {i.Name} - qty: {i.Quantity}, price: {i.Price:C}")
                        );
                        Console.WriteLine("\nPress any key to return.");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.E:
                        var csvPath = "export.csv";
                        srv.ExportToCsv(csvPath);
                        Console.WriteLine($"\nExported to {csvPath}");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.Q:
                        return;
                }
            }
        }
    }
}