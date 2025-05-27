import { Server } from "@/shared/model/store/servers";
import { Button } from "@/shared/ui/Button";

function hashToColor(str: string) {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  const hue = hash % 360;
  return `hsl(${hue}, 70%, 50%)`;
}

export default function ServerListItem({ name, avatarUrl }: Server) {
  const bgColor = hashToColor(name);

  return (
    <Button
      className="
        w-12 h-12 rounded-full flex items-center justify-center
        font-bold shrink-0 transition-transform duration-200
        hover:scale-110 p-0"
      style={{
        backgroundColor: avatarUrl ? 'transparent' : bgColor,
        color: "white",
        filter: 'drop-shadow(0 0 0 rgba(0,0,0,0))',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.filter = `drop-shadow(0 0 1em ${avatarUrl ? '#fff' : bgColor})`;
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.filter = `drop-shadow(0 0 0 rgba(0,0,0,0))`;
      }}
      variant={'link'}
      title={name}
      aria-label={`Server: ${name}`}
    >
      {avatarUrl ? (
        <img
          src={avatarUrl}
          alt={`${name.at(0)?.toUpperCase()}`}
          className="w-full h-full object-cover rounded-full"
        />
      ) : (
        name.at(0)?.toUpperCase()
      )}
    </Button>
  );
}

import { Skeleton } from "@/shared/ui/Skeleton";

export function ServerListItemSkeleton() {
  return (
    <div className="w-12 h-12 rounded-full overflow-hidden p-0">
      <Skeleton className="bg-blue-300 w-full h-full rounded-full" />
    </div>
  );
}