$srcRoot = "$($PSScriptRoot)\..\src"

function Build-TestServer {
    $testServerRoot = Join-Path $srcRoot "Test_Server"
    Push-Location $testServerRoot
    dotnet build /p:Configuration=Release
    Pop-Location
}
