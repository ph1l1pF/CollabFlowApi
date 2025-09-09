using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

public class AppleKey
{
    public string kty { get; set; }
    public string kid { get; set; }
    public string use { get; set; }
    public string alg { get; set; }
    public string n { get; set; }
    public string e { get; set; }
}

public static class AppleKeysProvider
{
    private static List<AppleKey> _keys = [];

    public static async Task<List<AppleKey>> GetKeysAsync()
    {
        if (_keys.Count != 0) return _keys;

        using var http = new HttpClient();
        var response = await http.GetFromJsonAsync<JsonElement>("https://appleid.apple.com/auth/keys");
        var keys = JsonSerializer.Deserialize<List<AppleKey>>(response.GetProperty("keys").GetRawText());
        _keys = keys;
        return _keys;
    }
}