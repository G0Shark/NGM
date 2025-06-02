import { create } from 'zustand'
import { persist } from 'zustand/middleware'
type LastPlaceType = 'chat' | 'server'
interface IUseLastPlaceStore {
  type: LastPlaceType
  serverId?: number
  chatId?: number
  setType: (type: LastPlaceType) => void
  setServerId: (serverId: number) => void
  setChatId: (chatId: number) => void
}

export const useLastPlaceStore = create<IUseLastPlaceStore>()(
  persist(
    (set) => ({
      type: 'chat',
      setType: (type: LastPlaceType) => set({ type }),
      setServerId: (serverId: number) => set({ serverId }),
      setChatId: (chatId: number) => set({ chatId }),
    }),
    {
      name: 'last-place',
    },
  ),
)
