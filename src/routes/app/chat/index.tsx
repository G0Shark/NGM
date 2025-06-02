import { createFileRoute } from '@tanstack/react-router'
import { ChatPage } from './$id'

export const Route = createFileRoute('/app/chat/')({
  component: ChatPage,
})
