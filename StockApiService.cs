using System;
using System.Threading.Tasks;
using Coinbot.Domain.Contracts;
using Coinbot.Domain.Contracts.Models;
using Coinbot.Domain.Contracts.Models.StockApiService;
using AutoMapper;
using System.Net.Http;
using Newtonsoft.Json;
using Coinbot.Bittrex.Models;
using System.Globalization;

namespace Coinbot.Bittrex
{
    public class StockApiService : IStockApiService
    {
        private readonly string _serviceUrl = "https://bittrex.com/api/v1.1/";
        private readonly IMapper _mapper;

        public StockApiService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Transaction>> GetOrder(string baseCoin, string targetCoin, string apiKey, string secret, string orderRefId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceUrl);

                var reqUrl = string.Format(CultureInfo.InvariantCulture, "account/getorder?apikey={0}&uuid={1}&nonce={2}",
                    apiKey,
                    orderRefId,
                    Helpers.GetUnixTimeInSeconds()
                );

                var apiSign = Helpers.GetHashSHA512(_serviceUrl + reqUrl, secret);
                client.DefaultRequestHeaders.Add("apisign", apiSign);

                var response = await client.GetAsync(reqUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<ResultWrapper<TransactionDTO>>(json);

                    if (deserialized.success)
                    {
                        return new ServiceResponse<Transaction>(0, _mapper.Map<TransactionDTO, Transaction>(deserialized.result));
                    }
                    else
                        throw new Exception(deserialized.message);
                }
                else
                    return new ServiceResponse<Transaction>((int)response.StatusCode, null, await response.Content.ReadAsStringAsync());
            }
        }

        public StockInfo GetStockInfo()
        {
            return new StockInfo
            {
                FillOrKill = false
            };
        }

        public async Task<ServiceResponse<Tick>> GetTicker(string baseCoin, string targetCoin)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceUrl);
                var response = await client.GetAsync(string.Format("public/getticker?market={0}-{1}", baseCoin, targetCoin));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<ResultWrapper<TickDTOResult>>(json);

                    if (deserialized.success)
                    {
                        return new ServiceResponse<Tick>(0, _mapper.Map<Tick>(deserialized.result));
                    }
                    else
                        throw new Exception(deserialized.message);
                }
                else
                    return new ServiceResponse<Tick>((int)response.StatusCode, null, await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceResponse<Transaction>> PlaceBuyOrder(string baseCoin, string targetCoin, double stack, string apiKey, string secret, double rate, bool? testOnly = false)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceUrl);

                var reqUrl = string.Format(CultureInfo.InvariantCulture, "market/buylimit?apikey={0}&market={1}-{2}&quantity={3}&rate={4}&nonce={5}",
                    apiKey,
                    baseCoin,
                    targetCoin,
                    (stack / rate).ToString("0.00", CultureInfo.InvariantCulture),
                    rate.ToString("0.00000000", CultureInfo.InvariantCulture),
                    Helpers.GetUnixTimeInSeconds()
                );

                var apiSign = Helpers.GetHashSHA512(_serviceUrl + reqUrl, secret);
                client.DefaultRequestHeaders.Add("apisign", apiSign);

                var response = await client.GetAsync(reqUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<ResultWrapper<TransactionMadeDTO>>(json);

                    if (deserialized.success)
                    {
                        return new ServiceResponse<Transaction>(0, _mapper.Map<Transaction>(deserialized.result));
                    }
                    else
                        throw new Exception(deserialized.message);
                }
                else
                    return new ServiceResponse<Transaction>((int)response.StatusCode, null, await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceResponse<Transaction>> PlaceSellOrder(string baseCoin, string targetCoin, double stack, string apiKey, string secret, double qty, double toSellFor, double? raisedChangeToSell = null, bool? testOnly = false)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceUrl);

                var reqUrl = string.Format(CultureInfo.InvariantCulture, "market/selllimit?apikey={0}&market={1}-{2}&quantity={3}&rate={4}&nonce={5}",
                    apiKey,
                    baseCoin,
                    targetCoin,
                    qty.ToString("0.00", CultureInfo.InvariantCulture),
                    raisedChangeToSell == null ? toSellFor.ToString("0.00000000", CultureInfo.InvariantCulture) : raisedChangeToSell.Value.ToString("0.00000000", CultureInfo.InvariantCulture),
                    Helpers.GetUnixTimeInSeconds()
                );

                var apiSign = Helpers.GetHashSHA512(_serviceUrl + reqUrl, secret);
                client.DefaultRequestHeaders.Add("apisign", apiSign);

                var response = await client.GetAsync(reqUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<ResultWrapper<TransactionMadeDTO>>(json);

                    if (deserialized.success)
                    {
                        return new ServiceResponse<Transaction>(0, _mapper.Map<Transaction>(deserialized.result));
                    }
                    else
                        throw new Exception(deserialized.message);
                }
                else
                    return new ServiceResponse<Transaction>((int)response.StatusCode, null, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
