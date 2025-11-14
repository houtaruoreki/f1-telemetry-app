namespace F1TelemetryApp.Converters;

using System.Globalization;

/// <summary>
/// Converts ISO country codes to flag emojis.
/// Uses Unicode regional indicator symbols.
/// </summary>
public class CountryCodeToFlagConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string countryCode || string.IsNullOrWhiteSpace(countryCode))
            return "🏴"; // Default flag for unknown

        // Convert country code to uppercase
        countryCode = countryCode.ToUpper().Trim();

        // Handle special cases and common codes
        var flag = countryCode switch
        {
            "NED" or "NL" => "🇳🇱", // Netherlands
            "UK" or "GB" or "GBR" => "🇬🇧", // United Kingdom
            "USA" or "US" => "🇺🇸", // United States
            "UAE" or "AE" => "🇦🇪", // United Arab Emirates
            "CHN" or "CN" => "🇨🇳", // China
            "JPN" or "JP" => "🇯🇵", // Japan
            "AUS" or "AU" => "🇦🇺", // Australia
            "BEL" or "BE" => "🇧🇪", // Belgium
            "CAN" or "CA" => "🇨🇦", // Canada
            "FRA" or "FR" => "🇫🇷", // France
            "DEU" or "DE" or "GER" => "🇩🇪", // Germany
            "ITA" or "IT" => "🇮🇹", // Italy
            "MEX" or "MX" => "🇲🇽", // Mexico
            "MON" or "MC" => "🇲🇨", // Monaco
            "ESP" or "ES" => "🇪🇸", // Spain
            "THA" or "TH" => "🇹🇭", // Thailand
            "FIN" or "FI" => "🇫🇮", // Finland
            "DNK" or "DK" => "🇩🇰", // Denmark
            "SWE" or "SE" => "🇸🇪", // Sweden
            "CHE" or "CH" => "🇨🇭", // Switzerland
            "AUT" or "AT" => "🇦🇹", // Austria
            "POL" or "PL" => "🇵🇱", // Poland
            "RUS" or "RU" => "🇷🇺", // Russia
            "SGP" or "SG" => "🇸🇬", // Singapore
            "BRA" or "BR" => "🇧🇷", // Brazil
            "ARG" or "AR" => "🇦🇷", // Argentina
            "COL" or "CO" => "🇨🇴", // Colombia
            "NZL" or "NZ" => "🇳🇿", // New Zealand
            "ZAF" or "ZA" => "🇿🇦", // South Africa
            "HUN" or "HU" => "🇭🇺", // Hungary
            "CZE" or "CZ" => "🇨🇿", // Czech Republic
            "PRT" or "PT" => "🇵🇹", // Portugal
            "IRL" or "IE" => "🇮🇪", // Ireland
            "KOR" or "KR" => "🇰🇷", // South Korea
            "IND" or "IN" => "🇮🇳", // India
            "IDN" or "ID" => "🇮🇩", // Indonesia
            "MYS" or "MY" => "🇲🇾", // Malaysia
            "VNM" or "VN" => "🇻🇳", // Vietnam
            "TUR" or "TR" => "🇹🇷", // Turkey
            "GRC" or "GR" => "🇬🇷", // Greece
            "HRV" or "HR" => "🇭🇷", // Croatia
            "SVN" or "SI" => "🇸🇮", // Slovenia
            "SVK" or "SK" => "🇸🇰", // Slovakia
            "ROU" or "RO" => "🇷🇴", // Romania
            "BGR" or "BG" => "🇧🇬", // Bulgaria
            _ => ConvertToFlagEmoji(countryCode)
        };

        return flag;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converts a 2 or 3-letter country code to a flag emoji using Unicode regional indicators.
    /// </summary>
    private static string ConvertToFlagEmoji(string countryCode)
    {
        // If it's a 3-letter code, try to get the 2-letter equivalent
        if (countryCode.Length == 3)
        {
            countryCode = Get2LetterCode(countryCode);
        }

        // Only process 2-letter codes
        if (countryCode.Length != 2)
            return "🏴";

        // Convert to regional indicator symbols
        // Regional indicators start at U+1F1E6 (A) to U+1F1FF (Z)
        var first = char.ToUpper(countryCode[0]);
        var second = char.ToUpper(countryCode[1]);

        if (!char.IsLetter(first) || !char.IsLetter(second))
            return "🏴";

        // Calculate Unicode regional indicator symbols
        var firstIndicator = 0x1F1E6 + (first - 'A');
        var secondIndicator = 0x1F1E6 + (second - 'A');

        return char.ConvertFromUtf32(firstIndicator) + char.ConvertFromUtf32(secondIndicator);
    }

    /// <summary>
    /// Converts 3-letter ISO codes to 2-letter codes.
    /// </summary>
    private static string Get2LetterCode(string code)
    {
        return code switch
        {
            "NED" => "NL",
            "GBR" => "GB",
            "USA" => "US",
            "UAE" => "AE",
            "CHN" => "CN",
            "JPN" => "JP",
            "AUS" => "AU",
            "BEL" => "BE",
            "CAN" => "CA",
            "FRA" => "FR",
            "DEU" or "GER" => "DE",
            "ITA" => "IT",
            "MEX" => "MX",
            "MON" => "MC",
            "ESP" => "ES",
            "THA" => "TH",
            "FIN" => "FI",
            "DNK" => "DK",
            "SWE" => "SE",
            "CHE" => "CH",
            "AUT" => "AT",
            "POL" => "PL",
            "RUS" => "RU",
            "SGP" => "SG",
            "BRA" => "BR",
            "ARG" => "AR",
            "COL" => "CO",
            "NZL" => "NZ",
            "ZAF" => "ZA",
            "HUN" => "HU",
            "CZE" => "CZ",
            "PRT" => "PT",
            "IRL" => "IE",
            "KOR" => "KR",
            "IND" => "IN",
            "IDN" => "ID",
            "MYS" => "MY",
            "VNM" => "VN",
            "TUR" => "TR",
            "GRC" => "GR",
            "HRV" => "HR",
            "SVN" => "SI",
            "SVK" => "SK",
            "ROU" => "RO",
            "BGR" => "BG",
            _ => code.Length >= 2 ? code.Substring(0, 2) : code
        };
    }
}
