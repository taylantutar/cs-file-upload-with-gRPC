using System.Reflection;
using System.IO;



Console.WriteLine(Environment.CurrentDirectory);


var files = Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "wwwroot"));

foreach (var file in files)
{
    using FileStream fileStream = new FileStream(file, FileMode.Open);
    byte[] buffer = new byte[2];
    int readedData = 0;
    while ((readedData = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
    {
        Console.WriteLine(readedData);
    }

}

Console.ReadLine();




