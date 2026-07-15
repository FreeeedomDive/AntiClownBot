import React, { useContext, useLayoutEffect, useMemo, useState } from "react";
import { SideBarContext } from "./SideBarContextValue";

export function SideBarProvider({ children }: { children: React.ReactNode }) {
  const [activeId, setActiveId] = useState<string | null>(null);
  const value = useMemo(() => ({ activeId, setActiveId }), [activeId]);
  return (
    <SideBarContext.Provider value={value}>{children}</SideBarContext.Provider>
  );
}

export function ActiveSidebar({
  id,
  children,
}: {
  id: string;
  children: React.ReactNode;
}) {
  const { setActiveId } = useContext(SideBarContext);
  useLayoutEffect(() => {
    setActiveId(id);
    return () => {
      setActiveId((curr) => (curr === id ? null : curr));
    };
  }, [id, setActiveId]);
  return <>{children}</>;
}
