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

function insertVbcTag() {
    targetProjectFile=$1
    foundVbcTag=$(grep -i '<VbcToolExe>vbc</VbcToolExe>' ${targetProjectFile} || true)
    if [[ -z "${foundVbcTag}" ]]; then
       # We need to do this, or else xmlstarlet does not parse and amend the XML file
       # And there is no equivalent xmlstarlet command for it, its a bit of a catch 22
       if [ "$(uname)" == "Darwin" ]; then
          sed -i -e "s/ xmlns.*=[\"].*[\"]//g" ${targetProjectFile}
       else
          sed -i 's/ xmlns.*=".*"//g' ${targetProjectFile}        
       fi
       ## This looks like a round about way to add a new node 'PropertyGroupVbc' and then rename it, because of the presence of multiple 'PropertyGroup' nodes
       xmlstarlet ed --inplace --subnode '/Project' --type elem --name PropertyGroupVbc --value "" ${targetProjectFile}
       xmlstarlet ed --inplace --subnode '//PropertyGroupVbc' --type elem --name VbcToolExe --value "vbc" ${targetProjectFile}
       xmlstarlet ed --inplace --rename  '/Project/PropertyGroupVbc' -v 'PropertyGroup' ${targetProjectFile}

       ## Add the attribute back or else build tools fail
       xmlstarlet ed --inplace --subnode '/Project' --type attr -n xmlns -v "http://schemas.microsoft.com/developer/msbuild/2003" ${targetProjectFile}
    fi
}

insertVbcTag "${SCRIPT_CURRENT_DIR}/src/BeFaster.App/BeFaster.App.vbproj"
insertVbcTag "${SCRIPT_CURRENT_DIR}/src/BeFaster.Runner/BeFaster.Runner.csproj"
insertVbcTag "${SCRIPT_CURRENT_DIR}/src/BeFaster.App.Tests/BeFaster.App.Tests.vbproj"

(
    cd ${SCRIPT_CURRENT_DIR} && \
        msbuild ${SCRIPT_CURRENT_DIR}/befaster.sln                \
            /p:buildmode=debug /p:TargetFrameworkVersion=v4.5 || true
)

[ -e ${SCRIPT_CURRENT_DIR}/__Instrumented ] && rm -fr ${SCRIPT_CURRENT_DIR}/__Instrumented
[ -e ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover ] && rm -fr ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover

# Instrument the binaries so that coverage can be collected
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

# Run the tests against the instrumented binaries
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
    COVERAGE_SUMMARY_FILE=${VBNET_TEST_COVERAGE_DIR}/coverage-summary-${CHALLENGE_ID}.xml
    COVERAGE_IN_PACKAGE=$(xmllint ${VBNET_INSTRUMENTED_COVERAGE_REPORT} \
                                  --xpath '//Class[starts-with(./FullName,"BeFaster.App.Solutions.'${CHALLENGE_ID}'.")]/Summary' || true)

   echo "<xml>${COVERAGE_IN_PACKAGE}</xml>" > ${COVERAGE_SUMMARY_FILE}
   if [[ ! -z "${COVERAGE_IN_PACKAGE}" ]]; then
     COVERED=$(xmllint ${COVERAGE_SUMMARY_FILE} --xpath  'sum(//Summary/@visitedSequencePoints)')
     TOTAL_LINES=$(xmllint ${COVERAGE_SUMMARY_FILE} --xpath  'sum(//Summary/@numSequencePoints)')
     TOTAL_COVERAGE_PERCENTAGE=$((${COVERED} * 100 / ${TOTAL_LINES}))
   fi

    echo ${TOTAL_COVERAGE_PERCENTAGE} > ${VBNET_CODE_COVERAGE_INFO}
    cat ${VBNET_CODE_COVERAGE_INFO}
    exit 0
else
    exitAfterNoCoverageReportFoundError
fi
