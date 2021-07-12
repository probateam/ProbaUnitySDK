using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Proba
{
    [ExecuteAlways]
    public class DatabaseConnection : MonoBehaviour
    {

        #region variables

        private static string _path;
        private static int _index;

        #endregion


        public static void InitialDatabaseConnection()
        {
            _path = Application.persistentDataPath + "/ProbaEvents.pnm";
            if (!File.Exists(_path))
            {
                var fileStream = new FileStream(_path, FileMode.Create);
                fileStream.Close();
            }

            _index = FindIndex();
        }

        /// <summary>Insert unsent event and requests body into database</summary>
        /// <param name="className">type of event or request</param>
        /// <param name="body">json body</param>
        public static void InsertUnsentEvent(string className, string body)
        {
            try
            {
                var unsentEvent = new EventDataScheme(_index++, className, body, DateTime.Now.ToString());

                var fileStream = new FileStream(_path, FileMode.Append);
                var streamWriter = new StreamWriter(fileStream);
                var eventJason = JsonConvert.SerializeObject(unsentEvent);

                streamWriter.WriteLine(eventJason);
                streamWriter.Flush();

                fileStream.Close();
            }
            catch (IOException) { }
        }


        public static void InsertUnsentEvents(List<string> bodies)
        {
            try
            {
                var fileStream = new FileStream(_path, FileMode.Append);
                var streamWriter = new StreamWriter(fileStream);

                foreach (var body in bodies)
                {
                    var unsentEvent = new EventDataScheme(_index++, "EVENT", body, DateTime.Now.ToString());
                    var eventJason = JsonConvert.SerializeObject(unsentEvent);
                    streamWriter.WriteLine(eventJason);
                }

                streamWriter.Flush();
                fileStream.Close();
            }
            catch (IOException) { }
        }

        /// <summary>Return all unsent events from table</summary>
        /// <returns>all events as DataTable<br /></returns>
        public static List<EventDataScheme> GetAllEvents()
        {
            var allEvents = new List<EventDataScheme>();
            try
            {
                var fileStream = new FileStream(_path, FileMode.Open);
                var streamReader = new StreamReader(fileStream);

                string line;

                for (var i = 0; i < PlayerPrefs.GetInt("ProbaDataLine"); i++)
                {
                    streamReader.ReadLine();
                }

                while ((line = streamReader.ReadLine()) != null)
                {
                    var data = JsonConvert.DeserializeObject<EventDataScheme>(line,
                        new JsonSerializerSettings
                        {
                            CheckAdditionalContent = true
                        });
                    allEvents.Add(data);

                }
                fileStream.Close();
            }
            catch (IOException) { }

            return allEvents;

        }

        /// <summary>Delete Oldest Event from DataBase </summary>
        public static void DeleteFirstEvent(int deleteNumber)
        {
            if (deleteNumber <= 0)
                return;
            try
            {
                PlayerPrefs.SetInt("ProbaDataLine", PlayerPrefs.GetInt("ProbaDataLine") + deleteNumber);
                EmptyFile();
            }
            catch (IOException) { }
        }


        /// <summary>Flush whole DataBase</summary>
        public static void FlushDatabase()
        {
            try
            {
                var fileStream = new FileStream(_path, FileMode.Create);
                fileStream.Close();
                _index = 0;
                PlayerPrefs.SetInt("ProbaDataLine", 0);
            }
            catch (IOException) { }
        }

        /// <summary>Returns the maximum identifier.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static int GetMaxID()
        {
            return _index - 1;
        }

        /// <summary>Returns the first event</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static EventDataScheme GetFirstEvent()
        {
            try
            {
                var fileStream = new FileStream(_path, FileMode.Open);
                var streamReader = new StreamReader(fileStream);
                for (var i = 0; i < PlayerPrefs.GetInt("ProbaDataLine"); i++)
                {
                    streamReader.ReadLine();
                }

                var data = JsonConvert.DeserializeObject<EventDataScheme>(streamReader.ReadLine(),
                    new JsonSerializerSettings
                    {
                        CheckAdditionalContent = true
                    });
                fileStream.Close();

                return data;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static int FindIndex()
        {
            var index = 0;
            try
            {
                var fileStream = new FileStream(_path, FileMode.Open);
                var streamReader = new StreamReader(fileStream);

                while ((streamReader.ReadLine()) != null)
                {
                    index++;
                }
                fileStream.Close();
            }
            catch (IOException) { }
            return index;
        }

        private static void EmptyFile()
        {
            var line = PlayerPrefs.GetInt("ProbaDataLine");
            if (line != 0 && line == _index)
            {
                FlushDatabase();
            }
        }
    }
}
