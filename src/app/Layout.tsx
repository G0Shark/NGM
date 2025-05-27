import { req } from "@/shared/api/request";
import { useLastServerAndDMStore } from "@/shared/model/store/lastServerAndDM";
import { useUserStore } from "@/shared/model/store/user";
import { ChannelList } from "@/widgets/ChannelList";
import { Header } from "@/widgets/Header";
import { ServerList } from "@/widgets/ServerList";
import { HTMLProps, FormEvent, useState } from "react";

interface FormData {
  username: string;
  password: string;
  email?: string;
  displayName?: string;
  description?: string;
}
const LoginPage = () => {
  const { jwtToken, setUserName, setPasswd, tryLogIn } = useUserStore();
  const [isRegistering, setIsRegistering] = useState(false);

  const onRegisterSubmit = ({
    username,
    password,
    email,
    displayName,
    description,
  }: FormData) => {
    const registerAndLogin = async () => {
      setUserName(username);
      setPasswd(password);

      await req.post(
        `Auth?USERNAME=${username}&PASSWORD=${password}&EMAIL=${email}&DISPLAYNAME=${displayName}&DESCRIPTION=${description}`
      );

      await tryLogIn();
    };
    registerAndLogin();
  };

  const onLoginSubmit = ({ username, password }: FormData) => {
    setUserName(username);
    setPasswd(password);
    tryLogIn();
  };

  const handleLoginSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.currentTarget;
    const data: FormData = {
      username: form.username.value,
      password: form.password.value,
    };
    onLoginSubmit(data);
  };

  const handleRegisterSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.currentTarget;
    const data: FormData = {
      username: form.username.value,
      password: form.password.value,
      email: form.email.value,
      displayName: form.displayName.value || undefined,
      description: form.description.value || undefined,
    };
    onRegisterSubmit(data);
  };
  console.log();
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-6 rounded shadow-md w-full max-w-md">
        <h2 className="text-2xl font-bold mb-4">
          {isRegistering ? "Register" : "Login"}
        </h2>

        {isRegistering ? (
          <form onSubmit={handleRegisterSubmit}>
            <div className="mb-4">
              <label htmlFor="username" className="block text-sm font-medium">
                Username
              </label>
              <input
                name="username"
                id="username"
                type="text"
                required
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <div className="mb-4">
              <label htmlFor="password" className="block text-sm font-medium">
                Password
              </label>
              <input
                name="password"
                id="password"
                type="password"
                required
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <div className="mb-4">
              <label htmlFor="email" className="block text-sm font-medium">
                Email
              </label>
              <input
                name="email"
                id="email"
                type="email"
                required
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <div className="mb-4">
              <label
                htmlFor="displayName"
                className="block text-sm font-medium"
              >
                Display Name (optional)
              </label>
              <input
                name="displayName"
                id="displayName"
                type="text"
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <div className="mb-4">
              <label
                htmlFor="description"
                className="block text-sm font-medium"
              >
                Description (optional)
              </label>
              <textarea
                name="description"
                id="description"
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <button
              type="submit"
              className="w-full py-2 px-4 bg-blue-600 text-white rounded"
            >
              Register
            </button>
          </form>
        ) : (
          <form onSubmit={handleLoginSubmit}>
            <div className="mb-4">
              <label htmlFor="username" className="block text-sm font-medium">
                Username
              </label>
              <input
                name="username"
                id="username"
                type="text"
                required
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <div className="mb-4">
              <label htmlFor="password" className="block text-sm font-medium">
                Password
              </label>
              <input
                name="password"
                id="password"
                type="password"
                required
                className="mt-1 block w-full border rounded p-2"
              />
            </div>

            <button
              type="submit"
              className="w-full py-2 px-4 bg-green-600 text-white rounded"
            >
              Login
            </button>
          </form>
        )}

        <p className="mt-4 text-center text-sm">
          {isRegistering ? (
            <>
              Already have an account?{" "}
              <button
                onClick={() => setIsRegistering(false)}
                className="text-blue-500 hover:underline"
              >
                Login
              </button>
            </>
          ) : (
            <>
              Don't have an account?{" "}
              <button
                onClick={() => setIsRegistering(true)}
                className="text-blue-500 hover:underline"
              >
                Register
              </button>
            </>
          )}
        </p>
      </div>
    </div>
  );
};

export const Layout = ({ children }: HTMLProps<HTMLDivElement>) => {
  // Main app layout when logged in
  const { jwtToken } = useUserStore();
  const isLastServer = useLastServerAndDMStore((state) => state.isLastServer);
  return (
    <>
      {jwtToken.length === 0 ? (
        <LoginPage />
      ) : (
        <div className="min-h-screen min-w-screen flex flex-col">
          <Header />
          <div className="flex flex-col grow">
            <ServerList />
            <div className="flex flex-row grow">
              <ChannelList variant={isLastServer ? "server" : "user"} />
              {children}
            </div>
          </div>
        </div>
      )}
    </>
  );
};
