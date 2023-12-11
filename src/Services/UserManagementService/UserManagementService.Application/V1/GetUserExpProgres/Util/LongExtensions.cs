namespace UserManagementService.Application.V1.GetUserExpProgres.Util;

public static class LongExtensions
{

   public static bool IsBetween(this long value, long lower, long upper, bool includeUpper = false)
   {
      return includeUpper ? value >= lower && value <= upper : value >= lower && value < upper;
   }
}