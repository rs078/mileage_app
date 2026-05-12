@echo off
echo ============================================
echo  Building MathLib C++ DLL
echo ============================================

REM Try MSVC (Visual Studio)
where cl >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo [MSVC detected]
    cl /LD /O2 /EHsc mathlib.cpp /Fe:MathLib.dll
    if %ERRORLEVEL% == 0 (
        copy /Y MathLib.dll ..\MathApi\MathLib.dll >nul
        echo SUCCESS: MathLib.dll built and copied to MathApi\
    ) else (
        echo FAILED: Compilation error.
    )
    goto :done
)

REM Try MinGW / GCC
where g++ >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo [MinGW g++ detected]
    g++ -shared -O2 -o MathLib.dll mathlib.cpp
    if %ERRORLEVEL% == 0 (
        copy /Y MathLib.dll ..\MathApi\MathLib.dll >nul
        echo SUCCESS: MathLib.dll built and copied to MathApi\
    ) else (
        echo FAILED: Compilation error.
    )
    goto :done
)

echo ERROR: No C++ compiler found.
echo Install Visual Studio Build Tools or MinGW and retry.
echo The .NET API will fall back to a pure C# implementation.

:done
pause
