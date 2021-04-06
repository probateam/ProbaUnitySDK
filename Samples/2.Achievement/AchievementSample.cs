using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proba;
using Proba.Scripts.SharedClasses;

public class AchievementSample : MonoBehaviour
{
    #region Variables

    public Text achievementText;
    private List<string> achievementIDs = new List<string>();

    #endregion

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

    #region Show All Achievements

    public void GetAllAchievement()
    {
        PROBA.GetAchievementsList();
    }

    private void AllAchievementReceived(IList<AchievementViewModel> achievements)
    {
        if (achievements.Count == 0)
        {
            achievementText.text = "There Is No Achievement";
            return;
        }

        var achievementsName = "";
        foreach (var achievement in achievements)
        {
            achievementsName += achievement.AchievementEnName + "\n";
            achievementIDs.Add(achievement.ID);
        }

        achievementText.text = achievementsName;
    }

    private void AllAchievementsError(RequestResponse response)
    {
        achievementText.text = response.ToString();
    }

    #endregion

    #region Show Player Achievements

    public void GetUserAchievements()
    {
        PROBA.GetUserAchievementsList();
    }

    private void UserAchievementsReceived(IList<UserAchievementViewModel> userAchievements)
    {
        if (userAchievements.Count == 0)
        {
            achievementText.text = "There Is No User Achievement";
            return;
        }

        var UserAchievementsName = "";
        foreach (var userAchievement in userAchievements)
        {
            UserAchievementsName += userAchievement.AchievementEnName + "\n";
        }

        achievementText.text = UserAchievementsName;
    }

    private void UserAchievementError(RequestResponse response)
    {
        achievementText.text = response.ToString();
    }
    #endregion

    #region Add Achievement To Player

    public void AddAchievementToUser()
    {
        if (achievementIDs.Count == 0)
        {
            achievementText.text = "Achievement ID List is Empty.\n Add Achievement in Dashboard or Click on 'Show all Achievements' to Fill List  ";
            return;
        }
        PROBA.UserNewAchievement(achievementIDs[0]);
    }

    #endregion


    void OnEnable()
    {
        PROBA.OnProgressionStatusReceived += ProgressionStatusReceived;

        PROBA.OnAchievementsListReceived += AllAchievementReceived;
        PROBA.OnAchievementsListCanceled += AllAchievementsError;

        PROBA.OnUserAchievementsListReceived += UserAchievementsReceived;
        PROBA.OnUserAchievementsListCanceled += UserAchievementError;
    }


    void OnDisable()
    {
        PROBA.OnProgressionStatusReceived -= ProgressionStatusReceived;

        PROBA.OnAchievementsListReceived -= AllAchievementReceived;
        PROBA.OnAchievementsListCanceled -= AllAchievementsError;

        PROBA.OnUserAchievementsListReceived -= UserAchievementsReceived;
        PROBA.OnUserAchievementsListCanceled -= UserAchievementError;
    }
}
