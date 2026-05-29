namespace MathApi.Services;

public sealed class MathService
{
    public string Engine => "MileCalc v1.0";

    public MpgResult Mpg(double miles, double gallons)
    {
        double mpg = miles / gallons;
        return new MpgResult(R(mpg), R(235.214 / mpg), miles, gallons, Engine);
    }

    public TripCostResult TripCost(double miles, double mpg, double ppg)
    {
        double gal  = miles / mpg;
        double cost = gal * ppg;
        return new TripCostResult(R(cost), R(gal), R(cost / miles), miles, mpg, ppg, Engine);
    }

    public RangeResult Range(double tank, double mpg)
    {
        double mi = tank * mpg;
        return new RangeResult(R(mi), R(mi * 1.60934), tank, mpg, Engine);
    }

    public EmissionsResult Emissions(double miles, double mpg)
    {
        double kg = (miles / mpg) * 8.887;
        return new EmissionsResult(R(kg), R(kg * 2.20462), R(kg / 21.77), miles, mpg, Engine);
    }

    public SavingsResult Savings(double currentMpg, double newMpg, double annualMiles, double ppg)
    {
        double galSaved  = (annualMiles / currentMpg) - (annualMiles / newMpg);
        double costSaved = galSaved * ppg;
        double co2Saved  = galSaved * 8.887;
        double pctImprove = (newMpg - currentMpg) / currentMpg * 100;
        return new SavingsResult(R(costSaved), R(galSaved), R(co2Saved), R(pctImprove), currentMpg, newMpg, annualMiles, ppg, Engine);
    }

    private static double R(double v) => double.IsNaN(v) || double.IsInfinity(v) ? v : Math.Round(v, 3);
}

public sealed record MpgResult(double Mpg, double L100km, double Miles, double Gallons, string ProcessedBy);
public sealed record TripCostResult(double TotalCost, double GallonsNeeded, double CostPerMile, double Miles, double Mpg, double PricePerGallon, string ProcessedBy);
public sealed record RangeResult(double RangeMiles, double RangeKm, double Tank, double Mpg, string ProcessedBy);
public sealed record EmissionsResult(double Co2Kg, double Co2Lbs, double TreesNeeded, double Miles, double Mpg, string ProcessedBy);
public sealed record SavingsResult(double AnnualSavings, double GallonsSaved, double Co2SavedKg, double MpgImprovement, double CurrentMpg, double NewMpg, double AnnualMiles, double Ppg, string ProcessedBy);