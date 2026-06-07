import { type ReactNode } from 'react'
import { render } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ToastProvider } from '@/contexts/ToastContext'

export function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
      mutations: { retry: false },
    },
  })
}

interface WrapperProps { children: ReactNode }

export function createWrapper(queryClient?: QueryClient) {
  const client = queryClient ?? createTestQueryClient()
  return function Wrapper({ children }: WrapperProps) {
    return (
      <QueryClientProvider client={client}>
        <ToastProvider>
          {children}
        </ToastProvider>
      </QueryClientProvider>
    )
  }
}

export function renderWithProviders(ui: ReactNode, queryClient?: QueryClient) {
  const client = queryClient ?? createTestQueryClient()
  return render(ui, { wrapper: createWrapper(client) })
}
