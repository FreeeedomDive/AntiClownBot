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
import { Casino, EmojiEvents, Flag, Groups } from "@mui/icons-material";
import DocumentTitle from "react-document-title";
import { ActiveSidebar } from "../../../SideBar/SideBarContext";
import F1PredictionsAdminList from "./Predictions/F1PredictionsAdminList";
import F1ChampionshipPredictionsAdmin from "./ChampionshipPredictions/F1ChampionshipPredictionsAdmin";
import F1BingoCardsEditor from "./Bingo/F1BingoCardsEditor";
import F1PredictionsTeamsEditor from "./Teams/F1PredictionsTeamsEditor";

const ADMIN_TABS = [
  {
    label: "Результаты гонок",
    path: "results",
    icon: <Flag />,
    element: <F1PredictionsAdminList />,
  },
  {
    label: "Чемпионат",
    path: "championship",
    icon: <EmojiEvents />,
    element: <F1ChampionshipPredictionsAdmin />,
  },
  {
    label: "Бинго",
    path: "bingo",
    icon: <Casino />,
    element: <F1BingoCardsEditor />,
  },
  {
    label: "Изменение команд",
    path: "teams",
    icon: <Groups />,
    element: <F1PredictionsTeamsEditor />,
  },
] as const;

const F1PredictionsAdminPage = () => {
  const navigate = useNavigate();
  const { userId } = useParams<"userId">();
  const location = useLocation();
  const [searchParams, setSearchParams] = useSearchParams();
  const currentYear = new Date().getFullYear();
  const requestedSeason = Number(searchParams.get("season") ?? currentYear);
  const season =
    Number.isInteger(requestedSeason) &&
    requestedSeason >= 2023 &&
    requestedSeason <= currentYear
      ? requestedSeason
      : currentYear;
  const seasons = Array.from(
    { length: currentYear - 2023 + 1 },
    (_, index) => currentYear - index,
  );

  const activeTab = ADMIN_TABS.findIndex((tab) =>
    location.pathname.endsWith(`/admin/f1Predictions/${tab.path}`),
  );

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    navigate(
      `/user/${userId}/admin/f1Predictions/${ADMIN_TABS[newValue].path}${location.search}`,
    );
  };

  return (
    <ActiveSidebar id="F1Admin">
      <Box>
        <Box
          sx={{
            display: "flex",
            flexDirection: { xs: "column", sm: "row" },
            alignItems: { xs: "stretch", sm: "center" },
            gap: { xs: 1, sm: 2 },
            minWidth: 0,
          }}
        >
          <Tabs
            value={activeTab >= 0 ? activeTab : 0}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            sx={{ minHeight: 32, flex: 1, minWidth: 0 }}
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
          {(activeTab === 0 || activeTab < 0) && (
            <FormControl
              size="small"
              sx={{
                minWidth: 90,
                alignSelf: { xs: "flex-end", sm: "auto" },
              }}
            >
              <Select
                value={season}
                inputProps={{ "aria-label": "Сезон гонок" }}
                onChange={(event) => {
                  const next = new URLSearchParams(searchParams);
                  next.set("season", String(event.target.value));
                  next.delete("active");
                  setSearchParams(next, { replace: true });
                }}
                sx={{ height: 32 }}
              >
                {seasons.map((value) => (
                  <MenuItem key={value} value={value}>
                    {value}
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
              element={<Navigate to={`results${location.search}`} replace />}
            />
            {ADMIN_TABS.map((tab) => (
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

export default F1PredictionsAdminPage;
