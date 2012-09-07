using VkApi.Methods.Base;

namespace VkApi.Base
{
    /// <summary>
    /// Called when an API request returns a response.
    /// </summary>
    /// <typeparam name="T">Type of response data.</typeparam>
    /// <param name="request">API request instance.</param>
    /// <param name="response">API response value.</param>
    public delegate void ApiResponseCallback<T>(ApiResponse<T> response)
        where T : class;
}
