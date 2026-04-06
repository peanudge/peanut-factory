import { test, expect } from "@playwright/test";

const API = "http://localhost:5000/api";

test.beforeEach(async ({ page, request }) => {
  // Ensure clean state: stop any running acquisition
  await request.post(`${API}/acquisition/stop`);
  // Navigate and switch to ACQUISITION tab
  await page.goto("/");
  await page.getByRole("tab", { name: /acquisition/i }).click();
  // Wait for the Capture button to appear (single mode default)
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible({
    timeout: 10_000,
  });
});

test("page loads and shows Acquisition tab", async ({ page }) => {
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible();
  await page.screenshot({ path: "test-results/01-page-loaded.png" });
});

test("Single/Continuous mode toggle works", async ({ page }) => {
  const singleBtn = page.getByRole("button", { name: /^single$/i });
  const continuousBtn = page.getByRole("button", { name: /^continuous$/i });

  await expect(singleBtn).toBeVisible();
  await expect(continuousBtn).toBeVisible();

  // Default is single mode — Capture button visible
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible();

  // Switch to continuous mode
  await continuousBtn.click();
  await expect(page.getByRole("button", { name: /^start$/i })).toBeVisible();
  await page.screenshot({ path: "test-results/02-continuous-mode.png" });

  // Switch back to single mode
  await singleBtn.click();
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible();
});

test("Single mode: Capture button starts single-frame acquisition", async ({ page }) => {
  // Wait for status to load so allowedActions enables the button
  await expect(page.getByRole("button", { name: /capture/i })).toBeEnabled({
    timeout: 10_000,
  });

  await page.getByRole("button", { name: /capture/i }).click();

  // Wait for the snackbar success message
  await expect(page.getByText("단일 프레임 촬영이 시작되었습니다")).toBeVisible({
    timeout: 10_000,
  });
  await page.screenshot({ path: "test-results/03-capture-started.png" });
});

test("Continuous mode: shows ContinuousSettings when selected", async ({
  page,
}) => {
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // ContinuousSettings should show Frame Count and Interval fields (use label locator to avoid MUI duplicate text)
  await expect(page.locator("label", { hasText: "Frame Count" })).toBeVisible();
  await expect(page.locator("label", { hasText: "Interval (ms)" })).toBeVisible();
  await expect(page.getByText("최소 50ms")).toBeVisible();
});

test("Continuous mode: Start → status changes → Stop flow", async ({
  page,
}) => {
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // Wait for Start button to be enabled
  await expect(page.getByRole("button", { name: /^start$/i })).toBeEnabled({
    timeout: 10_000,
  });

  // Start acquisition
  await page.getByRole("button", { name: /^start$/i }).click();
  await expect(page.getByText("촬영이 시작되었습니다")).toBeVisible({
    timeout: 10_000,
  });

  // Wait for status to reflect active state — StatusChip shows "Active (...)"
  await expect(page.getByText(/Active/)).toBeVisible({ timeout: 10_000 });
  await page.screenshot({ path: "test-results/04-acquisition-active.png" });

  // Stop button should appear
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible();
  await page.getByRole("button", { name: /^stop$/i }).click();

  await expect(page.getByText("촬영이 중지되었습니다")).toBeVisible({
    timeout: 10_000,
  });
  await page.screenshot({ path: "test-results/05-acquisition-stopped.png" });
});

test("allowedActions disables buttons correctly", async ({ page }) => {
  // In idle state: Capture should be enabled (after status loads)
  await expect(page.getByRole("button", { name: /capture/i })).toBeEnabled({
    timeout: 10_000,
  });

  // Switch to continuous mode, then select Manual sub-mode so Trigger is shown
  await page.getByRole("button", { name: /^continuous$/i }).click();
  await page.getByRole("button", { name: /^manual$/i }).click();
  await page.getByRole("button", { name: /^start$/i }).click();

  // After start in Manual mode: Stop and Trigger should both be visible
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible({
    timeout: 10_000,
  });
  await expect(page.getByRole("button", { name: /trigger/i })).toBeVisible();
  await page.screenshot({
    path: "test-results/06-allowed-actions-active.png",
  });
});

test("ContinuousSettings: Auto/Manual toggle renders both options", async ({
  page,
}) => {
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // Both toggle buttons must be present in ContinuousSettings
  await expect(page.getByRole("button", { name: /^auto$/i })).toBeVisible();
  await expect(page.getByRole("button", { name: /^manual$/i })).toBeVisible();
  await page.screenshot({ path: "test-results/07-submode-toggle.png" });
});

test("ContinuousSettings: Interval field hidden in Manual sub-mode", async ({
  page,
}) => {
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // Default is Auto — Interval field should be visible
  await expect(page.locator("label", { hasText: "Interval (ms)" })).toBeVisible();

  // Switch to Manual — Interval field must disappear
  await page.getByRole("button", { name: /^manual$/i }).click();
  await expect(
    page.locator("label", { hasText: "Interval (ms)" })
  ).not.toBeVisible();
  await page.screenshot({ path: "test-results/08-manual-no-interval.png" });
});

test("ContinuousSettings: Trigger hidden in Auto, visible in Manual while active", async ({
  page,
}) => {
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // Start in Auto sub-mode (default)
  await expect(page.getByRole("button", { name: /^start$/i })).toBeEnabled({
    timeout: 10_000,
  });
  await page.getByRole("button", { name: /^start$/i }).click();
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible({
    timeout: 10_000,
  });

  // Trigger button must NOT appear in Auto mode
  await expect(
    page.getByRole("button", { name: /trigger/i })
  ).not.toBeVisible();
  await page.screenshot({ path: "test-results/09-auto-no-trigger.png" });

  // Stop, switch to Manual, restart — Trigger must appear
  await page.getByRole("button", { name: /^stop$/i }).click();
  await expect(page.getByRole("button", { name: /^start$/i })).toBeVisible({
    timeout: 10_000,
  });
  await page.getByRole("button", { name: /^manual$/i }).click();
  await page.getByRole("button", { name: /^start$/i }).click();
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible({
    timeout: 10_000,
  });
  await expect(page.getByRole("button", { name: /trigger/i })).toBeVisible();
  await page.screenshot({ path: "test-results/10-manual-trigger-visible.png" });
});
