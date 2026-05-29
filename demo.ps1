# MileCalc — Program Flow Demo
# Run from the repo root: .\demo.ps1
# Requires: .NET API running on :5000  (dotnet run inside MathApi/)

$API = "http://localhost:5000/api/mileage"

function Title($text) {
    Write-Host ""
    Write-Host ("=" * 60) -ForegroundColor DarkGray
    Write-Host "  $text" -ForegroundColor Cyan
    Write-Host ("=" * 60) -ForegroundColor DarkGray
}

function Step($text) {
    Write-Host ""
    Write-Host ">> $text" -ForegroundColor Yellow
}

function Note($text) {
    Write-Host "   $text" -ForegroundColor DarkGray
}

function Show($label, $value, $unit = "") {
    Write-Host ("   {0,-28} " -f $label) -NoNewline
    Write-Host "$value $unit" -ForegroundColor Green
}

function Call($url) {
    try {
        return Invoke-RestMethod -Uri $url -Method GET
    } catch {
        Write-Host "   ERROR: could not reach $url" -ForegroundColor Red
        Write-Host "   Is the .NET server running?  cd MathApi && dotnet run" -ForegroundColor Red
        exit 1
    }
}

# ─────────────────────────────────────────────────────────────
Title "STEP 0 — Startup  (Program.cs)"
# ─────────────────────────────────────────────────────────────
Note "Program.cs bootstraps three things on startup:"
Note "  1. MathService     — registered as a singleton (pure math, no I/O)"
Note "  2. GasPriceService — registered with HttpClient (calls EIA API or falls back)"
Note "  3. CORS policy     — allows Angular dev server on :4200 / :4201"
Note ""
Note "Kestrel listens on PORT env var, falls back to :5000 locally."
Note ""

Step "Confirming API is alive..."
$ver = Call "$API/version"
Show "Engine" $ver.engine
Note ""
Note "Request path:  Angular fetch  ->  MathController  ->  MathService  ->  JSON"

# ─────────────────────────────────────────────────────────────
Title "STEP 1 — MPG  (GET /api/mileage/mpg)"
# ─────────────────────────────────────────────────────────────
Note "User fills: Miles Driven + Gallons Used"
Note "Controller validates both > 0, then delegates to MathService.Mpg()"
Note ""
Note "  MathService.Mpg(miles, gallons):"
Note "    mpg    = miles / gallons"
Note "    l100km = 235.214 / mpg          (international equivalent)"
Note ""

$miles = 350; $gallons = 11.5
Step "Calling: miles=$miles  gallons=$gallons"
$r = Call "$API/mpg?miles=$miles&gallons=$gallons"
Show "Miles per Gallon"       ("{0:F1}" -f $r.mpg)      "MPG"
Show "Liters per 100 km"      ("{0:F1}" -f $r.l100km)   "L/100km"
Show "Miles driven"           $r.miles                   "mi"
Show "Gallons used"           $r.gallons                 "gal"

# ─────────────────────────────────────────────────────────────
Title "STEP 2 — Trip Cost  (GET /api/mileage/trip-cost)"
# ─────────────────────────────────────────────────────────────
Note "User fills: Trip Distance + Fuel Economy + Gas Price"
Note "The UI also has a 'Use Current Avg' button that first hits /gas-price"
Note "(see Step 5) and auto-fills the gas price field."
Note ""
Note "  MathService.TripCost(miles, mpg, ppg):"
Note "    gallons   = miles / mpg"
Note "    totalCost = gallons * ppg"
Note "    perMile   = totalCost / miles"
Note ""

$tm = 400; $tmpg = 32; $tppg = 3.65
Step "Calling: miles=$tm  mpg=$tmpg  ppg=`$$tppg"
$r = Call "$API/trip-cost?miles=$tm&mpg=$tmpg&ppg=$tppg"
Show "Total trip cost"        ("`${0:F2}" -f $r.totalCost)
Show "Gallons needed"         ("{0:F2}" -f $r.gallonsNeeded)   "gal"
Show "Cost per mile"          ("`${0:F3}" -f $r.costPerMile)

# ─────────────────────────────────────────────────────────────
Title "STEP 3 — Range  (GET /api/mileage/range)"
# ─────────────────────────────────────────────────────────────
Note "User fills: Tank Size + Fuel Economy"
Note ""
Note "  MathService.Range(tank, mpg):"
Note "    rangeMiles = tank * mpg"
Note "    rangeKm    = rangeMiles * 1.60934"
Note ""

$tank = 14.5; $rmpg = 30
Step "Calling: tank=$tank gal  mpg=$rmpg"
$r = Call "$API/range?tank=$tank&mpg=$rmpg"
Show "Range"                  ("{0:F0}" -f $r.rangeMiles)   "miles"
Show "Range"                  ("{0:F0}" -f $r.rangeKm)      "km"

# ─────────────────────────────────────────────────────────────
Title "STEP 4 — Emissions  (GET /api/mileage/emissions)"
# ─────────────────────────────────────────────────────────────
Note "User fills: Miles Driven + Fuel Economy"
Note ""
Note "  MathService.Emissions(miles, mpg):"
Note "    gallons     = miles / mpg"
Note "    co2Kg       = gallons * 8.887   (EPA: 8.887 kg CO2 per gallon)"
Note "    co2Lbs      = co2Kg * 2.20462"
Note "    treesNeeded = co2Kg / 21.77     (avg tree sequesters 21.77 kg/yr)"
Note ""

$em = 15000; $empg = 28
Step "Calling: miles=$em  mpg=$empg"
$r = Call "$API/emissions?miles=$em&mpg=$empg"
Show "CO2 emitted"            ("{0:F1}" -f $r.co2Kg)          "kg"
Show "CO2 emitted"            ("{0:F1}" -f $r.co2Lbs)         "lbs"
Show "Trees to offset (1yr)"  ("{0:F1}" -f $r.treesNeeded)    "trees"

# ─────────────────────────────────────────────────────────────
Title "STEP 5 — Gas Price  (GET /api/mileage/gas-price)"
# ─────────────────────────────────────────────────────────────
Note "Unique endpoint: delegates to GasPriceService, not MathService."
Note ""
Note "  GasPriceService.GetCurrentAsync(state?):"
Note "    if EIA_API_KEY env var is set:"
Note "      -> POST to api.eia.gov (US Energy Info Administration)"
Note "      -> maps state code to best EIA duoarea (9 states have own series;"
Note "         rest fall back to their PADD region)"
Note "    else:"
Note "      -> returns hardcoded fallback `$3.45 (national estimate)"
Note ""

Step "Calling: no state (national)"
$r = Call "$API/gas-price"
Show "Price"                  ("`${0:F2}" -f $r.price)         "/ gal"
Show "Live data"              $r.live
Show "Source"                 $r.source

# ─────────────────────────────────────────────────────────────
Title "STEP 6 — Compare / Savings  (GET /api/mileage/savings)"
# ─────────────────────────────────────────────────────────────
Note "User fills: Current MPG + New MPG + Annual Miles + Gas Price"
Note ""
Note "  MathService.Savings(currentMpg, newMpg, annualMiles, ppg):"
Note "    galSaved    = (annualMiles/currentMpg) - (annualMiles/newMpg)"
Note "    annualSave  = galSaved * ppg"
Note "    co2Saved    = galSaved * 8.887"
Note "    mpgImprove  = (newMpg - currentMpg) / currentMpg * 100"
Note ""

$cur = 25; $new = 40; $ann = 12000; $ppg = 3.50
Step "Calling: currentMpg=$cur  newMpg=$new  annualMiles=$ann  ppg=`$$ppg"
$r = Call "$API/savings?currentMpg=$cur&newMpg=$new&annualMiles=$ann&ppg=$ppg"
Show "Annual fuel savings"    ("`${0:F2}" -f $r.annualSavings)
Show "Gallons saved / year"   ("{0:F1}" -f $r.gallonsSaved)     "gal"
Show "CO2 avoided / year"     ("{0:F1}" -f $r.co2SavedKg)       "kg"
Show "MPG improvement"        ("{0:F1}%" -f $r.mpgImprovement)

# ─────────────────────────────────────────────────────────────
Title "DONE"
# ─────────────────────────────────────────────────────────────
Note "All six endpoints exercised successfully."
Note ""
Note "Full request lifecycle:"
Note "  Angular AppComponent"
Note "    -> MileageService (HttpClient)"
Note "      -> MathController (validates inputs)"
Note "        -> MathService (pure math)  or  GasPriceService (EIA API)"
Note "          -> JSON response"
Note "            -> stat cards rendered in the UI"
Note ""
