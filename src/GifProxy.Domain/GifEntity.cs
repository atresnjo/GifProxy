using System;

namespace GifProxy.Domain
{
    public class GifEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WebpFilePath { get; set; }
        public string GifFilePath { get; set; }
        public string GiphyLink { get; set; }
    }
}