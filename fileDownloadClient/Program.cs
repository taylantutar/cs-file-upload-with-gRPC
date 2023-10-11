
using Grpc.Net.Client;
using fileClient;

var channel = GrpcChannel.ForAddress("http://localhost:5253");
var client = new FileUD.FileUDClient(channel);

string fileName = "deneme.txt";
var request = new fileInfo { FullName = fileName };


var response = client.FileDownload(request);

var ct = new CancellationTokenSource();
var count = 0;
FileStream fs = null;

var path = Directory.GetCurrentDirectory();
var url = Path.Combine(path, "deneme.txt");

while (await response.ResponseStream.MoveNext(ct.Token))
{
    if(count++ == 0)
    {
        fs = new FileStream(url, FileMode.CreateNew);
        fs.SetLength(response.ResponseStream.Current.FileSize);
    }

    var bufferValue = response.ResponseStream.Current.Buffer.ToByteArray();

    await fs.WriteAsync(bufferValue, 0, response.ResponseStream.Current.ReadedData);
}

await fs.DisposeAsync();

Console.ReadLine();