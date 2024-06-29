import {Route, Routes, useParams} from "react-router-dom";
import React from "react";
import {useStore} from "../../../Stores";
import UserOverview from "./UserOverview/UserOverview";
import UserInventory from "./ControlPanel/UserInventory";
import UserShop from "./ControlPanel/UserShop";
import UserEconomy from "./ControlPanel/UserEconomy";
import ItemsTrade from "./Interaction/ItemsTrade";
import {UserDto} from "../../../Dto/Users/UserDto";
import {Typography} from "@mui/material";
import {Loader} from "../../../Components/Loader/Loader";
import F1PredictionsList from "./ControlPanel/F1Predictions/F1PredictionsList";
import F1PredictionsAdminList from "./Admin/F1/F1PredictionsAdminList";
import EditSettings from "./Admin/Settings/EditSettings";
import F1PredictionsStandings from "./ControlPanel/F1Predictions/F1PredictionsStandings";

interface Props {
  user: UserDto | null | undefined;
}

const UserPageContent = ({ user }: Props) => {
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
        <Route path="/" element={<UserOverview />} />
      </Routes>

      {isMyPage && (
        <Routes>
          <Route path="/economy" element={<UserEconomy />} />
          <Route path="/inventory" element={<UserInventory />} />
          <Route path="/shop" element={<UserShop />} />
          <Route path="/f1Predictions/standings" element={<F1PredictionsStandings />} />
          <Route path="/f1Predictions/current" element={<F1PredictionsList />} />
          <Route path="/f1Predictions/admin" element={<F1PredictionsAdminList />} />
          <Route path="/settings" element={<EditSettings />} />
        </Routes>
      )}
      {Boolean(!isMyPage && currentLoggedInUserId && user) && (
        <Routes>
          <Route path="/itemsTrade" element={<ItemsTrade />} />
        </Routes>
      )}
    </>
  );
};

export default UserPageContent;
