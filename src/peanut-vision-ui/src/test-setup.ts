import { expect } from "vitest";

/**
 * Minimal custom matchers to approximate @testing-library/jest-dom
 * without requiring the package.
 */
expect.extend({
  toBeInTheDocument(received: Element | null | undefined) {
    const pass = received != null && document.documentElement.contains(received);
    return {
      pass,
      message: () =>
        pass
          ? `expected element not to be in the document`
          : `expected element to be in the document`,
    };
  },
});

// Augment the vitest Assertion interface so TypeScript recognises the custom matcher
declare module "vitest" {
  interface Assertion<T = unknown> {
    toBeInTheDocument(): T;
  }
  interface AsymmetricMatchersContaining {
    toBeInTheDocument(): void;
  }
}
