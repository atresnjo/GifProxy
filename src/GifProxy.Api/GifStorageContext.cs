using LiteDB;

namespace GifProxy.Api
{
    public class GifStorageContext
    {
        public readonly LiteDatabase Context;

        public GifStorageContext()
        {
            Context = new LiteDatabase("gifs.db");
        }
    }
}