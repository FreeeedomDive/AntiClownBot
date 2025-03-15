import React, {useEffect} from "react";
import {F1BingoCardDto} from "../../../../../../Dto/F1Bingo/F1BingoCardDto";
import F1BingoApi from "../../../../../../Api/F1BingoApi";
import {Table, TableBody, TableContainer} from "@mui/material";
import {Loader} from "../../../../../../Components/Loader/Loader";
import {RightsDto} from "../../../../../../Dto/Rights/RightsDto";
import {RightsWrapper} from "../../../../../../Components/RIghts/RightsWrapper";
import F1BingoCardsEditorRow from "./F1BingoCardsEditorRow";

export default function F1BingoCardsEditor() {
  const [isLoading, setIsLoading] = React.useState(false);
  const [cards, setCards] = React.useState<F1BingoCardDto[]>([]);

  useEffect(() => {
    async function load() {
      setIsLoading(true);
      const cards = await F1BingoApi.getCards(new Date().getFullYear());
      setCards(cards);
      setIsLoading(false);
    }

    load();
  }, []);

  return <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
    {isLoading ? (
      <Loader />
    ) : (
      <TableContainer>
        <Table>
          <TableBody>
            {cards.map((card) => (
              <F1BingoCardsEditorRow card={card} />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    )}
  </RightsWrapper>
}