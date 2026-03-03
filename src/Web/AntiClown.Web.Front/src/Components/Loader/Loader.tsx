import { CircularProgress } from "@mui/material";
import "./Loader.css";

interface IProps {
  size?: number;
}

export function Loader({ size = 64 }: IProps) {
  return (
    <div className={"loadingContainer"}>
      <CircularProgress color="inherit" size={size} />
    </div>
  );
}
