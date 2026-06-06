import { Route, Routes, useParams } from "react-router-dom";
import React from "react";
import DocumentTitle from "react-document-title";
import { useStore } from "../../../Stores";
import { ActiveSidebar } from "../SideBar/SideBarContext";
import UserOverview from "./UserOverview/UserOverview";
import UserInventory from "./ControlPanel/Inventory/UserInventory";
import UserShop from "./ControlPanel/Shop/UserShop";
import UserEconomy from "./ControlPanel/Economy/UserEconomy";
import ItemsTrade from "./Interaction/ItemsTrade";
import { UserDto } from "../../../Dto/Users/UserDto";
import { Typography } from "@mui/material";
import { Loader } from "../../../Components/Loader/Loader";
import EditSettings from "./Admin/Settings/EditSettings";
import F1BingoBoard from "./ControlPanel/F1/Bingo/F1BingoBoard";
import F1PredictionsPage from "./ControlPanel/F1/F1PredictionsPage";
import F1PredictionsAdminPage from "./Admin/F1/F1PredictionsAdminPage";

interface Props {
  user: UserDto | null | undefined;
}

const UserPageContentRouter = ({ user }: Props) => {
  const { authStore } = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const { userId } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;

  if (user === null) {
    return <Typography variant={"h5"}>Пользователь не найден</Typography>;
  }

  if (user === undefined) {
    return <Loader />;
  }

  return (
    <>
      <Routes>
        <Route
          path="/"
          element={
            <ActiveSidebar id="Overview">
              <DocumentTitle title="Профиль - Clown City">
                <UserOverview />
              </DocumentTitle>
            </ActiveSidebar>
          }
        />
      </Routes>

      {isMyPage && (
        <Routes>
          <Route
            path="/economy"
            element={
              <ActiveSidebar id="Economy">
                <DocumentTitle title="Экономика - Clown City">
                  <UserEconomy />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
          <Route
            path="/inventory"
            element={
              <ActiveSidebar id="Inventory">
                <DocumentTitle title="Инвентарь - Clown City">
                  <UserInventory />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
          <Route
            path="/shop"
            element={
              <ActiveSidebar id="Shop">
                <DocumentTitle title="Магазин - Clown City">
                  <UserShop />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
          <Route path="/f1Predictions/*" element={<F1PredictionsPage />} />
          <Route
            path="/admin/f1Predictions/*"
            element={<F1PredictionsAdminPage />}
          />
          <Route
            path="/admin/settings"
            element={
              <ActiveSidebar id="Settings">
                <DocumentTitle title="Настройки - Clown City">
                  <EditSettings />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
        </Routes>
      )}
      {Boolean(!isMyPage && currentLoggedInUserId && user) && (
        <Routes>
          <Route
            path="/itemsTrade"
            element={
              <ActiveSidebar id="ItemsTrade">
                <DocumentTitle title="Обмен предметами - Clown City">
                  <ItemsTrade />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
          <Route
            path="/f1Predictions/bingo"
            element={
              <ActiveSidebar id="F1PredictionsBingo">
                <DocumentTitle title="Бинго - Clown City">
                  <F1BingoBoard />
                </DocumentTitle>
              </ActiveSidebar>
            }
          />
        </Routes>
      )}
    </>
  );
};

export default UserPageContentRouter;
