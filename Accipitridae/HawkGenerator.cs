using System;
using System.Security.Cryptography;
using System.Text;

namespace Accipitridae
{
    public class HawkGenerator
    {
        private string _id;
        private byte[] _key;

        public HawkGenerator(string id, string key)
        {
            _id = id;
            _key = (new UTF8Encoding()).GetBytes(key);
        }

        public string GetMac(Uri url, string method, string appExtData, string nonce, int timestamp)
        {
            var header="hawk.1.header\n";
            header+=$"{timestamp}\n";
            header+=$"{nonce}\n";
            header+=$"{method}\n";
            header+=$"{url.PathAndQuery}\n";
            header+=$"{url.Host}\n";
            header+=$"{url.Port}\n";
            header+="\n";
            header+=$"{appExtData}\n";
            var hasher = new HMACSHA256(_key);
            var hashAsBytes = hasher.ComputeHash((new UTF8Encoding()).GetBytes(header));
            return Convert.ToBase64String(hashAsBytes);
        }
    }
}