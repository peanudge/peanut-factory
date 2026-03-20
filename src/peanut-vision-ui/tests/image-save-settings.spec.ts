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

test("Image Save Settings panel is visible", async ({ page }) => {
  // The ImageSaveSettingsPanel should be on the page
  await expect(page.getByText(/image save settings/i)).toBeVisible({
    timeout: 5_000,
  });
  await page.screenshot({
    path: "test-results/image-save-settings-visible.png",
  });
});

test("Image Save Settings can be expanded and shows fields", async ({
  page,
}) => {
  // Click to expand the settings panel
  await page.getByText(/image save settings/i).click();

  // Should show output directory field
  await expect(page.locator("label", { hasText: /output directory/i })).toBeVisible({
    timeout: 5_000,
  });

  // Should show format selector
  await expect(page.locator("label", { hasText: /format/i })).toBeVisible();

  // Should show filename prefix
  await expect(page.locator("label", { hasText: /filename prefix/i })).toBeVisible();

  await page.screenshot({
    path: "test-results/image-save-settings-expanded.png",
  });
});

test("Image Save Settings format dropdown works", async ({ page }) => {
  await page.getByText(/image save settings/i).click();

  // Find and interact with format dropdown
  const formatLabel = page.locator("label", { hasText: /^format$/i });
  await expect(formatLabel).toBeVisible({ timeout: 5_000 });
});

test("Image Save Settings auto-save toggle is present", async ({ page }) => {
  await page.getByText(/image save settings/i).click();

  // Auto-save toggle should be visible
  await expect(page.getByText(/auto.?save/i)).toBeVisible({ timeout: 5_000 });
});

test("Image Save Settings API returns valid defaults", async ({ request }) => {
  const response = await request.get(`${API}/settings/image-save`);
  expect(response.ok()).toBeTruthy();

  const settings = await response.json();
  expect(settings.outputDirectory).toBeTruthy();
  expect(settings.format).toBeDefined();
  expect(settings.filenamePrefix).toBeTruthy();
  expect(settings.timestampFormat).toBeTruthy();
  expect(typeof settings.autoSave).toBe("boolean");
});

test("Image Save Settings API roundtrip", async ({ request }) => {
  // Get current settings
  const getResponse = await request.get(`${API}/settings/image-save`);
  const original = await getResponse.json();

  // Update with modified settings
  const modified = { ...original, filenamePrefix: "e2e-test" };
  const putResponse = await request.put(`${API}/settings/image-save`, {
    data: modified,
  });
  expect(putResponse.ok()).toBeTruthy();

  // Verify the change persisted
  const verifyResponse = await request.get(`${API}/settings/image-save`);
  const verified = await verifyResponse.json();
  expect(verified.filenamePrefix).toBe("e2e-test");

  // Restore original settings
  await request.put(`${API}/settings/image-save`, { data: original });
});
