#pragma once

#ifdef _WIN32
  #define MATHLIB_API __declspec(dllexport)
#else
  #define MATHLIB_API __attribute__((visibility("default")))
#endif

extern "C" {
    MATHLIB_API double CalcMPG(double miles, double gallons);
    MATHLIB_API double CalcTripCost(double miles, double mpg, double pricePerGallon);
    MATHLIB_API double CalcRange(double tankGallons, double mpg);
    MATHLIB_API double CalcGallonsNeeded(double miles, double mpg);
    MATHLIB_API double CalcCO2Kg(double miles, double mpg);
    MATHLIB_API double MpgToL100km(double mpg);
    MATHLIB_API double MilesToKm(double miles);
    MATHLIB_API const char* GetVersion();
}
