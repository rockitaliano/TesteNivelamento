using Questao2.Services;

public class Program
{
    private const string API_URL = "https://jsonmock.hackerrank.com/api/football_matches";

    public static async Task Main()
    {
        var httpClient = new HttpClient();

        var footballService = new FootballMatchService(httpClient, API_URL);

        await RunExample(footballService, "Paris Saint-Germain", 2013);

        await RunExample(footballService, "Chelsea", 2014);
    }

    private static async Task RunExample(IFootballMatchService service, string teamName, int year)
    {
        try
        {
            int totalGoals = await service.GetTotalScoredGoalsByTeamAsync(teamName, year);
            Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no processamento em team {teamName}: {ex.Message}");
        }
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        var httpClient = new HttpClient();
        var service = new FootballMatchService(httpClient, API_URL);
        return service.GetTotalScoredGoalsByTeamAsync(team, year).Result;
    }

    public static async Task<int> getTotalScoredGoalsAsync(string team, int year)
    {
        var httpClient = new HttpClient();
        var service = new FootballMatchService(httpClient, API_URL);
        return await service.GetTotalScoredGoalsByTeamAsync(team, year);
    }
}

