import { create } from "zustand";
interface IUseLastServerAndDmStore {
  lastSeverId?: number;
  setServerId: (id?: number) => void;
  lastDmUserId?: number;
  setLastDmUserId: (id?: number) => void;
  isLastServer: boolean;
  setIsLastServer: (isLastServer: boolean) => void;
}
export const useLastServerAndDMStore = create<IUseLastServerAndDmStore>((set) => ({
  setServerId: (lastSeverId) => set({ lastSeverId }),
  setLastDmUserId: (lastDmUserId) => set({ lastDmUserId }),
  isLastServer: false,
  setIsLastServer: (isLastServer) => set({ isLastServer })
}));