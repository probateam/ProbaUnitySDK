using System;
using System.Collections.Generic;
using UnityEngine;

namespace Proba
{
    /// <summary>Main class to access PROBA Features</summary>
    public class PROBA
    {

        #region Events

        /// <summary>Call PROBA Achievement event</summary>
        /// <param name="achievementTypes">type of achievement</param>
        /// <param name="gameLevelName1">name of level</param>
        /// <param name="gameLevelName2">additional name of level or section (Optional)</param>
        /// <param name="gameLevelName3">additional name of level or section (Optional)</param>
        /// <param name="gameLevelName4">additional name of level or section (Optional)</param>
        /// <param name="relatedProgressionEventIds">list of related progression event's IDs (Optional)</param>
        /// <param name="newPlayerLevel">user's new gained level (Optional)</param>
        /// <param name="prevRank">user's previous Rank (Optional)</param>
        /// <param name="newRank">user's new Rank (Optional)</param>
        /// <param name="leaderBoardName">leader board's name if achievements belongs to leader board (Optional)</param>
        /// <param name="arenaMode">is achievements gained in arena mode (Optional)</param>
        /// <param name="arenaName">arena's name (Optional)</param>
        public static void AchievementEvent(AchievementTypes achievementTypes, string gameLevelName1, string gameLevelName2 = "",
                string gameLevelName3 = "", string gameLevelName4 = "", List<string> relatedProgressionEventIds = null,
                long newPlayerLevel = 0, long prevRank = 0, long newRank = 0, string leaderBoardName = "", bool arenaMode = false,
                string arenaName = "")
        {
            _ = new AchievementEventViewModel(achievementTypes, gameLevelName1, gameLevelName2, gameLevelName3, gameLevelName4,
                relatedProgressionEventIds, newPlayerLevel, prevRank, newRank, leaderBoardName, arenaMode, arenaName);
        }

        /// <summary>Call PROBA Advertisement event</summary>
        public static void AdvertisementEvent(string addId, bool skipped, AdActions action, bool firstTime = false,
            AdFailShowReasons failShowReason = AdFailShowReasons.Unknown, int duration = 0, string sdkName = "", AdTypes type = AdTypes.RewardedVideo, string placement = "", double amount = 0.0)
        {
            _ = new AdvertisementEventViewModel(addId, skipped, action,firstTime,failShowReason,duration,sdkName,type,placement,amount);
        }


        /// <summary>Call PROBA Business event</summary>

        public static void BusinessEvent(BusinessTypes businessType, double value = 0.0, string currency = "IRR", string itemName = "Item", int transactionCount = 1,
            string cartName = "Main Cart", string extraDetails = "No Extra Details", PaymentTypes paymentType = PaymentTypes.Unkown, bool specialEvent = false,
            string specialEventName = "", double amount = 0.0, bool virtualCurrency = false)
        {
            _ = new BusinessEventViewModel(businessType, value, currency, itemName, transactionCount, cartName, extraDetails,
                paymentType, specialEvent, specialEventName, amount, virtualCurrency);
        }

        /// <summary>Call PROBA ContentView event</summary>
        /// <param name="contentName">name of content user has seen</param>
        public static void ContentViewEvent(string contentName)
        {
            _ = new ContentViewEventViewModel(contentName);
        }

        /// <summary>Call PROBA Design event</summary>
        /// <param name="customDesigns">custom event</param>
        public static void DesignEvent(Dictionary<string, string> customDesigns)
        {
            _ = new DesignEventViewModel(customDesigns);
        }


        /// <summary>Call PROBA Progression event</summary>
        /// <param name="progressionType">type of progression</param>
        /// <param name="gameLevelName1">name of level</param>
        /// <param name="eventId">progression event ID for linking to achievements (Optional)</param>
        /// <param name="attempts">number of user's attempts (Optional)</param>
        /// <param name="score">user's Score (Optional)</param>
        /// <param name="gameLevelName2">additional name of level or section (Optional)</param>
        /// <param name="gameLevelName3">additional name of level or section (Optional)</param>
        /// <param name="gameLevelName4">additional name of level or section (Optional)</param>
        /// <param name="arenaMode">is progression in arena mode? (Optional)</param>
        /// <param name="arenaName">arena name (Optional)</param>
        public static void ProgressionEvent(ProgressionTypes progressionType, string gameLevelName1, string eventId = "", int attempts = 0, double score = 0.0,
                string gameLevelName2 = "level", string gameLevelName3 = "level", string gameLevelName4 = "level", bool arenaMode = false, string arenaName = "arena")
        {
            _ = new ProgressionEventViewModel(progressionType, attempts, score, gameLevelName1, gameLevelName2, gameLevelName3,
                gameLevelName4, eventId, arenaMode, arenaName);
        }

        /// <summary>Call PROBA Social event</summary>
        public static void SocialEvent(string socialMediaName, SocialEvenTypes socialEvenType, int value = 0)
        {
            _ = new SocialEventViewModel(socialMediaName, socialEvenType, value);
        }

        /// <summary>Call PROBA Tap event</summary>
        /// <param name="tapType">type of touch</param>
        /// <param name="btnName">name of button (Optional)</param>
        /// <param name="startX">start X of sweep (Optional)</param>
        /// <param name="startY">start Y of sweep (Optional)</param>
        /// <param name="endX">end X of sweep (Optional)</param>
        /// <param name="endY">end Y of sweep (Optional)</param>
        public static void TapEvent(TapTypes tapType, string btnName = "", double startX = -1f, double startY = -1f, double endX = -1f, double endY = -1f)
        {
            _ = new TapEventViewModel(tapType, btnName, startX, startY, endX, endY);
        }

        #endregion

        #region Account

        /// <summary>Determines whether user has registered.</summary>
        /// <returns>
        ///   <c>true</c> if user registered; otherwise, <c>false</c>.</returns>
        public static bool UserHasRegistered()
        {
            return !string.IsNullOrEmpty(PlayerPrefs.GetString("ProbaUserID"));
        }

        /// <summary>Initializes Proba SDK</summary>
        public static void Initialize()
        {
            _ = new StartSessionViewModel(true);
        }

        /// <summary>Check For Saved Progression On Proba</summary>
        public static void CheckProgressionStatus()
        {
            Broker.CallCheckProgressionStatus();
        }

        /// <summary>Occurs when Progression Status received</summary>
        public static event Action<bool> OnProgressionStatusReceived;

        internal static void ProgressionStatusReceived(bool ProgressionStatus)
        {
            OnProgressionStatusReceived?.Invoke(ProgressionStatus);
        }

        /// <summary>Registers user and Initializes Proba SDK</summary>
        /// <param name="username">The username.</param>
        /// <param name="newUser">is user new or not</param>
        public static void Register(string username, bool newUser)
        {
            Broker.CallRegister(username, newUser);
        }

        /// <summary>Occurs when A/B Test's Key after first register received</summary>
        public static event Action<string> OnABTestReceived;

        internal static void ABTestReceive(string abTest)
        {
            OnABTestReceived?.Invoke(abTest);
        }

        /// <summary>Updates the username</summary>
        /// <param name="username">new username</param>
        public static void UpdateUsername(string username)
        {
            Broker.CallUpdateUsername(username);
        }

        /// <summary>Saves the user's Progress</summary>
        /// <param name="progress">The Progress.</param>
        /// <param name="configuration">The configuration.</param>
        public static void SaveUserProgress(string progress, string configuration)
        {
            Broker.CallSaveUserProgress(progress, configuration);
        }


        /// <summary>gets Remote Configuration from Proba panel</summary>
        public static void GetRemoteConfiguration()
        {
            Broker.CallGetRemoteConfiguration();
        }

        /// <summary>Occurs when remote configuration received</summary>
        public static event Action<IList<RemoteConfigurationsViewModel>> OnRemoteConfigurationReceived;
        internal static void RemoteConfigurationReceived(IList<RemoteConfigurationsViewModel> remoteConfigurations)
        {
            OnRemoteConfigurationReceived?.Invoke(remoteConfigurations);
        }

        /// <summary>Occurs when remote configuration couldn't receive</summary>
        public static event Action<RequestResponse> OnRemoteConfigurationCanceled;
        internal static void RemoteConfigurationCanceled(RequestResponse response)
        {
            OnRemoteConfigurationCanceled?.Invoke(response);
        }

        /// <summary>gets user Progress from Proba panel</summary>
        public static void GetUserProgress()
        {
            Broker.CallGetUserProgress();
        }

        /// <summary>Occurs when user Progress received</summary>
        public static event Action<string, string> OnUserProgressReceive;
        internal static void UserDataReceived(string progress, string configurations)
        {
            OnUserProgressReceive?.Invoke(progress, configurations);
        }

        /// <summary>Occurs when user Progress couldn't receive</summary>
        public static event Action<RequestResponse> OnUserProgressCanceled;

        internal static void UserProgressCanceled(RequestResponse response)
        {
            OnUserProgressCanceled?.Invoke(response);
        }

        #endregion

        #region Trophy

        /// <summary>gets all achievements list from Proba panel</summary>
        public static void GetAchievementsList()
        {
            Broker.CallGetAchievementsList();
        }

        /// <summary>Occurs when all achievements list received</summary>
        public static event Action<IList<AchievementViewModel>> OnAchievementsListReceived;

        internal static void AchievementsListReceived(IList<AchievementViewModel> achievementViewList)
        {
            OnAchievementsListReceived?.Invoke(achievementViewList);
        }

        /// <summary>Occurs when all achievements list couldn't receive</summary>
        public static event Action<RequestResponse> OnAchievementsListCanceled;

        internal static void AchievementsListCanceled(RequestResponse response)
        {
            OnAchievementsListCanceled?.Invoke(response);
        }

        /// <summary>gets all leader boards list from Proba panel</summary>
        public static void GetLeaderBoardsList()
        {
            Broker.CallGetLeaderBoardsList();
        }

        /// <summary>Occurs when all leader boards list received</summary>
        public static event Action<IList<LeaderBoardViewModel>> OnLeaderBoardsListReceived;

        internal static void LeaderBoardsListReceived(IList<LeaderBoardViewModel> leaderBoardViewList)
        {
            OnLeaderBoardsListReceived?.Invoke(leaderBoardViewList);
        }

        /// <summary>Occurs when all leader boards list couldn't receive</summary>
        public static event Action<RequestResponse> OnLeaderBoardsListCanceled;

        internal static void LeaderBoardsListCanceled(RequestResponse response)
        {
            OnLeaderBoardsListCanceled?.Invoke(response);
        }

        /// <summary>Gets the user's achievements list</summary>
        public static void GetUserAchievementsList()
        {
            Broker.CallGetUserAchievementsList();
        }

        /// <summary>Occurs when user's achievements list received</summary>
        public static event Action<IList<UserAchievementViewModel>> OnUserAchievementsListReceived;

        internal static void UserAchievementsListReceived(IList<UserAchievementViewModel> userAchievementList)
        {
            OnUserAchievementsListReceived?.Invoke(userAchievementList);
        }

        /// <summary>Occurs when user's achievements list couldn't receive</summary>
        public static event Action<RequestResponse> OnUserAchievementsListCanceled;

        internal static void UserAchievementsListCanceled(RequestResponse response)
        {
            OnUserAchievementsListCanceled?.Invoke(response);
        }

        /// <summary>Gets leader board's user list</summary>
        /// <param name="self">show current user's place or top users <c>true</c></param>
        /// <param name="leaderBoardId">The leader board identifier.</param>
        public static void GetLeaderBoardUsersList(bool self, string leaderBoardId)
        {
            Broker.CallGetLeaderBoardUsersList(self, leaderBoardId);
        }

        /// <summary>Occurs when leader board's user list received</summary>
        public static event Action<IList<LeaderBoardUserViewModel>> OnLeaderBoardUserListReceived;

        internal static void LeaderBoardUserListReceived(IList<LeaderBoardUserViewModel> leaderBoardUserViewModels)
        {
            OnLeaderBoardUserListReceived?.Invoke(leaderBoardUserViewModels);
        }

        /// <summary>Occurs when leader board's user list couldn't receive</summary>
        public static event Action<RequestResponse> OnLeaderBoardUserListCanceled;

        internal static void LeaderBoardUserListCanceled(RequestResponse response)
        {
            OnLeaderBoardUserListCanceled?.Invoke(response);
        }

        /// <summary>put user's new Score in leader board</summary>
        /// <param name="leaderBoardId">The leader board identifier.</param>
        /// <param name="score">The Score.</param>
        /// <param name="userName">Name of the user.</param>
        public static void UserNewLeaderBoardScore(string leaderBoardId, long score, string userName = "")
        {
            Broker.CallNewLeaderBoardScore(leaderBoardId, score, userName);
        }

        /// <summary>user's new achievement.</summary>
        /// <param name="achievementId">The achievement identifier.</param>
        /// <param name="score">The Score.</param>
        /// <param name="achievementStep">The achievement step.</param>
        public static void UserNewAchievement(string achievementId)
        {
            Broker.CallUserNewAchievement(achievementId);
        }

        #endregion
    }
}
