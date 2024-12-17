using OpenNGS.SDK.Auth.Credentials;
using OpenNGS.SDK.Auth.Exceptions;
using OpenNGS.SDK.Auth.Models;
using OpenNGS.SDK.Auth.Models.Requests;
using OpenNGS.SDK.Auth.Network;
using OpenNGS.SDK.Core.Network;
using System;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth
{
    public class AuthcationServiceInternal : IAuthenticationService
    {
        static Credential<EegamesUserInfo> Credential;
        EegamesUserInfo user;
        public EegamesUserInfo User { get { return user; } }
        public string Token { get { return Credential.Password; } }

        public event Action<bool> AutoLoginCallback;
        public event Action<EegamesUserInfo> LoginCallback;
        public event Action VerificationCodeCallback;

        internal IAuthenticationNetworkClient NetworkClient { get; set; }

        internal IAuthenticationExceptionHandler ExceptionHandler { get; set; }

        public AuthcationServiceInternal()
        {
            NetworkClient = new AuthenticationNetworkClient("http://api.eegames.com/services/platform/auth");
            Credential = new Credential<EegamesUserInfo>();
            LocalStore<EegamesUserInfo>.Init();
            Credential.Target = "EEGAMES/Platform/eegames.com";
        }

        public async Task<bool> AutoLogin()
        {
            this.user = null;
            Credential.Load();
            if (Credential.Exists())
            {
                if (Credential.User != null)
                { 
                    var ts = DateTime.Now.ToLocalTime() - Credential.LastWriteTime;
                    Console.WriteLine("Last Login: {0} Minutes", ts.TotalMinutes);
                    if (ts.TotalHours < 24 * 30)
                    {
                        user = Credential.User;
                        AutoLoginCallback?.Invoke(true);
                        return true;
                    }
                    AutoLoginCallback?.Invoke(false);
                    return false;
                }
                await GetUserInfo();
                return true;
            }
            else
            {
                AutoLoginCallback?.Invoke(false);
                return false;
            }
        } 

        public Task LoginByUsernamePassword(string username, string password)
        {
            return HandleLoginRequestAsync(() => NetworkClient.LoginByUsernamePasswordAsync(BuildUsernamePasswordRequest(username, password)));
        }

        public Task GetUserInfo()
        {
            return HandleUserInfoRequestAsync(() => NetworkClient.GetUserInfo(Token));
        }


        public Task LoginOrRegisterByVerificationCode(VerificationType verificationType, string dest, string code)
        {
            return HandleLoginRequestAsync(() => NetworkClient.LoginOrRegisterByVerificationCode(BuildVerificationCodeRequest(verificationType, dest, code)));
        }


        public Task SendVerificationCode(VerificationType verificationType, string dest)
        {
            return HandleVerificationCodeRequestAsync(() => NetworkClient.SendVerificationCode(BuildSendVerificationCodeRequest(verificationType, dest)));
        }

        internal async Task HandleLoginRequestAsync(Func<Task<NetworkResponse<string>>> loginRequest)
        {
            CompleteLogin(await loginRequest());
            await GetUserInfo();
        }

        internal async Task HandleUserInfoRequestAsync(Func<Task<NetworkResponse<EegamesUserInfo>>> userInfoRequest)
        {
            CompleteUserInfo(await userInfoRequest());
        }

        internal async Task HandleVerificationCodeRequestAsync(Func<Task<NetworkResponse>> sendRequest)
        {
            CompleteVerificationCode(await sendRequest());
        }

        internal void CompleteVerificationCode(NetworkResponse response)
        {
            if (CompleteSuccess(response))
            {
                VerificationCodeCallback?.Invoke();
            }
        }

        internal void CompleteLogin(NetworkResponse<string> response)
        {
            if (CompleteSuccess(response))
            {
                Credential.Username = "accessToken";
                Credential.Password = response.Data;
                Credential.Save();
            }
        }

        internal void CompleteUserInfo(NetworkResponse<EegamesUserInfo> response)
        {
            if (CompleteSuccess(response))
            {
                this.user = response.Data;
                Credential.User = response.Data;
                Credential.Save();
                LoginCallback?.Invoke(response.Data);
            }
        }

        internal bool CompleteSuccess(NetworkResponse response)
        {
            if (response.Success)
            {
                return true;
            }
            else
            {
                Log.Error($"[AuthcationService] {response.Message}");
                return false;
            }
        }


        public void Logout()
        {
            Credential.Delete();
        }

        private UsernamePasswordRequest BuildUsernamePasswordRequest(string username, string password)
        {
            if (!ValidateCredentials(username, password))
            {
                throw ExceptionHandler.BuildInvalidCredentialsException();
            }

            return new UsernamePasswordRequest
            {
                Username = username,
                Password = password
            };
        }

        private VerificationCodeRequest BuildVerificationCodeRequest(VerificationType verificationType, string dest, string code)
        {
            if (!ValidateVerificationCode(dest, code))
            {
                throw ExceptionHandler.BuildVerificationCodeException();
            }

            return new VerificationCodeRequest()
            {
                Type = verificationType,
                Dest = dest,
                Code = code
            };
        }

        private VerificationCodeRequest BuildSendVerificationCodeRequest(VerificationType verificationType, string dest)
        {
            if (string.IsNullOrEmpty(dest))
            {
                throw ExceptionHandler.BuildVerificationCodeException();
            }

            return new VerificationCodeRequest()
            {
                Type = verificationType,
                Dest = dest,
            };
        }

        

        bool ValidateCredentials(string username, string password)
        {
            return !string.IsNullOrEmpty(username)
                && !string.IsNullOrEmpty(password);
        }

        bool ValidateVerificationCode(string dest, string code)
        {
            return !string.IsNullOrEmpty(dest)
                && !string.IsNullOrEmpty(code);
        }
    }
}
