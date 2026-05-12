#include "mathlib.h"
#include <cmath>
#include <limits>

extern "C" {

MATHLIB_API double CalcMPG(double miles, double gallons) {
    if (gallons <= 0.0) return std::numeric_limits<double>::quiet_NaN();
    return miles / gallons;
}

MATHLIB_API double CalcTripCost(double miles, double mpg, double pricePerGallon) {
    if (mpg <= 0.0) return std::numeric_limits<double>::quiet_NaN();
    return (miles / mpg) * pricePerGallon;
}

MATHLIB_API double CalcRange(double tankGallons, double mpg) {
    return tankGallons * mpg;
}

MATHLIB_API double CalcGallonsNeeded(double miles, double mpg) {
    if (mpg <= 0.0) return std::numeric_limits<double>::quiet_NaN();
    return miles / mpg;
}

// 8.887 kg CO2 per gallon of gasoline (EPA)
MATHLIB_API double CalcCO2Kg(double miles, double mpg) {
    if (mpg <= 0.0) return std::numeric_limits<double>::quiet_NaN();
    return (miles / mpg) * 8.887;
}

// 235.214 liters-per-100km conversion factor
MATHLIB_API double MpgToL100km(double mpg) {
    if (mpg <= 0.0) return std::numeric_limits<double>::quiet_NaN();
    return 235.214 / mpg;
}

MATHLIB_API double MilesToKm(double miles) {
    return miles * 1.60934;
}

MATHLIB_API const char* GetVersion() {
    return "MileageLib v1.0 (C++)";
}

} // extern "C"
