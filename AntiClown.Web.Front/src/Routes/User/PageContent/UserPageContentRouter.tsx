import {Route, Routes, useParams} from "react-router-dom";
import React from "react";
import {useStore} from "../../../Stores";
import UserOverview from "./UserOverview/UserOverview";
import UserInventory from "./ControlPanel/UserInventory";
import UserShop from "./ControlPanel/UserShop";
import UserEconomy from "./ControlPanel/Economy/UserEconomy";
import ItemsTrade from "./Interaction/ItemsTrade";
import {UserDto} from "../../../Dto/Users/UserDto";
import {Typography} from "@mui/material";
import {Loader} from "../../../Components/Loader/Loader";
import F1PredictionsList from "./ControlPanel/F1/Predictions/F1PredictionsList";
import F1PredictionsAdminList from "./Admin/F1/Predictions/F1PredictionsAdminList";
import EditSettings from "./Admin/Settings/EditSettings";
import F1PredictionsStandings from "./ControlPanel/F1/Standings/F1PredictionsStandings";
import F1PredictionsTeamsEditor from "./Admin/F1/Teams/F1PredictionsTeamsEditor";
import F1BingoBoard from "./ControlPanel/F1/Bingo/F1BingoBoard";
import F1BingoCardsEditor from "./Admin/F1/Bingo/F1BingoCardsEditor";
import F1ChampionshipPredictions from "./ControlPanel/F1/ChampionshipPredictions/F1ChampionshipPredictions";

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
          <Route path="/f1Predictions/teams" element={<F1PredictionsTeamsEditor />} />
          <Route path="/f1Predictions/bingo" element={<F1BingoBoard />} />
          <Route path="/f1Predictions/bingo/admin" element={<F1BingoCardsEditor />} />
          <Route path="/f1Predictions/championship" element={<F1ChampionshipPredictions />} />
          <Route path="/settings" element={<EditSettings />} />
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
