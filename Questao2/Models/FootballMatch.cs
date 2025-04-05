namespace Questao2.Models
{
    public class ApiResponse
    {
        public int total_pages { get; set; }
        public List<FootballMatch>? data { get; set; }
    }

    public class FootballMatch
    {
        public string? team1goals { get; set; }
        public string? team2goals { get; set; }

        public int Team1Goals => string.IsNullOrEmpty(team1goals) ? 0 : int.Parse(team1goals);
        public int Team2Goals => string.IsNullOrEmpty(team2goals) ? 0 : int.Parse(team2goals);
    }
}
