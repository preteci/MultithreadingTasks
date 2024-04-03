using Moq;
using System.Net;
using static TASK2.Program;
using System.Net.Http;
using Moq.Protected;


namespace TASK2TEST
{
    public class UnitTestTask2
    {
        [SetUp]
        public void Setup()
        {

        }

        public class HttpMessageHandlerMock : HttpMessageHandler
        {
            public string ContentString { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ContentString)
                }) ;
            }
        }

        [Test]
        public async Task GetThumbnailURL_ReturnsListOfThumbnailUrlsFromValidJson()
        {
            // ARRANGE
            // ARRANGE
            HttpMessageHandlerMock mockClient = new HttpMessageHandlerMock();
            mockClient.ContentString = "[{ 'thumbnailUrl': 'https://example.com/thumb1.jpg'}, { 'thumbnailUrl': 'https://example.com/thumb2.jpg'}]";
            var httpClient = new HttpClient(mockClient);

            // ACT
            List<string> thumbnailUrls = await GetThumbnailURL("https://jsonplaceholder.typicode.com/photos", httpClient);

            // ASSERT
            Assert.AreEqual(2, thumbnailUrls.Count);
            Assert.AreEqual("https://example.com/thumb1.jpg", thumbnailUrls[0]);
            Assert.AreEqual("https://example.com/thumb2.jpg", thumbnailUrls[1]);
        }

        [Test]
        public async Task GetThumbnailURL_ReturnsJsonThatIsEmpty()
        {
            // ARRANGE
            // ARRANGE
            HttpMessageHandlerMock mockClient = new HttpMessageHandlerMock();
            mockClient.ContentString = "[]";
            var httpClient = new HttpClient(mockClient);

            // ACT
            List<string> thumbnailUrls = await GetThumbnailURL("https://jsonplaceholder.typicode.com/photos", httpClient);

            // ASSERT
            Assert.AreEqual(0, thumbnailUrls.Count);
        }

    }
}