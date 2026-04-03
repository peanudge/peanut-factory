import { test, expect } from "@playwright/test";

const API = "http://localhost:5000/api";

test.beforeEach(async ({ page, request }) => {
  // Ensure clean state: stop any running acquisition and release the channel
  // (releasing clears the latestFrame / hasFrame flag, preventing infinite re-render loops in CapturePage)
  await request.post(`${API}/acquisition/stop`);
  await request.delete(`${API}/acquisition`);
  await page.goto("/");
  // Wait for the capture page AppBar to be visible
  await expect(page.getByRole("heading", { name: "PeanutVision" })).toBeVisible({ timeout: 10_000 });
});

// ── Layout ────────────────────────────────────────────────────────────────────

test("capture_page_has_two_panel_layout", async ({ page }) => {
  // Left panel should have capture controls (Acquisition controls include a Capture button)
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible({ timeout: 10_000 });

  // Image viewer area should be visible — it contains the viewer component
  // The ImageViewer renders an area with "LIVE" text or an img when a frame is available
  // The viewer container is on the right — confirm the 갤러리 nav button is also present
  await expect(page.getByRole("link", { name: /갤러리/i })).toBeVisible({ timeout: 5_000 });

  // Settings gear icon (link to /backoffice) should be in the DOM
  // Use a slightly longer timeout — the element may not be visible until fully rendered
  await expect(page.locator('a[href="/backoffice"]')).toBeVisible({ timeout: 10_000 });

  await page.screenshot({ path: "test-results/capture-page-01-two-panel-layout.png" });
});

// ── Exposure control ──────────────────────────────────────────────────────────

test("exposure_control_is_present", async ({ page }) => {
  // ExposureControl renders a Card with "Exposure" subtitle2 heading
  await expect(page.getByRole("heading", { name: "Exposure" })).toBeVisible({ timeout: 10_000 });

  // The exposure slider label shows current value in µs — match any text starting with "Exposure (" and ending with "s)"
  await expect(page.getByText(/Exposure \(\d+/)).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/capture-page-02-exposure-control.png" });
});

// ── Preset selector ───────────────────────────────────────────────────────────

test("preset_selector_is_present", async ({ page }) => {
  // PresetSelector renders "Save Preset" and "Load Preset" buttons
  await expect(page.getByRole("button", { name: /save preset/i })).toBeVisible({ timeout: 10_000 });
  await expect(page.getByRole("button", { name: /load preset/i })).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/capture-page-03-preset-selector.png" });
});

// ── Snapshot ──────────────────────────────────────────────────────────────────

test("single_snapshot_updates_viewer", async ({ page }) => {
  // The useCapture hook auto-selects the first camera profile when cameras load.
  // Wait for the Capture button to be enabled (camera profile auto-selected + status loaded).
  await expect(page.getByRole("button", { name: /capture/i })).toBeEnabled({ timeout: 10_000 });

  // Click Capture — triggers a snapshot
  await page.getByRole("button", { name: /capture/i }).click();

  // Success toast should appear
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  await page.screenshot({ path: "test-results/capture-page-04-snapshot-captured.png" });
});

// ── Gallery navigation ────────────────────────────────────────────────────────

test("gallery_link_navigates_correctly", async ({ page }) => {
  // Verify the 갤러리 link is present in the AppBar
  await expect(page.locator('a[href="/gallery"]')).toBeVisible({ timeout: 10_000 });

  // Navigate via evaluate to avoid React polling DOM detach issues
  await page.evaluate(() => {
    const link = document.querySelector('a[href="/gallery"]') as HTMLAnchorElement;
    link?.click();
  });

  // URL should become /gallery
  await expect(page).toHaveURL("/gallery", { timeout: 5_000 });
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/capture-page-05-gallery-navigation.png" });

  // Navigate back to "/"
  await page.goBack();
  await expect(page).toHaveURL("/", { timeout: 5_000 });
  await expect(page.getByRole("heading", { name: "PeanutVision" })).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/capture-page-05b-back-to-capture.png" });
});
