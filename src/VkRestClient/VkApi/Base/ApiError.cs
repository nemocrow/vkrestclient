using Newtonsoft.Json;

namespace VkApi.Base
{
    /// <summary>
    /// Regular error:
    /// 
    /// {"error":
    ///     {"error_code":5,
    ///     "error_msg":"User authorization failed: user revoke access for this token.",
    ///     "request_params":[
    ///         {"key":"oauth","value":"1"},
    ///         {"key":"method","value":"getProfiles"},
    ///         {"key":"uid","value":"66748"},
    ///         {"key":"access_token","value":"533bacf01e11f55b536a565b57531ac114461ae8736d6506a3"}]}}
    /// 
    /// Other errors:
    /// {"error":"invalid_client","error_description":"client_secret is incorrect"}
    /// {"error":"invalid_request","error_description":"Too many invalid requests per 15 seconds"}
    /// ...
    /// </summary>
    public class ApiError : ApiData
    {
        /// <summary>
        /// Valid for regular errors.
        /// </summary>
        [JsonProperty("error_code")]
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Valid for irregular errors.
        /// </summary>
        [JsonProperty("error")]
        public string ExtendedError { get; set; }
    }

    public enum ErrorCode
    {
        // if there was no "error_code" field in JSON response, this will
        // be the default value.
        NoErrorCode = 0,
        //1 Unknown error occured.
        UnknownError = 1,
        //2 Application is disabled. Enable your application or use test mode.
        ApplicationIsDisabled = 2,
        //3 Unknown method passed.
        UnknownMethodPassed = 3,
        //4 Incorrect signature.
        IncorrectSignature = 4,
        //5 User authorization failed.
        UserAuthorizationFailed = 5,
        //6 Too many requests per second.
        TooManyRequests = 6,
        //7 Permission to perform this action is denied by user.
        PermissionDenied = 7,
        //9 Flood control: message with same guid already sent.
        FloodControl = 9,
        //10 Internal server error.
        InternalServerError = 10,
        //12 Compilation error.
        CompilationError = 12,
        //13 Runtime error.
        RuntimeError = 13,
        //14 Captcha is needed
        CaptchaRequired = 14,
        //15 Access denied: you have no messages from this user, 15 Access denied: no access to this chat.
        AccessDenied = 15,
        //20 Permission to perform this action is denied for non-standalone applications.
        PermissionDeniedForNonStandalone = 20,
        //100 One of the parameters specified was missing or invalid.
        InvalidParameters = 100,
        //113 Invalid user ids.
        InvalidUserIds = 113,

        //1003 User already invited: message already sended, you can resend message in 300 seconds
        UserAlreadyInvited = 1003,
        //1004 This phone used by another user
        PhoneUsedByAnotherUser = 1004,
        //1112 Processing.. Try later
        ProcessingTryLater = 1112,
    }

}
