import { createContext, useContext } from "react";
import { authStore } from "./AuthStore";
import { rightsStore } from "./RightsStore";
import { mobileUserContextStore } from "./MobileUserContextStore";

const store = {
  authStore: authStore,
  rightsStore: rightsStore,
  mobileUserContextStore: mobileUserContextStore,
};

export const StoreContext = createContext(store);

export const useStore = () => {
  return useContext<typeof store>(StoreContext);
};