// widgets/Header/ui/Header.tsx
import { lazy, Suspense } from "react";
import { ChannelListVariants, ChannelListVariantPropsMap } from "../model/variants";
type HeaderProps<V extends ChannelListVariants> = {
  variant?: V;
} & ChannelListVariantPropsMap[V];

export function ChannelList<V extends ChannelListVariants>({ variant, ...props }: HeaderProps<V>) {
  const VariantComponent = lazy(() => import(`./variants/${variant}`));

  return (
    <Suspense fallback={<div>Loading...</div>}>
      <VariantComponent {...(props as any)} />
    </Suspense>
  );
}
