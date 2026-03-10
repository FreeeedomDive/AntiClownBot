import { RightsDto } from "../../Dto/Rights/RightsDto";
import { useStore } from "../../Stores";
import React from "react";
import { observer } from "mobx-react-lite";

interface Props {
  requiredRights: RightsDto[];
  children: React.ReactElement;
}

export const RightsWrapper = observer(function RightsWrapper({
  requiredRights,
  children,
}: Props): React.ReactElement | null {
  const { rightsStore } = useStore();
  const hasRights = requiredRights.some((x) =>
    rightsStore.userRights.includes(x),
  );
  return hasRights ? children : null;
});
