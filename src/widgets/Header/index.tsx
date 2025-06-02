// widgets/Header/index.tsx
import { lazy, Suspense } from 'react'
import type { HeaderVariantPropsMap, HeaderVariants } from './model/variants'
import { Skeleton } from '@/shared/ui/skeleton'
type HeaderProps<V extends HeaderVariants> = {
  variant?: V
} & HeaderVariantPropsMap[V]

export default function Header<V extends HeaderVariants = 'default'>({
  variant = 'default' as V,
  ...props
}: HeaderProps<V>) {
  const VariantComponent = lazy(() => import(`./ui/${variant}`))

  return (
    <Suspense fallback={<Skeleton />}>
      <VariantComponent {...(props as any)} />
    </Suspense>
  )
}
