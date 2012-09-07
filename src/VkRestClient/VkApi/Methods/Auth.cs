using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VkApi.Base;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi.Methods
{
    public static class Auth
    {
        /// <summary>
        /// Доверенные приложения могут получить неограниченный по времени access token 
        /// для доступа к API, передав логин и пароль пользователя. 
        /// 
        /// Обратите внимание, что приложение не должно хранить пароль пользователя. 
        /// Выдаваемый access token не привязан к IP-адресу пользвателя, поэтому его 
        /// достаточно для последующей работы с API без повторения процедуры авторизации.
        /// </summary>
        /// <param name="clientId">Application ID.</param>
        /// <param name="clientSecret">Application secret key.</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public static IApiRequest<VkAuthToken> GetToken(
            string username,
            string password,
            string clientId,
            string clientSecret)
        {
            // Пример запроса авторизации:
            // https://oauth.vk.com/token?grant_type=password&client_id=1914441&...=***&username=***&password=***
            // @NOTE: пиздеж, правильный пример запроса: https://api.vk.com/oauth/token?grant_type=password&client_id=1914441&...=***&username=***&password=***
            // 
            // Пример ответа:
            // {"access_token":"9d77c727986d7668986d7668049870402D1986d986d76684bbc9b1bf8488de9", "expires_in":0,"user_id":85635407}

            return new ApiRequest<VkAuthToken>(
                "oauth/token",
                Method.GET,
                new ParameterDict
                    {
                        {"grant_type", "password"},
                        {"client_id", clientId},
                        {"client_secret", clientSecret},
                        {"username", username},
                        {"password", password},
                        {"scope", "notify,friends,messages,notifications"}
                    },
                json =>
                {
                    // {"access_token":"9d77c727986d7668986d7668049870402D1986d986d76684bbc9b1bf8488de9", "expires_in":0,"user_id":85635407}
                    if (json["access_token"] != null)
                    {
                        var token = json.ToObject<VkAuthToken>();
                        token.CreationTime = DateTime.Now;
                        return token;
                    }
                    // unknown JSON (or other) content
                    else
                        throw new UnknownJsonFormatException(json);
                }
            );
        }

        /// <summary>
        /// В случае успешного выполнения метода на номер телефона, указанный пользователем, 
        /// будет отправлено SMS со специальным кодом, который может быть использован для 
        /// завершения регистрации методом auth.confirm. 
        /// 
        /// В качестве ответа будет возвращено поле sid, необходимое для повторного вызова 
        /// метода, в случае если SMS-сообщение не дошло. 
        /// </summary>
        public class SignUpReply : ApiData
        {
            [JsonProperty("sid")]
            public string SessionId { get; set; }
        }

        /// <summary>
        /// auth.signup
        /// Регистрирует нового пользователя по номеру телефона. 
        /// </summary>
        /// <param name="phone">Номер телефона регистрируемого пользователя. Номер телефона может быть проверен заранее методом auth.checkPhone.</param>
        /// <param name="firstName">Имя пользователя.</param>
        /// <param name="lastName">Фамилия пользователя.</param>
        /// <param name="sex">Пол пользователя: 1 - Женский, 2 - Мужской.</param>
        /// <param name="password">Пароль пользователя, который будет использоваться при входе. Не меньше 6 символов. Также пароль может быть указан позже, при вызове метода auth.confirm.</param>
        /// <param name="voice">1 - в случае если вместо SMS необходимо позвонить на указанный номер и продиктовать код голосом. 0 - (по умолчанию) необходимо отправить SMS.</param>
        /// <param name="sid">Идентификатор сессии, необходимый при повторном вызове метода, в случае если SMS сообщение доставлено не было.</param>
        /// <param name="testMode">1 - тестовый режим, при котором не будет зарегистрирован новый пользователь, но при этом номер не будет проверяться на использованность. 0 - (по умолчанию) рабочий.</param>
        /// <param name="clientId">Идентификатор Вашего приложения.</param>
        /// <param name="clientSecret">Секретный ключ Вашего приложения.</param>
        public static IApiRequest<SignUpReply> SignUp(
                string phone,
                string firstName,
                string lastName,
                string clientId,
                string clientSecret,
                string sex = null,
                string password = null,
                string voice = null,
                string sid = null,
                string testMode = "0"
)
        {
            return new ApiMethodRequest<SignUpReply>(
                "auth.singup",
                Method.GET,
                new ParameterDict
                    {
                        {"phone", phone},
                        {"first_name", firstName},
                        {"last_name", lastName},
                        {"client_id", clientId},
                        {"client_secret", clientSecret},
                        {"sex", sex},
                        {"password", password},
                        {"voice", voice},
                        {"sid", sid},
                        {"test_mode", testMode},
                    }
                );
        }

        /// <summary>
        /// auth.checkPhone
        /// Проверяет правильность введённого номера. 
        /// 
        /// В случае если номер пользователя является правильным будет возвращён 1. 
        /// 
        /// Пример ответа в формате JSON
        /// {"response": 1}
        /// 
        /// Обратите внимание, что в процессе выполнения запроса к данному методу, 
        /// несмотря на правильность ввода всех данных может быть возвращена ошибка 
        /// с кодом 1112. 
        /// 
        /// Данная ошибка означает, что введённый номер анализируется, поэтому следует 
        /// повторить вызов данного метода с теми же параметрами через некоторое время 
        /// (приблизительно 5 сек), либо через это время вызвать метод auth.signup.
        /// </summary>
        /// <param name="phone">Номер телефона пользователя.</param>
        public static IApiRequest<ApiData> CheckPhone(
            string phone,
            string clientId,
            string clientSecret)
        {
            return new ApiMethodRequest<ApiData>(
                "auth.checkPhone",
                Method.GET,
                new ParameterDict
                    {
                        {"phone", phone},
                        {"client_id", clientId},
                        {"client_secret", clientSecret},
                    }
                );
        }

        /// <summary>
        /// В случае в случае успешного завершения авторизации таким способом будет 
        /// возвращён объект содержащий поле success = 1 и uid = идентификатор 
        /// зарегистрированного пользователя. 
        /// </summary>
        public class ConfirmReply : ApiData
        {
            [JsonProperty("uid")]
            public string UserId { get; set; }
        }

        /// <summary>
        /// auth.confirm
        /// Завершает регистрацию нового пользователя, начатую методом auth.signup, по коду, полученному через SMS.
        /// </summary>
        /// <param name="phone">Номер телефона регистрируемого пользователя. Номер телефона может быть проверен заранее методом auth.checkPhone.</param>
        /// <param name="code">Код, полученный через SMS в результате выполнения метода auth.signup.</param>
        /// <param name="password">Пароль пользователя, который будет использоваться при входе. Не меньше 6 символов. Также пароль может быть указан позже, при вызове метода auth.signup.</param>
        /// <param name="clientId">Идентификатор Вашего приложения.</param>
        /// <param name="clientSecret">Секретный ключ Вашего приложения.</param>
        /// <param name="testMode">1 - тестовый режим, при котором не будет зарегистрирован новый пользователь, но при этом номер не будет проверяться на использованность. 0 - (по умолчанию) рабочий.</param>
        public static IApiRequest<ConfirmReply> Confirm(
                string phone,
                string code,
                string clientId,
                string clientSecret,
                string password = null,
                string testMode = "0")
        {
            return new ApiMethodRequest<ConfirmReply>(
                "auth.confirm",
                Method.GET,
                new ParameterDict
                    {
                        {"phone", phone},
                        {"code", code},
                        {"client_id", clientId},
                        {"client_secret", clientSecret},
                        {"password", password},
                        {"test_mode", testMode}
                    }
                );
        }
    }
}
