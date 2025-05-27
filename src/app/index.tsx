import Home from "@/pages/Home";
import { Layout } from "./Layout";
import { useUserStore } from "@/shared/model/store/user";
import { useEffect } from "react";
export const App = () => {
  const { tryLogIn, jwtToken } = useUserStore();
  useEffect(() => {
    if (!jwtToken) tryLogIn();
  }, []);
  return (
    <Layout>
      <Home />
    </Layout>
  );
};
