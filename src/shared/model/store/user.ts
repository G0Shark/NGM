import { req } from "@/shared/api/request";
import { create } from "zustand";
import { persist } from "zustand/middleware";
export interface User {
  id: number;
  userName: string;
  email: string;
  displayname: string;
  description: string;
}

interface IUseUserStore {
  user: Partial<User>;
  userName: string;
  passwd: string;
  jwtToken: string;
  patchUserData: (user: Partial<User>) => void;
  setUserName: (userName: string) => void;
  setPasswd: (passwd: string) => void;
  tryLogIn: () => Promise<void>;
  setJwtToken: (jwtToken: string) => void;
  logOut: () => void;
}

export const useUserStore = create(
  persist<IUseUserStore>(
    (set) => ({
      user: {},
      userName: "oleg",
      passwd: "11111111",
      jwtToken: "",
      patchUserData: (user) => {
        req.patch("user", user);
      },
      setUserName: (userName) => set({ userName }),
      setPasswd: (passwd) => set({ passwd }),
      setJwtToken: (jwtToken) => set({ jwtToken }),
      tryLogIn: async () => {
        const { userName, passwd } = useUserStore.getState();
        const UserAndToken = (
          await req.get(
            `Auth?USERNAME=${userName}&PASSWORD=${passwd}&FOR_LONG=false`
          )
        ).data;
        set({ user: UserAndToken.user, jwtToken: UserAndToken.token });
      },
      logOut: () => set({ jwtToken: "" }),
    }),
    {
      name: "user-storage",
    }
  )
);
