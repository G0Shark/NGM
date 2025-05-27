import axios from "axios";
import { useUserStore } from "../model/store/user";

export const req = axios.create({
  baseURL: "http://25.42.205.135:15414/api/",
});

// inject dynamic auth header before each request
req.interceptors.request.use((config) => {
  const { jwtToken } = useUserStore.getState();
  config.headers.Authorization = `Bearer ${jwtToken}`;

  return config;
});
