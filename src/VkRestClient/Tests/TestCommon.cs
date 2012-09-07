using System;
using System.Diagnostics;
using System.Threading;
using VkApi;
using VkApi.JsonTypes;
using System.Threading.Tasks;

namespace Tests
{
    public class TestCommon
    {
        /// <summary>
        /// Put valid users and/or auth tokens here (obtained manually).
        /// 
        /// http://oauth.vk.com/authorize?client_id=3061241&scope=messages,friends&redirect_uri=http://oauth.vk.com/blank.html&display=page&response_type=token
        /// http://vk.com/pages?oid=-1&p=%D0%9F%D1%80%D0%B0%D0%B2%D0%B0_%D0%B4%D0%BE%D1%81%D1%82%D1%83%D0%BF%D0%B0_%D0%BF%D1%80%D0%B8%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B9
        /// </summary>
        public static readonly TestUser MainTestUser = new TestUser(
            "testuser1@test.com", "userpassword"
                        , new VkAuthToken() { Value = "307c8b86606edd4f302ffe93c0306c8eb033042304a2b4a702ab9ad8999f291", UserId = "1234567" }
            );
        public static readonly TestUser SecondaryTestUser = new TestUser(
            "testuser2@test.com", "userpassword"
                        , new VkAuthToken() { Value = "34521f1b645a43b8341b6042b43458106133476347eb59bb0996bcad9232074", UserId = "7654321" }
            );

        /// <summary>
        /// Put your testing app ID and key here.
        /// </summary>
        public const string AppId = "1234567";
        public const string AppKey = "ZfF6Gdc3FSkoF1h8ZXYE";
        
        public static VkClient NewSession(string token)
        {
            return new VkClient(token ?? MainTestUser.AuthToken.Value);
        }
    }

    public class TestUser
    {
        public TestUser(string login, string password, VkAuthToken token = null)
        {
            this.Login = login;
            this.Password = password;
            this._authToken = token;
        }

        public readonly string Password;
        public readonly string Login;

        public string UserId { get { return this.AuthToken.UserId; } }

        public static VkAuthToken GetAuthTokenSync(string login, string password)
        {
            Debug.WriteLine("Getting authtoken synchronously for user {0}", login);

            var task = VkApi.Methods.Auth.GetToken(login, password, TestCommon.AppId, TestCommon.AppKey).ExecuteFree();
            task.Wait();
            var token = task.Result;

            if (token == null)
            {
                Debug.WriteLine("!!! Couldn't automatically login and acquire auth token.");
                throw new Exception("Couldn't automatically login.");
            }

            return token.Data;
        }

        private VkAuthToken _authToken = null;
        public VkAuthToken AuthToken
        {
            get
            {
                _authToken = _authToken ?? GetAuthTokenSync(this.Login, this.Password);
                return _authToken;
            }
        }

        public VkClient Session
        {
            get { return TestCommon.NewSession(this.AuthToken.Value); }
        }
    }
}
