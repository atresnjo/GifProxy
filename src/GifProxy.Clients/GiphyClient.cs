using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;

namespace GifProxy.Clients
{
    public class GiphyClient
    {
        private readonly FlurlClient _client;
        private readonly string _authKey;

        public GiphyClient()
        {
            _client = new FlurlClient("https://upload.giphy.com/v1/gifs");
            _authKey = "";
        }

        public async Task<string> UploadGif(string filePath)
        {
            var payload = File.ReadAllBytes(filePath);
            using var stream = new MemoryStream(payload);
            var result = await _client.Request().PostMultipartAsync(content =>
                    content
                        .AddString("\"api_key\"", _authKey)
                        .AddString("\"tags\"", "")
                        .AddString("\"source_post_url\"", "")
                        .AddFile("\"file\"", stream, Path.GetFileName(filePath), "image/gif"))
                .ReceiveJson<UploadResponse>();

            return result.Data.Id;
        }
    }

    public class UploadResponse
    {
        [JsonProperty("data")]
        public UploadResponseData Data { get; set; }
        [JsonProperty("meta")]
        public UploadResponseMeta Meta { get; set; }
    }

    public class UploadResponseData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class UploadResponseMeta
    {
        [JsonProperty("msg")]

        public string Msg { get; set; }
        [JsonProperty("status")]

        public int Status { get; set; }
    }
}