using Microsoft.AspNetCore.Mvc;
using RandomWordApi_Vom_Hias;
using RandomWordApi_Vom_Hias.Data.Interfaces;

namespace PrivateApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherMetricController : Controller
    {
        public WeatherMetricController(IDataManager pDataManager)
        {
            _dataManager = pDataManager;
        }

        [HttpGet(Name = "GetMetrics")]
        public async Task<List<WeatherMetrics>> GetMetrics()
        {
            return await _dataManager.GetWeather();
        }

        private readonly IDataManager _dataManager;

    }
}
