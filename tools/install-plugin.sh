#!/bin/bash

PKG_PATH="${HOME}/Downloads/ElgatoLightControlPlugin.zip"
TMP_DIR="$(mktemp -d)"
OUT_DIR="${TMP_DIR}/ElgatoLightControlPlugin"

cleanup() {
  rm -rf "${TMP_DIR:?}"
}
trap cleanup EXIT

mkdir -p "${OUT_DIR}"

if ! unzip -q "${PKG_PATH}" -d "${OUT_DIR}"; then
    echo "Failed to extract ${PKG_PATH}"
    exit 1
fi

PLG_PATH="${OUT_DIR}/ElgatoLightControlPlugin.lplug4"
DST_DIR="${HOME}/AppData/Local/Logi/LogiPluginService/Plugins/ElgatoLightControl"

if [ "$(ls -A ${DST_DIR})" ]; then
  rm -rf "${DST_DIR:?}"
fi

mkdir -p "${DST_DIR}"

if ! unzip -q "${PLG_PATH}" -d "${DST_DIR}"; then
    echo "Failed to extract ${PLG_PATH}"
    exit 1
fi

echo "Plugin installation complete."
echo "The plugin has been installed to: ${DST_DIR}"
