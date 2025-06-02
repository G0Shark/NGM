import { createFileRoute } from '@tanstack/react-router'
import { ServerPage } from './$id'

export const Route = createFileRoute('/app/server/')({
  component: ServerPage,
})
