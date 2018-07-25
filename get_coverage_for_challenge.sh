#!/usr/bin/env bash

set -x
set -e
set -u
set -o pipefail

SCRIPT_CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

CHALLENGE_ID=$1
VBNET_TEST_COVERAGE_DIR="${SCRIPT_CURRENT_DIR}/coverage"
VBNET_INSTRUMENTED_COVERAGE_REPORT="${VBNET_TEST_COVERAGE_DIR}/instrumented-coverage.xml"
VBNET_TEST_RUN_REPORT="${VBNET_TEST_COVERAGE_DIR}/test-report.xml"
VBNET_CODE_COVERAGE_INFO="${SCRIPT_CURRENT_DIR}/coverage.tdl"

exitAfterNoCoverageReportFoundError() {
  echo "No coverage report was found"
  exit -1
}

mkdir -p ${VBNET_TEST_COVERAGE_DIR}

( cd ${SCRIPT_CURRENT_DIR} && \
     nuget restore befaster.sln )

(
    cd ${SCRIPT_CURRENT_DIR} && \
        msbuild ${SCRIPT_CURRENT_DIR}/befaster.sln                \
            /p:buildmode=debug /p:TargetFrameworkVersion=v4.5 || true
)

[ -e ${SCRIPT_CURRENT_DIR}/__Instrumented ] && rm -fr ${SCRIPT_CURRENT_DIR}/__Instrumented
[ -e ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover ] && rm -fr ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover

(
    cd ${SCRIPT_CURRENT_DIR} && \
    mono ${SCRIPT_CURRENT_DIR}/packages/altcover.3.5.569/tools/net45/AltCover.exe \
      --opencover --linecover                                                     \
      --inputDirectory ${SCRIPT_CURRENT_DIR}/src/BeFaster.App.Tests/bin/Debug     \
      --assemblyFilter=Adapter                                                    \
      --assemblyFilter=Mono                                                       \
      --assemblyFilter=\.Recorder                                                 \
      --assemblyFilter=Sample                                                     \
      --assemblyFilter=nunit                                                      \
      --assemblyFilter=Tests                                                      \
      --assemblyExcludeFilter=.+\.Tests                                           \
      --assemblyExcludeFilter=AltCover.+                                          \
      --assemblyExcludeFilter=Mono\.DllMap.+                                      \
      --typeFilter=System.                                                        \
      --outputDirectory=${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover              \
      --xmlReport=${VBNET_INSTRUMENTED_COVERAGE_REPORT} || true
)

(
  cd ${SCRIPT_CURRENT_DIR} && \
    mono ${SCRIPT_CURRENT_DIR}/packages/altcover.3.5.569/tools/net45/AltCover.exe Runner                \
        --executable ${SCRIPT_CURRENT_DIR}/packages/NUnit.ConsoleRunner.3.7.0/tools/nunit3-console.exe  \
        --recorderDirectory ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover/                               \
        -w ${SCRIPT_CURRENT_DIR}                                                                        \
        -- --noheader --labels=All  --work=${SCRIPT_CURRENT_DIR}                                        \
        --result=${VBNET_TEST_RUN_REPORT}                                                     \
        ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover/BeFaster.App.Tests.dll || true
)

[ -e ${VBNET_CODE_COVERAGE_INFO} ] && rm ${VBNET_CODE_COVERAGE_INFO}

if [ -f "${VBNET_INSTRUMENTED_COVERAGE_REPORT}" ]; then
    TOTAL_COVERAGE_PERCENTAGE=0
    COVERAGE_IN_PACKAGE=$(xmllint ${VBNET_INSTRUMENTED_COVERAGE_REPORT} \
                                  --xpath '//Class[starts-with(./FullName,"BeFaster.App.Solutions.'${CHALLENGE_ID}'")]' || true)

   if [[ -z "${COVERAGE_IN_PACKAGE}" ]]; then
      echo $((TOTAL_COVERAGE_PERCENTAGE)) > ${VBNET_CODE_COVERAGE_INFO}
      cat ${VBNET_CODE_COVERAGE_INFO}
      exit 0
   fi

    COVERAGE_SUMMARY_FILE=${VBNET_TEST_COVERAGE_DIR}/coverage-summary-${CHALLENGE_ID}.xml
    echo "<xml>${COVERAGE_IN_PACKAGE}</xml>"               \
         | xmllint --xpath '//Class/Summary' - 2>/dev/null \
         | sed "s/<Summary/\\n<Summary/g" | sed '/^$/d' | sed -e '$a\' > ${COVERAGE_SUMMARY_FILE}

    while read -r eachSummaryLine
    do
        numMethods=$(echo ${eachSummaryLine} | xmllint --xpath 'string(//Summary/@numMethods)' - || true)
        visitedMethods=$(echo ${eachSummaryLine} | xmllint --xpath 'string(//Summary/@visitedMethods)' - || true)
        if [[ -z "${numMethods}" ]] || [[ ${numMethods} -eq 0 ]] || [[ -z "${visitedMethods}" ]] || [[ ${visitedMethods} -eq 0 ]]; then
            sequenceCoverage=0
        else
            sequenceCoverage=$(( 100 * ${visitedMethods} / ${numMethods} ))
        fi

        TOTAL_COVERAGE_PERCENTAGE=$(( ${TOTAL_COVERAGE_PERCENTAGE} + ${sequenceCoverage} ))
    done < ${COVERAGE_SUMMARY_FILE}

    summaryLines=$(wc -l < ${COVERAGE_SUMMARY_FILE})

    if [[ ${summaryLines} -eq 0 ]]; then
        TOTAL_COVERAGE_PERCENTAGE=0
    else
        TOTAL_COVERAGE_PERCENTAGE=$(( ${TOTAL_COVERAGE_PERCENTAGE} / ${summaryLines} ))
    fi

    if [[ -z "${TOTAL_COVERAGE_PERCENTAGE}" ]]; then
       TOTAL_COVERAGE_PERCENTAGE=0
    fi
    
    echo $((TOTAL_COVERAGE_PERCENTAGE)) > ${VBNET_CODE_COVERAGE_INFO}
    cat ${VBNET_CODE_COVERAGE_INFO}
    exit 0
else
    exitAfterNoCoverageReportFoundError
fi
