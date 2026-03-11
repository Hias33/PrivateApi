//intern:
using RandomWordApi_Vom_Hias.Data.Interfaces;

//extern:
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RandomWordApi_Vom_Hias.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoreController : Controller
    {

        public ScoreController(IDataManager pDataManager) 
        {
            _dataManager = pDataManager;
        }

        [HttpPost(Name ="AddScore")]
        public async Task AddScoreToDB([FromQuery][Required] string difficulty, [FromQuery][Required] int score)
        {
            if (difficulty != "easy" && difficulty != "medium" && difficulty != "hard")
            {
                throw new Exception("Invalid Difficulty!");
            }
            else
            {
                await _dataManager.PutScore(difficulty, score);
            }
        }

        private readonly IDataManager _dataManager;

    }
}
