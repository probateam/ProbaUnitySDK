using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace Proba.Scripts
{
    [ExecuteAlways]
    internal class DatabaseConnection : MonoBehaviour
    {


        #region variables

        private static string _connectionString;

        #endregion



        public static void InitialConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
#if UNITY_EDITOR
                _connectionString = "URI=file:" + Application.dataPath + "/Proba/DataBase/.EditorDatabase.s3db";
#else
                _connectionString = "URI=file:" + Application.dataPath + "/Proba/DataBase/ProbaDatabase.s3db";
#endif
        }

        /// <summary>Insert unsent event and requests body into database</summary>
        /// <param name="className">type of event or request</param>
        /// <param name="body">json body</param>
        public static void InsertUnsentEvent(string className, string body)
        {
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = $"INSERT INTO Events(Class,Body) VALUES(\'{className}\',\'{body}\')";
                    dbCommand.CommandText = sqlQuery;
                    dbCommand.ExecuteScalar();
                    dbConnection.Close();
                }
            }
        }

        /// <summary>Insert all event body into database just in editor</summary>
        /// <param name="className">type of event</param>
        /// <param name="body">json body</param>
        /// <param name="sent">has sent</param>
        public static void InsertEvent(string className, string body, bool sent)
        {
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = $"INSERT INTO Events(Class,Body,Sent) VALUES(\'{className}\',\'{body}\',\'{sent}\')";
                    dbCommand.CommandText = sqlQuery;
                    dbCommand.ExecuteScalar();
                    dbConnection.Close();
                }
            }
        }


        /// <summary>Gets all events from table</summary>
        /// <returns>all events as DataTable<br /></returns>
        public static DataTable GetAllEvents()
        {
            var data = new DataTable();
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = "SELECT * FROM Events";
                    dbCommand.CommandText = sqlQuery;

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        data.Load(reader);
                        dbConnection.Close();
                        reader.Close();
                    }
                }
            }
            return data;
        }

        /// <summary>Gets all unsent events from editors table</summary>
        /// <returns>all unsent events as DataTable</returns>
        public static DataTable GetUnsentEvents()
        {
            var data = new DataTable();
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = "SELECT * FROM Events WHERE Sent = 'false'";
                    dbCommand.CommandText = sqlQuery;

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        data.Load(reader);
                        dbConnection.Close();
                        reader.Close();
                    }
                }
            }

            return data;
        }

        /// <summary>Delete Event from Table </summary>
        /// <param name="ID">The events ID</param>
        public static void DeleteEvent(int ID)
        {
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = $"DELETE FROM Events WHERE ID = (\"{ID}\"); DELETE FROM SQLITE_SEQUENCE WHERE name='Events'";
                    dbCommand.CommandText = sqlQuery;
                    dbCommand.ExecuteScalar();
                    dbConnection.Close();
                }
            }
        }


        /// <summary>Flush whole table</summary>
        public static void FlushDatabase()
        {
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = $"DELETE FROM Events; DELETE FROM SQLITE_SEQUENCE WHERE name='Events'";
                    dbCommand.CommandText = sqlQuery;
                    dbCommand.ExecuteScalar();
                    dbConnection.Close();
                }
            }
        }

        /// <summary>Gets the maximum identifier.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static int GetMaxID()
        {
            var maxID = 0;
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT max (ID) FROM Events";

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            maxID = reader.GetInt32(0);
                        }
                        dbConnection.Close();
                        reader.Close();
                    }
                }
            }
            return maxID;
        }

        public static DataTable GetFirst()
        {
            var data = new DataTable();
            using (IDbConnection dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    var sqlQuery = "SELECT * FROM Events ORDER BY ID ASC LIMIT 1";
                    dbCommand.CommandText = sqlQuery;

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        data.Load(reader);
                        dbConnection.Close();
                        reader.Close();
                    }
                }
            }
            return data;
        }
    }
}
