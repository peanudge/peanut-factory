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
  await expect(page.getByText(/image save settings/i)).toBeVisible({
    timeout: 5_000,
  });
  await page.screenshot({
    path: "test-results/image-save-settings-visible.png",
  });
});

test("Image Save Settings can be expanded and shows output directory field", async ({
  page,
}) => {
  await page.getByText(/image save settings/i).click();

  await expect(page.locator("label", { hasText: /output directory/i })).toBeVisible({
    timeout: 5_000,
  });

  await page.screenshot({
    path: "test-results/image-save-settings-expanded.png",
  });
});

test("Image Save Settings API returns valid defaults", async ({ request }) => {
  const response = await request.get(`${API}/settings/image-save`);
  expect(response.ok()).toBeTruthy();

  const settings = await response.json();
  expect(settings.outputDirectory).toBeTruthy();
});

test("Image Save Settings API roundtrip", async ({ request }) => {
  const getResponse = await request.get(`${API}/settings/image-save`);
  const original = await getResponse.json();

  const modified = { outputDirectory: "e2e-test-output" };
  const putResponse = await request.put(`${API}/settings/image-save`, {
    data: modified,
  });
  expect(putResponse.ok()).toBeTruthy();

  const verifyResponse = await request.get(`${API}/settings/image-save`);
  const verified = await verifyResponse.json();
  expect(verified.outputDirectory).toBe("e2e-test-output");

  // Restore original settings
  await request.put(`${API}/settings/image-save`, { data: original });
});
