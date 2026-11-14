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
import { Casino, EmojiEvents } from "@mui/icons-material";
import DocumentTitle from "react-document-title";
import { ActiveSidebar } from "../../../SideBar/SideBarContext";
import F1ChampionshipPredictionsAdmin from "./ChampionshipPredictions/F1ChampionshipPredictionsAdmin";
import F1BingoCardsEditor from "./Bingo/F1BingoCardsEditor";

const ADMIN_TABS = [
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
] as const;

const F1PredictionsAdminPage = () => {
  const navigate = useNavigate();
  const { userId } = useParams<"userId">();
  const location = useLocation();

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
        </Box>
        <Box sx={{ mt: 2 }}>
          <Routes>
            <Route
              path="/"
              element={
                <Navigate to={`championship${location.search}`} replace />
              }
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
            <Route
              path="*"
              element={
                <Navigate to={`championship${location.search}`} replace />
              }
            />
          </Routes>
        </Box>
      </Box>
    </ActiveSidebar>
  );
};

export default F1PredictionsAdminPage;
