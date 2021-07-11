using System;
using System.Collections;
using System.Collections.Generic;
using Proba;
using UnityEngine;

public class SDKCreator : MonoBehaviour
{
    private static KeyHolder _keyHolder = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateSDK()
    {
        _keyHolder = GetKeyHolder();

        if (_keyHolder == null)
        {
            Debug.LogWarning("PROBA SDK: Enter Keys");
            return;
        }

        if (!_keyHolder.StartProba())
        {
            Debug.LogWarning("Proba is Disabled. (Enable it from config window)");
            return;
        }

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
        try
        {
            return Resources.Load<KeyHolder>("ProbaConfig");
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
}
