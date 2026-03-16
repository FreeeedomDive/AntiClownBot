import { Route, Routes, useLocation, useParams } from "react-router-dom";
import React, { useEffect } from "react";
import { useStore } from "../../../Stores";
import { getPageTitle } from "../../../Helpers/PageTitleHelper";
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
  const location = useLocation();

  useEffect(() => {
    document.title = getPageTitle(location.pathname, userId);
  }, [location.pathname, userId]);

  if (user === null) {
    return <Typography variant={"h5"}>Пользователь не найден</Typography>;
  }

  if (user === undefined) {
    return <Loader />;
  }

  return (
    <>
      <Routes>
        <Route path="/" element={<UserOverview />} />
      </Routes>

      {isMyPage && (
        <Routes>
          <Route path="/economy" element={<UserEconomy />} />
          <Route path="/inventory" element={<UserInventory />} />
          <Route path="/shop" element={<UserShop />} />
          <Route path="/f1Predictions/*" element={<F1PredictionsPage />} />
          <Route path="/admin/f1Predictions/*" element={<F1PredictionsAdminPage />} />
          <Route path="/admin/settings" element={<EditSettings />} />
        </Routes>
      )}
      {Boolean(!isMyPage && currentLoggedInUserId && user) && (
        <Routes>
          <Route path="/itemsTrade" element={<ItemsTrade />} />
          <Route path="/f1Predictions/bingo" element={<F1BingoBoard />} />
        </Routes>
      )}
    </>
  );
};

export default UserPageContentRouter;
