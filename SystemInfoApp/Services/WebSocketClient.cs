using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemInfoApp.Utils;

namespace SystemInfoApp.Services
{
    public class WebSocketClient
    {
        private ClientWebSocket? _webSocket = new ClientWebSocket();
        private readonly string _uri;
        private readonly Action<string> _onMessageReceived;
        private readonly Logger _logger;

        public WebSocketClient(string uri, Action<string> onMessageReceived, Logger logger)
        {
            _uri = uri;
            _onMessageReceived = onMessageReceived;
            _logger = logger;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket!.ConnectAsync(new Uri(_uri), CancellationToken.None);
                _logger.Log("WebSocket подключен.");
                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log($"Ошибка при подключении WebSocket: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
                    _logger.Log("WebSocket отключен.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Ошибка при отключении WebSocket: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _onMessageReceived?.Invoke(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        _logger.Log("WebSocket закрыт сервером.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Ошибка при получении сообщений WebSocket: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправка сообщения на сервер через WebSocket
        /// </summary>
        /// <param name="message">Сообщение для отправки</param>
        public async Task SendMessageAsync(string message)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.Log($"Сообщение отправлено через WebSocket: {message}");
            }
            else
            {
                _logger.Log("WebSocket не подключен. Невозможно отправить сообщение.");
            }
        }
    }
}
