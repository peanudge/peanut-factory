const _routerMap = {
  root: { name: 'root', children: ['home'] },
  home: { name: 'Home', Component: () => <div>Home</div> },
}

export type RoutePath = keyof typeof _routerMap

type BaseRoute = { name: string; link?: RoutePath }
export type ParentRoute = BaseRoute & { children: RoutePath[] }
export type ChildRoute = BaseRoute & { Component: React.ComponentType }

export type Route = ChildRoute | ParentRoute
export const routeMap = _routerMap as Record<RoutePath, Route>

export const isParentRoute = (route: Route): route is ParentRoute =>
  'children' in route

export const gnbRootList: [RoutePath, Route][] = (
  routeMap.root as ParentRoute
).children.map((r) => [r, routeMap[r]])
