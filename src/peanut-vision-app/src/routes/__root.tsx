import { Outlet, createRootRoute } from '@tanstack/react-router'
import { TanStackRouterDevtoolsPanel } from '@tanstack/react-router-devtools'
import { TanStackDevtools } from '@tanstack/react-devtools'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Gnb } from '@/components/Gnb'
import { ToastProvider } from '@/contexts/ToastContext'
import { ToastContainer } from '@/contexts/ToastContainer'

const queryClient = new QueryClient({
  defaultOptions: { queries: { refetchOnWindowFocus: false } },
})

export const Route = createRootRoute({ component: RootComponent })

function RootComponent() {
  return (
    <QueryClientProvider client={queryClient}>
      <ToastProvider>
        <Gnb />
        <main>
          <Outlet />
        </main>
        <ToastContainer />
        <TanStackDevtools
          config={{ position: 'bottom-right' }}
          plugins={[{ name: 'TanStack Router', render: <TanStackRouterDevtoolsPanel /> }]}
        />
      </ToastProvider>
    </QueryClientProvider>
  )
}
