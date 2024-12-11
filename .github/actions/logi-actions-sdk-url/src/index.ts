#!/usr/bin/env node

import crypto from "node:crypto";
import { setFailed, setOutput } from "@actions/core";
import { JSDOM } from "jsdom";

await main().catch((cause) => {
  const fallbackUrl =
    "https://marketplace.logi.com/resources/20/Logi_Plugin_Tool_Win_6_0_1_20790_ccd09903f8.zip";

  setFailed(
    new Error(
      "Failed to retrieve SDK URL from Logi Marketplace website. Using fallback URL instead.",
      { cause },
    ),
  );

  setOutput("url", fallbackUrl);
  setOutput("sha", strToSha256(fallbackUrl));
});

async function main(): Promise<void> {
  const req = await fetch(
    "https://www.logitech.com/en-us/software/marketplace/developer.html",
    {
      method: "GET",
      headers: {
        "Content-Type": "text/html",
      },
    },
  );
  const data = await req.text();
  const dom = new JSDOM(data);
  const { document } = dom.window;
  const downloadSdkAnchor = document.querySelector<HTMLAnchorElement>(
    'a[data-analytics-title="download-for-windows"]',
  );

  if (downloadSdkAnchor == null) {
    return setFailed("Could not find download link");
  }

  const url = downloadSdkAnchor.href;
  const sha = strToSha256(url);

  setOutput("url", url);
  setOutput("sha", sha);
}

function strToSha256(value: string): string {
  return crypto.createHash("sha256").update(value).digest("hex");
}
