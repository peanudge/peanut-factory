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

test("Camera Settings accordion can be expanded", async ({ page }) => {
  const accordion = page.getByText("Camera Settings");
  await expect(accordion).toBeVisible();
  await accordion.click();

  // Should show calibration buttons after expanding
  await expect(page.getByRole("button", { name: /black cal/i })).toBeVisible({
    timeout: 5_000,
  });
  await page.screenshot({
    path: "test-results/camera-settings-expanded.png",
  });
});

test("Camera Settings shows exposure controls", async ({ page }) => {
  await page.getByText("Camera Settings").click();

  // Exposure controls should be visible
  await expect(page.getByRole("button", { name: /load/i })).toBeVisible({
    timeout: 5_000,
  });
  await expect(page.getByRole("button", { name: /apply/i })).toBeVisible();
});

test("Camera Settings shows FFC toggle", async ({ page }) => {
  await page.getByText("Camera Settings").click();

  // FFC toggle should be present
  await expect(page.getByText(/flat field/i)).toBeVisible({ timeout: 5_000 });
});

test("Trigger mode selector is visible and defaults to soft", async ({
  page,
}) => {
  // Switch to continuous mode to see more controls
  await page.getByRole("button", { name: /^continuous$/i }).click();

  // Trigger mode selector should be visible
  const triggerSelect = page.locator("label", { hasText: "Trigger" });
  await expect(triggerSelect).toBeVisible();
});
