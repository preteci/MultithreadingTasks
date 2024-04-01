using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace TASK2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Downloader client = new Downloader();
            client.Download();
        }


        public class Downloader
        { 
            public void Download()
            {
                int maxDownloads = 10;
                string jsonUrl = "https://jsonplaceholder.typicode.com/photos";
                SemaphoreSlim throtller = new SemaphoreSlim(maxDownloads);

                List<string> thumbnailURL = GetThumbnailURL(jsonUrl);
                List<Thread> downloadThreads = new List<Thread>();
                int throttlingDelayMilliseconds = 500;

                for (int i = 0; i < 100; i++)
                {

                    throtller.Wait();

                    Thread downloadThread = new Thread(() =>
                    {
                        try
                        {
                            DownloadImage(thumbnailURL[i]);
                        }
                        finally
                        {
                            throtller.Release();
                        }
                    });

                    downloadThreads.Add(downloadThread);
                    downloadThreads[i].Start();

                    Thread.Sleep(throttlingDelayMilliseconds);
                }            

                foreach (Thread thread in downloadThreads)
                {
                    thread.Join();
                }

                Console.WriteLine("All images downloaded successfully.");
            }
        
        }


        // method for getting all the thumbails
        static List<String> GetThumbnailURL(string jsonUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonResponse = client.GetStringAsync(jsonUrl).Result;
                JArray jsonArray = JArray.Parse(jsonResponse);
                List<string> thumbnailUrls = new List<string>();

                foreach (JObject item in jsonArray)
                {
                    string thumbnailUrl = item.Value<string>("thumbnailUrl");
                    thumbnailUrls.Add(thumbnailUrl);
                }

                return thumbnailUrls;
            }
        }

        static void DownloadImage(string url)
        {
            using(HttpClient client = new HttpClient())
            {
               
                    byte[] imageData = client.GetByteArrayAsync(url).Result;
                    string fileName = Path.GetFileName(url);
                    fileName = fileName + ".jpg";
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        fs.Write(imageData, 0, imageData.Length);
                    }

                    Console.WriteLine($"Downlaoded: {fileName}");
               
            }
        }
    }
}
