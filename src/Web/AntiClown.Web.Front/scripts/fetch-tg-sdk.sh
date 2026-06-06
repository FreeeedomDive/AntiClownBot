#!/usr/bin/env bash
set -euo pipefail

# Fetch the latest Telegram WebApp SDK and place it into the front's public/ dir
# so it ships as a self-hosted static asset (avoids slow loads from telegram.org
# in regions where it's throttled/blocked). On any failure we fall back to the
# committed vendor/ copy and warn — the build must not break because of this.

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
URL="https://telegram.org/js/telegram-web-app.js"
TARGET="$PROJECT_ROOT/public/telegram-web-app.js"
FALLBACK="$PROJECT_ROOT/vendor/telegram-web-app.js"
TMP="$(mktemp)"
trap 'rm -f "$TMP"' EXIT

use_fallback() {
  local reason="$1"
  echo "[fetch-tg-sdk] WARN: $reason — using committed fallback from vendor/"
  if [[ ! -f "$FALLBACK" ]]; then
    echo "[fetch-tg-sdk] WARN: fallback $FALLBACK is missing — skipping (build will fail to load SDK at runtime)"
    return 0
  fi
  mkdir -p "$(dirname "$TARGET")"
  cp "$FALLBACK" "$TARGET"
  echo "[fetch-tg-sdk] fallback copied: $TARGET ($(wc -c < "$TARGET") bytes)"
}

echo "[fetch-tg-sdk] downloading $URL"
if ! curl --fail --max-time 30 --retry 3 --retry-delay 2 -sSL "$URL" -o "$TMP"; then
  use_fallback "curl failed"
  exit 0
fi

if [[ ! -s "$TMP" ]]; then
  use_fallback "downloaded file is empty"
  exit 0
fi

# Reject HTML error pages masquerading as JS (telegram.org sometimes returns one).
head_bytes="$(head -c 256 "$TMP" | tr '[:upper:]' '[:lower:]')"
if [[ "$head_bytes" == *"<!doctype html"* ]] || [[ "$head_bytes" == *"<html"* ]]; then
  use_fallback "response looks like HTML, not JS"
  exit 0
fi

mkdir -p "$(dirname "$TARGET")"
mv "$TMP" "$TARGET"
echo "[fetch-tg-sdk] downloaded fresh SDK: $TARGET ($(wc -c < "$TARGET") bytes)"
