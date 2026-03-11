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
import { Casino, EmojiEvents, Flag, Groups } from "@mui/icons-material";
import F1PredictionsAdminList from "./Predictions/F1PredictionsAdminList";
import F1ChampionshipPredictionsAdmin from "./ChampionshipPredictions/F1ChampionshipPredictionsAdmin";
import F1BingoCardsEditor from "./Bingo/F1BingoCardsEditor";
import F1PredictionsTeamsEditor from "./Teams/F1PredictionsTeamsEditor";

const ADMIN_TABS = [
  { label: "Результаты гонок", path: "results", icon: <Flag /> },
  { label: "Чемпионат", path: "championship", icon: <EmojiEvents /> },
  { label: "Бинго", path: "bingo", icon: <Casino /> },
  { label: "Изменение команд", path: "teams", icon: <Groups /> },
] as const;

const F1PredictionsAdminPage = () => {
  const navigate = useNavigate();
  const { userId } = useParams<"userId">();
  const location = useLocation();

  const activeTab = ADMIN_TABS.findIndex((tab) =>
    location.pathname.endsWith(`/admin/f1Predictions/${tab.path}`),
  );

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    navigate(`/user/${userId}/admin/f1Predictions/${ADMIN_TABS[newValue].path}`);
  };

  return (
    <Box>
      <Tabs
        value={activeTab >= 0 ? activeTab : 0}
        onChange={handleTabChange}
        sx={{ minHeight: 32 }}
      >
        {ADMIN_TABS.map((tab, index) => (
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
          <Route path="/" element={<Navigate to="results" replace />} />
          <Route path="results" element={<F1PredictionsAdminList />} />
          <Route path="championship" element={<F1ChampionshipPredictionsAdmin />} />
          <Route path="bingo" element={<F1BingoCardsEditor />} />
          <Route path="teams" element={<F1PredictionsTeamsEditor />} />
        </Routes>
      </Box>
    </Box>
  );
};

export default F1PredictionsAdminPage;
