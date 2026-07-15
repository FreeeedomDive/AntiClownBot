import { createContext, useContext } from "react";
import type React from "react";

interface SideBarContextValue {
  activeId: string | null;
  setActiveId: React.Dispatch<React.SetStateAction<string | null>>;
}

export const SideBarContext = createContext<SideBarContextValue>({
  activeId: null,
  setActiveId: () => {},
});

export function useIsSidebarActive(id: string): boolean {
  const { activeId } = useContext(SideBarContext);
  return activeId === id;
}
