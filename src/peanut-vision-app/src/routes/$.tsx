import { isParentRoute, routeMap } from '../routeMap'
import type { RoutePath } from '../routeMap'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/$')({
  component: ItemPage,
})

function ItemPage() {
  const { _splat } = Route.useParams()
  const route = routeMap[_splat as RoutePath]
  // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
  if (!route || isParentRoute(route) || !route.Component) {
    return null
  }
  const Component = route.Component
  return <Component />
}

export default ItemPage
