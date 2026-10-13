import React from "react";
import { useParams } from "react-router-dom";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionEditor from "./F1PredictionEditor";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1Prediction({ f1Race }: Props) {
  const { userId } = useParams<"userId">();
  if (!userId) return null;

  return <F1PredictionEditor f1Race={f1Race} userId={userId} />;
}
