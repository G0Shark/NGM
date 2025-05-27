// pages/ngm.tsx

import { Button } from "@/shared/ui/Button";

export default function Home() {
  const handleClick = () => {
    alert("Button clicked!");
  };

  return (
    <div className="bg-muted text-foreground grow">
      <main className="p-6 gap-1 grid w-fit">
        <p className="mb-4">This is the NGM page.</p>
        <Button onClick={handleClick}>Click Me</Button>
        <Button onClick={handleClick} disabled>Disabled click me</Button>
      </main>
    </div>
  );
}
