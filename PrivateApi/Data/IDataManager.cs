using PrivateApi;

namespace RandomWordApi_Vom_Hias.Data.Interfaces
{
    public interface IDataManager
    {
        Task<Word> GetWords(string pTable, int pNumber, int minLength, int maxLength);

        Task PutScore(string pDifficulty, int pScore);

        Task<List<WeatherMetrics>> GetWeather();
    }
}
