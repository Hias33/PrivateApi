//intern:
using RandomWordApi_Vom_Hias.Data.Interfaces;

//extern:
using Npgsql;
using Newtonsoft.Json.Linq;
using PrivateApi;

namespace RandomWordApi_Vom_Hias.Data
{
    public class DataManager : IDataManager
    {
        public DataManager(IConfiguration pConfig)
        {
            var secretsPath = Path.Combine(Directory.GetCurrentDirectory(), "secrets.json");
            var json = File.ReadAllText(secretsPath);
            var jsonObject = JObject.Parse(json);
            wordConnection = (string)jsonObject["Data"]["ConnectionStrings"]["randomWordDB"];
            iotConnection = (string)jsonObject["Data"]["ConnectionStrings"]["sensorNetworkDB"];
        }

        public async Task<Word> GetWords(string pTable, int pNumber, int minLength, int maxLength)
        {
            List<string> words = new List<string>();

            using (var connection = new NpgsqlConnection(wordConnection))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Verbindung zur Datenbank erfolgreich!");

                    string query = $"SELECT words FROM {pTable} WHERE LENGTH(words) BETWEEN {minLength} AND {maxLength} AND words !~ '[äöü]' ORDER BY RANDOM() LIMIT {pNumber};";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                words.Add(reader.GetString(reader.GetOrdinal("words")));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Verbindung geschlossen.");
                }
            }
            return new Word { words = words };
        }

        public async Task<List<WeatherMetrics>> GetWeather()
        {
            List<WeatherMetrics> metrics = new List<WeatherMetrics>();

            using (var connection = new NpgsqlConnection(iotConnection))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Verbindung zur Datenbank erfolgreich!");

                    string query = @"SELECT temperature, humidity, timestamp AT TIME ZONE 'Europe/Berlin' AS timestamp
                                     FROM airmetrics
                                     WHERE timestamp >= (NOW() AT TIME ZONE 'Europe/Berlin') - INTERVAL '2 days';";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                metrics.Add(new WeatherMetrics
                                {
                                    Temperature = reader.GetDouble(reader.GetOrdinal("temperature")),
                                    Humidity = reader.GetDouble(reader.GetOrdinal("humidity")),
                                    TimeStamp = reader.GetDateTime(reader.GetOrdinal("timestamp"))
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Verbindung geschlossen.");
                }
            }
            return metrics;
        }



        public async Task PutScore(string pDifficulty, int pScore)
        {
            string query = $@"INSERT INTO scores_{pDifficulty}_db (score) VALUES({pScore})";

            using (var connection = new NpgsqlConnection(wordConnection))
            {
                connection.Open();
                try
                {
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Score hinzugefügt.");
                }
            }
        }



        private readonly string wordConnection;
        private readonly string iotConnection;
    }
}
