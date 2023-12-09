using System.Security.Cryptography;
using System.Text;

namespace UserManagementServcie.Test.Shared.Builders;

public static class UniqueEventAccessCodeGenerator
{
    public static string GenerateUniqueString(string title, DateTimeOffset creationDate)
    {
        var combinedInfo = $"{title}_{creationDate.ToString("yyyyMMddHHmmssfffzzz")}";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInfo));

            // Convert the hashed bytes to a string
            var stringBuilder = new StringBuilder();
            foreach (var t in hashBytes)
            {
                stringBuilder.Append(t.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
    public static string GenerateUniqueStringForExternal(string title, string description, string hostId)
    {
        var combinedInfo = $"{title}_{description ?? ""}_{hostId}";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInfo));

            // Convert the hashed bytes to a string
            var stringBuilder = new StringBuilder();
            foreach (var t in hashBytes)
            {
                stringBuilder.Append(t.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}