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
import {RightsWrapper} from "../../../Components/RIghts/RightsWrapper";
import {RightsDto} from "../../../Dto/Rights/RightsDto";
import F1PredictionsList from "./ControlPanel/F1Predictions/F1PredictionsList";
import F1PredictionsAdminList from "./Admin/F1PredictionsAdminList";

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
          <Route path="/f1Predictions" element={<F1PredictionsList />} />
          <Route path="/f1PredictionsAdmin" element={<F1PredictionsAdminList />} />
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
