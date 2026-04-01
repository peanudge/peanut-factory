import { defineConfig } from "@playwright/test";

const UI_PORT = process.env.UI_PORT ?? "5173";
const UI_BASE_URL = process.env.UI_BASE_URL ?? `http://localhost:${UI_PORT}`;

export default defineConfig({
  testDir: "./tests",
  timeout: 30_000,
  retries: 0,
  use: {
    baseURL: UI_BASE_URL,
    screenshot: "only-on-failure",
  },
  projects: [
    {
      name: "chromium",
      use: { browserName: "chromium" },
    },
  ],
  webServer: [
    {
      command:
        "dotnet run --project ../PeanutVision.Api --environment Development",
      url: "http://localhost:5000/api/acquisition/status",
      reuseExistingServer: !process.env.CI,
      timeout: 60_000,
    },
    {
      command: `npm run dev -- --port ${UI_PORT}`,
      url: UI_BASE_URL,
      reuseExistingServer: !process.env.CI,
      timeout: 30_000,
    },
  ],
});
