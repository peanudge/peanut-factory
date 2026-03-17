# Test Report: Issues #13-#17

## Bug Fixes

### 1. Snapshot TOCTOU Race Condition (AcquisitionManager.Snapshot)
- **Problem:** `Snapshot()` released lock after `_channel != null` check, allowing `Start()` to race.
- **Fix:** Added `_snapshotInProgress` flag set inside lock before releasing; `Start()` and `GetAllowedActions()` check this flag.

### 2. intervalMs Minimum Validation
- **Problem:** `intervalMs=1` was silently clamped to 50ms, hiding user error.
- **Fix:** `Start()` now throws `ArgumentException` if `intervalMs` is between 1-49. Controller returns 400 Bad Request. Frontend `ContinuousSettings` shows "최소 50ms" helper text.

---

## Backend Tests

**Total: 325 passed, 0 failed**

| Test Suite | Passed | Duration |
|-----------|--------|----------|
| PeanutVision.MultiCamDriver.Tests | 197 | 938ms |
| PeanutVision.Api.Tests | 128 | 3s |

### New Tests Added

| Test | File | Category |
|------|------|----------|
| `Status_when_idle_allowed_actions_contains_start_and_snapshot` | AcquisitionStatusSpec.cs | #16 |
| `Status_when_active_allowed_actions_contains_stop_and_trigger` | AcquisitionStatusSpec.cs | #16 |
| `Status_includes_recentEvents_array` | AcquisitionStatusSpec.cs | #15 |
| `Status_after_start_stop_has_at_least_two_events` | AcquisitionStatusSpec.cs | #15 |
| `Status_when_active_statistics_include_copy_drop_and_cluster_counts` | AcquisitionStatusSpec.cs | #15 |
| `Start_with_frameCount_and_intervalMs_returns_ok` | AcquisitionStartSpec.cs | #14 |
| `Start_with_intervalMs_below_minimum_returns_bad_request` | AcquisitionStartSpec.cs | #14 |
| `Start_with_intervalMs_zero_returns_ok` | AcquisitionStartSpec.cs | #14 |
| `LatestFrame_when_no_frame_returns_no_content` | AcquisitionLatestFrameSpec.cs | #15 |
| `LatestFrame_after_trigger_returns_png` | AcquisitionLatestFrameSpec.cs | #15 |
| `Then_allowed_actions_contains_start_and_snapshot` (idle) | AcquisitionManagerTests.cs | #16 |
| `Then_allowed_actions_contains_stop_and_trigger` (started) | AcquisitionManagerTests.cs | #16 |
| `Then_snapshot_blocks_start` | AcquisitionManagerTests.cs | TOCTOU fix |
| `Then_allowed_actions_restored_after_snapshot` | AcquisitionManagerTests.cs | TOCTOU fix |
| `Then_intervalMs_below_minimum_throws_ArgumentException` | AcquisitionManagerTests.cs | #14 |
| `Then_intervalMs_at_minimum_succeeds` | AcquisitionManagerTests.cs | #14 |
| `Then_intervalMs_zero_does_not_create_timer` | AcquisitionManagerTests.cs | #14 |

---

## Playwright E2E Tests

**Total: 6 passed, 0 failed (17.2s)**

| Test | Status |
|------|--------|
| Page loads and shows Acquisition tab | PASS |
| Single/Continuous mode toggle works | PASS |
| Single mode: Capture button triggers snapshot | PASS |
| Continuous mode: shows ContinuousSettings when selected | PASS |
| Continuous mode: Start → status changes → Stop flow | PASS |
| allowedActions disables buttons correctly | PASS |

### Screenshots

| State | File |
|-------|------|
| Page loaded (Single mode) | `test-results/01-page-loaded.png` |
| Continuous mode | `test-results/02-continuous-mode.png` |
| Snapshot captured | `test-results/03-snapshot-captured.png` |
| Acquisition active | `test-results/04-acquisition-active.png` |
| Acquisition stopped | `test-results/05-acquisition-stopped.png` |
| Active allowed actions | `test-results/06-allowed-actions-active.png` |

---

## Issues Covered

- **#13** Mode-based acquisition UI (Single/Continuous toggle) — E2E verified
- **#14** Continuous acquisition settings (frameCount, intervalMs) — Backend + E2E verified
- **#15** Adaptive polling, live preview, latest-frame endpoint — Backend verified
- **#16** allowedActions button disable — Backend + E2E verified
- **#17** Warning/error visual indicators — Integrated in UI (StatusChip)
