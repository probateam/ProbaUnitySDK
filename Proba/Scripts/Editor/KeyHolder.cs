﻿using System;
using UnityEngine;

namespace Proba.Scripts.Editor
{
    /// <summary>Class to Save Config in Unity</summary>
    internal class KeyHolder : ScriptableObject
    {
        [SerializeField] [HideInInspector] private string _publicKey;
        [SerializeField] [HideInInspector] private string _secretKey;
        [SerializeField] [HideInInspector] private bool _recordAllTouches;
        [SerializeField] [HideInInspector] private bool _loggerShowInConsole;
        [SerializeField] [HideInInspector] private bool _loggerSaveInFile = true;

        public void SetKeys(string publicKey, string secretKey)
        {
            _publicKey = publicKey;
            _secretKey = secretKey;
        }

        public string GetKeys(KeyType type)
        {
            switch (type)
            {
                case KeyType.PublicKey:
                    return _publicKey;
                case KeyType.SecretKey:
                    return _secretKey;
                default:
                    return "ERROR";
            }
        }

        internal bool HasKeys()
        {
            return !string.IsNullOrEmpty(_publicKey) && !string.IsNullOrEmpty(_secretKey);
        }

        public void RecordAllTouches(bool recordAllTouches)
        {
            _recordAllTouches = recordAllTouches;
        }

        public bool IsRecordAllTouches()
        {
            return _recordAllTouches;
        }

        public void SetLoggerProperties(bool saveInFile, bool showInConsole)
        {
            _loggerSaveInFile = saveInFile;
            _loggerShowInConsole = showInConsole;
        }

        public bool ShowInConsole()
        {
            return _loggerShowInConsole;
        }

        public bool SaveInfile()
        {
            return _loggerSaveInFile;
        }
    }

    public enum KeyType
    {
        PublicKey,
        SecretKey
    }
}
