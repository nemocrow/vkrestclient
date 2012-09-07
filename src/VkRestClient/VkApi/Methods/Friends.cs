using System;
using System.Collections.Generic;
using RestSharp;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi.Methods
{
    public static class Friends
    {
        //http://vk.com/pages?oid=-1&p=friends.get
        /// <summary>
        /// Возвращает список идентификаторов друзей пользователя или расширенную информацию о друзьях пользователя (при использовании параметра fields). 
        /// 
        /// friends.get 
        ///  </summary>
        /// <param name="userId">идентификатор пользователя, для которого необходимо получить список друзей. Если параметр не задан, то считается, что он равен идентификатору текущего пользователя.</param>
        /// <param name="count">количество друзей, которое нужно вернуть. (по умолчанию – все друзья)</param>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества друзей.</param>
        /// <param name="nameCase">падеж для склонения имени и фамилии пользователя. Возможные значения: именительный – nom, родительный – gen, дательный – dat, винительный – acc, творительный – ins, предложный – abl. По умолчанию nom.</param>
        /// <param name="friendsListId">идентификатор списка друзей, полученный методом friends.getLists, друзей из которого необходимо получить. Данный параметр учитывается, только когда параметр uid равен идентификатору текущего пользователя.</param>
        /// <param name="sortByName">Порядок в котором нужно вернуть список друзей. Допустимые значения: name - сортировать по имени (работает только при переданном параметре fields). hints - сортировать по рейтингу, аналогично тому, как друзья сортируются в разделе Моя друзья</param>
        /// <param name="fields">перечисленные через запятую поля анкет, необходимые для получения. Доступные значения: uid, first_name, last_name, nickname, sex, bdate (birthdate), city, country, timezone, photo, photo_medium, photo_big, domain, has_mobile, rate, contacts, education.</param>
        /// <returns></returns>
        public static ISessionApiRequest<List<VkUserProfile>> Get(
            string userId = null,
            int? count = null,
            int? offset = null,
            UserNameCase? nameCase = null,
            string friendsListId = null,
            bool sortByName = false,
            string fields = null)
        {
            // Пример ответа в формате JSON (без использования fields)
            // {"response":[1,6492,35828305]}

            //Пример ответа в формате JSON (при использовании fields)
            //{"response":[{"uid":"1","first_name":"Павел","last_name":"Дуров",
            //"photo":"http:\/\/cs109.vkontakte.ru\/u00001\/c_df2abf56.jpg","online":"1","lists":[2,3]},
            //{"uid":"6492","first_name":"Andrew","last_name":"Rogozov",
            //"photo":"http:\/\/cs537.vkontakte.ru\/u06492\/c_28629f1d.jpg","online":"1"},{"uid":"35828305","first_name":"Виталий","last_name":"Лагунов",
            //"photo":"http:\/\/cs9917.vkontakte.ru\/u35828305\/c_e2117d04.jpg","online":"1","lists":[1]}]}

            return new SessionApiRequest<List<VkUserProfile>>(
                "friends.get",
                Method.GET,
                new ParameterDict
                    {
                        {"name_case", nameCase != null ? nameCase.Value.ToString() : null},
                        {"uid", userId},
                        {"count", count},
                        {"offset", offset},
                        {"lid", friendsListId},
                        {"order", sortByName ? "name" : "hints"},
                        {"fields", fields ?? VkUserProfile.AllFields}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=friends.getByPhones
        /// 
        /// friends.getByPhones
        /// Возвращает список друзей пользователя у которых завилидированные телефонные или 
        /// указанные в профайле мобильные номера входят в заданный список. 
        /// 
        /// Использование данного метода возможно только если у текущего пользователя 
        /// завалидирован номер мобильного телефона. Для проверки этого условия можно 
        /// использовать метод getProfiles c параметрами uids=API_USER и fields=has_mobile, 
        /// где API_USER равен идентификатору текущего пользователя. 
        /// </summary>
        /// <param name="phones">список телефонных номеров в формате MSISDN разделеннных запятыми. Например</param>
        /// <param name="fields">перечисленные через запятую поля анкет, необходимые для получения. Список доступных полей указан на странице Описание полей параметра fields.</param>
        /// <returns></returns>
        public static ISessionApiRequest<List<VkUserProfile>> GetByPhones(
            IEnumerable<string> phones,
            string fields = null)
        {
            return new SessionApiRequest<List<VkUserProfile>>(
                "friends.getByPhones",
                Method.GET,
                new ParameterDict
                {
                    {"phones", string.Join(",", phones)},
                    {"fields", fields ?? VkUserProfile.AllFields}
                });
        }

    }
}
