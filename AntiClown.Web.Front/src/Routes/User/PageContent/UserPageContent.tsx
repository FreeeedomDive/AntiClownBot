import { observer } from "mobx-react-lite";
import { Route, Routes, useParams } from "react-router-dom";
import React from "react";
import { useStore } from "../../../Stores";
import UserOverview from "./UserOverview/UserOverview";
import UserInventory from "./ControlPanel/UserInventory";
import UserShop from "./ControlPanel/UserShop";
import UserEconomy from "./ControlPanel/UserEconomy";
import ItemsTrade from "./Interaction/ItemsTrade";

const UserPageContent = observer(() => {
  const { authStore } = useStore();
  const currentLoggedInUserId = authStore.userId;
  const { userId } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;

  return (
    <Routes>
      <Route path="/" element={<UserOverview />} />
      {isMyPage ? (
        <>
          <Route path="/inventory" element={<UserInventory />} />
          <Route path="/shop" element={<UserShop />} />
          <Route path="/economy" element={<UserEconomy />} />
        </>
      ) : (
        <></>
      )}
      {!isMyPage && currentLoggedInUserId ? (
        <Route path="/itemsTrade" element={<ItemsTrade />} />
      ) : (
        <></>
      )}
    </Routes>
  );
});

export default UserPageContent;