using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using SystemInfoApp.Utils;

namespace SystemInfoApp.Services
{
    public static class HttpClientFactory
    {
        public static HttpClient CreateHttpClient(string baseAddress, Logger logger)
        {
            var handler = new HttpClientHandler();

            // Для разработки: игнорировать ошибки проверки сертификата
            // В продакшен среде используйте доверенные сертификаты и удалите следующую строку
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            logger.Log("HttpClient создан с базовым адресом: " + baseAddress);

            return client;
        }
    }
}
