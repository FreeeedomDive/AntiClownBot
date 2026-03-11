import React from "react";
import {
  Navigate,
  Route,
  Routes,
  useLocation,
  useNavigate,
  useParams,
} from "react-router-dom";
import { Box, Tab, Tabs } from "@mui/material";
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
  { label: "Таблица", path: "standings", icon: <Leaderboard /> },
  { label: "Предсказания гонок", path: "current", icon: <Assignment /> },
  { label: "Чемпионат", path: "championship", icon: <EmojiEvents /> },
  { label: "Бинго", path: "bingo", icon: <Casino /> },
  { label: "Регламент", path: "rulebook", icon: <MenuBook /> },
] as const;

const F1PredictionsPage = () => {
  const navigate = useNavigate();
  const { userId } = useParams<"userId">();
  const location = useLocation();

  const activeTab = TABS.findIndex((tab) =>
    location.pathname.endsWith(`/f1Predictions/${tab.path}`),
  );

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    navigate(`/user/${userId}/f1Predictions/${TABS[newValue].path}`);
  };

  return (
    <Box>
      <Tabs
        value={activeTab >= 0 ? activeTab : 0}
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
      <Box sx={{ mt: 2 }}>
        <Routes>
          <Route path="/" element={<Navigate to="standings" replace />} />
          <Route path="standings" element={<F1PredictionsStandings />} />
          <Route path="rulebook" element={<F1PredictionsRulebook />} />
          <Route path="current" element={<F1PredictionsList />} />
          <Route path="championship" element={<F1ChampionshipPredictions />} />
          <Route path="bingo" element={<F1BingoBoard />} />
        </Routes>
      </Box>
    </Box>
  );
};

export default F1PredictionsPage;
