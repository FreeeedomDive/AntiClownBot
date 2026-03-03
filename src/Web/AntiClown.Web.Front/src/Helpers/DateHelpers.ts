import { NextTribute } from "../Dto/Economy/NextTribute";

// понадобится, если вдруг втащу подношения на фронт

export function calculateDifferenceBetweenDates(date1: Date, date2: Date) {
  return date2.getTime() - date1.getTime();
}

export function getNextTribute(nextTributeString: string): NextTribute {
  const timestamp = Date.parse(nextTributeString);
  const nextTributeDate = new Date(timestamp);
  const difference = calculateDifferenceBetweenDates(nextTributeDate, new Date());
  if (difference > 0) {
    return {
      isReady: true,
      hours: 0,
      minutes: 0,
      seconds: 0
    };
  }

  const seconds = difference / 1000;
  return {
    isReady: false,
    hours: 0,
    minutes: 0,
    seconds: seconds
  };
}

const addLeadingZeros = (number: number) => {
  if (number < 10) {
    return "0" + number;
  }
  return number;
};

export function formatDate(utcDate: string, withTime = false) {
  const date = new Date(Date.parse(utcDate));

  const dateString = `${addLeadingZeros(date.getDate())}.${addLeadingZeros(date.getMonth() + 1)}.${date.getFullYear()}`;
  if (!withTime) {
    return dateString;
  }
  const timeString = `${addLeadingZeros(date.getHours())}:${addLeadingZeros(date.getMinutes())}:${addLeadingZeros(date.getSeconds())}`;

  return dateString + " " + timeString;
}