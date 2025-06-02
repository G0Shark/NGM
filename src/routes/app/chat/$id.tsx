import { routeTree } from '@/routeTree.gen'
import { useLastPlaceStore } from '@/shared/model/store/useLastPlaceStore'
import { createFileRoute } from '@tanstack/react-router'
import { useEffect } from 'react'

export const Route = createFileRoute('/app/chat/$id')({
  component: ChatPage,
})

export function ChatPage() {
  const { id } = routeTree.useParams() as { id?: string }
  const { setType, type, chatId, setChatId } = useLastPlaceStore()
  const navigate = routeTree.useNavigate()
  useEffect(() => {
    if (type !== 'chat') setType('chat')
    if (!id && chatId)
      navigate({
        from: '/app/chat',
        to: '/app/chat',
        params: { id: `${chatId}` },
      })
    else return
    if (chatId !== Number.parseInt(id!)) setChatId(Number.parseInt(id!))
  }, [id])
  return <>Привет, я чат с id: {id || 'сорян... id нет :('}</>
}
