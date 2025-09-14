using System.Text.Json;
namespace GPlus_V2_Redesign.Source
{
    public static class Steam
    {
        public static async Task<string?> GetSteamUsernameAsync(string steamId64)
        {
            using var http = new HttpClient();
            var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={SettingsManager.CurrentSettings.General.SteamAPIKey}&steamids={steamId64}";
            var json = await http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            var players = doc.RootElement
                             .GetProperty("response")
                             .GetProperty("players");

            if (players.GetArrayLength() == 0) return null; 
            return players[0].GetProperty("personaname").GetString();
        }
    }
}
