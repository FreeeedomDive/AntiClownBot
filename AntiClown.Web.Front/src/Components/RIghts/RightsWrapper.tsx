import {RightsDto} from "../../Dto/Rights/RightsDto";
import {useStore} from "../../Stores";
import React from "react";

interface Props {
  requiredRights: RightsDto[];
  children: React.ReactNode;
}

export function RightsWrapper({requiredRights, children}: Props) {
  const {rightsStore} = useStore();
  const hasRights = rightsStore.userRights.filter(x => requiredRights.includes(x)).length > 0;
  return hasRights ? {children} : null;
}