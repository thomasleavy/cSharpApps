using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _db;
        public InventoryService(AppDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();      // Create DB/file if missing
        }

        public List<Item> SearchByName(string term) =>
    _db.Items
       .Where(i => EF.Functions.Like(i.Name, $"%{term}%"))
       .OrderBy(i => i.Id)
       .ToList();


        public async Task<List<Item>> GetAllAsync()
            => await _db.Items.OrderBy(i => i.Id).ToListAsync();

        public async Task AddAsync(string name, int qty, decimal price)
        {
            _db.Items.Add(new Item { Name = name, Quantity = qty, Price = price });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateQtyAsync(int id, int newQty)
        {
            var item = await _db.Items.FindAsync(id);
            if (item != null)
            {
                item.Quantity = newQty;
                await _db.SaveChangesAsync();
            }
        }

        // Delete an item by ID
        public async Task DeleteAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item != null)
            {
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

// Bulk import from JSON on startup
public async Task ImportFromJsonAsync(string path)
{
    if (!File.Exists(path)) return;
    var json = await File.ReadAllTextAsync(path);
    var list = JsonSerializer.Deserialize<List<Item>>(json)
               ?? new List<Item>();
    _db.Items.AddRange(list);
    await _db.SaveChangesAsync();
}

// Export current inventory to CSV
public void ExportToCsv(string path)
{
    var lines = _db.Items
                   .Select(i => $"{i.Id},\"{i.Name}\",{i.Quantity},{i.Price}")
                   .ToList();

    // Header + data
    File.WriteAllLines(path,
        new[] { "Id,Name,Quantity,Price" }
        .Concat(lines)
    );
}

    }
}
