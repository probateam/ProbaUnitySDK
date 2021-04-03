﻿using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Proba.Scripts.Configuration;
using UnityEditor;
using UnityEngine;
using Ping = System.Net.NetworkInformation.Ping;

namespace Proba.Scripts.Editor
{
    internal class ConfigWindow : EditorWindow
    {
        #region variables

        [SerializeField] private string _secretKey;
        [SerializeField] private string _publicKey;
        [SerializeField] private bool _recordAllTouches;
        [SerializeField] private bool _saveInfile;
        [SerializeField] private bool _showInConsole;
        private static KeyHolder _keyHolder;
        private bool _connected;

        #endregion


        [MenuItem("Proba/Config Window")]
        private static void OpenWindow()
        {
            GetWindow<ConfigWindow>("PROBA config Window");
        }

        private async void OnGUI()
        {
            _keyHolder = GetKeyHolder();
            if (!_keyHolder)
            {
                _keyHolder = ScriptableObject.CreateInstance<KeyHolder>();
                AssetDatabase.CreateAsset(_keyHolder, ConfigurationModel.ConfigPath);
                EditorUtility.SetDirty(_keyHolder);
                AssetDatabase.SaveAssets();
            }
            else
            {
                _publicKey = _keyHolder.GetKeys(KeyType.PublicKey);
                _secretKey = _keyHolder.GetKeys(KeyType.SecretKey);
                _recordAllTouches = _keyHolder.IsRecordAllTouches();
                _saveInfile = _keyHolder.SaveInfile();
                _showInConsole = _keyHolder.ShowInConsole();
            }

            EditorGUI.BeginChangeCheck();
            _publicKey = EditorGUILayout.TextField("Public Key: ", _publicKey);
            _secretKey = EditorGUILayout.TextField("Secret Key: ", _secretKey);
            _recordAllTouches = EditorGUILayout.Toggle("Record All Touches: ", _recordAllTouches);
            _saveInfile = EditorGUILayout.Toggle("Save Logs in File: ", _saveInfile);
            _showInConsole = EditorGUILayout.Toggle("Show Logs In Console: ", _showInConsole);
            if (EditorGUI.EndChangeCheck())
            {
                _keyHolder.SetKeys(_publicKey, _secretKey);
                _keyHolder.RecordAllTouches(_recordAllTouches);
                _keyHolder.SetLoggerProperties(_saveInfile, _showInConsole);
                EditorUtility.SetDirty(_keyHolder);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Reset First Session"))
            {
                PlayerPrefs.SetString("ProbaSessionCount", "");
                PlayerPrefs.SetString("ProbaFirstSessionTime", "");
            }



            if (!_connected)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Connected: False");
                EditorGUILayout.EndHorizontal();
            }
            if (_connected)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Connected: True");
                EditorGUILayout.EndHorizontal();
            }
            _connected = await IsConnectedToInternet();
        }

        public static async Task<bool> IsConnectedToInternet()
        {

            using (var ping = new Ping())
            {
                try
                {
                    return (await ping.SendPingAsync(ConfigurationModel.ServerIPAddress, 2000)).Status == IPStatus.Success;
                }
                catch { }
            }

            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void CreateSDK()
        {
            _keyHolder = GetKeyHolder();
            if (!_keyHolder || !_keyHolder.HasKeys())
            {
                Debug.LogWarning("Proba SDK: Check Keys");
                return;
            }
            var sdkKey = new GameObject("Proba SDK");
            sdkKey.AddComponent<SDK>();
            sdkKey.GetComponent<SDK>().PublicKey = _keyHolder.GetKeys(KeyType.PublicKey);
            sdkKey.GetComponent<SDK>().SecretKey = _keyHolder.GetKeys(KeyType.SecretKey);
            sdkKey.GetComponent<SDK>().RecordAllTouches = _keyHolder.IsRecordAllTouches();
            sdkKey.GetComponent<SDK>().SaveInFile = _keyHolder.SaveInfile();
            sdkKey.GetComponent<SDK>().ShowInConsole = _keyHolder.ShowInConsole();
            sdkKey.AddComponent<DatabaseConnection>();
            sdkKey.AddComponent<ProbaLogger>();
            DontDestroyOnLoad(sdkKey);
        }

        private static KeyHolder GetKeyHolder()
        {
            return AssetDatabase.LoadAssetAtPath<KeyHolder>(ConfigurationModel.ConfigPath);
        }
    }
}