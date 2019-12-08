using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GifProxy.Clients;
using GifProxy.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GifProxy.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureLiteDb(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<GifStorageContext>();
        }
    }

    public class InitializationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InitializationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var storage = scope.ServiceProvider.GetService<GifStorageContext>();
            var giphyClient = scope.ServiceProvider.GetService<GiphyClient>();
            var context = storage.Context.GetCollection<GifEntity>();
            var gifsPath = Path.Combine(Environment.CurrentDirectory, "gifs");
            var ffmpegPath = Path.Combine(gifsPath, "ffmpeg.exe");
            var files = Directory.GetFiles(gifsPath, "*.gif");
            var list = new List<GifEntity>();

            foreach (var file in files)
            {
                if (context.FindOne(x => x.GifFilePath == file) != null)
                    continue;

                var fileName = Path.GetFileNameWithoutExtension(file);
                var finalPath = $"{Path.Combine(gifsPath, $"{fileName}.webp")}";

                if (!File.Exists(finalPath))
                {
                    var processInfo = new ProcessStartInfo {FileName = ffmpegPath};
                    processInfo.ArgumentList.Add("-i");
                    processInfo.ArgumentList.Add(file);
                    processInfo.ArgumentList.Add(finalPath);
                    Process.Start(processInfo);
                }

                var uploadedId = await giphyClient.UploadGif(file);
                var gifEntity = new GifEntity
                {
                    WebpFilePath = finalPath,
                    Name = fileName,
                    Id = Guid.NewGuid(),
                    GifFilePath = file,
                    GiphyLink = $"https://media0.giphy.com/media/{uploadedId}/giphy-downsized-small.mp4"
                };

                list.Add(gifEntity);
            }

            context.Insert(list);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}