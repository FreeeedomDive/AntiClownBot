import { CircularProgress } from "@mui/material";
import "./Loader.css";

export function Loader() {
  return (
    <div className={"loadingContainer"}>
      <CircularProgress color="inherit" size={64} />
    </div>
  );
}
