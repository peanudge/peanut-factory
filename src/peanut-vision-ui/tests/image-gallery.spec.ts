import { test, expect } from "@playwright/test";

const API = "http://localhost:5000/api";

test.beforeEach(async ({ page, request }) => {
  await request.post(`${API}/acquisition/stop`);
  await page.goto("/");
  await page.getByRole("tab", { name: /acquisition/i }).click();
  await expect(page.getByRole("button", { name: /capture/i })).toBeVisible({
    timeout: 10_000,
  });
});

// ── Gallery population ─────────────────────────────────────────────────────

test("gallery shows thumbnail after capture", async ({ page }) => {
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  // Gallery item should appear in the Captures panel
  const capturesPanel = page.locator("text=Captures").locator("..").locator("..");
  await expect(capturesPanel.locator("img").first()).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/gallery-01-thumbnail-appears.png" });
});

test("gallery item count increments on each capture", async ({ page }) => {
  for (let i = 0; i < 3; i++) {
    await page.getByRole("button", { name: /capture/i }).click();
    await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });
  }

  // Right panel should show count chip "3" in the Captures collapsible header
  await expect(page.getByText("3")).toBeVisible();
  await page.screenshot({ path: "test-results/gallery-02-count-chip.png" });
});

// ── Viewer mode badge ──────────────────────────────────────────────────────

test("viewer shows LIVE badge before any capture", async ({ page }) => {
  // Start acquisition so a frame is available
  await page.getByRole("button", { name: /^continuous$/i }).click();
  await page.getByRole("button", { name: /^start$/i }).click({ timeout: 10_000 });
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible({ timeout: 10_000 });

  await expect(page.getByText("LIVE")).toBeVisible({ timeout: 5_000 });
  await page.screenshot({ path: "test-results/gallery-03-live-badge.png" });

  await page.getByRole("button", { name: /^stop$/i }).click();
});

test("viewer switches to captured mode after selecting gallery item", async ({ page }) => {
  // Capture an image so we have something in the gallery
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  // Wait for gallery thumbnail to appear, then click it
  const galleryImg = page.locator("img").last();
  await expect(galleryImg).toBeVisible({ timeout: 5_000 });
  await galleryImg.click();

  // Viewer badge should show a timestamp (not "LIVE")
  // The Captured chip shows time in HH:MM:SS format
  await expect(page.getByText("LIVE")).not.toBeVisible();
  await page.screenshot({ path: "test-results/gallery-04-captured-mode.png" });
});

// ── Return to Live ─────────────────────────────────────────────────────────

test("Return to Live chip appears in captured mode", async ({ page }) => {
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  await expect(page.getByRole("button", { name: /return to live/i })).toBeVisible({
    timeout: 5_000,
  });
  await page.screenshot({ path: "test-results/gallery-05-return-to-live-chip.png" });
});

test("clicking Return to Live restores LIVE badge", async ({ page }) => {
  // Start continuous acquisition so there's a live frame
  await page.getByRole("button", { name: /^continuous$/i }).click();
  await page.getByRole("button", { name: /^start$/i }).click({ timeout: 10_000 });
  await expect(page.getByRole("button", { name: /^stop$/i })).toBeVisible({ timeout: 10_000 });

  // Wait for a frame to arrive, then stop
  await page.waitForTimeout(1_500);
  await page.getByRole("button", { name: /^stop$/i }).click();

  // Capture a second frame via snapshot
  await page.getByRole("button", { name: /^single$/i }).click();
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  // Should be in captured mode now — Return to Live chip visible
  await expect(page.getByRole("button", { name: /return to live/i })).toBeVisible({
    timeout: 5_000,
  });

  // Click it
  await page.getByRole("button", { name: /return to live/i }).click();

  // LIVE badge should be back, Return to Live chip gone
  await expect(page.getByText("LIVE")).toBeVisible({ timeout: 5_000 });
  await expect(page.getByRole("button", { name: /return to live/i })).not.toBeVisible();
  await page.screenshot({ path: "test-results/gallery-06-back-to-live.png" });
});

// ── Gallery management ─────────────────────────────────────────────────────

test("Clear All removes all gallery items", async ({ page }) => {
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  await page.getByRole("button", { name: /clear all/i }).click();

  await expect(page.getByText(/no captures yet/i)).toBeVisible({ timeout: 3_000 });
  await page.screenshot({ path: "test-results/gallery-07-clear-all.png" });
});

test("individual gallery item can be deleted", async ({ page }) => {
  // Add two captures
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });
  await page.getByRole("button", { name: /capture/i }).click();
  await expect(page.getByText("스냅샷이 촬영되었습니다")).toBeVisible({ timeout: 10_000 });

  // Hover the first item to reveal its delete button, then click
  const firstItem = page.locator("[class*='MuiBox']").filter({ has: page.locator("img") }).first();
  await firstItem.hover();
  await firstItem.locator("button").click();

  // Should still have one item (count chip shows "1")
  await expect(page.getByText("1")).toBeVisible({ timeout: 3_000 });
  await page.screenshot({ path: "test-results/gallery-08-item-deleted.png" });
});
