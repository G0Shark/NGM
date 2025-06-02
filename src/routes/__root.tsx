import { Outlet, createRootRouteWithContext } from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'

import Header from '../widgets/Header'

import type { QueryClient } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'

export interface NgmRouterContext {
  queryClient: QueryClient
}

export const Route = createRootRouteWithContext<NgmRouterContext>()({
  component: () => (
    <>
      <Header />
      <Outlet />

      <TanStackRouterDevtools />
      <ReactQueryDevtools buttonPosition="bottom-right" />
    </>
  ),
})
