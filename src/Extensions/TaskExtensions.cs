using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vysn.Commons.Extensions {
    /// <summary>
    /// 
    /// </summary>
    public static class TaskExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using (cancellationToken.Register(obj => { }, tcs)) {
                var resultTask = await Task.WhenAny(task, tcs.Task);
                if (resultTask == tcs.Task) {
                    throw new TaskCanceledException("Task was canceled.");
                }

                return await task;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public static async Task<T> CancelAfter<T>(this Task<T> task, TimeSpan timeout) {
            using var cts = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, cts.Token);
            var resultTask = Task.WhenAny(task, delayTask);

            if (resultTask == delayTask) {
                throw new TaskCanceledException("Task was canceled.");
            }

            cts.Cancel(false);
            return await task;
        }
    }
}