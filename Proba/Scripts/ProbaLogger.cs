using System;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Proba
{
    public class ProbaLogger : MonoBehaviour
    {

        #region variables

        private bool SaveInFile { set; get; }
        private bool ShowInConsole { set; get; }

        private string _path;

        #endregion

        internal void SetProperties(bool saveInFile, bool showInConsole)
        {
            SaveInFile = saveInFile;
            ShowInConsole = showInConsole;
            //create log file
            _path = Application.persistentDataPath + "/Log.pnm";
            if (!File.Exists(_path))
            {
                var fileStream = new FileStream(_path, FileMode.Create);
                fileStream.Close();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += SystemLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= SystemLog;
        }

        private void WriteLog(string date, string body)
        {
            var log = new LogDataScheme(date, body);
            var logJason = JsonConvert.SerializeObject(log);

            var fileStream = new FileStream(_path, FileMode.Append);
            var streamWriter = new StreamWriter(fileStream);

            streamWriter.WriteLine(logJason);
            streamWriter.Flush();

            fileStream.Close();
        }

        /// <summary>logs systems logs</summary>
        /// <param name="logString">The log string.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="type">The type.</param>
        private void SystemLog(string logString, string stackTrace, LogType type)
        {
            if (SaveInFile && type != LogType.Log)
            {
                WriteLog(DateTime.Now.ToString(), type + " : Log: " + logString + " stackTrace: " + stackTrace + "\n\n");
            }

            if (type == LogType.Error || type == LogType.Exception)
            {
                Broker.ThereIsaError();
            }
        }

        /// <summary>Logs warning manually with main thread</summary>
        /// <param name="warning">warning.</param>
        /// <param name="message">Message (Nullabe)</param>
        public void LogWarning(string warning, string message = null)
        {
            var builder = new StringBuilder();
            if (message != null)
                builder = new StringBuilder(" -> " + message);

            if (ShowInConsole)
                Debug.Log("Proba Warning: " + warning + builder);

            if (SaveInFile)
            {
                WriteLog(DateTime.Now.ToString(), "Proba Warning: " + warning + builder + "\n\n");
            }
        }

        /// <summary>Logs error manually with main thread</summary>
        /// <param name="error">Error</param>
        /// <param name="message">Message (Nullabe)</param>
        public void LogError(string error, string message = null)
        {
            var builder = new StringBuilder();
            if (message != null)
                builder = new StringBuilder(" -> " + message);

            if (ShowInConsole)
                Debug.Log("Proba Error: " + error + builder);

            if (SaveInFile)
            {
                WriteLog(DateTime.Now.ToString(), "Proba Error: " + error + builder + "\n\n");
            }
            Broker.ThereIsaError();
        }
    }
}
