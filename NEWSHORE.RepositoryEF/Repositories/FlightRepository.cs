using NEWSHORE.DTOs.data;
using NEWSHORE.DTOs.Flight;
using NEWSHORE.DTOs.Journey;
using NEWSHORE.DTOs.Transport;
using NEWSHORE.Entities.Interfaces;
using Newtonsoft.Json;


namespace NEWSHORE.RepositoryEF.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private const string ROUNDTRIP = "2";
        private const string MULTITRIP = "1";
        private const string DIRECT = "0";

        readonly HttpClient client;
        readonly ICurrenryService currenryService;
        private List<DataDTO> ReturnFlights { get; set; }
        private List<DataDTO> GoFlight { get; set; }

        public FlightRepository()
        {
            client = new HttpClient() { BaseAddress = new("https://recruiting-api.newshore.es") };
            currenryService = new CurrenryService();

        }
        public async Task<List<DataDTO>> Request(string TypeFlight)
        {
            HttpResponseMessage response = await this.client.GetAsync($"api/flights/{TypeFlight}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<DataDTO>>(json);

            }
            return new();
        }
        public async Task<JourneyDTO> Get(List<FlightDTO> dataFilter, string TypeFlight)
        {
            double price = 0;
            string currency = dataFilter[0].Currency;
            JourneyDTO journey = new()
            {
                Flights = new List<FlightDTO[]>()
            };
            List<DataDTO> flights = await Request(TypeFlight);

            dataFilter.ForEach(f =>
            {
                journey.Flights.AddRange(Scales(f, flights));
            });


            return journey;
        }
        public async Task<JourneyDTO> Get(FlightDTO flight)
        {
            double price = 0;
            JourneyDTO journey = new();
            journey.Flights = new List<FlightDTO[]>();

            switch (flight.TypeFlight)
            {
                case ROUNDTRIP:
                    var flightsResponse = await Request(flight.TypeFlight);

                    int middleIndex = flightsResponse.Count / 2;
                    GoFlight = flightsResponse.Take(middleIndex).ToList();
                    ReturnFlights = flightsResponse.Skip(middleIndex).ToList();

                    journey.Flights.AddRange(Scales(new() { Origin = flight.Destination, Destination = flight.Origin }, ReturnFlights));
                    journey.Flights.AddRange(Scales(flight, GoFlight));

                    break;
                case MULTITRIP:
                    journey = await Get(flight.Multitrip, flight.TypeFlight);
                    break;
                case DIRECT:
                    GoFlight = await Request(flight.TypeFlight);
                    journey.Flights.AddRange(Scales(flight, GoFlight));
                    break;
            }
            if (flight.Scales!= 0 && (journey.Flights.Sum(c=>c.Length)>flight.Scales))
            {
                journey.Flights = new List<FlightDTO[]>();
                return journey;
            }

            price += journey.Flights.Sum(c => c.Sum(c => c.Price));
            journey.Price = !string.IsNullOrEmpty(flight.Currency) && flight.Currency != "" && flight.Currency != "USD" ? await currenryService.Get(price, flight.Currency) : price;

            journey.Origin = flight.Origin;
            journey.Destination = flight.Destination;

            return journey;
        }

        private List<FlightDTO[]> Scales(FlightDTO flight, List<DataDTO> flighs)
        {

            List<FlightDTO[]> scales = new List<FlightDTO[]>();
            // Buscar los vuelos directos desde el origen al destino
            var directFlights = flighs.Where(f => f.DepartureStation == flight.Origin && f.ArrivalStation == flight.Destination);

            // Agregar los vuelos directos a la lista de escalas
            foreach (var directFlight in directFlights)
            {
                scales.Add(new FlightDTO[] { new()  {
                        Transport = new TransportDTO()
                        {
                            FlightCarrier = directFlight.FlightCarrier,
                            FlightNumber = directFlight.FlightNumber
                        },
                        Origin = directFlight.DepartureStation,
                        Destination = directFlight.ArrivalStation,
                        Price = directFlight.Price
                    } });
            }


            // Buscar los vuelos desde el origen a otros aeropuertos como escalas
            var transitFlights = flighs.Where(f => f.DepartureStation == flight.Origin && f.ArrivalStation != flight.Destination);

            // Buscar los vuelos desde los aeropuertos de transito al destino
            foreach (var transitFlight in transitFlights)
            {
                var nextFlights = Scales(new() { Destination = flight.Destination, Origin = transitFlight.ArrivalStation }, flighs);

                // Agregar los vuelos desde el origen a los aeropuertos de transito como escalas
                foreach (var nextFlight in nextFlights)
                {
                    FlightDTO[] scale = new FlightDTO[nextFlight.Length + 1];
                    Array.Copy(nextFlight, 0, scale, 1, nextFlight.Length);
                    scale[0] = new()
                    {
                        Transport = new TransportDTO()
                        {
                            FlightCarrier = transitFlight.FlightCarrier,
                            FlightNumber = transitFlight.FlightNumber
                        },
                        Origin = transitFlight.DepartureStation,
                        Destination = transitFlight.ArrivalStation,
                        Price = transitFlight.Price
                    };
                    scales.Add(scale);
                }
            }
            return scales;
        }

        async Task<List<FlightDTO>> IFlightRepository.GetAll()
        {

            List<FlightDTO> journey = new List<FlightDTO>();
            HttpResponseMessage vuelos = await this.client.GetAsync("https://recruiting-api.newshore.es/api/flights/2");
            if (vuelos.IsSuccessStatusCode)
            {
                var json = await vuelos.Content.ReadAsStringAsync();

                List<DataDTO> data = JsonConvert.DeserializeObject<List<DataDTO>>(json);

                foreach (var item in data)
                {
                    journey.Add(new FlightDTO()
                    {
                        Transport = new DTOs.Transport.TransportDTO()
                        {
                            FlightCarrier = item.FlightCarrier,
                            FlightNumber = item.FlightNumber
                        },
                        Origin = item.ArrivalStation,
                        Destination = item.DepartureStation,
                        Price = item.Price
                    });
                }

            }

            return journey;
        }
    }
}
