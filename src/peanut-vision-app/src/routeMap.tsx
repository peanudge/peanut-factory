import SystemState from './components/SystemState'
import Acquisition from './components/Acquisition'
import Gallery from './components/Gallery'
import Latency from './components/Latency'

const _routerMap = {
  root: { name: 'root', children: ['system-state', 'acquisition', 'gallery', 'latency'] as const },
  'system-state': { name: '시스템 상태', Component: SystemState },
  'acquisition': { name: '촬영', Component: Acquisition },
  'gallery': { name: '갤러리', Component: Gallery },
  'latency': { name: '레이턴시', Component: Latency },
}

export type RoutePath = keyof typeof _routerMap

type BaseRoute = { name: string; link?: RoutePath }
export type ParentRoute = BaseRoute & { children: readonly RoutePath[] }
export type ChildRoute = BaseRoute & { Component: React.ComponentType }

export type Route = ChildRoute | ParentRoute
export const routeMap = _routerMap as Record<RoutePath, Route>

export const isParentRoute = (route: Route): route is ParentRoute =>
  'children' in route

export const gnbRootList: [RoutePath, Route][] = (
  routeMap.root as ParentRoute
).children.map((r) => [r, routeMap[r]])
