using System.Runtime.InteropServices;

namespace MathApi.NativeInterop;

public sealed class MathService
{
    private readonly bool _native;

    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double CalcMPG(double miles, double gallons);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double CalcTripCost(double miles, double mpg, double ppg);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double CalcRange(double tank, double mpg);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double CalcGallonsNeeded(double miles, double mpg);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double CalcCO2Kg(double miles, double mpg);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double MpgToL100km(double mpg);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl)] private static extern double MilesToKm(double miles);
    [DllImport("MathLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    [return: MarshalAs(UnmanagedType.LPStr)] private static extern string GetVersion();

    public MathService()
    {
        try { GetVersion(); _native = true; }
        catch { _native = false; }
    }

    public string Engine => _native ? GetVersion() : "MileageLib v1.0 (C# fallback)";

    public object Mpg(double miles, double gallons)
    {
        double mpg    = _native ? CalcMPG(miles, gallons)   : miles / gallons;
        double l100km = _native ? MpgToL100km(mpg)          : 235.214 / mpg;
        return new { mpg = R(mpg), l100km = R(l100km), miles, gallons, processedBy = Engine };
    }

    public object TripCost(double miles, double mpg, double ppg)
    {
        double gal  = _native ? CalcGallonsNeeded(miles, mpg) : miles / mpg;
        double cost = _native ? CalcTripCost(miles, mpg, ppg) : gal * ppg;
        return new { totalCost = R(cost), gallonsNeeded = R(gal), costPerMile = R(cost / miles), miles, mpg, pricePerGallon = ppg, processedBy = Engine };
    }

    public object Range(double tank, double mpg)
    {
        double mi = _native ? CalcRange(tank, mpg)  : tank * mpg;
        double km = _native ? MilesToKm(mi)          : mi * 1.60934;
        return new { rangeMiles = R(mi), rangeKm = R(km), tank, mpg, processedBy = Engine };
    }

    public object Emissions(double miles, double mpg)
    {
        double kg  = _native ? CalcCO2Kg(miles, mpg) : (miles / mpg) * 8.887;
        double lbs = kg * 2.20462;
        double trees = kg / 21.77;
        return new { co2Kg = R(kg), co2Lbs = R(lbs), treesNeeded = R(trees), miles, mpg, processedBy = Engine };
    }

    private static double R(double v) => double.IsNaN(v) || double.IsInfinity(v) ? v : Math.Round(v, 3);
}
