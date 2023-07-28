using System.Security.Claims;
using System.Text.Json;

namespace UserManagement.UI.Helper;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        if (keyValuePairs == null) return claims;
        foreach (var keyValue in keyValuePairs)
        {
            if (keyValue.Key == ClaimTypes.Role)
            {
                var value = keyValue.Value.ToString();
                var isListRole = value?[0] == '[';
                var roles = isListRole ? value?.Substring(1, value.Length - 2).Split(","): new []{value} ;
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        var roleValue = isListRole ? role?.Substring(1, role.Length - 2): role;
                        if (roleValue != null) 
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
                continue;
            }
            if (keyValue.Key == "UserName")
            {
                claims.Add(new Claim("UserName",keyValue.Value.ToString() ?? ""));
                continue;
            }
            if (keyValue.Key == "UserId")
            {
                claims.Add(new Claim("UserId",keyValue.Value.ToString() ?? ""));
            }
        }
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
