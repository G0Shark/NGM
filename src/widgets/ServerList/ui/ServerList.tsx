import { useEffect, useRef, useState } from "react";
import ServerListItem, { ServerListItemSkeleton } from "./ServerListItem";
import { useUserStore } from "@/shared/model/store/user";
import { req } from "@/shared/api/request";
export interface Server {
  id: number;
  ownerId: number;
  name: string;
  description?: string;
  avatarUrl?: string;
  //avatarShadowColor?: string;
  visible: boolean;
}

export interface Member {
  id: number;
  serverid: number;
  userid: number;
  roleid: number;
  displayname: string;
  nullable: true;
  textmute: boolean;
  voicemute: boolean;
}

export function ServerList() {
  const scrollRef = useRef<HTMLUListElement>(null);
  const user = useUserStore((state) => state.user);
  const [servers, setServers] = useState<Server[] | undefined>(undefined);
  useEffect(() => {
    const fetchServers = async () => {
      if (!user.id) return;
      try {
        const in_which_servers_we_are = (
          await req<Member[]>(`members/get_byuserid?userId=${user.id}`)
        ).data;
        const servers = await Promise.all(
          in_which_servers_we_are.map(async ({ serverid }) => {
            return (await req<Server>(`Servers/get_byid?SERVERID=${serverid}`))
              .data;
          })
        );
        setServers(servers);
      } catch (err) {
        console.error("Failed to fetch servers", err);
      }
    };
    fetchServers();
  }, [setServers, user]);
  useEffect(() => {
    if (servers && Object.keys(servers!).length > 0) {
      console.table(servers);
    }
  }, [servers]);
  // Mouse wheel scroll (vertical => horizontal)
  useEffect(() => {
    const el = scrollRef.current;
    if (!el) return;

    const onWheel = (e: WheelEvent) => {
      if (e.deltaY === 0) return;
      e.preventDefault();
      el.scrollBy({ left: e.deltaY, behavior: "smooth" });
    };

    el.addEventListener("wheel", onWheel, { passive: false });
    return () => el.removeEventListener("wheel", onWheel);
  }, []);

  // Drag to scroll
  useEffect(() => {
    const el = scrollRef.current;
    if (!el) return;

    let isDown = false;
    let startX = 0;
    let scrollLeft = 0;

    const onMouseDown = (e: MouseEvent) => {
      isDown = true;
      el.classList.add("dragging");
      startX = e.pageX - el.offsetLeft;
      scrollLeft = el.scrollLeft;
    };

    const onMouseMove = (e: MouseEvent) => {
      if (!isDown) return;
      e.preventDefault();
      const x = e.pageX - el.offsetLeft;
      const walk = (x - startX) * 1.5; // speed multiplier
      el.scrollLeft = scrollLeft - walk;
    };

    const onMouseUp = () => {
      isDown = false;
      el.classList.remove("dragging");
    };

    const onMouseLeave = () => {
      isDown = false;
      el.classList.remove("dragging");
    };

    el.addEventListener("mousedown", onMouseDown);
    el.addEventListener("mousemove", onMouseMove);
    el.addEventListener("mouseup", onMouseUp);
    el.addEventListener("mouseleave", onMouseLeave);

    return () => {
      el.removeEventListener("mousedown", onMouseDown);
      el.removeEventListener("mousemove", onMouseMove);
      el.removeEventListener("mouseup", onMouseUp);
      el.removeEventListener("mouseleave", onMouseLeave);
    };
  }, []);

  const content =
    servers === undefined
      ? Array.from({ length: 5 }).map((_, i) => (
          <li key={i}>
            <ServerListItemSkeleton />
          </li>
        ))
      : Object.entries(servers!).length > 0
      ? Object.entries(servers!).map(([id, server]) => (
          <li key={id}>
            <ServerListItem {...server} />
          </li>
        ))
      : [
          <li className="bg-accent text-xs p-2 font-bold" key="Empty">
            Пора найти какой нить сервак :)
          </li>,
        ];

  return (
    <ul
      ref={scrollRef}
      className="no-scrollbar flex flex-row gap-2 p-2 bg-accent-foreground overflow-x-auto overflow-y-hidden select-none"
    >
      {content}
    </ul>
  );
}
