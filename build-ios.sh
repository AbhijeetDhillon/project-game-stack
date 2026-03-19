#!/bin/bash
# ─────────────────────────────────────────────────────────────
#  One-click iOS build script for Mac
#  Usage: ./build-ios.sh
#  What it does:
#    1. Pulls latest code from GitHub
#    2. Runs Unity headless to generate the Xcode project
#    3. Opens the Xcode project ready to Archive & submit
# ─────────────────────────────────────────────────────────────

set -e  # Exit immediately on any error

UNITY_VERSION="2022.3.62f3"
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="$(cd "$(dirname "$0")" && pwd)"
BUILD_OUTPUT="${PROJECT_PATH}/iOS app"
LOG_FILE="${PROJECT_PATH}/build.log"

# ── Colours ──────────────────────────────────────────────────
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Colour

echo ""
echo -e "${GREEN}==============================${NC}"
echo -e "${GREEN}  Stack Game — iOS Builder${NC}"
echo -e "${GREEN}==============================${NC}"
echo ""

# ── 1. Check Unity exists ─────────────────────────────────────
if [ ! -f "$UNITY_PATH" ]; then
    echo -e "${RED}[ERROR] Unity ${UNITY_VERSION} not found at:${NC}"
    echo "  $UNITY_PATH"
    echo ""
    echo "Install it via Unity Hub: https://unity.com/download"
    exit 1
fi

# ── 2. Pull latest from GitHub ────────────────────────────────
echo -e "${YELLOW}[1/3] Pulling latest from GitHub...${NC}"
cd "$PROJECT_PATH"
git pull origin main
echo -e "${GREEN}      Done.${NC}"
echo ""

# ── 3. Run Unity headless build ───────────────────────────────
echo -e "${YELLOW}[2/3] Building Xcode project (this takes a few minutes)...${NC}"
echo "      Log: $LOG_FILE"
echo ""

"$UNITY_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -executeMethod iOSBuilder.Build \
    -buildTarget iOS \
    -logFile "$LOG_FILE"

BUILD_EXIT=$?

if [ $BUILD_EXIT -ne 0 ]; then
    echo -e "${RED}[ERROR] Unity build failed! Check the log:${NC}"
    echo "  $LOG_FILE"
    echo ""
    # Print last 30 lines of log for quick diagnosis
    echo "── Last 30 lines of build.log ──────────────────────────"
    tail -30 "$LOG_FILE"
    exit 1
fi

echo -e "${GREEN}      Build succeeded!${NC}"
echo ""

# ── 4. Open Xcode ─────────────────────────────────────────────
echo -e "${YELLOW}[3/3] Opening Xcode project...${NC}"

XCWORKSPACE=$(find "$BUILD_OUTPUT" -name "*.xcworkspace" -maxdepth 2 | head -1)
XCODEPROJ=$(find "$BUILD_OUTPUT" -name "*.xcodeproj" -maxdepth 2 | head -1)

if [ -n "$XCWORKSPACE" ]; then
    open "$XCWORKSPACE"
    echo -e "${GREEN}      Opened: $(basename "$XCWORKSPACE")${NC}"
elif [ -n "$XCODEPROJ" ]; then
    open "$XCODEPROJ"
    echo -e "${GREEN}      Opened: $(basename "$XCODEPROJ")${NC}"
else
    echo -e "${RED}[WARN] Could not find Xcode project in: $BUILD_OUTPUT${NC}"
fi

echo ""
echo -e "${GREEN}==============================${NC}"
echo -e "${GREEN}  All done!${NC}"
echo -e "${GREEN}  In Xcode: Product → Archive${NC}"
echo -e "${GREEN}==============================${NC}"
echo ""
