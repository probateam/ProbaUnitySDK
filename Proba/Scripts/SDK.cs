using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Proba.Scripts.Client;
using Proba.Scripts.SharedClasses;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Proba.Scripts
{
    internal class SDK : MonoBehaviour
    {
        #region variables
        internal string SecretKey { set; get; }
        internal string PublicKey { set; get; }
        internal bool RecordAllTouches { set; get; }
        internal bool SaveInFile { set; get; }
        internal bool ShowInConsole { set; get; }

        private static BasicData _basicData;
        private ProbaHttpClient _probaHttpClient;
        private ProbaLogger _probaLogger;
        private BatchEventViewModel _batchEventViewModel;
        private readonly Dictionary<int, TouchPhase> _touchePhases = new Dictionary<int, TouchPhase>();
        private readonly Dictionary<int, Tuple<float, float>> _starTuples = new Dictionary<int, Tuple<float, float>>();
        private bool _sending;
        private int _totalEventCount;
        private int _currentIndex;

        private const int MAX_EVENT_COUNT = 20;
        private const float MAX_TIME = 30;

        #endregion


        /// <summary>initialize database,httpClient,request queue and session</summary>
        private void Start()
        {
            _basicData = new BasicData();
            _probaLogger = GetComponent<ProbaLogger>();
            _probaLogger.SetProperties(SaveInFile, ShowInConsole);
            _probaHttpClient = new ProbaHttpClient(_probaLogger, SecretKey, PublicKey);
            SendFirstRecordFromDB();
        }

        private void OnEnable()
        {
            DatabaseConnection.InitialConnectionString();
            Broker.StartSession += START_SESSION_EVENT;
            Broker.AchievementEvent += EVENT;
            Broker.AdvertisementEvent += EVENT;
            Broker.BusinessEvent += EVENT;
            Broker.ContentViewEvent += EVENT;
            Broker.DesignEvent += EVENT;
            Broker.ProgressionEvent += EVENT;
            Broker.SocialEvent += EVENT;
            Broker.TapEventView += EVENT;
            Broker.EndSession += END_SESSION_EVENT;
            //account
            Broker.Register += RegisterAsync;
            Broker.CheckProgressionStatus += CheckProgressionStatusAsync;
            Broker.UpdateUserName += UpdateUsernameAsync;
            Broker.SaveUserProgress += SaveUserConfigAsync;
            Broker.GetRemoteConfiguration += GetRemoteConfigurationAsync;
            Broker.GetUserProgress += GetUserProgressAsync;
            //trophy
            Broker.GetAchievementsList += GetAchievementsListAsync;
            Broker.GetLeaderBoardsList += GetLeaderBoardsListAsync;
            Broker.GetUserAchievementsList += GetUserAchievementsListAsync;
            Broker.GetLeaderBoardUsersList += GetLeaderBoardUsersListAsync;
            Broker.UserNewAchievement += UserNewAchievementAsync;
            Broker.UserNewLeaderBoardScore += UserNewLeaderBoardScoreAsync;

            Broker.Error += HAS_ERROR;
        }

        private void OnApplicationQuit()
        {
            new EndSessionViewModel(_basicData.UserId, _basicData.CurrentSessionId);

            Broker.StartSession -= START_SESSION_EVENT;
            Broker.AchievementEvent -= EVENT;
            Broker.AdvertisementEvent -= EVENT;
            Broker.BusinessEvent -= EVENT;
            Broker.ContentViewEvent -= EVENT;
            Broker.DesignEvent -= EVENT;
            Broker.ProgressionEvent -= EVENT;
            Broker.SocialEvent -= EVENT;
            Broker.TapEventView -= EVENT;
            Broker.EndSession -= END_SESSION_EVENT;
            //account
            Broker.Register -= RegisterAsync;
            Broker.CheckProgressionStatus -= CheckProgressionStatusAsync;
            Broker.UpdateUserName -= UpdateUsernameAsync;
            Broker.SaveUserProgress -= SaveUserConfigAsync;
            Broker.GetRemoteConfiguration -= GetRemoteConfigurationAsync;
            Broker.GetUserProgress -= GetUserProgressAsync;
            //trophy
            Broker.GetAchievementsList -= GetAchievementsListAsync;
            Broker.GetLeaderBoardsList -= GetLeaderBoardsListAsync;
            Broker.GetUserAchievementsList -= GetUserAchievementsListAsync;
            Broker.GetLeaderBoardUsersList -= GetLeaderBoardUsersListAsync;
            Broker.UserNewAchievement -= UserNewAchievementAsync;
            Broker.UserNewLeaderBoardScore -= UserNewLeaderBoardScoreAsync;

            Broker.Error -= HAS_ERROR;
        }

        private void HAS_ERROR()
        {
            _basicData.HasError = true;
        }

        private void START_SESSION_EVENT(StartSessionViewModel startSessionViewModel)
        {

            if (PlayerPrefs.GetInt("ProbaHasOpenSession") == 1)
            {
                EmptyEndSession();
            }
            PlayerPrefs.SetInt("ProbaHasOpenSession", 1);
            StartSessionAsync(startSessionViewModel);
        }

        private void EVENT(BaseEventDataViewModel baseEventDataViewModel)
        {
            AddEventToQueue(baseEventDataViewModel);
        }

        private void END_SESSION_EVENT(EndSessionViewModel endSessionViewModel)
        {
            PlayerPrefs.SetInt("ProbaHasOpenSession", 0);
            EndSessionAsync(endSessionViewModel);
        }


        private void SendFirstRecordFromDB()
        {
            try
            {
                var firstData = DatabaseConnection.GetFirst().Rows[0];
                _currentIndex = int.Parse(firstData["ID"].ToString());
                SendOldRecord(firstData["CLASS"].ToString(), firstData["BODY"].ToString());
            }
            catch (IndexOutOfRangeException)
            {
                Invoke("SendFirstRecordFromDB", 10);
            }
        }



        private void SendOldRecord(string className, string content)
        {
            _sending = true;
            switch (className)
            {
                case "REGISTER":
                    ExistingRegister(JsonConvert.DeserializeObject<BaseEventDataViewModel>(content.ToString()));
                    break;
                case "START_SESSION":
                    ExistingStartSession(JsonConvert.DeserializeObject<StartSessionViewModel>(content.ToString()));
                    break;
                case "END_SESSION":
                    ExistingEndSession(JsonConvert.DeserializeObject<EndSessionViewModel>(content.ToString()));
                    break;
                case "BATCH_EVENT":
                    ExistingBatchEvent(JsonConvert.DeserializeObject<BatchEventViewModel>(content.ToString()));
                    break;
                case "EVENT":
                    ExistingEvent(JsonConvert.DeserializeObject<BaseEventDataViewModel>(content.ToString()));
                    break;
                case "USER_PROGRESS":
                    ExistingSaveUserConfig(JsonConvert.DeserializeObject<ProgressViewModel>(content.ToString()));
                    break;
                case "NEW_ACHIEVEMENT":
                    ExistingNewAchievement(JsonConvert.DeserializeObject<TrophyRequest>(content.ToString()));
                    break;
                case "NEW_LEADER_BOARD":
                    ExistingNewLeaderBoard(JsonConvert.DeserializeObject<TrophyRequest>(content.ToString()));
                    break;
                default:
                    SendFirstRecordFromDB();
                    break;
            }
        }

        private void OldRecordSent()
        {
            _sending = false;
            DatabaseConnection.DeleteEvent(_currentIndex);
            Invoke("SendFirstRecordFromDB", 10);
        }

        private void OldRecordCouldNotSent()
        {
            _sending = false;
            Invoke("SendFirstRecordFromDB", 60);
        }


        #region Account

        private async void RegisterAsync(string username, bool newUser)
        {
            var baseEvent = new BaseEventDataViewModel { UserName = username, NewUser = newUser };

            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveRegisterInDB(baseEvent);
                return;
            }
            try
            {
                var (success, statusCode, registerResponse) = await _probaHttpClient.RegisterAsync(baseEvent);
                if (!success)
                {
                    SaveRegisterInDB(baseEvent);
                    return;
                }

                _basicData.UserId = registerResponse.UserId;
                _basicData.ABTest = registerResponse.AbTest;
                _basicData.CurrentUserName = username;
                _basicData.SessionCount = 0;
                _basicData.PurchesesCount = 0;
                _basicData.VirtualPurchesesCount = 0;
                _basicData.CreationTime = DateTime.UtcNow;
                _basicData.OverallPlayTime = 0;

                PlayerPrefs.SetString("ProbaUserID", _basicData.UserId);
                PlayerPrefs.SetString("ProbaUserName", _basicData.CurrentUserName);
                PlayerPrefs.SetString("ProbaABTest", _basicData.ABTest);

                PROBA.ABTestReceive(_basicData.ABTest);

                new StartSessionViewModel(true);

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveRegisterInDB(baseEvent);
            }
        }

        private async void ExistingRegister(BaseEventDataViewModel baseEvent)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode, registerResponse) = await _probaHttpClient.RegisterAsync(baseEvent);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                _basicData.UserId = registerResponse.UserId;
                _basicData.ABTest = registerResponse.AbTest;
                _basicData.CurrentUserName = baseEvent.UserName;
                _basicData.SessionCount = 0;
                _basicData.PurchesesCount = 0;
                _basicData.VirtualPurchesesCount = 0;
                _basicData.CreationTime = DateTime.UtcNow;
                _basicData.OverallPlayTime = 0;

                PlayerPrefs.SetString("ProbaUserID", _basicData.UserId);
                PlayerPrefs.SetString("ProbaUserName", _basicData.CurrentUserName);
                PlayerPrefs.SetString("ProbaABTest", _basicData.ABTest);

                PROBA.ABTestReceive(_basicData.ABTest);

                OldRecordSent();

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void CheckProgressionStatusAsync()
        {
            var baseEvent = new BaseEventDataViewModel();

            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveCheckProgressionStatusInDB(baseEvent);
                return;
            }
            try
            {
                var (success, statusCode, progressionStatus) = await _probaHttpClient.CheckProgressionStatusAsync(baseEvent);
                if (!success)
                {
                    SaveCheckProgressionStatusInDB(baseEvent);
                    return;
                }
                PROBA.ProgressionStatusReceived(progressionStatus);

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveCheckProgressionStatusInDB(baseEvent);
            }
        }

        private async void ExsitingCheckProgressionStatusAsync(BaseEventDataViewModel baseEvent)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode, progressionStatus) = await _probaHttpClient.CheckProgressionStatusAsync(baseEvent);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }
                PROBA.ProgressionStatusReceived(progressionStatus);
                OldRecordSent();

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void UpdateUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _probaLogger.LogWarning("UpdateUserName: Username is Empty ,Username Changed to Random Value");
                username = "User" + Random.Range(0, 1000);
            }
            var baseEvent = new BaseEventDataViewModel { UserName = username };
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveUpdateUserInDB(baseEvent);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.UpdateUserInfoAsync(baseEvent);
                if (!success)
                {
                    SaveUpdateUserInDB(baseEvent);
                    return;
                }

                _basicData.CurrentUserName = username;

                PlayerPrefs.SetString("ProbaUserName", _basicData.CurrentUserName);

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveUpdateUserInDB(baseEvent);
            }
        }

        private async void ExistingUpdateUsernameAsync(BaseEventDataViewModel baseEvent)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.UpdateUserInfoAsync(baseEvent);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                _basicData.CurrentUserName = baseEvent.UserName;
                PlayerPrefs.SetString("ProbaUserName", _basicData.CurrentUserName);

                OldRecordSent();
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void SaveUserConfigAsync(string progress, string configuration)
        {
            if (string.IsNullOrEmpty(progress))
            {
                _probaLogger.LogWarning("SaveUserProgress: Progress is Empty ,Progress Changed to default Value");
                progress = "Progress";
            }
            if (string.IsNullOrEmpty(configuration))
            {
                _probaLogger.LogWarning("SaveUserProgress: configuration is Empty ,configuration Changed to default Value");
                configuration = "configuration";
            }
            var progressViewModel = new ProgressViewModel()
            {
                Progress = progress,
                Configurations = configuration
            };
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveSaveUserProgressInDB(progressViewModel);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.SaveUserProgressAsync(progressViewModel);
                if (!success)
                {
                    SaveSaveUserProgressInDB(progressViewModel);
                }

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveSaveUserProgressInDB(progressViewModel);
            }
        }

        private async void ExistingSaveUserConfig(ProgressViewModel progressViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.SaveUserProgressAsync(progressViewModel);
                if (!success)
                {
                    OldRecordCouldNotSent();
                }
                else
                {
                    OldRecordSent();
                }

            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void GetRemoteConfigurationAsync()
        {
            var baseEventData = new BaseEventDataViewModel();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.RemoteConfigurationCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, configurations) = await _probaHttpClient.GetRemoteConfigurationsAsync(baseEventData);
                if (!success)
                {
                    PROBA.RemoteConfigurationCanceled(RequestResponse.Error);
                }

                PROBA.RemoteConfigurationReceived(configurations);

            }
            catch (Exception e)
            {
                PROBA.RemoteConfigurationCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void GetUserProgressAsync()
        {
            var baseEventData = new BaseEventDataViewModel();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.UserProgressCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, configurations) = await _probaHttpClient.GetUserDataAsync(baseEventData);
                if (!success)
                {
                    PROBA.UserProgressCanceled(RequestResponse.Error);
                }

                PROBA.UserDataReceived(configurations.Progress, configurations.Configurations);

            }
            catch (Exception e)
            {
                PROBA.UserProgressCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        #endregion

        #region Events
        private void AddEventToQueue(BaseEventDataViewModel baseEventDataViewModel)
        {
            if (_batchEventViewModel == null || _totalEventCount == 0)
            {
                _batchEventViewModel = new BatchEventViewModel();
                Invoke("SendByTime", MAX_TIME);

            }

            var className = GetClass(baseEventDataViewModel);
            switch (className)
            {
                case "AchievementEvent":
                    _batchEventViewModel.Achievements.Add((AchievementEventViewModel)baseEventDataViewModel);
                    break;
                case "AdvertisementEvent":
                    _batchEventViewModel.Advertisements.Add((AdvertisementEventViewModel)baseEventDataViewModel);
                    break;
                case "BusinessEvent":
                    _batchEventViewModel.Businesses.Add((BusinessEventViewModel)baseEventDataViewModel);
                    break;
                case "ContentViewEvent":
                    _batchEventViewModel.ContentViews.Add((ContentViewEventViewModel)baseEventDataViewModel);
                    break;
                case "DesignEvent":
                    _batchEventViewModel.DesignEvent.Add((DesignEventViewModel)baseEventDataViewModel);
                    break;
                case "ProgressionEvent":
                    _batchEventViewModel.Progressions.Add((ProgressionEventViewModel)baseEventDataViewModel);
                    break;
                case "SocialEvent":
                    _batchEventViewModel.Socials.Add((SocialEventViewModel)baseEventDataViewModel);
                    break;
                case "TapEvent":
                    _batchEventViewModel.Taps.Add((TapEventViewModel)baseEventDataViewModel);
                    break;
                default:
                    className = "Default";
                    break;
            }

            _totalEventCount++;

            if (_totalEventCount >= MAX_EVENT_COUNT)
            {
                if (_sending)
                {
                    return;
                }
                SendBatchEventAsync(_batchEventViewModel);
            }
        }

        private void SendByTime()
        {
            if (_sending)
            {
                Invoke("SendByTime", MAX_TIME);
                return;
            }
            SendBatchEventAsync(_batchEventViewModel);
        }

        private async void StartSessionAsync(StartSessionViewModel sessionViewModel)
        {
            if (string.IsNullOrEmpty(_basicData.UserId))
            {
                _basicData.UserId = PlayerPrefs.GetString("ProbaUserID");
                sessionViewModel.UserId = _basicData.UserId;
            }
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveStartSessionInDB(sessionViewModel);
                return;
            }
            try
            {
                var (success, statusCode, sessionResponse) = await _probaHttpClient.SendStartSessionAsync(sessionViewModel);
                if (!success)
                {
                    SaveStartSessionInDB(sessionViewModel);
                    return;
                }
                if (statusCode != HttpStatusCode.OK)
                {
                    SaveStartSessionInDB(sessionViewModel);
                    _probaLogger.LogWarning($"couldn't send start session with {statusCode} status");
                }

                _basicData.CurrentSessionId = sessionResponse.SessionId;
                _basicData.CurrentSessionLocation = sessionResponse.Location;
                _basicData.CurrentSessionStartTime = DateTime.UtcNow;
                _basicData.HasActiveSession = true;
                _basicData.SessionCount = sessionViewModel.SessionCount;
                _basicData.FirstSessionStartTime = new DateTime(sessionViewModel.FirstSessionTime);

                PlayerPrefs.SetString("ProbaSessionID", _basicData.CurrentSessionId);
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveStartSessionInDB(sessionViewModel);
            }
        }

        private async void ExistingStartSession(StartSessionViewModel sessionViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode, sessionResponse) = await _probaHttpClient.SendStartSessionAsync(sessionViewModel);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }
                if (statusCode != HttpStatusCode.OK)
                {
                    OldRecordCouldNotSent();
                    _probaLogger.LogWarning($"couldn't send start session with {statusCode} status");
                    return;
                }

                _basicData.CurrentSessionId = sessionResponse.SessionId;
                _basicData.CurrentSessionLocation = sessionResponse.Location;
                _basicData.CurrentSessionStartTime = DateTime.UtcNow;
                _basicData.HasActiveSession = true;
                _basicData.SessionCount = sessionViewModel.SessionCount;
                _basicData.FirstSessionStartTime = new DateTime(sessionViewModel.FirstSessionTime);

                PlayerPrefs.SetString("ProbaSessionID", _basicData.CurrentSessionId);

                OldRecordSent();
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void SendEventAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveEventInDB(baseEventDataViewModel);
                return;
            }

            var className = GetClass(baseEventDataViewModel);

            try
            {
                var (success, statusCode) = await _probaHttpClient.SendEventAsync(baseEventDataViewModel, className);
                if (!success)
                {
                    SaveEventInDB(baseEventDataViewModel);
                    return;
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    SaveEventInDB(baseEventDataViewModel);
                    _probaLogger.LogWarning($"couldn't send event with {statusCode} status");
                }
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveEventInDB(baseEventDataViewModel);
            }
        }

        private async void ExistingEvent(BaseEventDataViewModel baseEventDataViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }

            var className = GetClass(baseEventDataViewModel);

            try
            {
                var (success, statusCode) = await _probaHttpClient.SendEventAsync(baseEventDataViewModel, className);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    OldRecordCouldNotSent();
                    _probaLogger.LogWarning($"couldn't send event with {statusCode} status");
                    return;
                }

                OldRecordSent();
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void SendBatchEventAsync(BatchEventViewModel batchEventViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveBatchEventInDB(batchEventViewModel);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.SendBatchEventAsync(batchEventViewModel);
                if (!success)
                {
                    SaveBatchEventInDB(batchEventViewModel);
                    return;
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    SaveBatchEventInDB(batchEventViewModel);
                    _probaLogger.LogWarning($"couldn't send batch event with {statusCode} status");
                    return;
                }

                _totalEventCount = 0;
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                SaveBatchEventInDB(batchEventViewModel);
            }
        }

        private async void ExistingBatchEvent(BatchEventViewModel batchEventViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.SendBatchEventAsync(batchEventViewModel);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    OldRecordCouldNotSent();
                    _probaLogger.LogWarning($"couldn't send batch event with {statusCode} status");
                    return;
                }

                OldRecordSent();
            }
            catch (Exception e)
            {
                _probaLogger.LogError(e.Message, e.StackTrace);
                OldRecordCouldNotSent();
            }
        }

        private async void EndSessionAsync(EndSessionViewModel endSessionViewModel)
        {
            endSessionViewModel.Error = _basicData.HasError;

            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveEndSessionInDB(endSessionViewModel);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.SendEndSessionAsync(endSessionViewModel);
                if (!success)
                {
                    SaveEndSessionInDB(endSessionViewModel);
                    return;
                }
                if (statusCode != HttpStatusCode.OK)
                {
                    SaveEndSessionInDB(endSessionViewModel);
                    _probaLogger.LogWarning($"couldn't send end session with {statusCode} status");
                }
            }
            catch (Exception e)
            {
                SaveEndSessionInDB(endSessionViewModel);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void ExistingEndSession(EndSessionViewModel endSessionViewModel)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }

            try
            {
                endSessionViewModel.SessionId = _basicData.CurrentSessionId;
                endSessionViewModel.Location = _basicData.CurrentSessionLocation;
                var (success, statusCode) = await _probaHttpClient.SendEndSessionAsync(endSessionViewModel);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }
                if (statusCode != HttpStatusCode.OK)
                {
                    OldRecordCouldNotSent();
                    _probaLogger.LogWarning($"couldn't send end session with {statusCode} status");
                    return;
                }

                OldRecordSent();
            }
            catch (Exception e)
            {
                OldRecordCouldNotSent();
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private void EmptyEndSession()
        {
            var end = new EndSessionViewModel
            {
                SessionId = PlayerPrefs.GetString("ProbaSessionID"),
                UserId = PlayerPrefs.GetString("ProbaUserID")
            };
            EndSessionAsync(end);
        }

        #endregion

        #region Trophy

        private async void GetAchievementsListAsync()
        {
            var trophyRequest = new TrophyRequest();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.AchievementsListCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, achievements) = await _probaHttpClient.GetAchievementsListAsync(trophyRequest);
                if (!success)
                {
                    PROBA.AchievementsListCanceled(RequestResponse.Error);
                    return;
                }

                PROBA.AchievementsListReceived(achievements);

            }
            catch (Exception e)
            {
                PROBA.AchievementsListCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void GetLeaderBoardsListAsync()
        {
            var trophyRequest = new TrophyRequest();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.LeaderBoardsListCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, leaderBoards) = await _probaHttpClient.GetLeaderBoardsListAsync(trophyRequest);
                if (!success)
                {
                    PROBA.LeaderBoardsListCanceled(RequestResponse.Error);
                    return;
                }

                PROBA.LeaderBoardsListReceived(leaderBoards);

            }
            catch (Exception e)
            {
                PROBA.LeaderBoardsListCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void GetUserAchievementsListAsync()
        {
            var trophyRequest = new TrophyRequest()
            {
                UserId = _basicData.UserId
            };
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.UserAchievementsListCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, userAchievements) = await _probaHttpClient.GetUserAchievementsAsync(trophyRequest);
                var (allSuccess, allStatusCode, allAchievements) = await _probaHttpClient.GetAchievementsListAsync(new TrophyRequest());
                if (!success || !allSuccess)
                {
                    PROBA.UserAchievementsListCanceled(RequestResponse.Error);
                    return;
                }

                var prunedUserAchievements = new List<UserAchievementViewModel>();
                foreach (var Achievement in allAchievements)
                {
                    foreach (var userAchievement in userAchievements)
                    {
                        if (userAchievement.AchievementId == Achievement.ID)
                        {
                            userAchievement.AchievementEnName = Achievement.AchievementEnName;
                            userAchievement.AchievementName = Achievement.AchievementName;
                            prunedUserAchievements.Add(userAchievement);
                            break;
                        }
                    }
                }
                PROBA.UserAchievementsListReceived(prunedUserAchievements);

            }
            catch (Exception e)
            {
                PROBA.UserAchievementsListCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void GetLeaderBoardUsersListAsync(bool self, string leaderBoardId)
        {
            var trophyRequest = new TrophyRequest()
            {
                LeaderBoardId = leaderBoardId,
                UserId = self ? _basicData.UserId : Guid.Empty.ToString()
            };
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PROBA.LeaderBoardUserListCanceled(RequestResponse.NoInternet);
                return;
            }
            try
            {
                var (success, statusCode, userLeaderBoards) = await _probaHttpClient.GetLeaderBoardUsersAsync(trophyRequest);
                if (!success)
                {
                    PROBA.LeaderBoardUserListCanceled(RequestResponse.Error);
                    return;
                }

                PROBA.LeaderBoardUserListReceived(userLeaderBoards);

            }
            catch (Exception e)
            {
                PROBA.LeaderBoardUserListCanceled(RequestResponse.Error);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void UserNewLeaderBoardScoreAsync(string leaderBoardId, long score, string userName = "")
        {
            var trophyRequest = new TrophyRequest()
            {
                UserId = _basicData.UserId,
                LeaderBoardId = leaderBoardId,
                UserName = string.IsNullOrWhiteSpace(userName) ? _basicData.CurrentUserName : userName,
                TrophyScore = score
            };
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveUserNewLeaderBoardScoreInDB(trophyRequest);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.AddNewLeaderBoardScoreAsync(trophyRequest);
                if (!success)
                {
                    SaveUserNewLeaderBoardScoreInDB(trophyRequest);
                }

            }
            catch (Exception e)
            {
                SaveUserNewLeaderBoardScoreInDB(trophyRequest);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void ExistingNewLeaderBoard(TrophyRequest trophyRequest)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.AddNewLeaderBoardScoreAsync(trophyRequest);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                OldRecordSent();

            }
            catch (Exception e)
            {
                OldRecordCouldNotSent();
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void UserNewAchievementAsync(string achievementId)
        {
            var trophyRequest = new TrophyRequest()
            {
                UserId = _basicData.UserId,
                AchievementId = achievementId
            };
            if (Application.internetReachability == NetworkReachability.NotReachable || _sending)
            {
                SaveUserNewAchievementsInDB(trophyRequest);
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.AddUserNewAchievementAsync(trophyRequest);
                if (!success)
                {
                    SaveUserNewAchievementsInDB(trophyRequest);
                }

            }
            catch (Exception e)
            {
                SaveUserNewAchievementsInDB(trophyRequest);
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        private async void ExistingNewAchievement(TrophyRequest trophyRequest)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OldRecordCouldNotSent();
                return;
            }
            try
            {
                var (success, statusCode) = await _probaHttpClient.AddUserNewAchievementAsync(trophyRequest);
                if (!success)
                {
                    OldRecordCouldNotSent();
                    return;
                }

                OldRecordSent();
            }
            catch (Exception e)
            {
                OldRecordCouldNotSent();
                _probaLogger.LogError(e.Message, e.StackTrace);
            }
        }

        #endregion

        private void SaveRegisterInDB(BaseEventDataViewModel registerViewModel)
        {
            var regDB = JsonConvert.SerializeObject(registerViewModel);
            DatabaseConnection.InsertUnsentEvent("REGISTER", regDB);
        }

        private void SaveUpdateUserInDB(BaseEventDataViewModel updatedUser)
        {
            var userDB = JsonConvert.SerializeObject(updatedUser);
            DatabaseConnection.InsertUnsentEvent("UPDATE", userDB);
        }

        private void SaveCheckProgressionStatusInDB(BaseEventDataViewModel progressionStatus)
        {
            var progStatDB = JsonConvert.SerializeObject(progressionStatus);
            DatabaseConnection.InsertUnsentEvent("STATUS", progStatDB);
        }

        private void SaveStartSessionInDB(StartSessionViewModel sessionViewModel)
        {
            var ssDB = JsonConvert.SerializeObject(sessionViewModel);
            DatabaseConnection.InsertUnsentEvent("START_SESSION", ssDB);
        }

        private void SaveEndSessionInDB(EndSessionViewModel endSessionViewModel)
        {
            var esDB = JsonConvert.SerializeObject(endSessionViewModel);
            DatabaseConnection.InsertUnsentEvent("END_SESSION", esDB);
        }

        private void SaveEventInDB(BaseEventDataViewModel eventViewModel)
        {
            var eDB = JsonConvert.SerializeObject(eventViewModel);
            DatabaseConnection.InsertUnsentEvent("EVENT", eDB);
        }

        private void SaveBatchEventInDB(BatchEventViewModel batchEventViewModel)
        {
            var beDB = JsonConvert.SerializeObject(batchEventViewModel);
            DatabaseConnection.InsertUnsentEvent("BATCH_EVENT", beDB);
        }

        private void SaveSaveUserProgressInDB(ProgressViewModel progressViewModel)
        {
            var usProDB = JsonConvert.SerializeObject(progressViewModel);
            DatabaseConnection.InsertUnsentEvent("USER_PROGRESS", usProDB);
        }

        private void SaveUserNewAchievementsInDB(TrophyRequest trophyRequest)
        {
            var nuaDB = JsonConvert.SerializeObject(trophyRequest);
            DatabaseConnection.InsertUnsentEvent("NEW_ACHIEVEMENT", nuaDB);
        }

        private void SaveUserNewLeaderBoardScoreInDB(TrophyRequest trophyRequest)
        {
            var nulbDB = JsonConvert.SerializeObject(trophyRequest);
            DatabaseConnection.InsertUnsentEvent("NEW_LEADER_BOARD", nulbDB);
        }

        private string GetClass(BaseEventDataViewModel baseEventDataViewModel)
        {
            string className;
            switch (baseEventDataViewModel.Class)
            {
                case "AchievementEventViewModel":
                    className = "AchievementEvent";
                    break;
                case "AdvertisementEventViewModel":
                    className = "AdvertisementEvent";
                    break;
                case "BusinessEventViewModel":
                    className = "BusinessEvent";
                    break;
                case "ContentViewEventViewModel":
                    className = "ContentViewEvent";
                    break;
                case "DesignEventViewModel":
                    className = "DesignEvent";
                    break;
                case "ProgressionEventViewModel":
                    className = "ProgressionEvent";
                    break;
                case "SocialEventViewModel":
                    className = "SocialEvent";
                    break;
                case "TapEventViewModel":
                    className = "TapEvent";
                    break;
                default:
                    className = "Default";
                    break;
            }

            return className;
        }

        private void Update()
        {
            if (!RecordAllTouches || Input.touchCount <= 0)
                return;

            for (var i = 0; i < Input.touchCount; i++)
            {
                var theTouch = Input.GetTouch(i);

                switch (theTouch.phase)
                {
                    case TouchPhase.Began:
                        try
                        {
                            _touchePhases[i] = TouchPhase.Began;
                        }
                        catch (Exception)
                        {
                            _touchePhases.Add(i, TouchPhase.Began);
                        }

                        try
                        {
                            _starTuples[i] = new Tuple<float, float>(theTouch.position.x, theTouch.position.y);
                        }
                        catch (Exception)
                        {
                            _starTuples.Add(i, new Tuple<float, float>(theTouch.position.x, theTouch.position.y));
                        }

                        break;

                    case TouchPhase.Moved:
                        _touchePhases[i] = TouchPhase.Moved;
                        break;

                    case TouchPhase.Ended:
                        switch (_touchePhases[i])
                        {
                            case TouchPhase.Began:
                                PROBA.TapEvent(TapTypes.Tap, startX: _starTuples[i].Item1, startY: _starTuples[i].Item2);
                                break;
                            case TouchPhase.Moved:
                                PROBA.TapEvent(TapTypes.Sweep, startX: _starTuples[i].Item1, startY: _starTuples[i].Item2, endX: theTouch.position.x, endY: theTouch.position.y);
                                break;
                        }

                        _touchePhases[i] = TouchPhase.Ended;
                        break;
                }
            }
        }
    }
}