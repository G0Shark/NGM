import { useLastPlaceStore } from '@/shared/model/store/useLastPlaceStore'
import { createFileRoute, Outlet, useLocation } from '@tanstack/react-router'
import { useLayoutEffect } from 'react'

export const Route = createFileRoute('/app/')({
  component: App,
})

function App() {
  const { type } = useLastPlaceStore()
  const navigate = Route.useNavigate()
  const loc = useLocation()
  useLayoutEffect(() => {
    if (loc.pathname !== '/app' && loc.pathname !== '/app/') return
    navigate({ from: '/app', to: `/app/${type}` })
  })

  return (
    <>
      Test
      <Outlet />
      Test
    </>
  )
}
