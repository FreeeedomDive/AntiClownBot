import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  build: {
    target: "es2015",
  },
  server: {
    host: "0.0.0.0",
    allowedHosts: ["host.docker.internal"],
  },
});
