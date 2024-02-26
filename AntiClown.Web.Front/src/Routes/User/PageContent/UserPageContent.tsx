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

interface Props {
  user: UserDto | undefined
}

const UserPageContent = ({user}: Props) => {
  const {authStore} = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const {userId} = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;

  const renderUserNotFound = () => {
    return <Typography variant={"h5"}>Пользователь не найден</Typography>;
  }

  return (
    <>
      {
        user ? (
          <Routes>
            <Route path="/" element={<UserOverview/>}/>
          </Routes>
        ) : (renderUserNotFound())
      }
      {isMyPage ? (
        <Routes>
          <Route path="/economy" element={<UserEconomy/>}/>
          <Route path="/inventory" element={<UserInventory/>}/>
          <Route path="/shop" element={<UserShop/>}/>
        </Routes>
      ) : (
        <></>
      )}
      {!isMyPage && currentLoggedInUserId && user ? (
        <Routes>
          <Route path="/itemsTrade" element={<ItemsTrade/>}/>
        </Routes>
      ) : <></>
      }
    </>
  )
};

export default UserPageContent;