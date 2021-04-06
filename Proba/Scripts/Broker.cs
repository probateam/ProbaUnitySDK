using System;
using System.Security;
using Proba.Scripts.SharedClasses;
using UnityEngine;

namespace Proba.Scripts
{
    internal class Broker
    {
        internal static event Action<AchievementEventViewModel> AchievementEvent;
        internal static void AchievementEventCreated(AchievementEventViewModel achievementEventViewModel)
        {
            AchievementEvent?.Invoke(achievementEventViewModel);
        }

        internal static event Action<AdvertisementEventViewModel> AdvertisementEvent;
        internal static void AdvertisementEventCreated(AdvertisementEventViewModel advertisementEventViewModel)
        {
            AdvertisementEvent?.Invoke(advertisementEventViewModel);
        }

        internal static event Action<BusinessEventViewModel> BusinessEvent;

        internal static void BusinessEventCreated(BusinessEventViewModel businessEventViewModel)
        {
            BusinessEvent?.Invoke(businessEventViewModel);
        }

        internal static event Action<ContentViewEventViewModel> ContentViewEvent;
        internal static void ContentViewEventCreated(ContentViewEventViewModel contentViewEventViewModel)
        {
            ContentViewEvent?.Invoke(contentViewEventViewModel);
        }

        internal static Action<DesignEventViewModel> DesignEvent;
        internal static void DesignEventCreated(DesignEventViewModel designEventViewModel)
        {
            DesignEvent?.Invoke(designEventViewModel);
        }

        internal static Action<EndSessionViewModel> EndSession;
        internal static void EndSessionCreated(EndSessionViewModel endSessionViewModel)
        {
            EndSession?.Invoke(endSessionViewModel);
        }

        internal static Action<ProgressionEventViewModel> ProgressionEvent;
        internal static void ProgressionEventCreated(ProgressionEventViewModel progressionEventViewModel)
        {
            ProgressionEvent?.Invoke(progressionEventViewModel);
        }

        internal static Action<SocialEventViewModel> SocialEvent;
        internal static void SocialEventCreated(SocialEventViewModel socialEventViewModel)
        {
            SocialEvent?.Invoke(socialEventViewModel);
        }

        internal static Action<StartSessionViewModel> StartSession;
        internal static void StartSessionCreated(StartSessionViewModel startSessionViewModel)
        {
            StartSession?.Invoke(startSessionViewModel);
        }

        internal static Action<TapEventViewModel> TapEventView;
        internal static void TapEventViewCreated(TapEventViewModel tapEventViewModel)
        {
            TapEventView?.Invoke(tapEventViewModel);
        }

        internal static Action CheckProgressionStatus;

        internal static void CallCheckProgressionStatus()
        {
            CheckProgressionStatus?.Invoke();
        }

        internal static Action<string, bool> Register;
        internal static void CallRegister(string username, bool newUser)
        {
            Register?.Invoke(username, newUser);
        }

        internal static Action Error;
        internal static void ThereIsaError()
        {
            Error?.Invoke();
        }

        internal static Action<string> UpdateUserName;
        internal static void CallUpdateUsername(string username)
        {
            UpdateUserName?.Invoke(username);
        }

        internal static Action<string, string> SaveUserProgress;
        internal static void CallSaveUserProgress(string progress, string configuration)
        {
            SaveUserProgress?.Invoke(progress, configuration);
        }

        internal static Action GetRemoteConfiguration;

        internal static void CallGetRemoteConfiguration()
        {
            GetRemoteConfiguration?.Invoke();
        }

        internal static Action GetUserProgress;

        internal static void CallGetUserProgress()
        {
            GetUserProgress?.Invoke();
        }

        internal static Action GetAchievementsList;

        internal static void CallGetAchievementsList()
        {
            GetAchievementsList?.Invoke();
        }

        internal static Action GetLeaderBoardsList;
        internal static void CallGetLeaderBoardsList()
        {
            GetLeaderBoardsList?.Invoke();
        }

        internal static Action GetUserAchievementsList;
        internal static void CallGetUserAchievementsList()
        {
            GetUserAchievementsList?.Invoke();
        }


        internal static Action<bool, string> GetLeaderBoardUsersList;
        internal static void CallGetLeaderBoardUsersList(bool self, string leaderBoardId)
        {
            GetLeaderBoardUsersList?.Invoke(self, leaderBoardId);
        }

        internal static Action<string, long, string> UserNewLeaderBoardScore;

        internal static void CallNewLeaderBoardScore(string leaderBoardId, long score, string userName)
        {
            UserNewLeaderBoardScore?.Invoke(leaderBoardId, score, userName);
        }


        internal static event Action<string> UserNewAchievement;

        internal static void CallUserNewAchievement(string achievementId)
        {
            UserNewAchievement?.Invoke(achievementId);
        }
    }
}
