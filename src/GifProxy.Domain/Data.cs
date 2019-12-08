using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GifProxy.Domain
{

    public class BaseResponse
    {
        [JsonProperty("data")] public List<Data> Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("images")] public Images Images { get; set; }

    }

    public class Images
    {

        [JsonProperty("preview_webp")] public PreviewWebp PreviewWebp { get; set; }


        [JsonProperty("downsized_small")] public DownsizedSmall DownsizedSmall { get; set; }

    }

    public class PreviewWebp
    {
        [JsonProperty("url")] public Uri Url { get; set; }

        [JsonProperty("width")] public long Width { get; set; }

        [JsonProperty("height")] public long Height { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }
    }

    public class DownsizedSmall
    {
        [JsonProperty("height")] public long Height { get; set; }

        [JsonProperty("mp4")] public Uri Mp4 { get; set; }

        [JsonProperty("mp4_size")] public long Mp4Size { get; set; }

        [JsonProperty("width")] public long Width { get; set; }
    }
}