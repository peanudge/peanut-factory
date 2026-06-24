import { defineConfig } from 'vitest/config'
import { devtools } from '@tanstack/devtools-vite'

import { tanstackRouter } from '@tanstack/router-plugin/vite'

import viteReact from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const config = defineConfig({
  resolve: { tsconfigPaths: true },
  plugins: [
    devtools(),
    tailwindcss(),
    tanstackRouter({ target: 'react', autoCodeSplitting: true }),
    viteReact(),
  ],
  server: {
    host: true,            // bind 0.0.0.0 so external clients (e.g. ngrok) can reach the dev server
    allowedHosts: true,    // accept ngrok / arbitrary forwarded host headers
    proxy: {
      // Forward API + SSE calls to the backend so the browser talks to a single origin.
      // localhost:5000 is resolved here (on the dev machine), not in the remote browser.
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.ts'],
    css: false,
  },
})

export default config
