using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Proba.Scripts.Client;
using Proba.Scripts.Configuration;
using Proba.Scripts.SharedClasses;
using UnityEngine;

namespace Proba.Scripts
{
    internal class ProbaHttpClient
    {
        private ProbaLogger Logger { get; }
        private HttpClient Client { get; }
        private string SecretKey { get; }
        private string PublicKey { get; }
        private HmacService HmacService { get; }
        private CancellationTokenSource CancellationTokenSource { get; }

        internal ProbaHttpClient(ProbaLogger logger, string secretKey, string publicKey)
        {
            Logger = logger;
            SecretKey = secretKey;
            PublicKey = publicKey;
            Client = new HttpClient();
            HmacService = new HmacService();
            CancellationTokenSource = new CancellationTokenSource();
        }

        private string APIVersion => ConfigurationModel.CurrentAPIVersion;
        private string BaseURL => ConfigurationModel.BaseURL;

        #region Events

        //id begir
        /// <summary>Sends a event batch asynchronous.</summary>
        /// <param name="batchEventViewModel">event batch</param>
        /// <returns>
        ///   <br />
        /// </returns>
        internal async Task<(bool sucess, HttpStatusCode statusCode)> SendBatchEventAsync(BatchEventViewModel batchEventViewModel)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/BatchEvent/{PublicKey}", JsonConvert.SerializeObject(batchEventViewModel), CancellationTokenSource);
                return success ? (success, statusCode) : (default, statusCode);

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        /// <summary>Sends a event asynchronous.</summary>
        /// <param name="baseEventDataViewModel">body as json</param>
        /// <param name="eventClass">The event class name</param>
        /// <returns>
        ///   <br />
        /// </returns>
        internal async Task<(bool sucess, HttpStatusCode statusCode)> SendEventAsync(BaseEventDataViewModel baseEventDataViewModel, string eventClass)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/{eventClass}/{PublicKey}", JsonConvert.SerializeObject(baseEventDataViewModel), CancellationTokenSource);
                return success ? (success, statusCode) : (default, statusCode);

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode, CreateSessionResponseModel sessionResponse)> SendStartSessionAsync(StartSessionViewModel startSessionViewModel)
        {
            Debug.Log("start: " + JsonConvert.SerializeObject(startSessionViewModel));
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/SessionStart/{PublicKey}", JsonConvert.SerializeObject(startSessionViewModel), CancellationTokenSource);
                if (success)
                {
                    Debug.Log("result: " + content);
                    var result = JsonConvert.DeserializeObject<CreateSessionResponseModel>(content);
                    return (success, statusCode, result);
                }

                return (default, statusCode, default);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool sucess, HttpStatusCode statusCode)> SendEndSessionAsync(EndSessionViewModel endSessionViewModel)
        {
            Debug.Log("end: " + JsonConvert.SerializeObject(endSessionViewModel));
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/SessionEnd/{PublicKey}", JsonConvert.SerializeObject(endSessionViewModel), CancellationTokenSource);
                return success ? (success, statusCode) : (default, statusCode);

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        #endregion

        #region Account
        internal async Task<(bool success, HttpStatusCode statusCode, RegisterResponseViewModel sessionResponse)> RegisterAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/Register/{PublicKey}", JsonConvert.SerializeObject(baseEventDataViewModel), CancellationTokenSource);
                if (success)
                {
                    var result = JsonConvert.DeserializeObject<RegisterResponseViewModel>(content);

                    if (!string.IsNullOrEmpty(result.progress))
                        result.progress = Encoding.UTF8.GetString(Convert.FromBase64String(result.progress));
                    else
                        Logger.LogWarning("Registered, no progress on server");

                    if (!string.IsNullOrEmpty(result.configurations))
                        result.configurations = Encoding.UTF8.GetString(Convert.FromBase64String(result.configurations));
                    else
                        Logger.LogWarning("Registered, no configuration on server");

                    return (true, statusCode, result);
                }
                else
                {
                    Logger.LogWarning($"register not successful with {statusCode} status");
                    return (default, statusCode, default);
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode)> UpdateUserInfoAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/UpdateUserInfo/{PublicKey}", JsonConvert.SerializeObject(baseEventDataViewModel), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (success, statusCode);
                    else
                    {
                        Logger.LogWarning($"user update status was {statusCode}");
                        return (success, statusCode);
                    }
                }
                else
                {
                    Logger.LogWarning($"user update not successful with {statusCode} status");
                    return (default, statusCode);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }
        internal async Task<(bool success, HttpStatusCode statusCode)> SaveUserProgressAsync(ProgressViewModel progress)
        {
            try
            {
                progress.progress = Convert.ToBase64String(Encoding.UTF8.GetBytes(progress.progress));
                progress.configurations = Convert.ToBase64String(Encoding.UTF8.GetBytes(progress.configurations));
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/UpdateUserProgress/{PublicKey}", JsonConvert.SerializeObject(progress), CancellationTokenSource);
                if (success)
                    return (success, statusCode);
                else
                {
                    Logger.LogWarning($"User progress save not successful with {statusCode} status");
                    return (default, statusCode);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        //ba inam mitonam check konam
        internal async Task<(bool success, HttpStatusCode statusCode, RegisterResponseViewModel sessionResponse)> GetUserDataAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/GetUserData/{PublicKey}", JsonConvert.SerializeObject(baseEventDataViewModel), CancellationTokenSource);
                if (success)
                {
                    var result = JsonConvert.DeserializeObject<RegisterResponseViewModel>(content);
                    if (!string.IsNullOrEmpty(result.progress))
                        result.progress = Encoding.UTF8.GetString(Convert.FromBase64String(result.progress));
                    else
                        Logger.LogWarning("no progress in get user data");

                    if (string.IsNullOrEmpty(result.configurations))

                        result.progress = Encoding.UTF8.GetString(Convert.FromBase64String(result.progress));
                    else
                        Logger.LogWarning("no configuration in get user data");
                    return (success, statusCode, result);
                }
                else
                {
                    Logger.LogWarning($"get user not successful with {statusCode} status");
                    return (default, statusCode, default);
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<(bool success, HttpStatusCode statusCode, IList<RemoteConfigurationsViewModel> remoteConfigurations)> GetRemoteConfigurationsAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/RemoteConfigurations/{PublicKey}", JsonConvert.SerializeObject(baseEventDataViewModel), CancellationTokenSource);
                if (success)
                {
                    var congifs = JsonConvert.DeserializeObject<List<RemoteConfigurationsViewModel>>(content,
                        new JsonSerializerSettings
                        {
                            CheckAdditionalContent = true
                        });
                    if (statusCode == HttpStatusCode.OK)
                        return (success, statusCode, congifs);
                    else
                    {
                        Logger.LogWarning(statusCode.ToString(), $"user update status was {statusCode}");
                        return (success, statusCode, default);
                    }
                }
                else
                {
                    Logger.LogWarning($"get remote Configuration not successful with {statusCode} status");
                    return (default, statusCode, default);
                }

            }
            catch (Exception e)
            {//TODO
                Logger.LogWarning(e.Source, e.Message);
                throw;
            }
        }

        #endregion

        #region Trophy

        internal async Task<(bool success, HttpStatusCode statusCode, IList<AchievementViewModel> achievements)> GetAchievementsListAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetAchievementsList/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                    {
                        var achievemnts = JsonConvert.DeserializeObject<List<AchievementViewModel>>(content,
                            new JsonSerializerSettings
                            {
                                CheckAdditionalContent = true
                            });
                        return (success, statusCode, achievemnts);
                    }
                    else
                    {
                        Logger.LogWarning($"get available achievements list status was {statusCode}");
                        return (success, statusCode, default);
                    }

                }
                else
                {
                    Logger.LogWarning($"get available achievements list not successful with {statusCode} status");
                    return (default, statusCode, default);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode, IList<LeaderBoardViewModel> leaderBoards)> GetLeaderBoardsListAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetLeaderboardsList/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                    {
                        var leaderBoards = JsonConvert.DeserializeObject<List<LeaderBoardViewModel>>(content,
                            new JsonSerializerSettings
                            {
                                CheckAdditionalContent = true
                            });
                        return (success, statusCode, leaderBoards);
                    }
                    else
                    {
                        Logger.LogWarning($"get leader board list status was {statusCode}");
                        return (success, statusCode, default);
                    }

                }
                else
                {
                    Logger.LogWarning($"get leader board list not successful with {statusCode} status");
                    return (default, statusCode, default);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode, IList<UserAchievementViewModel> userAchievements)> GetUserAchievementsAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetUserAchievements/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                    {
                        var userAchievements = JsonConvert.DeserializeObject<List<UserAchievementViewModel>>(content,
                            new JsonSerializerSettings
                            {
                                CheckAdditionalContent = true
                            });
                        return (success, statusCode, userAchievements);
                    }
                    else
                    {
                        Logger.LogWarning($"get user achievements status was {statusCode}");
                        return (success, statusCode, default);
                    }
                }
                else
                {
                    Logger.LogWarning($"get user achievements not successful with {statusCode} status");
                    return (default, statusCode, default);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode, IList<LeaderBoardUserViewModel> userLeaderBoards)> GetLeaderBoardUsersAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetUserLeaderBoards/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                    {
                        var users = JsonConvert.DeserializeObject<List<LeaderBoardUserViewModel>>(content,
                            new JsonSerializerSettings
                            {
                                CheckAdditionalContent = true
                            });
                        return (success, statusCode, users);
                    }
                    else
                    {
                        Logger.LogWarning($"get user leader boards status was {statusCode}");
                        return (success, statusCode, default);
                    }
                }
                else
                {
                    Logger.LogWarning($"get user leader boards not successful with {statusCode} status");
                    return (default, statusCode, default);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode)> AddNewLeaderBoardScoreAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/AddNewLeaderBoardScore/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (success, statusCode);
                    else
                    {
                        Logger.LogWarning($"add leader board score status was {statusCode}");
                        return (success, statusCode);
                    }
                }
                else
                {
                    Logger.LogWarning($"add leader board score not successful with {statusCode} status");
                    return (default, statusCode);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        internal async Task<(bool success, HttpStatusCode statusCode)> AddUserNewAchievementAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (success, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/UserNewAchievement/{PublicKey}", JsonConvert.SerializeObject(trophyRequest), CancellationTokenSource);
                if (success)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (success, statusCode);
                    else
                    {
                        Logger.LogWarning($"add user new achievement status was{statusCode}");
                        return (success, statusCode);
                    }
                }
                else
                {
                    Logger.LogWarning($"add user new achievement not successful with {statusCode} status");
                    return (default, statusCode);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        #endregion

        private async Task<(bool success, HttpStatusCode statusCode, string content)> PostJsonRequestAsync(string url, string body, CancellationTokenSource cts)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("hmac", HmacService.GenerateHmacSignature(SecretKey, body));
            try
            {
                var response = await Client.SendAsync(request, cts.Token).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogWarning($"Unsuccessful http request response for request {url} with {response.StatusCode} status code and response: {content}");
                }

                return (response.IsSuccessStatusCode, response.StatusCode, content);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + $" Error while sending http request {url}", e.StackTrace);
                throw;
            }
        }
    }
}

