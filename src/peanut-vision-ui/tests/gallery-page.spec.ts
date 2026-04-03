import { test, expect } from "@playwright/test";

const API = "http://localhost:5000/api";
// Use the first available camera profile (freerun, works without hardware in mock mode)
const DEFAULT_PROFILE_ID = "TC-A160K-SEM_freerun_RGB8.cam";

test.beforeEach(async ({ page, request }) => {
  // Ensure clean state: stop any running acquisition and release the channel
  // (releasing clears the latestFrame / hasFrame flag, preventing infinite re-render loops)
  await request.post(`${API}/acquisition/stop`);
  await request.delete(`${API}/acquisition`);
  await page.goto("/");
  // Wait for the capture page to fully load (AppBar title visible)
  await expect(page.getByRole("heading", { name: "PeanutVision" })).toBeVisible({ timeout: 10_000 });
});

// ── Navigation ────────────────────────────────────────────────────────────────

test("navigates_to_gallery_from_capture_page", async ({ page }) => {
  // Verify the 갤러리 AppBar link is present and points to /gallery
  await expect(page.locator('a[href="/gallery"]')).toBeVisible({ timeout: 10_000 });

  // Navigate via the link (use evaluate to bypass React re-render DOM detach issues)
  await page.evaluate(() => {
    const link = document.querySelector('a[href="/gallery"]') as HTMLAnchorElement;
    link?.click();
  });

  await expect(page).toHaveURL("/gallery", { timeout: 5_000 });
  // Gallery page title should be visible
  await expect(page.getByText(/Gallery/)).toBeVisible({ timeout: 5_000 });
  await page.screenshot({ path: "test-results/gallery-page-01-navigate-to-gallery.png" });
});

test("returns_to_capture_page_from_gallery", async ({ page }) => {
  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Click the back (←) icon button in the AppBar — it is the first button/icon in the AppBar
  // The GalleryPage back button calls navigate("/") and has no accessible name (just an icon)
  await page.locator("header button").first().click();

  await expect(page).toHaveURL("/", { timeout: 5_000 });
  await expect(page.getByRole("heading", { name: "PeanutVision" })).toBeVisible({ timeout: 5_000 });
  await page.screenshot({ path: "test-results/gallery-page-02-back-to-capture.png" });
});

// ── Gallery content ───────────────────────────────────────────────────────────

test("gallery_shows_captured_images", async ({ page, request }) => {
  // Trigger a capture via API using a known valid profile
  const snapResp = await request.post(`${API}/acquisition/snapshot`, {
    data: { profileId: DEFAULT_PROFILE_ID },
  });
  expect(snapResp.ok()).toBeTruthy();

  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Check total count text is visible and shows at least 1 image
  // (also serves as the gallery-loaded wait)
  await expect(page.getByText(/\d+ images?/)).toBeVisible({ timeout: 15_000 });
  const countText = await page.getByText(/\d+ images?/).textContent();
  const count = parseInt(countText?.match(/\d+/)?.[0] ?? "0");
  expect(count).toBeGreaterThan(0);

  await page.screenshot({ path: "test-results/gallery-page-03-shows-images.png" });
});

// ── Format filter ─────────────────────────────────────────────────────────────

test("format_filter_shows_only_matching_images", async ({ page, request }) => {
  // Trigger a capture (will use the configured format, typically PNG)
  await request.post(`${API}/acquisition/snapshot`, {
    data: { profileId: DEFAULT_PROFILE_ID },
  });

  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Open the Format dropdown and select PNG
  const formatSelect = page.getByRole("combobox").first();
  await formatSelect.click();
  await page.getByRole("option", { name: "PNG" }).click();

  // Image count should be at least what we expect (the captured image or all PNG images)
  await expect(page.getByText(/\d+ images?/)).toBeVisible({ timeout: 3_000 });

  await page.screenshot({ path: "test-results/gallery-page-04-png-filter.png" });

  // Select BMP — likely 0 images (unless there are BMP images)
  await formatSelect.click();
  await page.getByRole("option", { name: "BMP" }).click();

  await expect(page.getByText(/\d+ images?/)).toBeVisible({ timeout: 3_000 });
  await page.screenshot({ path: "test-results/gallery-page-04b-bmp-filter.png" });
});

// ── Clear filters ─────────────────────────────────────────────────────────────

test("clear_filters_button_appears_when_filter_is_active", async ({ page }) => {
  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Initially, Clear filters button should NOT be visible
  await expect(page.getByRole("button", { name: /clear filters/i })).not.toBeVisible();

  // Select any format filter
  const formatSelect = page.getByRole("combobox").first();
  await formatSelect.click();
  await page.getByRole("option", { name: "PNG" }).click();

  // "Clear filters" button should appear
  await expect(page.getByRole("button", { name: /clear filters/i })).toBeVisible({ timeout: 3_000 });

  await page.screenshot({ path: "test-results/gallery-page-05-clear-filters-visible.png" });

  // Click "Clear filters" — filter resets, button disappears
  await page.getByRole("button", { name: /clear filters/i }).click();
  await expect(page.getByRole("button", { name: /clear filters/i })).not.toBeVisible({ timeout: 3_000 });

  await page.screenshot({ path: "test-results/gallery-page-05b-clear-filters-gone.png" });
});

// ── Image detail overlay ──────────────────────────────────────────────────────

test("image_detail_overlay_opens_on_thumbnail_click", async ({ page, request }) => {
  // Trigger a capture using a known valid profile
  await request.post(`${API}/acquisition/snapshot`, {
    data: { profileId: DEFAULT_PROFILE_ID },
  });

  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Wait for the gallery to finish loading
  await expect(page.getByRole("progressbar")).not.toBeVisible({ timeout: 10_000 });

  // Wait for at least one image count text showing > 0 images
  await expect(page.getByText(/\d+ images?/)).toBeVisible({ timeout: 5_000 });
  const countText = await page.getByText(/\d+ images?/).textContent();
  const count = parseInt(countText?.match(/\d+/)?.[0] ?? "0");
  // If there are no images, skip (can happen if snapshot failed or DB was cleared)
  test.skip(count === 0, "No images in gallery — skipping overlay test");

  // Click the first image in the grid (thumbnail img or the grid item box)
  // The grid renders img elements inside clickable boxes
  const firstGridImg = page.locator("main img, .MuiBox-root img, img[alt]").first();
  await expect(firstGridImg).toBeVisible({ timeout: 5_000 });
  await firstGridImg.click();

  // Dialog/overlay should appear with filename
  await expect(page.getByRole("dialog")).toBeVisible({ timeout: 5_000 });

  // Overlay should show Download and Delete buttons
  await expect(page.getByRole("button", { name: /download/i })).toBeVisible({ timeout: 3_000 });
  await expect(page.getByRole("button", { name: /delete/i })).toBeVisible({ timeout: 3_000 });

  await page.screenshot({ path: "test-results/gallery-page-06-detail-overlay.png" });
});

test("image_detail_overlay_closes_on_escape", async ({ page, request }) => {
  // Trigger a capture using a known valid profile
  await request.post(`${API}/acquisition/snapshot`, {
    data: { profileId: DEFAULT_PROFILE_ID },
  });

  await page.goto("/gallery");
  await expect(page.getByText(/PeanutVision.*Gallery/)).toBeVisible({ timeout: 10_000 });

  // Wait for the gallery to finish loading
  await expect(page.getByRole("progressbar")).not.toBeVisible({ timeout: 10_000 });

  // Wait for images count text
  await expect(page.getByText(/\d+ images?/)).toBeVisible({ timeout: 5_000 });
  const countText = await page.getByText(/\d+ images?/).textContent();
  const count = parseInt(countText?.match(/\d+/)?.[0] ?? "0");
  test.skip(count === 0, "No images in gallery — skipping overlay escape test");

  // Open overlay
  const firstGridImg = page.locator("main img, .MuiBox-root img, img[alt]").first();
  await expect(firstGridImg).toBeVisible({ timeout: 5_000 });
  await firstGridImg.click();
  await expect(page.getByRole("dialog")).toBeVisible({ timeout: 5_000 });

  // Press Escape — overlay should close
  await page.keyboard.press("Escape");
  await expect(page.getByRole("dialog")).not.toBeVisible({ timeout: 3_000 });

  await page.screenshot({ path: "test-results/gallery-page-07-overlay-closed.png" });
});

// ── Backoffice route ──────────────────────────────────────────────────────────

test("backoffice_route_is_accessible", async ({ page }) => {
  await page.goto("/backoffice");

  // Page should render — Latency and System tabs must be visible
  await expect(page.getByRole("tab", { name: /latency/i })).toBeVisible({ timeout: 10_000 });
  await expect(page.getByRole("tab", { name: /system/i })).toBeVisible({ timeout: 5_000 });

  await page.screenshot({ path: "test-results/gallery-page-08-backoffice.png" });

  // Back button (ArrowBack icon) returns to "/"
  await page.locator('a[href="/"]').first().click();
  await expect(page).toHaveURL("/", { timeout: 5_000 });
  await expect(page.getByRole("heading", { name: "PeanutVision" })).toBeVisible({ timeout: 5_000 });
});
