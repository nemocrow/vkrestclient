using System;
using System.Threading.Tasks;
using System.Windows;
using VkApi.Base;

namespace VkApi
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Same as ContinueWithResult, but the continuation is wrapped in a
        /// Deployment.Current.Dispatcher.BeginInvoke.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public static Task ContinueWithResultDispatched<T>(this Task<T> task, Action<T> continuation)
        {
            return task.ContinueWith(t => Deployment.Current.Dispatcher.BeginInvoke(() => continuation(t.Result)));
        }

        /// <summary>
        /// Same as Task.ContinueWith, but passes an enclosed task.Result to the
        /// continuation, instead of a Task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public static Task ContinueWithResult<T>(this Task<T> task, Action<T> continuation)
        {
            return task.ContinueWith(t => continuation(t.Result));
        }
    }

}
