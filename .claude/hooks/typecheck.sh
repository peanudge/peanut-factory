#!/bin/bash
set -euo pipefail

input=$(cat)
files=$(echo "$input" | jq -r '(.tool_input.file_path // (.tool_input.edits // [])[].file_path) // empty' 2>/dev/null)

echo "$files" | grep -qE '\.tsx?$' || exit 0

cd src/peanut-vision-ui
output=$(npm run type 2>&1)
code=$?
echo "$output" | head -50
exit $code
