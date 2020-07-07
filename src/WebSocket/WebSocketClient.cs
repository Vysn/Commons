using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Vysn.Commons.WebSocket.EventArgs;

namespace Vysn.Commons.WebSocket {
    using System.Net.WebSockets;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WebSocketClient : IAsyncDisposable {
        /// <summary>
        /// 
        /// </summary>
        public AsyncEvent<OpenEventArgs> OnOpenAsync;

        /// <summary>
        /// 
        /// </summary>
        public AsyncEvent<MessageEventArgs> OnMessageAsync;

        /// <summary>
        /// 
        /// </summary>
        public AsyncEvent<ErrorEventArgs> OnErrorAsync;

        /// <summary>
        /// 
        /// </summary>
        public AsyncEvent<CloseEventArgs> OnCloseAsync;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri Host { get; }

        private readonly WebSocket _webSocket;
        private readonly ConcurrentQueue<byte[]> _messageQueue;
        private CancellationTokenSource _connectionTokenSource;

        /// <summary>
        /// 
        /// </summary>
        public WebSocketClient(string hostname, int port, bool isSecure = false) {
            if (string.IsNullOrWhiteSpace(hostname)) {
                throw new ArgumentNullException(nameof(hostname), "");
            }

            if (port <= 0) {
                throw new ArgumentOutOfRangeException(nameof(port), "");
            }

            Host = new Uri(isSecure ? $"wss://{hostname}:{port}" : $"ws://{hostname}:{port}");
            _webSocket = new ClientWebSocket();
            _messageQueue = new ConcurrentQueue<byte[]>();

            OnOpenAsync = new AsyncEvent<OpenEventArgs>();
            OnCloseAsync = new AsyncEvent<CloseEventArgs>();
            OnErrorAsync = new AsyncEvent<ErrorEventArgs>();
            OnMessageAsync = new AsyncEvent<MessageEventArgs>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask ConnectAsync() {
            if (_webSocket.State == WebSocketState.Open) {
                throw new InvalidOperationException();
            }

            await (_webSocket as ClientWebSocket).ConnectAsync(Host, _connectionTokenSource.Token)
                .ContinueWith(async task => {
                    await task;
                    IsConnected = true;

                    _connectionTokenSource = new CancellationTokenSource();
                    await Task.WhenAll(OnOpenAsync.DispatchAsync(new OpenEventArgs()), ReceiveAsync(), SendAsync());
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="closeStatus"></param>
        /// <param name="closeReason"></param>
        /// <returns></returns>
        public async ValueTask DisconnectAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
                                               string closeReason = "Normal closure.") {
            if (_webSocket.State != WebSocketState.Open) {
                throw new InvalidOperationException("");
            }

            try {
                await _webSocket.CloseAsync(closeStatus, closeReason, _connectionTokenSource.Token);
            }
            catch (Exception exception) {
                await OnErrorAsync.DispatchAsync(new ErrorEventArgs(exception));
            }
            finally {
                IsConnected = false;
                await OnCloseAsync.DispatchAsync(new CloseEventArgs());
                _connectionTokenSource.Cancel(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bypassQueue"></param>
        /// <param name="serializerOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async ValueTask SendAsync<T>(T data, bool bypassQueue = false,
                                            JsonSerializerOptions serializerOptions = default) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data), "");
            }

            if (_webSocket.State != WebSocketState.Open) {
                throw new InvalidOperationException("");
            }

            try {
                var serializedData = JsonSerializer.SerializeToUtf8Bytes(data, serializerOptions);
                if (bypassQueue) {
                    _messageQueue.Enqueue(serializedData);
                }
                else {
                    await _webSocket.SendAsync(serializedData, WebSocketMessageType.Text,
                        true, _connectionTokenSource.Token);
                }
            }
            catch (Exception exception) {
                await OnErrorAsync.DispatchAsync(new ErrorEventArgs(exception));
            }
        }

        private async Task ReceiveAsync() {
            try {
                WebSocketReceiveResult receiveResult;
                var buffer = new byte[1024];
                do {
                    receiveResult = await _webSocket.ReceiveAsync(buffer, _connectionTokenSource.Token);
                    if (!receiveResult.EndOfMessage) {
                        continue;
                    }

                    switch (receiveResult.MessageType) {
                        case WebSocketMessageType.Text:
                            break;

                        case WebSocketMessageType.Close:
                            await DisconnectAsync();
                            break;
                    }
                } while (_webSocket.State == WebSocketState.Open &&
                         !_connectionTokenSource.IsCancellationRequested);
            }
            catch (Exception exception) {
                if (exception is TaskCanceledException
                    || exception is OperationCanceledException
                    || exception is ObjectDisposedException) {
                    return;
                }

                await OnErrorAsync.DispatchAsync(new ErrorEventArgs(exception));
            }
        }

        private async Task SendAsync() {
            try {
                do {
                    if (!_messageQueue.TryDequeue(out var content)) {
                        await Task.Delay(500);
                    }

                    await _webSocket.SendAsync(content, WebSocketMessageType.Text,
                        true, _connectionTokenSource.Token);
                } while (_webSocket.State == WebSocketState.Open &&
                         !_connectionTokenSource.IsCancellationRequested);
            }
            catch (Exception exception) {
                await OnErrorAsync.DispatchAsync(new ErrorEventArgs(exception));
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync() {
            if (IsConnected) {
                await DisconnectAsync();
            }

            _connectionTokenSource?.Dispose();
            _webSocket.Dispose();
            _messageQueue.Clear();
        }
    }
}