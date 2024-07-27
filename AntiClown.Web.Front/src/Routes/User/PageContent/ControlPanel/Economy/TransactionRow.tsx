import {TransactionDto} from "../../../../../Dto/Economy/TransactionDto";
import {Stack, Typography} from "@mui/material";

interface Props {
  transaction: TransactionDto;
  scamCoinsBefore: number;
  scamCoinsAfter: number;
}

export default function TransactionRow({transaction, scamCoinsBefore, scamCoinsAfter}: Props) {
  const transactionDate = new Date(Date.parse(transaction.dateTime));

  const addLeadingZeros = (number: number) => {
    if (number < 10) {
      return "0" + number;
    }
    return number;
  }

  const isTransactionPositive = transaction.scamCoinDiff >= 0;
  return (
    <Stack direction="column">
      <Typography variant="h6">{transaction.reason}</Typography>
      <Typography variant="body2">
        {scamCoinsBefore}
        {" "}
        {"→"}
        {" "}
        {scamCoinsAfter}
        {" "}
        <Typography display={"inline"} variant="body2" color={isTransactionPositive ? "green" : "red"}>
          ({isTransactionPositive ? `+${transaction.scamCoinDiff}` : `${transaction.scamCoinDiff}`})
        </Typography>
      </Typography>
      <Typography variant="body2" color="textSecondary">
        {addLeadingZeros(transactionDate.getDate())}
        .
        {addLeadingZeros(transactionDate.getMonth() + 1)}
        .
        {transactionDate.getFullYear()}
        {" "}
        {addLeadingZeros(transactionDate.getHours())}
        :
        {addLeadingZeros(transactionDate.getMinutes())}
        :
        {addLeadingZeros(transactionDate.getSeconds())}
      </Typography>
    </Stack>
  )
}