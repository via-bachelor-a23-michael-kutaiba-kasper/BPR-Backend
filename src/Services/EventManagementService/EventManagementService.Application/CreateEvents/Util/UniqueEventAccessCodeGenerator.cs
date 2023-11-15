using System.Security.Cryptography;
using System.Text;

namespace EventManagementService.Application.CreateEvents.Util;

internal static class UniqueEventAccessCodeGenerator
{
    internal static string GenerateUniqueString(string title, DateTimeOffset creationDate)
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
}