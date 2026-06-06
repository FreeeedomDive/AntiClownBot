import React, {
  createContext,
  useContext,
  useLayoutEffect,
  useMemo,
  useState,
} from "react";

interface SideBarContextValue {
  activeId: string | null;
  setActiveId: React.Dispatch<React.SetStateAction<string | null>>;
}

const SideBarContext = createContext<SideBarContextValue>({
  activeId: null,
  setActiveId: () => {},
});

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

export function useIsSidebarActive(id: string): boolean {
  const { activeId } = useContext(SideBarContext);
  return activeId === id;
}
