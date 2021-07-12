using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proba;

public class StartUpSample : MonoBehaviour
{
    #region Variables

    public InputField startUpInputField1, startUpInputField2;
    public Text startUpText;

    #endregion

    #region Register

    void Start()
    {
        if (PROBA.UserHasRegistered())
        {
            Debug.Log("Welcome Back");
            PROBA.Initialize();
        }
        else
        {
            PROBA.CheckProgressionStatus();
        }
    }

    private void ProgressionStatusReceived(bool ProgressionStatus)
    {
        if (ProgressionStatus)
        {
            PROBA.Register("Username", false);
            //or
            //PROBA.Register("Username", true);
        }
        else
        {
            PROBA.Register("Username", true);
        }
    }

    #endregion

    #region Change Username

    public void ChangeUsername()
    {
        if (String.IsNullOrEmpty(startUpInputField1.text))
        {
            startUpText.text = "Enter a name in Input 1";
            return;
        }
        PROBA.UpdateUsername(startUpInputField1.text);
    }

    #endregion

    #region Save Progress

    public void SaveProgress()
    {
        var error = false;
        var errorText = "";
        if (String.IsNullOrEmpty(startUpInputField1.text))
        {
            errorText += "Enter Progress in Input 1";
            error = true;
        }

        if (String.IsNullOrEmpty(startUpInputField2.text))
        {
            errorText += "Enter Configuration in Input 2";
            error = true;
        }
        startUpText.text = errorText;

        if (!error)
        {
            PROBA.SaveUserProgress(startUpInputField1.text, startUpInputField2.text);
        }
    }

    #endregion

    #region Get User Progress

    public void GetUserProgress()
    {
        PROBA.GetUserProgress();
    }

    private void UserProgressReceived(string progress, string configuration)
    {
        if (string.IsNullOrEmpty(progress) || string.IsNullOrEmpty(configuration))
        {
            startUpText.text = "There Is No Progress";
            return;
        }

        var progressText = "Progress: " + progress + "\n" + "Config: " + configuration;
        startUpText.text = progressText;
    }

    private void UserProgressError(RequestResponse response)
    {
        startUpText.text = response.ToString();
    }

    #endregion

    #region Get Remote Configurations

    public void GetRemoteConfig()
    {
        PROBA.GetRemoteConfiguration();
    }

    private void RemoteConfigRecieved(IList<RemoteConfigurationsViewModel> remoteConfigurations)
    {
        var remoteConfigsKeys = "";
        foreach (var remoteConfiguration in remoteConfigurations)
        {
            remoteConfigsKeys += remoteConfiguration.ConfigKey + "\n";
        }

        startUpText.text = remoteConfigsKeys;
    }

    private void RemoteConfigError(RequestResponse response)
    {
        startUpText.text = response.ToString();
    }

    #endregion

    #region A/B Test

    private void ABTestKeyRecieved(string ABKey)
    {
        startUpText.text = ABKey;
    }

    #endregion


    void OnEnable()
    {
        PROBA.OnProgressionStatusReceived += ProgressionStatusReceived;

        PROBA.OnUserProgressReceive += UserProgressReceived;
        PROBA.OnUserProgressCanceled += UserProgressError;

        PROBA.OnRemoteConfigurationReceived += RemoteConfigRecieved;
        PROBA.OnRemoteConfigurationCanceled += RemoteConfigError;

        PROBA.OnABTestReceived += ABTestKeyRecieved;
    }

    void OnDisable()
    {
        PROBA.OnProgressionStatusReceived -= ProgressionStatusReceived;

        PROBA.OnUserProgressReceive -= UserProgressReceived;
        PROBA.OnUserProgressCanceled -= UserProgressError;

        PROBA.OnRemoteConfigurationReceived -= RemoteConfigRecieved;
        PROBA.OnRemoteConfigurationCanceled -= RemoteConfigError;

        PROBA.OnABTestReceived -= ABTestKeyRecieved;
    }
}
