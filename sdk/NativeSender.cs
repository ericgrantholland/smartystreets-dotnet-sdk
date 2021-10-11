namespace SmartyStreets
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class NativeSender : ISender
    {
        private static readonly Version AssemblyVersion = typeof(NativeSender).Assembly.GetName().Version;

        private static readonly string UserAgent = string.Format("smartystreets (sdk:dotnet@{0}.{1}.{2})",
            AssemblyVersion.Major, AssemblyVersion.Minor, AssemblyVersion.Build);

        private readonly TimeSpan timeout;
        private readonly IWebProxy proxy;

        public NativeSender()
        {
            this.timeout = TimeSpan.FromSeconds(10);
        }

        public NativeSender(TimeSpan timeout, Proxy proxy = null) : this()
        {
            this.timeout = timeout;
            this.proxy = (proxy ?? new Proxy()).NativeProxy;
        }

        public async Task<Response> SendAsync(Request request)
        {
            var frameworkRequest = this.BuildAsyncRequest(request);
            CopyHeaders(request, frameworkRequest);

            if (request.Payload != null)
                frameworkRequest.Content = new ByteArrayContent(request.Payload);
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(this.timeout.TotalMilliseconds);
                var response = await client.SendAsync(frameworkRequest);
                var responsePayload = await GetDecompressedResponse(response);
                return new Response((int)response.StatusCode, responsePayload);
            }
        }
        private async Task<byte[]> GetDecompressedResponse(HttpResponseMessage response)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        private HttpRequestMessage BuildAsyncRequest(Request request)
        {
            var httpRequest = new HttpRequestMessage();
            var requestMethod = HttpMethod.Get;
            switch (request?.Method?.ToLower())
            {
                case "get": requestMethod = HttpMethod.Get; break;
                case "post": requestMethod = HttpMethod.Post; break;
                case "delete": requestMethod = HttpMethod.Delete; break;
                case "put": requestMethod = HttpMethod.Put; break;
            }
            httpRequest.Method = requestMethod;
            httpRequest.RequestUri = new Uri(request.GetUrl());

            return httpRequest;
        }
        private static void CopyHeaders(Request request, HttpRequestMessage frameworkRequest)
        {
            foreach (var item in request.Headers)
                frameworkRequest.Headers.Add(item.Key, item.Value);

            frameworkRequest.Headers.Add("User-Agent", UserAgent);
        }
    }
}