using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    public class VkMessage : ApiData
    {
        /// <summary>
        /// ID сообщения. Не передаётся для пересланных сообщений.
        /// </summary>
        [JsonProperty("mid")]
        public string Id { get; set; }

        /// <summary>
        /// автор сообщения
        /// </summary>
        [JsonProperty("uid", Required = Required.Always)]
        public string UserId { get; set; }

        /// <summary>
        /// дата отправки сообщения
        /// </summary>
        [JsonProperty("date", Required = Required.Always)]
        public string CreationTime { get; set; }

        /// <summary>
        /// статус прочтения сообщения (0 – не прочитано, 1 – прочитано) Не передаётся для пересланных сообщений.
        /// </summary>
        [JsonProperty("read_state")]
        public VkMessageReadState? ReadState { get; set; }

        /// <summary>
        /// тип сообщения (0 – полученное сообщение, 1 – отправленное сообщение). Не передаётся для пересланных сообщений.
        /// </summary>
        [JsonProperty("out")]
        public VkMessageType MessageType { get; set; }

        /// <summary>
        /// заголовок сообщения или беседы
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// текст сообщения
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// массив медиа-вложений (если есть)
        /// </summary>
        [JsonProperty("attachments")]
        public List<VkAttachment> Attachments { get; set; }

        /// <summary>
        /// массив пересланных сообщений (если есть)
        /// </summary>
        [JsonProperty("fwd_messages")]
        public List<VkMessage> ForwardedMessages { get; set; }

        /// <summary>
        /// (только для удалённых сообщений) Удалено ли сообщение
        /// </summary>
        [JsonProperty("deleted")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// (только для групповых бесед) ID беседы
        /// </summary>
        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        /// <summary>
        /// (только для групповых бесед) ID последних участников беседы, разделённых запятыми, но не более 6.
        /// </summary>
        [JsonProperty("chat_active")]
        public string LastActiveUsers { get; set; }

        /// <summary>
        /// (только для групповых бесед) количество участников в беседе
        /// </summary>
        [JsonProperty("users_count")]
        public int? ParticipantsCount { get; set; }

        /// <summary>
        /// (только для групповых бесед) ID создателя беседы
        /// </summary>
        [JsonProperty("admin_id")]
        public string AdminUserId { get; set; }
    }

    public enum VkMessageType
    {
        ReceivedMessage = 0,
        SentMessage = 1
    }

    public enum VkMessageReadState
    {
        Unread = 0,
        Read = 1
    }
}
