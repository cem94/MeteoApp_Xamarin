using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace MeteoApp
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection database;

        public DataBase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocationsSQLite.db3");
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Location>().Wait();
        }

        /*
         * Get all item.
         */
        public Task<List<Location>> GetItemsAsync()
        {
            return database.Table<Location>().ToListAsync();
        }

        /*
         * Get item.       
         * Query con query SQL.
         */
        public Task<List<Location>> GetItemsWithWhere(Location location)
        {
            return database.QueryAsync<Location>("SELECT * FROM [Location] WHERE [ID] =?", location.ID);
        }

        /*
         * Get item       
         * Query con LINQ.
         */
        public Task<Location> GetItemAsync(Location location)
        {
            return database.Table<Location>().Where(l => l.ID == location.ID).FirstOrDefaultAsync();
        }

        /*
         * Insert item.
         */
        public Task<int> InsertItemAsync(Location location)
        {
            return database.InsertAsync(location);
        }

        /*
         * Update item.
         */
        public Task<int> UpdateItemAsync(Location location)
        {
            return database.UpdateAsync(location);

        }

        /*
         * Delete item.
         */
        public Task<int> DeleteItemAsync(Location item)
        {
            return database.DeleteAsync(item);
        }
    }
}