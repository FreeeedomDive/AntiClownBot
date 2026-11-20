import React from "react";
import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import DocumentTitle from "react-document-title";
import { ActiveSidebar } from "../../../SideBar/SideBarContext";
import F1BingoCardsEditor from "./Bingo/F1BingoCardsEditor";

export default function F1PredictionsAdminPage() {
  const { search } = useLocation();

  return (
    <ActiveSidebar id="F1Admin">
      <Routes>
        <Route
          path="bingo"
          element={
            <DocumentTitle title="Бинго F1 - Clown City">
              <F1BingoCardsEditor />
            </DocumentTitle>
          }
        />
        <Route path="*" element={<Navigate to={`bingo${search}`} replace />} />
      </Routes>
    </ActiveSidebar>
  );
}
