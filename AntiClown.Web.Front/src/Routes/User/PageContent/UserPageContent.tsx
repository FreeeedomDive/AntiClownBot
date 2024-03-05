import { Route, Routes, useParams } from "react-router-dom";
import React from "react";
import { useStore } from "../../../Stores";
import UserOverview from "./UserOverview/UserOverview";
import UserInventory from "./ControlPanel/UserInventory";
import UserShop from "./ControlPanel/UserShop";
import UserEconomy from "./ControlPanel/UserEconomy";
import ItemsTrade from "./Interaction/ItemsTrade";
import { UserDto } from "../../../Dto/Users/UserDto";
import { Typography } from "@mui/material";
import F1Predictions from "./ControlPanel/F1Predictions/F1Predictions";
import { Loader } from "../../../Components/Loader/Loader";

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
          <Route path="/f1Predictions" element={<F1Predictions />} />
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
