import { useParams } from "react-router-dom";
import React, { useCallback, useEffect, useState } from "react";
import EconomyApi from "../../../../../Api/EconomyApi";
import { Loader } from "../../../../../Components/Loader/Loader";
import {
  Box, 
  List,
  ListItem,
  ListItemButton, ListItemText,
  Stack,
  Typography,
} from "@mui/material";
import { LineChart } from "@mui/x-charts/LineChart";
import { TransactionDto } from "../../../../../Dto/Economy/TransactionDto";
import { EconomyDto } from "../../../../../Dto/Economy/EconomyDto";
import TransactionRow from "./TransactionRow";
import {AddOutlined} from "@mui/icons-material";

export default function UserEconomy() {
  const { userId } = useParams<"userId">();
  const [loading, setLoading] = useState(true);

  const [economy, setEconomy] = useState<EconomyDto>();

  const transactionsStep = 50;
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [transactionsLimit, setTransactionsLimit] = useState(transactionsStep);
  const [transactionsMin, setTransactionsMin] = useState(0);
  const [transactionsMax, setTransactionsMax] = useState(0);
  const [transactionsHistory, setTransactionsHistory] = useState<number[]>([]);
  const [selectedTransaction, setSelectedTransaction] = useState(-1);

  const [isLoadMoreLoading, setIsLoadMoreLoading] = useState(false);

  useEffect(() => {
    loadEconomy(transactionsLimit, true);
  }, []);

  useEffect(() => {
    let timeout: NodeJS.Timeout | undefined;
    if (selectedTransaction > -1) {
      timeout = setTimeout(() => {
        setSelectedTransaction(-1);
      }, 1500);
    }
    return () => {clearTimeout(timeout);};
  }, [selectedTransaction]);

  const loadEconomy = useCallback(
    async (transactionsCount: number, showLoading: boolean = false) => {
      if (showLoading) {
        setLoading(true);
      }

      const economy = await EconomyApi.get(userId!);
      setEconomy(economy);

      const transactions = await EconomyApi.getTransactions(
        userId!,
        0,
        transactionsCount,
      );
      setTransactions(transactions);

      const result = Array.from<number>([]);
      let currentScamCoinsBalance = economy.scamCoins;
      let min = currentScamCoinsBalance;
      let max = currentScamCoinsBalance;
      result.push(currentScamCoinsBalance);

      for (let transaction of transactions) {
        currentScamCoinsBalance -= transaction.scamCoinDiff;
        result.push(currentScamCoinsBalance);
        min = Math.min(currentScamCoinsBalance, min);
        max = Math.max(currentScamCoinsBalance, max);
      }

      setTransactionsHistory(result.reverse());
      setTransactionsMin(min);
      setTransactionsMax(max);
      setTransactionsLimit(transactionsCount);
      setLoading(false);
    },
    [userId],
  );

  const createTransactionsHistoryListElements = (): React.ReactElement[] => {
    let scamCoinsNow = economy!.scamCoins;
    const items = transactions.map((transaction, i): React.ReactElement => {
      const scamCoinsAfter = scamCoinsNow;
      const scamCoinsBefore = scamCoinsNow - transaction.scamCoinDiff;
      scamCoinsNow = scamCoinsBefore;
      return (
        <ListItem
          disablePadding
          id={`transaction_${i}`}
          key={`transaction_${i}`}
        >
          <ListItemButton selected={selectedTransaction === i}>
            <TransactionRow
              transaction={transaction}
              scamCoinsBefore={scamCoinsBefore}
              scamCoinsAfter={scamCoinsAfter}
            />
          </ListItemButton>
        </ListItem>
      );
    });
    items.push((
      <ListItem
        disablePadding
        id={`transaction_${transactionsLimit}`}
        key={`transaction_${transactionsLimit}`}
      >
        <ListItemButton
          disabled={isLoadMoreLoading}
          selected={selectedTransaction === transactionsLimit}
          onClick={async () => {
            setIsLoadMoreLoading(true);
            await loadEconomy(transactionsLimit + transactionsStep);
            setIsLoadMoreLoading(false);
          }}
        >
          <AddOutlined/>
          <ListItemText primary={isLoadMoreLoading ? "Загрузка..." : "Загрузить еще"} />
        </ListItemButton>
      </ListItem>
    ));

    return items;
  };

  return (
    <>
      {loading && <Loader />}
      {!loading && (
        <Stack direction={"column"} spacing={2}>
          <Typography variant={"h5"}>История баланса скам-койнов</Typography>
          <Stack
            direction={{ xs: "column", md: "row" }}
            spacing={{ xs: 0, md: 4 }}
            sx={{ width: "100%" }}
          >
            <Box sx={{ flexGrow: 1 }}>
              <LineChart
                xAxis={[
                  {
                    data: Array.from(Array(transactionsLimit + 1).keys()),
                    scaleType: "linear",
                  },
                ]}
                yAxis={[
                  {
                    min: transactionsMin,
                    max: transactionsMax,
                  },
                ]}
                onMarkClick={(_, d) => {
                  const itemToScroll = transactionsLimit - (d.dataIndex ?? 0);
                  setSelectedTransaction(itemToScroll);
                  document
                    .getElementById(`transaction_${itemToScroll}`)
                    ?.scrollIntoView({
                      behavior: "smooth",
                      block: "center",
                    });
                }}
                series={[
                  {
                    id: "transactions",
                    data: transactionsHistory,
                    area: true,
                  },
                ]}
                height={600}
                grid={{ vertical: true, horizontal: true }}
              />
            </Box>
            <Stack
              direction="column"
              sx={{
                width: { xs: "100%", md: "40%" },
                height: 600,
                overflowY: "auto",
              }}
            >
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <List>{createTransactionsHistoryListElements()}</List>
              </Box>
            </Stack>
          </Stack>
        </Stack>
      )}
    </>
  );
}
