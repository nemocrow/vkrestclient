using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using VkApi.Base;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi.Methods
{
    public static class Messages
    {
        public class GetMessagesReply
        {
            public int TotalCount { get; set; }
            public IEnumerable<VkMessage> Messages { get; set; }
        }

        private static ISessionApiRequest<GetMessagesReply> GenericGetMessages(
            string methodName, Method httpMethod, ParameterDict parameters)
        {
            // {"response":[4680,{"body":"thank you...<br>yeah...it'll pass....","title":"Re:  ...",
            // "date":1268213453,"uid":28672529,"mid":10836,"read_state":0},{"body":"nothing for)",
            // "title":"Re:  ...","date":1268213443,"uid":58416643,"mid":10835,"read_state":0}]}

            return new SessionApiRequest<GetMessagesReply>(
                methodName,
                httpMethod,
                parameters,
                json => new GetMessagesReply
                    {
                        TotalCount = json.First().ToObject<int>(),
                        Messages = from message in json.Skip(1) select message.ToObject<VkMessage>()
                    }
                );
        }

        /// <summary>
        /// messages.get
        /// Returns a list of all received or sent private messages of the current user. 
        /// </summary>
        /// <param name="count">number of messages needed to obtain (no more than 100).</param>
        /// <param name="offset">offset required for selecting a certain subcollection of messages.</param>
        /// <param name="timeOffsetSeconds">Maximum time elapsed from the moment of sending the message up until the current time in seconds. 0 if you want to receive message without any time limitations.</param>
        /// <param name="previewLength">number of words that need to be cut. Enter 0 if you do not want to cut the message. (by default – 90).</param>
        /// <param name="onlyOutgoing">if this parameter equals 1, the server will return sent messages.</param>
        /// <param name="filterFlags">filter of returning messages: 1 - only unread; 2 - not from chat; 4 - only from friends. If set to 4, then 1 and 2 are not taken into account.</param>
        public static ISessionApiRequest<GetMessagesReply> Get(
            int count,
            int? offset = null,
            int? timeOffsetSeconds = null,
            int? previewLength = null,
            int? onlyOutgoing = null,
            int? filterFlags = null)
        {
            return GenericGetMessages(
                "messages.get",
                Method.GET,
                new ParameterDict
                    {
                        {"count", count},
                        {"offset", offset},
                        {"time_offset", timeOffsetSeconds},
                        {"out", onlyOutgoing},
                        {"filters", filterFlags},
                        {"preview_length", previewLength}
                    }
                );
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.getById
        /// 
        /// messages.getById 
        /// Возвращает сообщения по их ID. 
        /// </summary>
        /// <param name="messageIds">ID сообщений, которые необходимо вернуть, разделенные запятыми (не более 100).</param>
        /// <param name="previewLength">Количество слов, по которому нужно обрезать сообщение. Укажите 0, если Вы не хотите обрезать сообщение. (по умолчанию сообщения не обрезаются).</param>
        /// <returns>Возвращает массив личных сообщений</returns>
        public static ISessionApiRequest<GetMessagesReply> GetByIds(
            IEnumerable<string> messageIds,
            int? previewLength = null)
        {
            return GenericGetMessages(
                "messages.getById",
                Method.GET,
                new ParameterDict
                    {
                        {"mids", string.Join(",", messageIds)},
                        {"preview_length", previewLength}
                    }
                );            
        }

        public static ISessionApiRequest<GetMessagesReply> GetById(
            string messageId,
            int? previewLength = null)
        {
            return GetByIds(new[] {messageId}, previewLength);
        }

        /// <summary>
        /// messages.getDialogs
        /// Возвращает список диалогов текущего пользователя. 
        /// </summary>
        public static ISessionApiRequest<GetMessagesReply> GetDialogs(
            string userId = null,
            string chatId = null,
            int? count = null,
            int? offset = null,
            int? previewLength = null)
        {
            return GenericGetMessages(
                "messages.getDialogs",
                Method.GET,
                new ParameterDict
                    {
                        {"count", count},
                        {"offset", offset},
                        {"uid", userId},
                        {"chat_id", chatId},
                        {"preview_length", previewLength}
                    }
                );
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.getHistory
        /// 
        /// messages.getHistory 
        /// Возвращает историю сообщений для указанного пользователя или групповой беседы. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="chatId"></param>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества сообщений.</param>
        /// <param name="count"></param>
        /// <param name="startMessageId">идентификатор сообщения, начиная с которго необходимо получить последующие сообщения.</param>
        /// <param name="inChronologicalOrder">1 – возвращать сообщения в хронологическом порядке. 0 – возвращать сообщения в обратном хронологическом порядке (по умолчанию)</param>
        /// <returns></returns>
        public static ISessionApiRequest<GetMessagesReply> GetHistory(
            string userId = null,
            string chatId = null,
            int? offset = null,
            int? count = null,
            string startMessageId = null,
            bool? inChronologicalOrder = null)
        {
            return GenericGetMessages(
                "messages.getHistory",
                Method.GET,
                new ParameterDict
                    {
                        {"count", count},
                        {"offset", offset},
                        {"uid", userId},
                        {"chat_id", chatId},
                        {"start_mid", startMessageId},
                        // @TODO how does bool serialize to parameter?
                        {"rev", inChronologicalOrder}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.send
        /// 
        /// Посылает сообщение. 
        /// 
        /// </summary>
        /// <param name="targetUserId">uid ID пользователя (по умолчанию - текущий пользователь).</param>
        /// <param name="targetChatId">chat_id ID беседы, к которой будет относиться сообщение</param>
        /// <param name="messageTitle">title заголовок сообщения.</param>
        /// <param name="messageText">message текст личного cообщения (является обязательным, если не задан параметр attachment)</param>
        /// <param name="isAChatMessage">type 0 - обычное сообщение, 1 - сообщение из чата. (по умолчанию 0)</param>
        /// <param name="attachments">attachment медиа-приложения к личному сообщению, перечисленные через запятую. Каждое прикрепление представлено в формате:</param>
        /// <param name="forwardedMessageIds">forward_messages идентификаторы пересылаемых сообщений, перечисленные через запятую. Перечисленные сообщения отправителя будут отображаться в теле письма у получателя.</param>
        /// <param name="latitude">lat latitude, широта при добавлении метоположения.</param>
        /// <param name="longitude">long longitude, долгота при добавлении метоположения.</param>
        /// <returns>Возвращает ID сообщения или код ошибки.</returns>
        public static ISessionApiRequest<string> Send(
            string targetUserId,
            string targetChatId,
            string messageText = null,
            string messageTitle = null,
            int? isAChatMessage = null,
            IEnumerable<VkAttachment> attachments = null,
            IEnumerable<string> forwardedMessageIds = null,
            string latitude = null,
            string longitude = null)
        {
            return new SessionApiRequest<string>(
                "messages.send",
                Method.POST,
                new ParameterDict
                    {
                        {"uid", targetUserId},
                        {"chat_id", targetChatId},
                        {"message", messageText},
                        {"title", messageTitle},

                        //медиа-приложения к личному сообщению, перечисленные через запятую. Каждое прикрепление представлено в формате:
                        //<type><owner_id>_<media_id>
                        //
                        //<type> - тип медиа-приложения:
                        //photo - фотография
                        //video - видеозапись
                        //audio - аудиозапись
                        //doc - документ
                        //wall - запись на стене
                        //
                        //<owner_id> - идентификатор владельца медиа-приложения
                        //<media_id> - идентификатор медиа-приложения.
                        //
                        //Например:
                        //photo100172_166443618
                        {"attachment", 
                            attachments == null ? null : 
                            string.Join(",", attachments.Select(a => string.Format("{0}{1}_{2}", a.Type, a.OwnerId, a.MediaId)))},

                        {"type", isAChatMessage},
                        {"lat", latitude},
                        {"long", longitude},
                        {"guid", string.Format("huypizda{0}", DateTime.Now.ToFileTimeUtc())}
                    });
        }

        public class DeleteReply : ApiData
        {
            public bool AllDeleted
            {
                get { return this.DeleteResults.Values.All(d => d); }
            }

            public Dictionary<string, bool> DeleteResults { get; set; }
        }
        
        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.delete
        /// messages.delete
        /// Удаляет сообщение. 
        /// </summary>
        /// <param name="messageIds">Список идентификаторов сообщений, разделённых через запятую.</param>
        /// <returns>Возвращает 1 в случае успешного удаления или код ошибки.</returns>
        public static ISessionApiRequest<DeleteReply> Delete(
            IEnumerable<string> messageIds)
        {
            return new SessionApiRequest<DeleteReply>(
                "messages.delete",
                Method.POST,
                new ParameterDict
                    {
                        {"mids", string.Join(",", messageIds)}
                    },
                json => new DeleteReply
                    {
                        DeleteResults = json.ToObject<Dictionary<string, bool>>()
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.restore 
        /// 
        /// messages.restore 
        /// Восстанавливает удаленное сообщение. 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns>Возвращает 1 или код ошибки. </returns>
        public static ISessionApiRequest<string> Restore(
            string messageId)
        {
            return new SessionApiRequest<string>(
                "messages.restore",
                Method.POST,
                new ParameterDict {{"mid", messageId}});
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.createChat
        /// 
        /// messages.createChat
        /// Создаёт беседу с несколькими участниками. 
        /// </summary>
        /// <param name="userIds">список идентификаторов друзей текущего пользователя с которыми необходимо создать беседу.</param>
        /// <param name="title">название мультидиалога.</param>
        /// <returns>Возвращает идентификатор созданного чата в случае успешного выполнения данного метода. </returns>
        public static ISessionApiRequest<string> CreateChat(
            IEnumerable<string> userIds,
            string title = null)
        {
            return new SessionApiRequest<string>(
                "messages.createChat",
                Method.POST,
                new ParameterDict
                    {
                        {"title", title},
                        //{"uids", string.Join(",", userIds)}
                        // @TODO how do I pass this one?
                        {"uids", userIds}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.editChat
        /// 
        /// messages.editChat Изменяет название беседы. 
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="title">название беседы.</param>
        /// <returns>Возвращает 1 в случае успешного выполнения данного метода.</returns>
        public static ISessionApiRequest<string> EditChat(
            string chatId,
            string title)
        {
            return new SessionApiRequest<string>(
                "messages.editChat",
                Method.POST,
                new ParameterDict
                    {
                        {"title", title},
                        {"chat_id", chatId}
                    });
        }

        /// <summary>
        /// See GetChatUsers.
        /// </summary>
        public class VkChatUserProfile : VkUserProfile
        {
            [JsonProperty("invited_by")]
            public string InvitedByUserId { get; set; }
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.getChatUsers
        /// 
        /// messages.getChatUsers
        /// Получить список пользователей мультидиалога по его id. 
        /// </summary>
        /// <param name="chatId">ID беседы, пользователей которой необходимо получить</param>
        /// <returns>При использовании параметра fields возвращает информацию о друзьях пользователя в виде набора массива объектов, каждый из которых может иметь поля</returns>
        public static ISessionApiRequest<List<VkChatUserProfile>> GetChatUsers(
            string chatId)

        {
            return new SessionApiRequest<List<VkChatUserProfile>>(
                "messages.getChatUsers",
                Method.GET,
                new ParameterDict
                    {
                        {"chat_id", chatId},
                        // @TODO wtf to do with this?
                        {"fields", "uid,first_name,last_name,nickname,photo"}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.addChatUser
        /// 
        /// messages.addChatUser
        /// Добавляет в мультидиалог нового пользователя. 
        /// </summary>
        /// <param name="chatId">ID беседы, в которую необходимо добавить пользователя</param>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Возвращает 1 или код ошибки. </returns>
        public static ISessionApiRequest<string> AddChatUser(
            string chatId,
            string userId)
        {
            return new SessionApiRequest<string>(
                "messages.addChatUser",
                Method.POST,
                new ParameterDict
                    {
                        {"chat_id", chatId},
                        {"uid", userId}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.removeChatUser
        /// 
        /// messages.removeChatUser
        /// Исключает из мультидиалога пользователя, если текущий пользователь был создателем беседы либо пригласил исключаемого пользователя. Также может быть использован для выхода текущего пользователя из беседы, в которой он состоит. 
        /// </summary>
        /// <param name="chatId">ID беседы, в которую необходимо добавить пользователя</param>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Возвращает 1 или код ошибки. </returns>
        public static ISessionApiRequest<string> RemoveChatUser(
            string chatId,
            string userId)
        {
            return new SessionApiRequest<string>(
                "messages.removeChatUser",
                Method.POST,
                new ParameterDict
                    {
                        {"chat_id", chatId},
                        {"uid", userId}
                    });
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.getChat
        /// 
        /// messages.getChat
        /// Получает информацию о беседе. 
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <returns>Возвращает объект, содержащий поля type, chat_id, title, admin_id, users в случае успешного выполнения данного метода. </returns>
        public static ISessionApiRequest<VkChat> GetChat(
            string chatId)
        {
            // {"response":{"type":"chat","chat_id":149,"title":"test","admin_id":"66748","users":[66748,85635407,4766,-2000000001]}}

            return new SessionApiRequest<VkChat>(
                "messages.getChat",
                Method.GET,
                new ParameterDict
                    { {"chat_id", chatId} });
        }

    }
}
