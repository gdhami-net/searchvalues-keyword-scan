# Runs the suite that backs the post's claims. Exits non-zero if any fail.
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

Write-Host 'searchvalues-keyword-scan - checking the post''s claims'
Write-Host '  AgreementTests       the Contains loop, SearchValues and the generated Regex return the'
Write-Host '                       same answer and the same first-match index, ordinal and ignore-case'
Write-Host '  SearchValuesApiTests StringComparison restriction, the empty-string trap, the Teddy'
Write-Host '                       implementation the runtime picks, and which-keyword recovery'
Write-Host ''

dotnet test --configuration Release --logger 'console;verbosity=normal'
exit $LASTEXITCODE
