using System.Collections.Generic;
using Proba;
using ProbaDotnetSDK.SharedEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSample : MonoBehaviour
{
    #region Register

    void Start()
    {
        if (PROBA.UserHasRegistered())
        {
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

    #region Events

    public void AchievementEvent()
    {
        PROBA.AchievementEvent(AchievementTypes.TrophyGained, "Event Trophy");
    }

    public void AdvertisementEvent()
    {
        PROBA.AdvertisementEvent("RandomId" + Random.Range(0, 100), false, AdActions.RewardReceived, amount: 20);
    }

    public void BusinessEvent()
    {
        PROBA.BusinessEvent(BusinessTypes.RechargeWallet, value: 100);
    }

    public void ContentViewEvent()
    {
        PROBA.ContentViewEvent(SceneManager.GetActiveScene().name);
    }

    public void DesignEvent()
    {
        var designersDictionary = new Dictionary<string, string> {{"Designers", "Are The Best"}};
        PROBA.DesignEvent(designersDictionary);
    }

    public void ProgressionEvent()
    {
        PROBA.ProgressionEvent(ProgressionTypes.Start,"Event Sample");
    }

    public void SocialEvent()
    {
        PROBA.SocialEvent("Instagram" , SocialEvenTypes.Follow);
    }

    //Tap Event Used with listener

    #endregion
}
