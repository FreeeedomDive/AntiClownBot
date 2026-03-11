import React from "react";
import {
  Navigate,
  Route,
  Routes,
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import { Box, FormControl, MenuItem, Select, Tab, Tabs } from "@mui/material";
import {
  Assignment,
  Casino,
  EmojiEvents,
  Leaderboard,
  MenuBook,
} from "@mui/icons-material";
import F1PredictionsStandings from "./Standings/F1PredictionsStandings";
import F1PredictionsRulebook from "./Rulebook/F1PredictionsRulebook";
import F1PredictionsList from "./Predictions/F1PredictionsList";
import F1ChampionshipPredictions from "./ChampionshipPredictions/F1ChampionshipPredictions";
import F1BingoBoard from "./Bingo/F1BingoBoard";

const TABS = [
  { label: "Регламент", path: "rulebook", icon: <MenuBook /> },
  { label: "Таблица", path: "standings", icon: <Leaderboard /> },
  { label: "Предсказания гонок", path: "races", icon: <Assignment /> },
  { label: "Чемпионат", path: "championship", icon: <EmojiEvents /> },
  { label: "Бинго", path: "bingo", icon: <Casino /> },
] as const;

const FIRST_SEASON = 2023;

const F1PredictionsPage = () => {
  const navigate = useNavigate();
  const { userId } = useParams<"userId">();
  const location = useLocation();
  const [searchParams, setSearchParams] = useSearchParams();

  const currentYear = new Date().getFullYear();
  const seasons = Array.from(
    { length: currentYear - FIRST_SEASON + 1 },
    (_, i) => FIRST_SEASON + i,
  );
  const season = Number(searchParams.get("season") ?? currentYear);
  const showSeasonSelector =
    location.pathname.endsWith("/f1Predictions/standings") ||
    location.pathname.endsWith("/f1Predictions/bingo");

  const activeTab = TABS.findIndex((tab) =>
    location.pathname.endsWith(`/f1Predictions/${tab.path}`),
  );

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    navigate(`/user/${userId}/f1Predictions/${TABS[newValue].path}`);
  };

  return (
    <Box>
      <Box sx={{ display: "flex", alignItems: "center" }}>
        <Tabs
          value={activeTab}
          onChange={handleTabChange}
          sx={{ minHeight: 32 }}
        >
          {TABS.map((tab, index) => (
            <Tab
              key={tab.path}
              label={tab.label}
              icon={tab.icon}
              iconPosition="start"
              value={index}
              sx={{
                minHeight: 32,
                py: 0.5,
                fontSize: "0.8rem",
                "& .MuiSvgIcon-root": { fontSize: 16 },
              }}
            />
          ))}
        </Tabs>
        {showSeasonSelector && (
          <FormControl size="small" sx={{ ml: "auto", minWidth: 90 }}>
            <Select
              value={season}
              onChange={(e) =>
                setSearchParams(
                  { season: String(e.target.value) },
                  { replace: true },
                )
              }
              sx={{ height: 32 }}
            >
              {seasons.map((s) => (
                <MenuItem key={s} value={s}>
                  {s}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        )}
      </Box>
      <Box sx={{ mt: 2 }}>
        <Routes>
          <Route path="/" element={<Navigate to="standings" replace />} />
          <Route path="standings" element={<F1PredictionsStandings />} />
          <Route path="rulebook" element={<F1PredictionsRulebook />} />
          <Route path="races" element={<F1PredictionsList />} />
          <Route path="championship" element={<F1ChampionshipPredictions />} />
          <Route path="bingo" element={<F1BingoBoard />} />
        </Routes>
      </Box>
    </Box>
  );
};

export default F1PredictionsPage;
