using Newtonsoft.Json;
using Questao2.Models;

namespace Questao2.Services
{
    public interface IFootballMatchService
    {
        Task<int> GetTotalScoredGoalsByTeamAsync(string team, int year);
    }

    public class FootballMatchService : IFootballMatchService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public FootballMatchService(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
        }

        public async Task<int> GetTotalScoredGoalsByTeamAsync(string team, int year)
        {
            if (string.IsNullOrEmpty(team))
                throw new ArgumentException("O nome sdo Time não pode ser Vazio", nameof(team));

            if (year <= 0)
                throw new ArgumentException("Ano deve ser positivo", nameof(year));

            int totalGoals = 0;

            // time 1
            totalGoals += await GetGoalsAsTeam1Async(team, year);

            // time 2
            totalGoals += await GetGoalsAsTeam2Async(team, year);

            return totalGoals;
        }

        private async Task<int> GetGoalsAsTeam1Async(string team, int year)
        {
            return await GetGoalsByTeamPositionAsync(team, year, "team1");
        }

        private async Task<int> GetGoalsAsTeam2Async(string team, int year)
        {
            return await GetGoalsByTeamPositionAsync(team, year, "team2");
        }

        private async Task<int> GetGoalsByTeamPositionAsync(string team, int year, string teamPosition)
        {
            int totalGoals = 0;
            int page = 1;
            int totalPages = 1;

            try
            {
                do
                {
                    string url = $"{_apiUrl}?year={year}&{teamPosition}={team}&page={page}";
                    string response = await _httpClient.GetStringAsync(url);
                    var result = JsonConvert.DeserializeObject<ApiResponse>(response);

                    if (result != null && result.data != null)
                    {
                        totalPages = result.total_pages;

                        foreach (var match in result.data)
                        {
                            if (teamPosition == "team1" && !string.IsNullOrEmpty(match.team1goals))
                            {
                                totalGoals += match.Team1Goals;
                            }
                            else if (teamPosition == "team2" && !string.IsNullOrEmpty(match.team2goals))
                            {
                                totalGoals += match.Team2Goals;
                            }
                        }
                    }

                    page++;
                } while (page <= totalPages);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao buscar dados de partida: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Erro ao analisar dados de partidas: {ex.Message}", ex);
            }

            return totalGoals;
        }
    }
}