import { Server } from "@/widgets/ServerList/ui/ServerList";
import { create } from "zustand";


interface ServersStore {
  servers: Record<string, Server>;
  isLoading: boolean;
  setServers: (servers: Server[]) => void;
  setIsLoading: (isLoading: boolean) => void;
  updateServer: (id: string, partial: Partial<Server>) => void;
}

export const useServersStore = create<ServersStore>((set) => ({
  servers: {},
  isLoading: true,
  setServers: (servers) => set(() => ({
    servers: Object.fromEntries(servers.map((s) => [s.id, s])),
    isLoading: false,
  })),
  setIsLoading: (isLoading) => set({ isLoading }),
  updateServer: (id, partial) => set((state) => ({
    servers: {
      ...state.servers,
      [id]: {
        ...state.servers[id],
        ...partial
      },
    },
  })),
}));