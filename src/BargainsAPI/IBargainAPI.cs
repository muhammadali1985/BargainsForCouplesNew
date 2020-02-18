using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HotelDetailResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

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
            using var x = ilogger.BeginScope("Calling Bargain API for nights {0}, and destination {1}", nights, destination);
            var uri = new Uri(httpClient.BaseAddress, $"?destinationId={destination}&nights={nights}&code=aWH1EX7ladA8C/oWJX5nVLoEa4XKz2a64yaWVvzioNYcEo8Le8caJw==");
            // var result = await httpClient.GetAsync("https://webbedsdevtest.azurewebsites.net/api/findBargain?destinationId=1419&nights=2&code=aWH1EX7ladA8C/oWJX5nVLoEa4XKz2a64yaWVvzioNYcEo8Le8caJw==");
            var result = await httpClient.GetAsync(uri);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();
            var hotelResponseDetail = JsonConvert.DeserializeObject<List<HotelResponse>>(response);
            var task=hotelResponseDetail.Select(i => GetUpdatedRates(i.Rates,nights));
            var lists = await Task.WhenAll(task).ConfigureAwait(false);
            return hotelResponseDetail;
        }

        private  Task<List<Rate>> GetUpdatedRates(List<Rate> rate,int nights)
        {
            for(int a=0;a<rate.Count;a++)
            {
                if (string.Equals(rate[a].RateType, "PerNight", StringComparison.InvariantCultureIgnoreCase))
                    rate[a] = new Rate(rate[a].RateType, rate[a].BoardType, rate[a].Value * nights);
            }
            return Task.FromResult(rate);
        }
    }
}
