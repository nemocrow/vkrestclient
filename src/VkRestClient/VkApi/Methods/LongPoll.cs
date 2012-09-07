using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using VkApi.Base;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi.Methods
{
    public enum LongPollUpdateType
    {
        MessageRemoved = 0,
        MessageFlagsUpdated = 1,
        MessageFlagsSet = 2,
        MessageFlagsReset = 3,
        MessageAdded = 4,
        FriendStatusOnline = 8,
        FriendStatusOffline = 9,
        ChatSettingsChanged = 51,
        UserTypingInConversation = 61,
        UserTypingInChat = 62,
        UserPerformedCall = 70,

        UndocumentedUpdateType1 = 101
    }

    public enum LongPollMessageFlags
    {
        //+1	UNREAD	сообщение не прочитано
        Unread = 1,
        //+2	OUTBOX	исходящее сообщение
        Outbox = 2,
        //+4	REPLIED	на сообщение был создан ответ
        Replied = 4,
        //+8	IMPORTANT	помеченное сообщение
        Important = 8,
        //+16	CHAT	сообщение отправлено через чат
        Chat = 16,
        //+32	FRIENDS	сообщение отправлено другом
        Friends = 32,
        //+64	SPAM	сообщение помечено как "Спам"
        Spam = 64,
        //+128	DELЕTЕD	сообщение удалено (в корзине)
        Deleted = 128,
        //+256	FIXED	сообщение проверено пользователем на спам
        Fixed = 256,
        //+512	MEDIA	сообщение содержит медиаконтент
        Media = 512
    }

    public class LongPollUpdate
    {
        public class MessageRemoved : LongPollUpdate
        {
            public string MessageId;
        }

        public class MessageFlagsUpdated : LongPollUpdate
        {
            public string MessageId;
            public LongPollMessageFlags MessageFlags;
        }

        public class MessageFlagsSet : LongPollUpdate
        {
            public string MessageId;
            public LongPollMessageFlags MessageFlags;
            public string UserId;
        }

        public class MessageFlagsReset : LongPollUpdate
        {
            public string MessageId;
            public LongPollMessageFlags MessageFlags;
            public string UserId;
        }

        public class MessageAdded : LongPollUpdate
        {
            public string MessageId;
            public LongPollMessageFlags MessageFlags;
            public string UserId;
            public string FromId;
            public string Timestamp;
            public string Subject;
            public string Text;
            public IEnumerable<VkAttachment> Attachments;
        }

        public class FriendStatusOnline : LongPollUpdate
        {
            public string UserId;
        }

        public class FriendStatusOffline : LongPollUpdate
        {
            public string UserId;
            public bool UserTimedOut;
        }

        public class ChatSettingsChanged : LongPollUpdate
        {
            public string ChatId;
            public bool ChangedByCurrentUser;
        }

        public class UserTypingInConversation : LongPollUpdate
        {
            public string UserId;
        }

        public class UserTypingInChat : LongPollUpdate
        {
            public string UserId;
            public string ChatId;
        }

        public class UserPerformedCall : LongPollUpdate
        {
            public string UserId;
        }

        public LongPollUpdateType EventType;

        public static LongPollUpdate FromRaw(JToken[] parameters)
        {
            var eventType = (LongPollUpdateType)parameters[0].ToObject<int>();

            //Первым параметром каждого события передаётся его код, поддерживаются следующие коды событий: 
            switch (eventType)
            {
                //0,$message_id,0 -- удаление сообщения с указанным local_id
                case LongPollUpdateType.MessageRemoved:
                    return new MessageRemoved
                        {
                            EventType = eventType,
                            MessageId = parameters[1].ToString()
                        };

                //1,$message_id,$flags -- замена флагов сообщения (FLAGS:=$flags)
                case LongPollUpdateType.MessageFlagsUpdated:
                    return new MessageFlagsUpdated
                    {
                        EventType = eventType,
                        MessageId = parameters[1].ToString(),
                        MessageFlags = (LongPollMessageFlags)parameters[2].ToObject<int>()
                    };

                //2,$message_id,$mask[,$user_id] -- установка флагов сообщения (FLAGS|=$mask)
                case LongPollUpdateType.MessageFlagsSet:
                    return new MessageFlagsSet
                    {
                        EventType = eventType,
                        MessageId = parameters[1].ToString(),
                        MessageFlags = (LongPollMessageFlags)parameters[2].ToObject<int>(),
                        UserId = parameters.Length == 4 ? parameters[3].ToString() : null
                    };

                //3,$message_id,$mask[,$user_id] -- сброс флагов сообщения (FLAGS&=~$mask)
                case LongPollUpdateType.MessageFlagsReset:
                    return new MessageFlagsReset
                    {
                        EventType = eventType,
                        MessageId = parameters[1].ToString(),
                        MessageFlags = (LongPollMessageFlags)parameters[2].ToObject<int>(),
                        UserId = parameters.Length == 4 ? parameters[3].ToString() : null
                    };

                //4,$message_id,$flags,$from_id,$timestamp,$subject,$text,$attachments -- добавление нового сообщения
                case LongPollUpdateType.MessageAdded:
                    return new MessageAdded
                    {
                        EventType = eventType,
                        MessageId = parameters[1].ToString(),
                        MessageFlags = (LongPollMessageFlags)parameters[2].ToObject<int>(),
                        FromId = parameters[3].ToString(),
                        Timestamp = parameters[4].ToString(),
                        Subject = parameters[5].ToString(),
                        Text = parameters[6].ToString(),
                        Attachments = parameters.Length >= 8 ? 
                            CreateAttachmentArray(parameters[7].ToObject<JObject>()) : 
                            null
                    };

                //8,-$user_id,0 -- друг $user_id стал онлайн
                case LongPollUpdateType.FriendStatusOnline:
                    return new FriendStatusOnline
                        {
                            EventType = eventType,
                            UserId = parameters[1].ToString().Substring(1)
                        };

                //9,-$user_id,$flags -- друг $user_id стал оффлайн ($flags равен 0, если пользователь покинул сайт (например, нажал выход) и 1, если оффлайн по таймауту (например, статус away))
                case LongPollUpdateType.FriendStatusOffline:
                    return new FriendStatusOffline
                    {
                        EventType = eventType,
                        UserId = parameters[1].ToString().Substring(1),
                        UserTimedOut = parameters[2].ToString() == "1"
                    };

                //51,$chat_id,$self -- один из параметров (состав, тема) беседы $chat_id были изменены. $self - были ли изменения вызываны самим пользователем
                case LongPollUpdateType.ChatSettingsChanged:
                    return new ChatSettingsChanged
                    {
                        EventType = eventType,
                        ChatId = parameters[1].ToString(),
                        ChangedByCurrentUser = !string.IsNullOrEmpty(parameters[2].ToString())
                    };

                //61,$user_id,$flags -- пользователь $user_id начал набирать текст в диалоге. событие должно приходить раз в ~5 секунд при постоянном наборе текста. $flags = 1
                case LongPollUpdateType.UserTypingInConversation:
                    return new UserTypingInConversation
                    {
                        EventType = eventType,
                        UserId = parameters[1].ToString(),
                    };

                //62,$user_id,$chat_id -- пользователь $user_id начал набирать текст в беседе $chat_id.
                case LongPollUpdateType.UserTypingInChat:
                    return new UserTypingInChat
                    {
                        EventType = eventType,
                        UserId = parameters[1].ToString(),
                        ChatId = parameters[2].ToString()
                    };

                //70,$user_id,$call_id -- пользователь $user_id совершил звонок имеющий идентификатор $call_id, дополнительную информацию о звонке можно получить используя метод voip.getCallInfo.
                case LongPollUpdateType.UserPerformedCall:
                    return new UserPerformedCall
                    {
                        EventType = eventType,
                        UserId = parameters[1].ToString(),
                        // fuck call id
                    };

                //101,-1,-1 -- WTF??
                case LongPollUpdateType.UndocumentedUpdateType1:
                    return null;
                
                default:
                    throw new InvalidOperationException(string.Format("Unknown long poll event type: {0}", eventType));
            }
        }

        /// <summary>
        /// Creates VkAttachments from longpoll JSON data.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static IEnumerable<VkAttachment> CreateAttachmentArray(JObject json)
        {
            //Прикрепления
            //Если указан параметр mode, то вместе с текстом и заголовком сообщения, может быть передан JSON объект содержащий прикрепления, а также другие полезные поля. Ниже приведено описание полей этого объекта. 
            //
            //Поле	Значение	Описание
            //attach{$i}_type	photo, video, audio, doc	тип $i-го прикрепления, где i > 0
            //attach{$i}	{$owner_id}_{$item_id}	идентификатор $i-го прикрепления, где i > 0
            //fwd	{$user_id}_{$msg_id},{$user_id}_{$msg2_id},...	идентификаторы прикреплённых сообщений
            //from	{$user_id}	идентификатор реального автора сообщения если сообщение получено из беседы

            var s = json.ToObject<Dictionary<string, string>>();

            return
                from kvp in s
                where kvp.Key.EndsWith("_type")
                let idx = kvp.Key.Substring("attach".Length, kvp.Key.Length - "attach".Length - "_type".Length)
                let type = kvp.Value
                let attachData = s["attach" + idx]
                let ownerId = attachData.TakeWhile(c => c != '_').ToString()
                let mediaId = attachData.Substring(ownerId.Length + 1)
                select new VkAttachment()
                    {
                        RawJson = json,
                        Type = type,
                        Audio = type == "audio" ? new VkAudioAttachment {Id = mediaId, OwnerId = ownerId} : null,
                        Photo = type == "photo" ? new VkPhotoAttachment {Id = mediaId, OwnerId = ownerId} : null,
                        Video = type == "video" ? new VkVideoAttachment {Id = mediaId, OwnerId = ownerId} : null,
                        Document = type == "document" ? new VkDocumentAttachment {Id = mediaId, OwnerId = ownerId} : null,
                    };
        }
    }

    public class LongPollExpiredException : ApiException
    {
        public LongPollExpiredException()
            : base("Long poll server credentials have expired.") {}
    }

    public static class LongPoll
    {
        public class LongPollReply : ApiData
        {
            public string Timestamp { get; set; }

            public IEnumerable<LongPollUpdate> Updates { get; set; }
        }

        /// <summary>
        /// Возвращает объект, который содержит поля key, server, ts. 
        /// Используя эти данные, Вы можете подключиться к серверу быстрых сообщений 
        /// для мгновенного получения приходящих сообщений и других событий. 
        /// 
        /// Пример ответа в формате JSON
        /// {"response":{"key":"4521b167a954cad338c28ea29dc4985fc81c36af","server":"im0.vkontakte.ru\/im748","ts":1063177508}}
        /// </summary>
        public class GetServerReply : ApiData
        {
            /// <summary>
            /// секретный ключ сессии
            /// </summary>
            [JsonProperty("key")]
            public string Key { get; set; }

            /// <summary>
            /// адрес сервера к которому нужно отправлять запрос
            /// </summary>
            [JsonProperty("server")]
            public string ServerUrl { get; set; }

            [JsonProperty("ts")]
            public string Timestamp { get; set; }
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=messages.getLongPollServer
        /// 
        /// Возвращает данные, необходимые для подключения к Long Poll серверу. 
        /// Long Poll подключение позволит Вам моментально узнавать о приходе новых 
        /// сообщений и других событий. 
        /// 
        /// Этот метод не имеет параметров
        /// </summary>
        public static ISessionApiRequest<GetServerReply> GetServer()
        {
            return new SessionApiRequest<GetServerReply>(
                "messages.getLongPollServer",
                Method.GET,
                ParameterDict.Empty,
                null,
                ApiRequestPriority.LongPoll);
        }

        /// <summary>
        /// http://vk.com/pages?oid=-1&p=%D0%9F%D0%BE%D0%B4%D0%BA%D0%BB%D1%8E%D1%87%D0%B5%D0%BD%D0%B8%D0%B5_%D0%BA_LongPoll_%D1%81%D0%B5%D1%80%D0%B2%D0%B5%D1%80%D1%83
        /// 
        /// Принцип работы Long Poll соединения заключается в том, что сервер, получив 
        /// запрос, удерживает его до тех пор, пока не произойдёт событие или не истечёт 
        /// время, указанное в параметре wait (Так как некоторые прокси серверы обрывают 
        /// соединение по истечении 30 секунд - мы советуем указывать wait=25). 
        /// 
        /// Как только клиент получает ответ, он может послать новый запрос, указав в 
        /// параметре ts новое значение, полученное в ответе, для получения новых событий. 
        /// 
        /// Long Poll поддерживает передачу сообщений только в формате JSON. 
        /// </summary>
        /// <param name="serverInfo">секретный ключ сессии, адрес сервера к которому нужно отправлять запрос</param>
        /// <param name="timestamp">номер последнего события, начиная с которого Вы хотите получать данные</param>
        /// <param name="waitTimeSeconds">получив запрос, удерживает его до тех пор, пока не произойдёт событие или не истечёт время, указанное в параметре wait</param>
        public static IStandaloneApiRequest<LongPollReply> Poll(
            GetServerReply serverInfo,
            string timestamp = null,
            int waitTimeSeconds = 25)
        {
            return new StandaloneApiRequest<LongPollReply>(
                new RestClient("http://" + serverInfo.ServerUrl)
                    {Timeout = (waitTimeSeconds + 5) * 1000},
                ApiRequest<LongPollReply>.CreateRestRequest(
                    Method.GET, "",
                    new ParameterDict
                        {
                            {"act", "a_check"},
                            {"ts", timestamp ?? serverInfo.Timestamp},
                            {"key", serverInfo.Key},
                            {"wait", waitTimeSeconds},
                            // mode - параметр, определяющий наличие поля прикреплений в получаемых данных. Значения: 2 - получать прикрепления, 0 - не получать.
                            {"mode", "2"}
                        }),
                json =>
                {
                    // При каждом ответе сервер будет возвращать новый ts, пример ответа сервера: 
                    // { ts: 934518069, updates: [] }

                    //Время действия ключа для подключения к LongPoll серверу может истечь через некоторое время, сервер вернёт параметр failed: 
                    //{ failed: 2 }
                    //в таком случае требуется перезапросить его, используя метод messages.getLongPollServer. 

                    //Пример ответа с событием: 
                    //{ ts: 196851352, updates: [ [ 9, -835293, 1 ], [ 9, -23498, 1 ] ] }
                    //
                    //Это значит, что пользователи 835293 и 23498 покинули сайт. 
                    //
                    //Пример ответа с сообщением: 
                    //{ ts: 196851367, updates: [ [ 4, 16929, 1, 85635407, 1280307577, ' ... ', 'hello', {'attach1_type': 'photo', 'attach1': '123_456'} ] ] }

                    if (json["failed"] != null)
                        throw new LongPollExpiredException();

                    var ts = json["ts"];
                    var updates = json["updates"];

                    if (ts == null || updates == null)
                        throw new UnknownJsonFormatException(json);

                    return new LongPollReply
                        {
                            RawJson = json,
                            Timestamp = ts.ToString(),
                            Updates = from jsonUpdate in updates
                                      let lpevent = LongPollUpdate.FromRaw(jsonUpdate.ToObject<JToken[]>())
                                      where lpevent != null
                                      select lpevent
                        };
                });
        }
    }
}
