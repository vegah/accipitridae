using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accipitridae
{
    public class HawkAuthHandler : DelegatingHandler
    {
        private string _id;
        private HawkGenerator _gen;

        public HawkAuthHandler(string id,string key)
        {
            _id = id;
            _gen = new HawkGenerator(id,key);
            InnerHandler = new HttpClientHandler();
        }
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var header = await CreateHeaderValue(request);
            request.Headers.Authorization=new AuthenticationHeaderValue("Hawk",header);
            return await base.SendAsync(request,cancellationToken);
        }

        private async Task<string> CreateHeaderValue(HttpRequestMessage request)
        {
            var timestamp = (int)Math.Floor((DateTime.UtcNow-new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)).TotalSeconds);
            var nonce = Guid.NewGuid().ToString();
            string extData = null;
            if (request.Content!=null){
                var content = await request.Content.ReadAsStringAsync();
                extData = CreateHashOfContent(content);
            }
            var hash = _gen.GetMac(request.RequestUri,request.Method.ToString(),extData,nonce,timestamp);
            var header = $"id=\"{_id}\", ts=\"{timestamp}\", nonce=\"{nonce}\"";
            if (!string.IsNullOrEmpty(extData))
                header+=$", ext=\"{extData}\"";
            header+=$", mac=\"{hash}\"";
            return header;
        }

        private string CreateHashOfContent(string content)
        {
            var hasher = new HMACSHA256();
            var hashAsBytes = hasher.ComputeHash((new UTF8Encoding()).GetBytes(content));
            return Convert.ToBase64String(hashAsBytes);
        }
    }
}
