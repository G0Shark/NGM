import { routeTree } from '@/routeTree.gen'
import { useLastPlaceStore } from '@/shared/model/store/useLastPlaceStore'
import { createFileRoute } from '@tanstack/react-router'
import { useEffect } from 'react'

export const Route = createFileRoute('/app/server/$id')({
  component: ServerPage,
})

export function ServerPage() {
  const { id } = routeTree.useParams() as { id?: string }
  const { setType, type, serverId, setServerId } = useLastPlaceStore()
  const navigate = routeTree.useNavigate()
  useEffect(() => {
    if (type !== 'server') setType('server')
    if (!id && serverId)
      navigate({
        from: '/app/server',
        to: '/app/server/$id',
        params: { id: `${serverId}` },
      })
    else return
    if (serverId !== Number.parseInt(id!)) setServerId(Number.parseInt(id!))
  }, [id])
  return (
    <>
      <>Привет, я сервер с id: {id || 'сорян... id нет :('}</>
    </>
  )
}
