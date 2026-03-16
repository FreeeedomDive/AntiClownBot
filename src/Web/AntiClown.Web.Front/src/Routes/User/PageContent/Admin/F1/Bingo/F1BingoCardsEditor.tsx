import React, { useEffect } from "react";
import { F1BingoCardDto } from "../../../../../../Dto/F1Bingo/F1BingoCardDto";
import F1BingoApi from "../../../../../../Api/F1BingoApi";
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../../Components/Rights/RightsWrapper";
import F1BingoCardsEditorRow from "./F1BingoCardsEditorRow";

const HEADER_SX = {
  color: "text.secondary",
  fontWeight: 600,
  fontSize: "0.72rem",
  textTransform: "uppercase",
  letterSpacing: "0.06em",
  py: 1.5,
  px: 2,
} as const;

export default function F1BingoCardsEditor() {
  const [isLoading, setIsLoading] = React.useState(false);
  const [cards, setCards] = React.useState<F1BingoCardDto[]>([]);

  useEffect(() => {
    async function load() {
      setIsLoading(true);
      const cards = await F1BingoApi.getCards(new Date().getFullYear());
      setCards(cards);
    }

    load()
      .catch(console.error)
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      {isLoading ? (
        <Loader />
      ) : (
        <Paper variant="outlined" sx={{ borderRadius: 2, overflow: "hidden" }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={HEADER_SX}>Карточка</TableCell>
                  <TableCell sx={HEADER_SX}>Прогресс</TableCell>
                  <TableCell sx={{ ...HEADER_SX, width: 56 }} />
                </TableRow>
              </TableHead>
              <TableBody>
                {cards.map((card) => (
                  <F1BingoCardsEditorRow key={card.id} card={card} />
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      )}
    </RightsWrapper>
  );
}
