using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fileServer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace fileServer.Services
{
    public class FileUDService : FileUD.FileUDBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUDService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<BoolValue> FileUpload(IAsyncStreamReader<fileUploadRequest> requestStream, ServerCallContext context)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "files");

            try
            {
                FileStream fileStream = null;
                decimal progresedSize = 0;
                int count = 0;
                while (await requestStream.MoveNext())
                {
                    if (count++ == 0)
                    {
                        fileStream = new FileStream($"{path}/{requestStream.Current.Fullname}", FileMode.Create);
                        fileStream.SetLength(requestStream.Current.FileSize);

                    }

                    var willAddData = requestStream.Current.Buffer.ToByteArray();
                    await fileStream.WriteAsync(willAddData, 0, willAddData.Length);

                    progresedSize += requestStream.Current.ReadedData;
                    Console.WriteLine($"%{Math.Round(progresedSize * 100 / requestStream.Current.FileSize)}");

                }

                await fileStream.DisposeAsync();

                return new BoolValue();

            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found: {path}. ERR: {ex.Message}");
                return new BoolValue();
            }
        }
    }
}