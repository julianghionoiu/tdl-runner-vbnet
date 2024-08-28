#!/usr/bin/env bash

set -x
set -e
set -u
set -o pipefail

SCRIPT_CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

CHALLENGE_ID=$1
CODE_COVERAGE_OUTPUT_FILE="${SCRIPT_CURRENT_DIR}/coverage.tdl"

dotnet build ${SCRIPT_CURRENT_DIR} || true 1>&2

coverlet_settings_file=$(mktemp)
cat << EOF > ${coverlet_settings_file}
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Include>[*]BeFaster.App.Solutions.${CHALLENGE_ID}.*</Include>
          <SingleHit>true</SingleHit>
          <SkipAutoProps>true</SkipAutoProps>
          <DeterministicReport>false</DeterministicReport>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
EOF

dotnet test --collect:"XPlat Code Coverage" --settings ${coverlet_settings_file} ${SCRIPT_CURRENT_DIR} || true 1>&2
last_test_run_id=$(ls -rt1 ${SCRIPT_CURRENT_DIR}/src/BeFaster.App.Tests/TestResults/  | tail -n 1)

# Make sense of the coverage report
coverage_report_file=${SCRIPT_CURRENT_DIR}/src/BeFaster.App.Tests/TestResults/${last_test_run_id}/coverage.cobertura.xml

## Special case to remove empty reports
lines_covered=$(xmllint ${coverage_report_file}  --xpath 'string(/coverage/@lines-covered)')
echo "lines_covered=${lines_covered}"
lines_valid=$(xmllint ${coverage_report_file}  --xpath 'string(/coverage/@lines-valid)')
echo "lines_valid=${lines_valid}"

if [ "${lines_valid}" -gt "0" ]; then
  printf "%.0f\n" $(echo "($lines_covered/$lines_valid)*100" | bc -l)   > ${CODE_COVERAGE_OUTPUT_FILE}
fi

cat ${CODE_COVERAGE_OUTPUT_FILE}
exit 0
