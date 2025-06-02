import { create } from 'zustand'
import { persist } from 'zustand/middleware'
interface IUseLoginStore {}

export const useLastPlaceStore = create<IUseLoginStore>()(
  persist((set) => ({}), {
    name: 'login',
  }),
)
