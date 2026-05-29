using System.Net.Http.Json;

namespace MathApi.Services;

public sealed class GasPriceService(HttpClient http, IConfiguration config)
{
    private const double FallbackPrice = 3.45;

    public async Task<GasPriceResult> GetCurrentAsync(string? state = null)
    {
        var key = config["EIA_API_KEY"] ?? Environment.GetEnvironmentVariable("EIA_API_KEY");

        var area      = ResolveArea(state);
        var areaLabel = area == "NUS" ? "US National Average" : AreaLabel(state!, area);

        if (!string.IsNullOrWhiteSpace(key))
        {
            try
            {
                var url = "https://api.eia.gov/v2/petroleum/pri/gnd/data/" +
                          $"?api_key={key}&frequency=weekly&data[0]=value" +
                          $"&facets[product][]=EPM0&facets[duoarea][]={area}" +
                          "&sort[0][column]=period&sort[0][direction]=desc&length=1";

                var response = await http.GetFromJsonAsync<EiaResponse>(url);
                var price    = response?.Response?.Data?.FirstOrDefault()?.Value;

                if (price is not null)
                    return new GasPriceResult(price.Value, true, $"EIA Weekly — {areaLabel}");
            }
            catch { /* fall through to fallback */ }
        }

        return new GasPriceResult(FallbackPrice, false, "US National Average (estimated)");
    }

    // Maps a 2-letter state code to the best available EIA duoarea code.
    // Nine states have their own weekly series; the rest map to their PADD region.
    private static string ResolveArea(string? state)
    {
        if (string.IsNullOrWhiteSpace(state)) return "NUS";
        return state.Trim().ToUpperInvariant() switch
        {
            // States with individual EIA weekly series
            "CA" => "SCA", "CO" => "SCO", "FL" => "SFL",
            "MA" => "SMA", "MN" => "SMN", "NY" => "SNY",
            "OH" => "SOH", "TX" => "STX", "WA" => "SWA",
            // PADD 1 — East Coast
            "CT" or "DC" or "DE" or "GA" or "MD" or "ME" or "NC"
                or "NH" or "NJ" or "PA" or "RI" or "SC" or "VA"
                or "VT" or "WV" => "R10",
            // PADD 2 — Midwest
            "IA" or "IL" or "IN" or "KS" or "KY" or "MI" or "MO"
                or "ND" or "NE" or "OK" or "SD" or "TN" or "WI" => "R20",
            // PADD 3 — Gulf Coast
            "AL" or "AR" or "LA" or "MS" or "NM" => "R30",
            // PADD 4 — Rocky Mountain
            "ID" or "MT" or "UT" or "WY" => "R40",
            // PADD 5 — West Coast
            "AK" or "AZ" or "HI" or "NV" or "OR" => "R50",
            _ => "NUS"
        };
    }

    private static string AreaLabel(string state, string area) => area switch
    {
        "R10" => $"{state.ToUpperInvariant()} (East Coast avg)",
        "R20" => $"{state.ToUpperInvariant()} (Midwest avg)",
        "R30" => $"{state.ToUpperInvariant()} (Gulf Coast avg)",
        "R40" => $"{state.ToUpperInvariant()} (Rocky Mountain avg)",
        "R50" => $"{state.ToUpperInvariant()} (West Coast avg)",
        _     => $"{state.ToUpperInvariant()} avg",
    };
}

public sealed record GasPriceResult(double Price, bool Live, string Source);

internal sealed class EiaResponse  { public EiaData?           Response { get; set; } }
internal sealed class EiaData      { public List<EiaDataPoint>? Data     { get; set; } }
internal sealed class EiaDataPoint { public double?             Value    { get; set; } }
