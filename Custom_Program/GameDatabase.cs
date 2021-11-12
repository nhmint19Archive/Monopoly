using System;
using System.IO;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// The game database, created using Singleton pattern
    /// </summary>
    public class GameDatabase
    {
        // The SplashKit Database
        private Database _database;
        // Singleton: only one db instance exist in the program
        private static GameDatabase _instance;
        private GameDatabase() {
            _database = SplashKit.OpenDatabase("GameDB", "GameDB");
            Console.WriteLine("DB connected!");
            if (!HaveCellData())
            {
                Console.WriteLine("DB not initiatialize!");
                try
                {
                    InitCellData("DbInitializeSql.txt");
                    Console.WriteLine("DB initialized!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public static GameDatabase GetDatabase()
        {
            if (_instance == null)
                _instance = new GameDatabase();
            return _instance;
        }
        // Make query to the game db
        public QueryResult Query(string sql)
        {
            return _database.RunSql(sql);
        }
        // Free Database and All the queries made (usually used at the end of the game)
        public void FreeDB()
        {
            SplashKit.FreeDatabase(_database);
            SplashKit.FreeAllQueryResults();
            Console.WriteLine("DB disconnected!");
        }
        // Check if db contains cells info
        public bool HaveCellData()
        {
            QueryResult qr = Query("SELECT COUNT(*) FROM cells");
            return (qr.Successful && qr.QueryColumnForInt(0) > 0);
        }
        // Initialize cells data
        public void InitCellData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            try
            {
                int count = reader.ReadInteger();
                for (int i = 0; i < count; i++)
                {
                    Query(reader.ReadLine());
                }
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
