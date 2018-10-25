using Dowsingman2.UtilityClass;
using Dowsingman2.Error;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dowsingman2.UtilityClass
{
    internal class MyHttpClient
    {
        private const int requestLimit_ = 3;

        private string userAgent_;

        public MyHttpClient()
        {
            userAgent_ = "DowsingApp";
        }

        public async Task<Stream> GetStreamAsync(string url, Dictionary<string, string> param)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (param != null)
            {
                string[] array = new string[param.Count];
                int num = 0;
                foreach (KeyValuePair<string, string> item in param)
                {
                    array[num] = item.Key + "=" + Uri.EscapeDataString(item.Value);
                    num++;
                }
                url = url + "?" + string.Join("&", array);
            }
            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Proxy = null;
                    httpWebRequest.UserAgent = userAgent_;
                    httpWebRequest.Timeout = 15000;
                    httpWebRequest.ReadWriteTimeout = 15000;
                    WebHeaderCollection headers = httpWebRequest.Headers;
                    headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3");
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Information, "[MyHttpClient] " + url);
#endif
                    using (HttpWebResponse httpWebResponse = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream stream = httpWebResponse.GetResponseStream())
                            {
                                stream.CopyTo(memoryStream);
                            }
                            break;
                        }
                    }
                }
                catch (WebException ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                    MyTraceSource.TraceEvent(TraceEventType.Error, "WebException：" + url);
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
                        if (httpWebResponse2.StatusCode == HttpStatusCode.InternalServerError || httpWebResponse2.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            throw new ServerErrorException(url, ex);
                        }
                        if (httpWebResponse2.StatusCode == HttpStatusCode.BadRequest || httpWebResponse2.StatusCode == HttpStatusCode.Forbidden || httpWebResponse2.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new ClientErrorException(url, ex);
                        }
                    }
                    if (i >= 3)
                    {
                        throw new HttpClientException(url, ex);
                    }
                    if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    {
#if DEBUG
                    MyTraceSource.TraceEvent(TraceEventType.Information, "[MyHttpClient] NameResolutionFailure RetryWait Start");
#endif
                        await Task.Delay(15000);
#if DEBUG
                    MyTraceSource.TraceEvent(TraceEventType.Information, "[MyHttpClient] NameResolutionFailure RetryWait end");
#endif
                    }
                }
                catch (Exception innerException)
                {
                    throw new HttpClientException(url, innerException);
                }
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, "[MyHttpClient] RetryWait Start");
#endif
                await Task.Delay(5000);
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, "[MyHttpClient] RetryWait End");
#endif
            }
            memoryStream.Position = 0L;
            return memoryStream;
        }

        public async Task<string> GetStringAsync(string url, Dictionary<string, string> param)
        {
            try
            {
                using (Stream stream = await GetStreamAsync(url, param))
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (ServerErrorException ex)
            {
                throw ex;
            }
            catch (ClientErrorException ex2)
            {
                throw ex2;
            }
            catch (HttpClientException ex3)
            {
                throw ex3;
            }
            catch (Exception innerException)
            {
                throw new HttpClientException(url, innerException);
            }
        }
    }
}