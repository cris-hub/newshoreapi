namespace NEWSHORE.DTOs.currency
{



    public class ExchangeRateResponse
    {
        public string? Disclaimer { get; set; }

        public string? License { get; set; }

        public long Timestamp { get; set; }

        public string? BaseCurrency { get; set; }

        public Dictionary<string, decimal>? Rates { get; set; }
    }

}
