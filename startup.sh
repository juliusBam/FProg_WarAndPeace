#!/bin/bash

# Set environment variables for file paths 
export BOOK_PATH="./Resources/book.txt"
export WAR_TERMS="./Resources/warTerms.txt"
export PEACE_TERMS="./Resources/peaceTerms.txt"
export OUTPUT_FILE="./Resources/output.txt"

# Compile project
dotnet restore
dotnet build

# Execute project
./FProg_WarAndPeace_GruppeG/bin/Debug/net8.0/FProg_WarAndPeace_GruppeG

reference="./Resources/reference.txt" 

# Check if files exist
if [ ! -f "$reference" ]; then
    echo "Reference file does not exist"
    exit 1
fi

if [ ! -f "$OUTPUT_FILE" ]; then
  echo "Output file cannot be found"
  exit 1
fi

# Use diff command to compare files and count differences
differences=$(diff -y --suppress-common-lines "$reference" "$OUTPUT_FILE" | wc -l)

echo "Number of differences: $differences"