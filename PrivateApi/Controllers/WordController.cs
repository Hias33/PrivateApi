//intern:
using RandomWordApi_Vom_Hias.Data.Interfaces;

//extern:
using Microsoft.AspNetCore.Mvc;

namespace RandomWordApi_Vom_Hias.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordController : ControllerBase
    {
        public WordController(IDataManager pDataManager)
        {
            _dataManager = pDataManager;
        }

        [HttpGet(Name = "GetWord")]
        public async Task<Word> GetWord(string? lang = null, int? number = null, int? min = null, int? max = null)
        {
            string table = lang switch
            {
                "de" => "german_db",
                "en" => "english_db",
                "it" => "italian_db",
                _ => "german_db" // Default fallback
            };

            int finalNumber = number ?? 1;
            int finalMin = min ?? 1;
            int finalMax = max ?? 100;
            string finalLang = lang ?? "de";

            return await _dataManager.GetWords(table, finalNumber, finalMin, finalMax);
        }

        private readonly IDataManager _dataManager;
    }
}
