#!/usr/bin/env bash
# Runs the suite that backs the post's claims. Exits non-zero if any fail.
set -euo pipefail
cd "$(dirname "$0")"

echo "searchvalues-keyword-scan — checking the post's claims"
echo "  AgreementTests       the Contains loop, SearchValues and the generated Regex return the"
echo "                       same answer and the same first-match index, ordinal and ignore-case"
echo "  SearchValuesApiTests StringComparison restriction, the empty-string trap, the Teddy"
echo "                       implementation the runtime picks, and which-keyword recovery"
echo

dotnet test --configuration Release --logger "console;verbosity=normal"
