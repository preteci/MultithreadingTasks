using System.Net.Http.Headers;
using System.IO;
using System.Numerics;

namespace TASK4
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string urlPath = "";
            string input = "";
            Progress progress = new Progress();
            progress.Value = 0;
            Console.WriteLine("Please provide a valid url:");
            input = Console.ReadLine();
            urlPath = input;
            // wait for the file size
            int fileSize = (int)await FileDownloader.GetFileSizeAsync(urlPath);
            Console.WriteLine("File size in bytes: " + fileSize);
            Console.WriteLine("Please provide number of paraller downloads:");
            input = Console.ReadLine();

            int numberOfChunks = Int32.Parse(input);
            await FileDownloader.DownloadFile(urlPath, numberOfChunks, fileSize);
            FileDownloader.MergeFiles("file.exe", numberOfChunks);
            FileDownloader.DeleteFiles(numberOfChunks);
        }
    }

    public class Progress
    {
        private int _value;
        public int Value { get; set; }
    }

    public static class FileDownloader
    {
        public static async Task<long> GetFileSizeAsync(string fileUrl)
        {
            var httpClient = new HttpClient();
            var headRequest = new HttpRequestMessage(HttpMethod.Head, fileUrl);
            var response = await httpClient.SendAsync(headRequest, HttpCompletionOption.ResponseHeadersRead);
            return response.Content.Headers.ContentLength.Value;
        }

        static async Task DownloadPartOfFile(string url, long start, long end, string fileName, IProgress<int> progress)
        {
            Directory.CreateDirectory("tempfiles");
            var client = new HttpClient();
            var buffer = new byte[8192];
            var isReading = true;
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Range = new RangeHeaderValue(start, end);
            var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            var s = await res.Content.ReadAsStreamAsync();
            var fs = File.Create(@"./tempfiles/" + fileName);
            while (isReading)
            {
                var read = await s.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    isReading = false;
                    progress.Report(read);
                }
                else
                {
                    await fs.WriteAsync(buffer, 0, read);
                    progress.Report(read);
                }
            }
            fs.Close();
            s.Close();
        }

        public static async Task DownloadFile(string fileUrl, long numberOfChunks, long fileSize)
        {
            long currentChunk = 0;
            long chunk = fileSize / numberOfChunks;
            int numberOfbytes = 0;
            var tasks = new List<Task>();
            object lockObject = new object();

            var progress = new Progress<int>(partialSum =>
            {
                lock (lockObject)
                {
                    numberOfbytes += partialSum;
                    double percentege = (100 * (double)numberOfbytes) / (double)fileSize;
                    Console.Write("\r{0}           ", $"Downloading: {Math.Round(percentege)}%");

                }
            });
            for (int i = 0; i < numberOfChunks; i++)
            {
                long startChunk = currentChunk;
                long endChunk = currentChunk + chunk;
                currentChunk = endChunk + 1;
                if (i == numberOfChunks - 1)
                {
                    endChunk = fileSize;
                }
                Console.WriteLine(startChunk + "/" + endChunk);
                tasks.Add(DownloadPartOfFile(fileUrl, startChunk, endChunk, $"tempfile{i}", progress));
            }
            await Task.WhenAll(tasks);
        }

        public static void MergeFiles(string filePath, int numberOfChunks)
        {
            var fss = File.Create(filePath);
            for (int i = 0; i < numberOfChunks; i++)
            {
                using (var chunkFileStream = File.OpenRead(@"./tempfiles/" + $"tempFile{i}"))
                {
                    chunkFileStream.CopyTo(fss);
                }
            }
            fss.Close();
        }

        public static void DeleteFiles(int numberOfchunks)
        {
            for (int i = 0; i < numberOfchunks; i++)
            {
                File.Delete(@"./tempfiles/" + $"tempFile{i}");
            }
        }


    }

}