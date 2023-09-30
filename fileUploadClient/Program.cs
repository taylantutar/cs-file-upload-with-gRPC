using Grpc.Net.Client;
using fileServer;
using Google.Protobuf;
using System.Text;

Console.WriteLine(Environment.CurrentDirectory);

var channel = GrpcChannel.ForAddress("http://localhost:5253");
var client = new FileUD.FileUDClient(channel);

var path = Directory.GetCurrentDirectory();
var url = Path.Combine(path, "deneme.txt");

Console.WriteLine($"URL: {url}");

var fileStream = new FileStream(url, FileMode.Open);

var request = new fileUploadRequest
{
    FileSize = fileStream.Length,
    Fullname = $"{Path.GetFileNameWithoutExtension(fileStream.Name)}.{Path.GetExtension(fileStream.Name)}",
    ReadedData = 0
};

var clientStreamingCall = client.FileUpload();

var buffer = new byte[64];

while ((request.ReadedData = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
    Console.Write(Encoding.Default.GetString(buffer));

    if (request.ReadedData < buffer.Length)
        Array.Resize(ref buffer, request.ReadedData);

    request.Buffer = ByteString.CopyFrom(buffer);

    await clientStreamingCall.RequestStream.WriteAsync(request);

    Array.Clear(buffer, 0, request.ReadedData);
}

await clientStreamingCall.RequestStream.CompleteAsync();
fileStream.Close();

Console.ReadLine();




