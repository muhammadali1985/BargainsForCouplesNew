using System;
using System.Collections.Generic;

namespace HotelDetailResponse
{
    public class Hotel
    {
        public int PropertyID { get; }
        public string Name { get; }
        public int GeoId { get; }
        public int Rating { get; }

        public Hotel(int propertyId, string name, int geoId, int rating) =>
        (PropertyID, Name, GeoId, Rating) = (propertyId, name, geoId, rating);
    }

    public class Rate
    {
        public string RateType { get; }
        public string BoardType { get; }
        public double Value { get; }

        public Rate(string rateType, string boardType, double value)
        => (RateType, BoardType, Value) = (rateType, boardType, value);
    }

    public class HotelResponse
    {
        public Hotel Hotel { get; }
        public List<Rate> Rates { get; }
        public HotelResponse(Hotel hotel, List<Rate> rates)
      => (Hotel, Rates) = (hotel, rates);
    }
}
