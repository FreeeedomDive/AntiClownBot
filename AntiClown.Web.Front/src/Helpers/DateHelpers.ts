import {NextTribute} from "../Dto/Economy/NextTribute";

// понадобится, если вдруг втащу подношения на фронт

export function calculateDifferenceBetweenDates(date1: Date, date2: Date) {
  return date2.getTime() - date1.getTime()
}

export function getNextTribute(nextTributeString: string): NextTribute {
  const timestamp = Date.parse(nextTributeString);
  const nextTributeDate = new Date(timestamp);
  const difference = calculateDifferenceBetweenDates(nextTributeDate, new Date());
  if (difference > 0){
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
  }
}