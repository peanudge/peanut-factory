import { gnbRootList, isParentRoute, routeMap } from '../routeMap'
import type { ChildRoute, ParentRoute, Route, RoutePath } from '../routeMap'
import { Link, useParams } from '@tanstack/react-router'
import classNames from 'classnames'

export const Gnb = () => {
  return (
    <aside>
      <h1>
        <Link to="/">Vision</Link>
      </h1>
      <ul className="mainRoutes">
        {gnbRootList.map(([link, r]) => (
          <GnbItem key={link} link={link} route={r} />
        ))}
      </ul>
    </aside>
  )
}

interface GnbItemProps {
  link: RoutePath
  route: Route
}

interface ParentGnbItemProps extends GnbItemProps {
  route: ParentRoute
}

interface ChildGnbItemProps extends GnbItemProps {
  route: ChildRoute
}

const GnbItem = ({ link, route }: GnbItemProps) => {
  return isParentRoute(route) ? (
    <ParentGnbItem link={link} route={route} />
  ) : (
    <ChildGnbItem link={link} route={route} />
  )
}

function ParentGnbItem({
  link,
  route: { children, name, link: routeLink },
}: ParentGnbItemProps) {
  const { _splat } = useParams({ strict: false })
  const currentPath = _splat as RoutePath
  const open = children.includes(currentPath)
  const actualLink = routeLink ?? link
  return (
    <li
      className={classNames('parent', `items-${children.length}`, {
        open,
      })}
    >
      <Link to="/$" params={{ _splat: actualLink }}>
        {name}
      </Link>
      <ul className="subRoutes">
        {children.map((r) => (
          <GnbItem key={r} link={r} route={routeMap[r]} />
        ))}
      </ul>
    </li>
  )
}

function ChildGnbItem({ link, route: { name, Component } }: ChildGnbItemProps) {
  const { _splat } = useParams({ strict: false })
  const currentPath = _splat as RoutePath
  return (
    <li
      className={classNames({
        active: link === currentPath,
        disabled: !Component,
      })}
    >
      <Link to="/$" params={{ _splat: link }}>
        {name}
      </Link>
    </li>
  )
}
