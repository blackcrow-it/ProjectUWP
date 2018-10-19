using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgainUWP.Data
{
    public static class DataAccess
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=Song.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Recent (" +
                    "id BIGINT PRIMARY KEY, " +
                    "name NVARCHAR(250) NOT NULL, " +
                    "description TEXT NULL," +
                    "singer NVARCHAR(250) NOT NULL," +
                    "author NVARCHAR(250) NOT NULL," +
                    "thumbnail TEXT NOT NULL," +
                    "link TEXT NOT NULL," +
                    "createdAt NVARCHAR(50) NOT NULL," +
                    "updatedAt NVARCHAR(50) NOT NULL" +
                    ")";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
