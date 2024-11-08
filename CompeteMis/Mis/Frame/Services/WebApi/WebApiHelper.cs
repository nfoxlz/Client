using Compete.Extensions;
using Compete.Mis.Plugins;
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
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

        private readonly Runtime.Security.SignHelper signHelper = new(password);

        //private readonly HttpClientHandler handler = new() { UseCookies = true };

        //private CookieCollection? cookieCollection = null;

        private readonly HttpClient httpClient = new(new HttpClientHandler() { UseCookies = true }) { BaseAddress = new Uri(baseAddress) };

        ~WebApiHelper() => httpClient.Dispose();

        //private HttpClient GetHttpClient()
        //{
        //    var client = new HttpClient(new HttpClientHandler() { UseCookies = true }) { BaseAddress = new Uri(baseAddress) };
        //    //var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    return client;
        //}

        private T Execute<T>(Func<HttpClient, Task<T>> func)
        {
            try
            {
                //using var handler = new HttpClientHandler() { UseCookies = true };
                //if (cookieCollection is not null)
                //    foreach (Cookie cookie in cookieCollection)
                //        handler.CookieContainer.Add(cookie);
                //using var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
                //using var result = func(client);
                //while (!result.IsCompleted)
                //    Threading.IApplicationManager.DefaultManager.DoEvents();
                //cookieCollection = handler.CookieContainer.GetAllCookies();
                //return result.Result;

                //using var client = GetHttpClient();
                //var result = func(client);
                //while (!result.IsCompleted)
                //    Threading.IApplicationManager.DefaultManager.DoEvents();
                //return result.Result;

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

        public void Post(string requestUri, IDictionary<string, object?>? parameters = null) => httpClient.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters), DefaultJsonSerializerOptions);
        //{
        //    using var handler = new HttpClientHandler() { UseCookies = true };
        //    if (cookieCollection is not null)
        //        foreach (Cookie cookie in cookieCollection)
        //            handler.CookieContainer.Add(cookie);
        //    using var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
        //    client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters), DefaultJsonSerializerOptions);
        //    cookieCollection = handler.CookieContainer.GetAllCookies();

        //    //using var client = GetHttpClient();
        //    //client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters), DefaultJsonSerializerOptions);
        //}

        public object? Post(Type type, string requestUri, IDictionary<string, object?>? parameters = null, object? defaultValue = default) =>
            Execute(async client =>
            {
                var response = await client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters), DefaultJsonSerializerOptions);
                try
                {
                    var responseMessage = response.EnsureSuccessStatusCode();
                    //return responseMessage.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync(type) : throw new WebApiServiceException(responseMessage);
                    if (!responseMessage.IsSuccessStatusCode)
                        throw new WebApiServiceException(responseMessage);

                    //var result = await response.Content.ReadFromJsonAsync(type);
                    //if (result == null)
                    //{
                    //    var resultString = await response.Content.ReadAsStringAsync();
                    //    try
                    //    {
                    //        return Convert.ChangeType(resultString, type);
                    //    }
                    //    catch (Exception exception)
                    //    {
                    //        throw new Exception(resultString, exception);
                    //    }
                    //}

                    //if (type.IsAssignableFrom(result.GetType()))
                    //    return result;

                    //return Convert.ChangeType(result, type);
                    if (type.IsPrimitive || type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(TimeSpan) || type == typeof(TimeSpan?))
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        if (resultString == null)
                            return defaultValue;

                        resultString = resultString.Trim();

                        if (resultString.ToLower() == "null" || string.IsNullOrWhiteSpace(resultString))
                            return defaultValue;
                        //#if !JAVA_LANGUAGE
                        //                        if (type == typeof(bool) || type == typeof(bool?))
                        //                            return resultString != "0";
                        //#endif

                        if (type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            if ("\"0001-01-01T00:00:00Z\"" == resultString)
                                return type == typeof(DateTime?) ? null : new DateTime();
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
                    var logger = factory.CreateLogger<UIPlugin>();
                    logger.LogError(exception, exception.Message);
                    return defaultValue;
                }
            });
        //{
        //    var response = await GetHttpClient().PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));
        //    //return response.Content.ReadFromJsonAsync(type);
        //    return response.EnsureSuccessStatusCode().IsSuccessStatusCode ? response.Content.ReadFromJsonAsync(type) : defaultValue;
        //}


        //public T? Post<T>(string requestUri, IDictionary<string, object>? parameters = null, T? defaultValue = default) =>
        //    Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));
        //        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        //            return await response.Content.ReadFromJsonAsync<T>();
        //        else
        //            return defaultValue;
        //    });

        //public T? Post<T>(string requestUri, IDictionary<string, object> parameters, Exception exception) =>
        //    Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));
        //        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        //            return await response.Content.ReadFromJsonAsync<T>();
        //        else
        //            throw exception;
        //    });

        //public Task<T?> PostAsync<T>(string requestUri, IDictionary<string, object> value)
        //{
        //    return Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, value);
        //        response.EnsureSuccessStatusCode();
        //        return response.Content.ReadFromJsonAsync<T>();
        //    });
        //}

        //public void Post(string requestUri, object? parameters = null) => GetHttpClient().PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));

        //public T? Post<T>(string requestUri, object? parameters = null, T? defaultValue = default) =>
        //    Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));
        //        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        //            return await response.Content.ReadFromJsonAsync<T>();
        //        else
        //            return defaultValue;
        //    });

        //public T? Post<T>(string requestUri, object parameters, Exception exception) =>
        //    Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, signHelper.GenerateSignParameter(parameters));
        //        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        //            return await response.Content.ReadFromJsonAsync<T>();
        //        else
        //            throw exception;
        //    });

        //public Task<T?> PostAsync<T>(string requestUri, object value)
        //{
        //    return Execute(async client =>
        //    {
        //        var response = await client.PostAsJsonAsync(requestUri, value);
        //        response.EnsureSuccessStatusCode();
        //        return response.Content.ReadFromJsonAsync<T>();
        //    });
        //}
    }
}
