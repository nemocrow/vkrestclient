using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi.Methods
{
    public static class Users
    {
        /// <summary>
        /// http://vk.com/pages?oid=-1&p=users.get
        /// </summary>
        /// <param name="userIds">перечисленные через запятую ID пользователей или их короткие имена (screen_name). Максимум 1000 пользователей.</param>
        /// <param name="fields">перечисленные через запятую поля анкет, необходимые для получения. Доступные значения: uid, first_name, last_name, nickname, screen_name, sex, bdate (birthdate), city, country, timezone, photo, photo_medium, photo_big, has_mobile, rate, contacts, education, online, counters.</param>
        /// <param name="nameCase">падеж для склонения имени и фамилии пользователя. Возможные значения: именительный – nom, родительный – gen, дательный – dat, винительный – acc, творительный – ins, предложный – abl. По умолчанию nom.</param>
        /// <returns></returns>
        public static ISessionApiRequest<List<VkUserProfile>> Get(
            IEnumerable<string> userIds,
            string fields = null,
            UserNameCase? nameCase = null)
        {
            return new SessionApiRequest<List<VkUserProfile>>(
                "users.get",
                Method.GET,
                new ParameterDict
                    {
                        {"uids", string.Join(",", userIds)},
                        {"fields", fields ?? VkUserProfile.AllFields},
                        {"name_case", nameCase != null ? nameCase.ToString() : null}
                    });
        }
    }

    public enum UserNameCase
    {
        Nominative,
        Accusative,
        Dative,
        Ablative,
        Genitive,
        Instrumental
    }

    public static class UserNameCaseEx
    {
        public static string ToString(this UserNameCase nameCase)
        {
            switch (nameCase)
            {
                case UserNameCase.Nominative:
                    return "nom";
                case UserNameCase.Accusative:
                    return "acc";
                case UserNameCase.Ablative:
                    return "abl";
                case UserNameCase.Dative:
                    return "dat";
                case UserNameCase.Instrumental:
                    return "ins";
                case UserNameCase.Genitive:
                    return "gen";

                default:
                    return null;
            }
        }
    }
}
