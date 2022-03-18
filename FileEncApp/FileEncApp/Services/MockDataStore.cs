using FileEncApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileEncApp.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            // new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." }
            var folderPathDir = Path.Combine("/storage/emulated/0/Documents", "Encrypted Files");
            String[] temp = Directory.GetFiles(folderPathDir);

            if(temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    items.Add(new Item { Id = Guid.NewGuid().ToString(), Text = temp[i].Split(Path.DirectorySeparatorChar).Last(), Description = temp[i] });
                }
            }
            else
            {
                items.Add(new Item { Id = Guid.NewGuid().ToString(), Text = "No files found!", Description = "Encrypt some files first before they get listed here." });
            }

        } // TODO: Consider functionality where clicking on items calls on password entry and file decryption? Big overhaul of MockDataStore

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id) // clicking on entry calls this
        {
            
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}