// widgets/Header/ui/Header.tsx
import { lazy, Suspense } from "react";
import { HeaderVariantPropsMap, HeaderVariants } from "../model/variants";
type HeaderProps<V extends HeaderVariants> = {
  variant?: V;
} & HeaderVariantPropsMap[V];

export function Header<V extends HeaderVariants = "default">({
  variant = "default" as V,
  ...props
}: HeaderProps<V>) {
  const VariantComponent = lazy(() => import(`./variants/${variant}`));

  return (
    <Suspense fallback={<div>Loading header...</div>}>
      <VariantComponent {...(props as any)} />
    </Suspense>
  );
}
