import { useParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
import F1BingoApi from "../../../../../Api/F1BingoApi";
import { F1BingoCardDto } from "../../../../../Dto/F1Bingo/F1BingoCardDto";
import { RightsDto } from "../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../Components/RIghts/RightsWrapper";
import {Grid, Stack, Typography} from "@mui/material";
import { Loader } from "../../../../../Components/Loader/Loader";
import F1BingoCard from "./F1BingoCard";

export default function F1BingoBoard() {
  const { userId } = useParams<"userId">();
  const season = new Date().getFullYear();
  const [isLoading, setIsLoading] = useState(false);
  const [bingoUserCards, setBingoUserCards] = useState<F1BingoCardDto[]>([]);

  useEffect(() => {
    async function load() {
      setIsLoading(true);
      const cards = await F1BingoApi.getCards(season);
      const boardCardsIds = await F1BingoApi.getBoard(userId!, season);
      const board = boardCardsIds.map(
        (cardId) => cards.find((card) => card.id === cardId)!,
      );
      setBingoUserCards(board);
      setIsLoading(false);
    }

    load();
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
      <Stack direction={"column"}>
        {isLoading && <Loader />}
        {!isLoading && <Grid container spacing={1} sx={{ width: "100%", height: "100%", margin: "auto" }}>
          {bingoUserCards.map((card, index) => (
            <Grid item key={index} xs={12} sm={6} md={4} lg={2.4} sx={{ display: "flex", justifyContent: "center", alignItems: "center" }}>
              <F1BingoCard card={card} />
            </Grid>
          ))}
        </Grid>}
      </Stack>
    </RightsWrapper>
  );
}
