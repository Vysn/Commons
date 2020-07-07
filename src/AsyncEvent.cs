using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vysn.Commons {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public sealed class AsyncEvent<TEventArgs> where TEventArgs : struct {
        private readonly HashSet<Func<TEventArgs, Task>> _invocations;
        private readonly object _lockObject;

        /// <summary>
        /// 
        /// </summary>
        public AsyncEvent() {
            _invocations = new HashSet<Func<TEventArgs, Task>>();
            _lockObject = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncEvent"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncEvent<TEventArgs> operator +(AsyncEvent<TEventArgs> asyncEvent,
                                                        Func<TEventArgs, Task> callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            asyncEvent ??= new AsyncEvent<TEventArgs>();
            lock (asyncEvent._lockObject) {
                asyncEvent._invocations.Add(callback);
            }

            return asyncEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncEvent"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncEvent<TEventArgs> operator -(AsyncEvent<TEventArgs> asyncEvent,
                                                        Func<TEventArgs, Task> callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            if (asyncEvent == null) {
                throw new ArgumentNullException(nameof(asyncEvent));
            }

            lock (asyncEvent._lockObject) {
                asyncEvent._invocations.Remove(callback);
            }

            return asyncEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async Task DispatchAsync(TEventArgs eventArgs) {
            HashSet<Func<TEventArgs, Task>> invocations;
            lock (_lockObject) {
                invocations = _invocations;
            }

            var invokes = invocations.Select(x => x.Invoke(eventArgs));
            await Task.WhenAll(invokes);
        }
    }
}