import { WebApp } from "@grammyjs/types";

declare global {
  interface Window {
    Telegram: {
      WebApp: WebApp;
    };
  }
}
