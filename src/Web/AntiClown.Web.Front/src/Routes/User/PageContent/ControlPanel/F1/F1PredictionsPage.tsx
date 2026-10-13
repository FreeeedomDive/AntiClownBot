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
  BarChart,
  Casino,
  EmojiEvents,
  Leaderboard,
  MenuBook,
} from "@mui/icons-material";
import DocumentTitle from "react-document-title";
import { ActiveSidebar } from "../../../SideBar/SideBarContext";
import F1PredictionsStandings from "./Standings/F1PredictionsStandings";
import F1PredictionsRulebook from "./Rulebook/F1PredictionsRulebook";
import F1PredictionsList from "./Predictions/F1PredictionsList";
import F1ChampionshipPredictions from "./ChampionshipPredictions/F1ChampionshipPredictions";
import F1BingoBoard from "./Bingo/F1BingoBoard";
import F1PredictionsStats from "./Statistics/F1PredictionsStats";

const TABS = [
  {
    label: "Регламент",
    path: "rulebook",
    icon: <MenuBook />,
    element: <F1PredictionsRulebook />,
  },
  {
    label: "Таблица",
    path: "standings",
    icon: <Leaderboard />,
    element: <F1PredictionsStandings />,
  },
  {
    label: "Предсказания гонок",
    path: "races",
    icon: <Assignment />,
    element: <F1PredictionsList />,
  },
  {
    label: "Чемпионат",
    path: "championship",
    icon: <EmojiEvents />,
    element: <F1ChampionshipPredictions />,
  },
  {
    label: "Бинго",
    path: "bingo",
    icon: <Casino />,
    element: <F1BingoBoard />,
  },
  {
    label: "Статистика",
    path: "statistics",
    icon: <BarChart />,
    element: <F1PredictionsStats />,
  },
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
  const requestedSeason = Number(searchParams.get("season") ?? currentYear);
  const season =
    Number.isInteger(requestedSeason) &&
    requestedSeason >= FIRST_SEASON &&
    requestedSeason <= currentYear
      ? requestedSeason
      : currentYear;
  const showSeasonSelector =
    location.pathname.endsWith("/f1Predictions/standings") ||
    location.pathname.endsWith("/f1Predictions/races") ||
    location.pathname.endsWith("/f1Predictions/bingo") ||
    location.pathname.endsWith("/f1Predictions/statistics");

  const activeTab = TABS.findIndex((tab) =>
    location.pathname.endsWith(`/f1Predictions/${tab.path}`),
  );

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    navigate(
      `/user/${userId}/f1Predictions/${TABS[newValue].path}${location.search}`,
    );
  };

  return (
    <ActiveSidebar id="F1Predictions">
      <Box>
        <Box
          sx={{ display: "flex", alignItems: "center", gap: 2, minWidth: 0 }}
        >
          <Tabs
            value={activeTab >= 0 ? activeTab : 1}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            sx={{ minHeight: 32, flex: 1, minWidth: 0 }}
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
                inputProps={{ "aria-label": "Сезон F1" }}
                onChange={(event) => {
                  const next = new URLSearchParams(searchParams);
                  next.set("season", String(event.target.value));
                  next.delete("active");
                  setSearchParams(next, { replace: true });
                }}
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
            <Route
              path="/"
              element={<Navigate to={`standings${location.search}`} replace />}
            />
            {TABS.map((tab) => (
              <Route
                key={tab.path}
                path={tab.path}
                element={
                  <DocumentTitle title={`${tab.label} - Clown City`}>
                    {tab.element}
                  </DocumentTitle>
                }
              />
            ))}
          </Routes>
        </Box>
      </Box>
    </ActiveSidebar>
  );
};

export default F1PredictionsPage;
