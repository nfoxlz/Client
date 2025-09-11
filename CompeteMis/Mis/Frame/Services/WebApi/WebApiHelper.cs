using Compete.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Compete.Mis.Frame.Services.WebApi
{
    internal sealed class WebApiHelper(string baseAddress, string password)
    {
        //private const string dataSignature = "X-Data-Signature";

        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

        private readonly Runtime.Security.SignHelper signHelper = new(password);

        private readonly HttpClient httpClient = new(new HttpClientHandler() { UseCookies = true }) { BaseAddress = new Uri(baseAddress) };

        public bool UseDataSignature { get; set; } = true;

        public WebApiHelper(string baseAddress)
            : this(baseAddress, string.Empty)
        {
            // 仅用于测试。
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        ~WebApiHelper() => httpClient.Dispose();

        private T Execute<T>(Func<HttpClient, Task<T>> func)
        {
            try
            {
                using var result = func(httpClient);
                while (!result.IsCompleted)
                    Threading.IApplicationManager.DefaultManager.DoEvents();
                return result.Result;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                using (var factory = GlobalCommon.CreateLoggerFactory())
                    factory.CreateLogger<WebApiHelper>().LogError(GlobalCommon.LogMessage, exception.ToString());

                throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Message.NetError"), exception);
            }
        }

        public void Post(string requestUri, IDictionary<string, object?>? parameters = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri, UriKind.RelativeOrAbsolute),
                Content = JsonContent.Create(parameters, null, DefaultJsonSerializerOptions)
            };
            //if (UseDataSignature && parameters != null && parameters.Count > 0)    // 禁用数据签名。
            //    request.Headers.Add(dataSignature, signHelper.GenerateSignParameter(parameters));

            httpClient.SendAsync(request);
        }

        public object? Post(Type type, string requestUri, IDictionary<string, object?>? parameters = null, object? defaultValue = default) =>
            Execute(async client =>
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(requestUri, UriKind.RelativeOrAbsolute),
                    Content = JsonContent.Create(parameters, null, DefaultJsonSerializerOptions)
                };
                //if (UseDataSignature && parameters != null && parameters.Count > 0)    // 禁用数据签名。
                //    request.Headers.Add(dataSignature, signHelper.GenerateSignParameter(parameters));

                var response = await client.SendAsync(request);
                try
                {
                    var responseMessage = response.EnsureSuccessStatusCode();
                    if (!responseMessage.IsSuccessStatusCode)
                        throw new WebApiServiceException(responseMessage);

                    if (type.IsPrimitive || typeof(DateTime) == type || typeof(DateTime?) == type || typeof(TimeSpan) == type || typeof(TimeSpan?) == type || typeof(string) == type || typeof(byte[]) == type)
                    {

                        if (typeof(byte[]) == type)
                            return await response.Content.ReadAsByteArrayAsync();

                        var resultString = await response.Content.ReadAsStringAsync();
                        if (resultString == null)
                            return defaultValue;

                        resultString = resultString.Trim();

                        if (resultString.ToLower() == "null" || string.IsNullOrWhiteSpace(resultString))
                            return defaultValue;

                        if (typeof(string) == type)
                            return resultString;

                        if (typeof(DateTime) == type || typeof(DateTime?) == type)
                        {
                            if ("\"0001-01-01T00:00:00Z\"" == resultString)
                                return typeof(DateTime?) == type ? null : new DateTime();
                            resultString = resultString.Replace('t', ' ').Replace("z", string.Empty);
                        }

                        if (!type.IsNumeric() && type != typeof(bool) && type != typeof(bool?) && resultString[0] == '"' && resultString[^1] == '"')
                            resultString = resultString[1..^1];

                        try
                        {
                            return Convert.ChangeType(resultString, type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type.GetGenericArguments()[0] : type);
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(resultString, exception);
                        }
                    }
                    else
                        return await response.Content.ReadFromJsonAsync(type);
                }
                catch (Exception exception)
                {
                    using var factory = GlobalCommon.CreateLoggerFactory();
                    var logger = factory.CreateLogger<Plugins.UIPlugin>();
                    logger.LogError(exception, exception.Message);
                    return defaultValue;
                }
            });
    }
}
