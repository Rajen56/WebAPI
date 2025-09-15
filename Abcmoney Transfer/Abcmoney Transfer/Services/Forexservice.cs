using Abcmoney_Transfer.View_model;
using System.Text.Json;

namespace Abcmoney_Transfer.Services
{
    
      public interface IForexService
        {
            Task<IEnumerable<ForexviewModel>> GetExchangeRatesAsync();
            Task<ForexviewModel> GetExchangeRateForCurrencyAsync(string currencyIso3);
        }

        public class ForexService : IForexService
        {
            private readonly HttpClient _httpClient;
            private const string BaseUrl = "https://www.nrb.org.np/api/forex/v1/rates";

            public ForexService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<IEnumerable<ForexviewModel>> GetExchangeRatesAsync()
            {
                try
                {
                    var rates = await FetchRatesAsync();
                    // Convert JsonElement to IEnumerable and then use LINQ
                    return rates
                        .EnumerateArray()
                        .Select(MapToForexOutputVm)
                        .Where(rate => rate != null)
                        .ToList()!;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching exchange rates: {ex.Message}");
                }
            }

            public async Task<ForexviewModel> GetExchangeRateForCurrencyAsync(string currencyIso3)
            {
                try
                {
                    var rates = await FetchRatesAsync();
                    var matchedRate = rates
                        .EnumerateArray()
                        .FirstOrDefault(rate =>
                            rate.GetProperty("currency").GetProperty("iso3").GetString()
                                .Equals(currencyIso3, StringComparison.OrdinalIgnoreCase));

                    if (matchedRate.ValueKind == JsonValueKind.Undefined)
                    {
                        throw new Exception($"Exchange rate for currency '{currencyIso3}' not found.");
                    }

                    var forexVm = MapToForexOutputVm(matchedRate);
                    if (forexVm == null)
                    {
                        throw new Exception($"Exchange rate data is invalid for currency: {currencyIso3}");
                    }

                    return forexVm;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching exchange rate for currency '{currencyIso3}': {ex.Message}");
                }
            }

            private async Task<JsonElement> FetchRatesAsync()
            {
                var todayDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"{BaseUrl}?page=1&per_page=5&from={todayDate}&to={todayDate}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Unable to fetch exchange rates: API request failed.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(json);

                return data.RootElement
                           .GetProperty("data")
                           .GetProperty("payload")[0]
                           .GetProperty("rates");
            }

            private ForexviewModel? MapToForexOutputVm(JsonElement rate)
            {
                try
                {
                    var currency = rate.GetProperty("currency").GetProperty("iso3").GetString();
                    var unit = int.Parse(rate.GetProperty("currency").GetProperty("unit").ToString());
                    var buy = ParseDecimal(rate.GetProperty("buy").GetString());
                    var sell = ParseDecimal(rate.GetProperty("sell").GetString());

                    if (buy == 0 || sell == 0) return null;

                    return new ForexviewModel
                    {
                        Currency = currency!,
                        Unit = unit,
                        Buy = buy,
                        Sell = sell
                    };
                }
                catch
                {
                    return null;
                }
            }

            private decimal ParseDecimal(string value)
            {
                return decimal.TryParse(value, out var result) ? result : 0;
            }
        }

    
}
