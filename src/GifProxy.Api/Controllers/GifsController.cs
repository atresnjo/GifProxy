using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GifProxy.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GifProxy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GifsController : ControllerBase
    {
        private readonly GifStorageContext _gifDatabase;

        public GifsController(GifStorageContext gifDatabase)
        {
            _gifDatabase = gifDatabase;
        }

        [HttpGet]
        public IActionResult FindGifs(string searchTerm)
        {
            var foundGifs = _gifDatabase.Context.GetCollection<GifEntity>().Find(x => x.Name.Contains(searchTerm));
            var response = new BaseResponse {Data = new List<Data>()};

            foreach (var gifEntity in foundGifs)
            {
                var data = new Data
                {
                    Images = new Images
                    {
                        DownsizedSmall = new DownsizedSmall
                        {
                            Mp4 = new Uri(gifEntity.GiphyLink), Height = 250, Width = 250, Mp4Size = 169264
                        }
                    }
                };

                data.Images.PreviewWebp = new PreviewWebp
                {
                    Height = 250,
                    Width = 250,
                    Size = 169264,
                    Url = new Uri($"http://localhost:5000/api/gifs/{gifEntity.Id}")
                };

                response.Data.Add(data);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(Guid id)
        {
            var gif = _gifDatabase.Context.GetCollection<GifEntity>().FindOne(entity => entity.Id == id);
            var stream = await System.IO.File.ReadAllBytesAsync(gif.WebpFilePath);
            return File(stream, "image/webp");
        }
    }
}