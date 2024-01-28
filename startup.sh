#!/bin/bash

# Set environment variables for file paths 
export BOOK_PATH="./Resources/book.txt"
export WAR_TERMS="./Resources/warTerms.txt"
export PEACE_TERMS="./Resources/peaceTerms.txt"
export OUTPUT_FILE="./output.txt"
export RUNNABLE_PATH="./FProg_WarAndPeace_GruppeG/bin/Release/net8.0/FProg_WarAndPeace_GruppeG"

# Compile project --> needed if compiled project does not work
# requires installation of .NET 8 SDK and dotnet in $PATH variable
if [ "$1" = "--build" ] || [ "$1" = "-b" ]; then
    echo "The project's solution will be built"
    dotnet restore
    dotnet build -c Release
    dotnet test
else
    echo "The project solution will not be rebuilt, using present binaries instead"
fi

#Check if binaries exists
if [ ! -f "$RUNNABLE_PATH" ]; then
    echo "Runnable file not found"
    exit 1
fi

# Execute project
$RUNNABLE_PATH

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