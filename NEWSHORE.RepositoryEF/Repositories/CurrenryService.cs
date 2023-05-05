using NEWSHORE.DTOs.currency;
using NEWSHORE.Entities.Interfaces;
using Newtonsoft.Json;

namespace NEWSHORE.RepositoryEF.Repositories
{
    internal class CurrenryService : ICurrenryService
    {
        readonly string apiUrl = "https://openexchangerates.org/api/latest.json?app_id=116ecc800c804082a17a7370a516340c&base=";

        public async Task<double> Get(double Amount, string to, string from)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}{from}&symbols={to}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            ExchangeRateResponse exchangeResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(jsonResponse);

            double conversionRate = (double)exchangeResponse.Rates[to];
            double convertedAmount = Amount * conversionRate;
            return convertedAmount;
        }
    }
}
