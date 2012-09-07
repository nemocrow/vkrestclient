using System.Collections.Generic;
using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// http://vk.com/developers.php?oid=-17680044&p=Description_of_Fields_of_the_fields_Parameter
    /// </summary>
    public class VkUserProfile : ApiData
    {
        public static string AllFields = "uid,first_name,last_name,nickname,screen_name,sex,photo,photo_medium,photo_big,online";

        [JsonProperty("uid")]
        public string Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// Returning values: 1 - female, 2 - male, 0 - not specified. 
        /// </summary>
        [JsonProperty("sex")]
        public VkUserProfileSex Sex { get; set; }

        public string ScreenName { get; set; }

        /// <summary>
        /// Returns user's rating. 
        /// </summary>
        [JsonProperty("rate")]
        public string UserRating { get; set; }

        /// <summary>
        /// Shows city id that is specified by the user in their "Contacts". The city 
        /// name can be determined based on its id by using the getCities method. 
        /// </summary>
        [JsonProperty("city")]
        public string CityId { get; set; }

        /// <summary>
        /// Date is shown in the following format: "23.11.1981" or "21.9" (if the year is hidden).
        /// </summary>
        [JsonProperty("bdate")]
        public string BirthDate { get; set; }

        /// <summary>
        /// Shows user's photo url that is 50 pixels wide. 
        /// </summary>
        [JsonProperty("photo")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Shows user's photo url that is 100 pixels wide. 
        /// </summary>
        [JsonProperty("photo_medium")]
        public string PhotoMediumUrl { get; set; }

        /// <summary>
        /// Shows user's photo url that is 200 pixels wide. 
        /// </summary>
        [JsonProperty("photo_big")]
        public string PhotoBigUrl { get; set; }

        /// <summary>
        /// Shows user's square photo url that is 50 pixels wide. 
        /// </summary>
        [JsonProperty("photo_rec")]
        public string PhotoSquareUrl { get; set; }

        /// <summary>
        /// Shows whether the user is online right now. Returning values: 1- online, 0 - offline. 
        /// </summary>
        [JsonProperty("online")]
        public VkUserOnlineStatus Online { get; set; }

        /// <summary>
        /// A list containing ids of the friend lists that the current user is 
        /// a part of. The method for obtaining list names and ids: friends.getLists. 
        /// The field is available only when calling the method that returns a list 
        /// of the user's friends. 
        /// </summary>
        [JsonProperty("lists")]
        public List<string> FriendLists { get; set; }

    }

    public enum VkUserProfileSex
    {
        NotSpecified = 0,
        Male = 2,
        Female = 1
    }

    public enum VkUserOnlineStatus
    {
        Offline = 0,
        Online = 1
    }
}
