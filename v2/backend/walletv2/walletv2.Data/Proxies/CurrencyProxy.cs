using System.Text.Json;
using System.Xml.Serialization;
using walletv2.Data.Entities.Models;

namespace walletv2.Data.Proxies;

public interface ICurrencyProxy
{
    /// <summary>
    /// gets the currency rates from the API asynchronously.
    /// </summary>
    /// <returns>IEnumerable<Currency>?</returns>
    /// <exception cref="Exception"></exception>
    Task<TarihDate> GetRates();
}

public class CurrencyProxy : ICurrencyProxy
{
    private HttpClient client;

    public CurrencyProxy()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri("https://www.tcmb.gov.tr/kurlar/today.xml");
    }

    /// <summary>
    /// gets the currency rates from the API asynchronously.
    /// </summary>
    /// <returns>IEnumerable<Currency>?</returns>
    /// <exception cref="Exception"></exception>
    public async Task<TarihDate> GetRates()
    {
        try
        {
            var res = await GetCurrencyDataAsync();
            if (res is null || string.IsNullOrEmpty(res))
                throw new Exception("Failed to retrieve currency data.");

            var serializer = new XmlSerializer(typeof(TarihDate));
            using var stringReader = new StringReader(res);
            var obj = serializer.Deserialize(stringReader);
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return (TarihDate)obj;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching currency rates.", ex);
        }
    }

    /// <summary>
    /// gets the currency data from the API asynchronously.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> GetCurrencyDataAsync()
    {
        try
        {
            var response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error fetching currency data: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching currency data.", ex);
        }
    }
}
