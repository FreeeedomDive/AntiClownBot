import React from "react";
import { observer } from "mobx-react-lite";
import { F1RaceSummary as F1RaceDto } from "../../Dto/F1Predictions/F1InternalDto";
import { useStore } from "../../Stores";
import F1PredictionEditor from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/F1PredictionEditor";

interface Props {
  f1Race: F1RaceDto;
}

const F1Prediction = observer(({ f1Race }: Props) => {
  const { mobileUserContextStore } = useStore();
  const userId = mobileUserContextStore.user?.id;
  if (mobileUserContextStore.isUnknown || !userId) return null;

  return <F1PredictionEditor f1Race={f1Race} userId={userId} mobile />;
});

export default F1Prediction;
