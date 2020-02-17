using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using BargainsForCouples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BargainsAPI;
using HotelDetailResponse;

namespace BargainsForCouples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelListController : ControllerBase
    {
        private readonly ILogger<HotelListController> ilogger;
        private readonly IBargainAPI bargainAPI;

        public HotelListController(ILogger<HotelListController> ilogger, IBargainAPI bargainAPI) =>
            (this.ilogger, this.bargainAPI) = (ilogger, bargainAPI);

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<List<HotelResponse>> Get([FromQuery] HotelInfoRequest hotelInfoRequest)
        {
            using var ilogging = ilogger.BeginScope($"Start Service Call for hotelID{0}", hotelInfoRequest.DestinationId);
            return await bargainAPI.FindBargain(hotelInfoRequest.Nights, hotelInfoRequest.DestinationId);
        }
    }
}