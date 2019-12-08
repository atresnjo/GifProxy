using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Util;
using GifProxy.Domain;
using Newtonsoft.Json;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace GifProxy.App
{
    class Program
    {
        private const string url = "http://localhost:5000";
        private static readonly FlurlClient _client = new FlurlClient(url);

        public static void Main(string[] args)
        {
            var proxyServer = new ProxyServer();
            proxyServer.CertificateManager.EnsureRootCertificate();
            proxyServer.CertificateManager.TrustRootCertificate();
            proxyServer.BeforeRequest += OnRequest;

            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 1234);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            Console.Read();
            proxyServer.Stop();
        }

        public static async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (!e.HttpClient.Request.RequestUri.Host.Equals("api.giphy.com") ||
                !e.HttpClient.Request.RequestUri.AbsolutePath.Equals("/v1/gifs/search"))
                return;

            var headers = new Dictionary<string, HttpHeader>
            {
                {
                    "Access-Control-Allow-Origin",
                    new HttpHeader("Access-Control-Allow-Origin", "https://web.whatsapp.com")
                }
            };

            var queryParameters = e.HttpClient.Request.Url.ToKeyValuePairs();
            var searchTerm = queryParameters.FirstOrDefault().Value;
            var gifResponse = await _client.Request("api", "gifs").SetQueryParam("searchTerm", searchTerm)
                .GetJsonAsync<BaseResponse>();

            var finalResponse = JsonConvert.SerializeObject(gifResponse);
            e.Ok(finalResponse, headers);
        }
    }
}