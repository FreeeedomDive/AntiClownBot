import {DRIVERS} from "../../../../Dto/F1Predictions/F1DriversHelpers";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {HTML5Backend} from "react-dnd-html5-backend";
import {DndProvider} from "react-dnd";

export default function F1RaceClassifications(){
  return (
    <DndProvider backend={HTML5Backend}>
      {DRIVERS.map((driver) => (<F1RaceClassificationsElement f1Driver={driver} />))}
    </DndProvider>
  )
}