## Remarks

The application is structured as a solution containing two projects:
- FProg_WarAndPeace_GruppeG
- FProg_WarAndPeace_GruppeGTests

The reference.txt file was downloaded from moodle (https://moodle.technikum-wien.at/pluginfile.php/2059280/mod_resource/content/0/output.txt) and a new line was added at the end of the file, in order to avoid a discrepancy in the file format between the reference and the output.

## Requirements:

- The CLI tool "diff" must be installed https://phoenixnap.com/kb/linux-diff
- The .NET runtime v8 has to installed in order to execute the compiled application https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Test-run execution

The bash script `startup.sh` is provided to run the program and automatically compare the results with the provided reference. It will output the time elapsed for the parsing of the book content as well as for the computation of the chapters' contents.

The bash script can be called with one option, namely `--build` or `-b` this will compile the solution, run the tests and run the newly compiled binaries, for example:

`./startup.sh --build` or `./startup.sh -b` --> The usage of the build option requires the .NET 8.0 SKD to be installed

### Troubleshooting

If the provided script does not work (it is possible that the compiled .dll files have to be compiled once again in the target machine), following points should be checked:

- The .NET SDK v8 has to be installed https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- The solution FProg_WarAndPeace_GruppeG has to be built in "release" profile
- `dotnet` should be added to the `$PATH` environment variable
- The environment variables used for the file locations in `startup.sh` should be checked and corrected if necessary