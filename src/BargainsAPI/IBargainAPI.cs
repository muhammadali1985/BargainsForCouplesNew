using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HotelDetailResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BargainsAPI
{
    public interface IBargainAPI
    {
        Task<List<HotelResponse>> FindBargain(int nights, int destination);
    }

    public sealed class BargainAPI : IBargainAPI
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<BargainAPI> ilogger;

        public BargainAPI(HttpClient httpClient, ILogger<BargainAPI> ilogger) =>
            (this.httpClient, this.ilogger) = (httpClient, ilogger);

        public async Task<List<HotelResponse>> FindBargain(int nights, int destination)
        {
            var uri = new Uri(httpClient.BaseAddress, $"?destinationId={destination}&nights={nights}&code=aWH1EX7ladA8C/oWJX5nVLoEa4XKz2a64yaWVvzioNYcEo8Le8caJw==");
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();

            response.EnsureSuccessStatusCode();
            var hotelResponseDetail = DeserializeJsonFromStream<List<HotelResponse>>(stream);
            var hotels = new List<HotelResponse>();
            foreach (var hotel in hotelResponseDetail)
            {
                var hotelrate = new List<Rate>();
                foreach (var rate in hotel.Rates)
                {
                    if (string.Equals(rate.RateType, "PerNight", StringComparison.InvariantCultureIgnoreCase))
                        hotelrate.Add(new Rate(rate.RateType, rate.BoardType, rate.Value * nights));
                    else
                        hotelrate.Add(rate);
                }
                hotels.Add(new HotelResponse(hotel.Hotel, hotelrate));
            }
            return hotels;
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream?.CanRead != true)
                return default;

            using var sr = new StreamReader(stream);
            using var jtr = new JsonTextReader(sr);
            var js = new JsonSerializer();
            return js.Deserialize<T>(jtr);
        }
    }
}
