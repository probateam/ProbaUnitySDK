using UnityEngine;
using System.IO;
using System.Text;

namespace Proba.Scripts
{
    public class ProbaLogger : MonoBehaviour
    {

        #region variables

        private bool SaveInFile { set; get; }
        private bool ShowInConsole { set; get; }

        private StringBuilder mainLog;

        #endregion

        internal void SetProperties(bool saveInFile, bool showInConsole)
        {
            SaveInFile = saveInFile;
            ShowInConsole = showInConsole;
            //open log file
            var path = Application.dataPath + "/ProbaLog.log";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "PROBA LOG\n");
            }
            mainLog = new StringBuilder(File.ReadAllText(path));
        }

        private void OnEnable()
        {
            Application.logMessageReceived += SystemLog;
        }

        private void OnDisable()
        {
            WriteLog();
            Application.logMessageReceived -= SystemLog;
        }

        private void WriteLog()
        {
            var path = Application.dataPath + "/ProbaLog.log";
            File.WriteAllText(path, mainLog.ToString());

        }
        /// <summary>logs systems logs</summary>
        /// <param name="logString">The log string.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="type">The type.</param>
        private void SystemLog(string logString, string stackTrace, LogType type)
        {
            if (SaveInFile && type != LogType.Log)
            {
                var stringBuilder = new StringBuilder(type + " : Log: " + logString + " stackTrace: " + stackTrace + "\n\n");
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
                mainLog.Append("Proba Warning: " + warning + " Message: " + message + "\n\n");
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
                mainLog.Append("Proba Error: " + error + " Message: " + message + "\n\n");
            }
            Broker.ThereIsaError();
        }
    }
}
