#!/bin/bash

SDK_URL="https://marketplace.logi.com/resources/20/Logi_Plugin_Tool_Win_6_0_1_20790_ccd09903f8.zip"

DIR="$(cd "$(dirname "${0}")" && pwd)"

PKG_PATH="${DIR}/LogiPluginSdkTools.zip"
OUT_PATH="${DIR}/LogiPluginSdkTools"
NESTED_PATH="${OUT_PATH}/LogiPluginSdkTools"

# remove output directory if it exists
rm -rf "${OUT_PATH}"

# recreate output directory
mkdir -p "${OUT_PATH}"

# download package file
curl "${SDK_URL}" -o "${PKG_PATH}"

# extract downloaded package
unzip "${PKG_PATH}" -d "${OUT_PATH}"

# remove downloaded package
rm "${PKG_PATH}"

# move the extracted files back to output directory if they are nested
if [ -d "${NESTED_PATH}" ]; then
  mv "${NESTED_PATH}"/* "${OUT_PATH}"
  rmdir "${NESTED_PATH}"
fi
