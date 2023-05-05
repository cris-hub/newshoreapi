﻿

using NEWSHORE.DTOs.Flight;
using NEWSHORE.DTOs.Journey;

namespace NEWSHORE.Entities.Interfaces
{
    public interface IFlightRepository
    {
        Task<List<FlightDTO>> GetAll();
        Task<JourneyDTO> Get(List<FlightDTO> dataFilter, string TypeFlight);

        Task<JourneyDTO> Get(FlightDTO flight);
    }
}
