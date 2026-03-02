import {TransactionDto} from "../../../../../Dto/Economy/TransactionDto";
import {Stack, Typography} from "@mui/material";
import { formatDate } from "../../../../../Helpers/DateHelpers";

interface Props {
  transaction: TransactionDto;
  scamCoinsBefore: number;
  scamCoinsAfter: number;
}

export default function TransactionRow({transaction, scamCoinsBefore, scamCoinsAfter}: Props) {

  const isTransactionPositive = transaction.scamCoinDiff >= 0;
  return (
    <Stack direction="column">
      <Typography variant="h6">{transaction.reason}</Typography>
      <Typography variant="body2">
        {scamCoinsBefore} {"→"} {scamCoinsAfter}{" "}
        <Typography
          display={"inline"}
          variant="body2"
          color={isTransactionPositive ? "green" : "red"}
        >
          (
          {isTransactionPositive
            ? `+${transaction.scamCoinDiff}`
            : `${transaction.scamCoinDiff}`}
          )
        </Typography>
      </Typography>
      <Typography variant="body2" color="textSecondary">
        {formatDate(transaction.dateTime, true)}
      </Typography>
    </Stack>
  );
}