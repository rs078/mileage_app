# MathCalc — C++ · .NET 8 · Angular 17

A full-stack calculator app showing how three technologies work together:

```
[Angular UI :4200]  ──HTTP──>  [.NET 8 API :5000]  ──P/Invoke──>  [C++ DLL]
```

| Layer    | Tech             | Role                                       |
|----------|------------------|--------------------------------------------|
| Frontend | Angular 17       | Reactive UI with Angular Material          |
| Backend  | ASP.NET Core 8   | REST API, calls C++ library via P/Invoke   |
| Engine   | C++ (native DLL) | Performs the actual math operations        |

Operations: Add · Subtract · Multiply · Divide · Power · Square Root · Factorial

---

## Quick Start

### Step 1 — Build the C++ DLL  *(optional)*

The backend falls back to a pure C# implementation if the DLL is absent — so this step is optional but recommended to see the full stack in action.

Requires either **Visual Studio Build Tools** (MSVC) or **MinGW** (g++) on your PATH.

```cmd
cd MathLib
build.bat
```

This creates `MathLib.dll` and copies it into `MathApi\`.

---

### Step 2 — Start the .NET API

```cmd
cd MathApi
dotnet run
```

API runs at **http://localhost:5000**

---

### Step 3 — Start the Angular UI

```cmd
cd math-ui
npm install
npm start
```

UI runs at **http://localhost:4200**

---

## API Reference

| Method | Endpoint                                     | Description                  |
|--------|----------------------------------------------|------------------------------|
| GET    | `/api/math/calculate?op=add&a=5&b=3`         | Perform a calculation        |
| GET    | `/api/math/version`                          | Show which engine is in use  |

**Supported `op` values:** `add`, `subtract`, `multiply`, `divide`, `power`, `sqrt`, `factorial`

**Example response:**
```json
{
  "operation":   "multiply",
  "a":           7,
  "b":           6,
  "result":      42,
  "processedBy": "MathLib v1.0 (C++)"
}
```

---

## Project Structure

```
MathLib/          C++ source + build script
  mathlib.h
  mathlib.cpp
  build.bat

MathApi/          ASP.NET Core Web API
  MathApi.csproj
  Program.cs
  NativeInterop/
    MathService.cs    P/Invoke wrapper + C# fallback
  Controllers/
    MathController.cs

math-ui/          Angular 17 frontend
  src/
    main.ts
    styles.scss
    app/
      app.component.ts / .html / .scss
      services/
        math.service.ts
```

---

## Notes

- The C# fallback is identical in behaviour to the C++ version — the only difference is which engine processes the request (shown in the UI).
- CORS is configured to allow `http://localhost:4200` only.
- To allow other origins, edit `Program.cs`.
