using Xunit;
using FakeItEasy;
using BargainsAPI;
using Microsoft.Extensions.Logging;
using System.Text;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using HotelDetailResponse;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace UnitTest
{
    public class BargainsAPITest
    {
        private readonly MockHttpClient.MockHttpClient httpClient;
        private readonly ILogger<BargainAPI> logger;
        private readonly IBargainAPI bargainAPI;

        public BargainsAPITest()
        {
            httpClient = new MockHttpClient.MockHttpClient();
            this.httpClient.BaseAddress = new Uri("http://localhost/findBargain");
            logger = A.Fake<ILogger<BargainAPI>>();
            bargainAPI = new BargainAPI(httpClient, logger);
            //test
        }

        [Fact]
        public async Task GetBargainsHotelSuccess()
        {
            httpClient.When(req => req.Method == HttpMethod.Get
                        && req.RequestUri == new Uri($"{httpClient.BaseAddress}?destinationId=1419&nights=3&code=aWH1EX7ladA8C/oWJX5nVLoEa4XKz2a64yaWVvzioNYcEo8Le8caJw==")
                        ).Then(_ => GetHotelFromAPI());
            List<HotelResponse> value = await bargainAPI.FindBargain(3, 1419);
            var actualresponsestr = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            }); 
            var expectedResponsestr=GetExpectedHotel();
            Assert.True(CompareJsonValues(expectedResponsestr, actualresponsestr));   
        }

        private static bool CompareJsonValues(string actualValue, string expectedValue)
        {
            var actualObject = JsonConvert.DeserializeObject<JArray>(actualValue);
            var expectedObject = JsonConvert.DeserializeObject<JArray>(expectedValue);
            return JToken.DeepEquals(actualObject, expectedObject);
        }
        private Task<HttpResponseMessage> GetHotelFromAPI()
        {
            var content = new StringContent("[{\"hotel\":{\"propertyID\":79732,\"name\":\"JAC Canada (CA$)8314\",\"geoId\":279,\"rating\":3},\"rates\":[{\"rateType\":\"Stay\",\"boardType\":\"No Meals\",\"value\":679.2},{\"rateType\":\"Stay\",\"boardType\":\"Half Board\",\"value\":792.4},{\"rateType\":\"Stay\",\"boardType\":\"Full Board\",\"value\":905.6}]},{\"hotel\":{\"propertyID\":79821,\"name\":\"JAC Canada (CA$)8555\",\"geoId\":279,\"rating\":3},\"rates\":[{\"rateType\":\"PerNight\",\"boardType\":\"No Meals\",\"value\":198.0},{\"rateType\":\"PerNight\",\"boardType\":\"Half Board\",\"value\":231.0},{\"rateType\":\"PerNight\",\"boardType\":\"Full Board\",\"value\":264.0}]}]",
                         Encoding.UTF8, "application/json");
            var HttpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            };
            return Task.FromResult(HttpResponseMessage);
        }

        private string GetExpectedHotel() => System.IO.File.ReadAllText(@"Data\ExpectedResponse.txt");
    }
}
