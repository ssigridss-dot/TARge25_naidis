
namespace GemApp.Models;

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = "Mängija";

    public int Score { get; set; }

    public DateTime PlayedAt { get; set; } = DateTime.Now;
}
