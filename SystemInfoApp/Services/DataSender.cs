using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SystemInfoApp.Models;
using SystemInfoApp.Utils;

namespace SystemInfoApp.Services
{
    public class DataSender
    {
        private readonly HttpClient _client;
        private readonly Logger _logger;

        public DataSender(HttpClient client, Logger logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Аутентификация и получение JWT токена
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        public async Task AuthenticateAsync(string username, string password)
        {
            try
            {
                var authData = new
                {
                    username = username,
                    password = password
                };

                var json = JsonConvert.SerializeObject(authData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("auth/token", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(responseString);
                string accessToken = result?.access_token ?? string.Empty;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                _logger.Log("Аутентификация прошла успешно. Токен получен.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Ошибка при аутентификации: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Отправка диагностической информации
        /// </summary>
        /// <param name="info">Объект с диагностической информацией</param>
        public async Task SendDiagnosticInfoAsync(DiagnosticInfo info)
        {
            try
            {
                var json = JsonConvert.SerializeObject(info);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("diagnostic/", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Log("Диагностическая информация успешно отправлена.");
                }
                else
                {
                    _logger.Log($"Ошибка при отправке диагностической информации: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Исключение при отправке диагностической информации: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправка видеокадра
        /// </summary>
        /// <param name="frameData">Бинарные данные видеокадра</param>
        public async Task SendVideoFrameAsync(byte[] frameData)
        {
            try
            {
                var content = new ByteArrayContent(frameData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await _client.PostAsync("video/", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Log("Видеокадр успешно отправлен.");
                }
                else
                {
                    _logger.Log($"Ошибка при отправке видеокадра: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Исключение при отправке видеокадра: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправка снимка экрана
        /// </summary>
        /// <param name="screenshotData">Бинарные данные снимка экрана</param>
        public async Task SendScreenshotAsync(byte[] screenshotData)
        {
            try
            {
                var content = new ByteArrayContent(screenshotData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await _client.PostAsync("screenshots/", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Log("Снимок экрана успешно отправлен.");
                }
                else
                {
                    _logger.Log($"Ошибка при отправке снимка экрана: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Исключение при отправке снимка экрана: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправка информации о процессах
        /// </summary>
        /// <param name="processes">Список объектов с информацией о процессах</param>
        public async Task SendProcessInfoAsync(List<ProcessInfo> processes)
        {
            try
            {
                var json = JsonConvert.SerializeObject(processes);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("processes/", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Log("Информация о процессах успешно отправлена.");
                }
                else
                {
                    _logger.Log($"Ошибка при отправке информации о процессах: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Исключение при отправке информации о процессах: {ex.Message}");
            }
        }
    }
}
