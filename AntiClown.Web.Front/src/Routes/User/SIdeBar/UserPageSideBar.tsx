import { useNavigate, useParams } from "react-router-dom";
import React, { useState } from "react";
import {
  Collapse,
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Stack,
} from "@mui/material";
import { useStore } from "../../../Stores";
import { UserDto } from "../../../Dto/Users/UserDto";
import { RightsWrapper } from "../../../Components/RIghts/RightsWrapper";
import { RightsDto } from "../../../Dto/Rights/RightsDto";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import UserPageSideBarItem from "./UserPageSideBarItem";

const buildLink = (userId: string, subLink?: string): string => {
  return `/user/${userId}` + (subLink ? `/${subLink}` : "");
};

interface Props {
  user: UserDto | null | undefined;
}

const UserPageSideBar = ({ user }: Props) => {
  const { authStore } = useStore();
  const { rightsStore } = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const { userId = "" } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;
  const navigate = useNavigate();
  const [isF1PredictionsCollapseOpened, setIsF1PredictionsCollapseOpened] =
    useState(false);
  const [
    isF1AdminPredictionsCollapseOpened,
    setIsF1AdminPredictionsCollapseOpened,
  ] = useState(false);
  const userHasAnyAdminRights = rightsStore.userRights.find(
    (right) =>
      right === RightsDto.F1PredictionsAdmin ||
      right === RightsDto.EditSettings,
  );

  return (
    <Stack
      direction="column"
      sx={{
        paddingTop: "8px",
      }}
    >
      <List>
        <UserPageSideBarItem
          sidebarKey="Overview"
          link={buildLink(userId)}
          text="Профиль"
        />
      </List>
      {isMyPage && (
        <>
          <Divider />
          <List>
            <UserPageSideBarItem
              sidebarKey="Economy"
              link={buildLink(userId, "economy")}
              text="Экономика"
            />
            <UserPageSideBarItem
              sidebarKey="Inventory"
              link={buildLink(userId, "inventory")}
              text="Инвентарь"
            />
            <UserPageSideBarItem
              sidebarKey="Shop"
              link={buildLink(userId, "shop")}
              text="Магазин"
            />
            <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
              <>
                <ListItem key={"F1Predictions"} disablePadding>
                  <ListItemButton
                    onClick={() =>
                      setIsF1PredictionsCollapseOpened(
                        !isF1PredictionsCollapseOpened,
                      )
                    }
                  >
                    <ListItemText primary={"Предсказания F1"} />
                    {isF1PredictionsCollapseOpened ? (
                      <ExpandLess />
                    ) : (
                      <ExpandMore />
                    )}
                  </ListItemButton>
                </ListItem>
                <Collapse
                  in={isF1PredictionsCollapseOpened}
                  timeout="auto"
                  unmountOnExit
                >
                  <List disablePadding>
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsStandings"
                      link={buildLink(userId, "f1Predictions/standings")}
                      text="Таблица"
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsCurrent"
                      link={buildLink(userId, "f1Predictions/current")}
                      text="Текущие предсказания"
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsBingo"
                      link={buildLink(userId, "f1Predictions/bingo")}
                      text="Бинго"
                      nesting={2}
                      showBadge
                    />
                  </List>
                </Collapse>
              </>
            </RightsWrapper>
          </List>
        </>
      )}
      {isMyPage && userHasAnyAdminRights && (
        <>
          <Divider />
          <List>
            <ListItem key={"F1Predictions"} disablePadding>
              <ListItemButton
                onClick={() =>
                  setIsF1AdminPredictionsCollapseOpened(
                    !isF1AdminPredictionsCollapseOpened,
                  )
                }
              >
                <ListItemText primary={"Админка F1"} />
                {isF1AdminPredictionsCollapseOpened ? (
                  <ExpandLess />
                ) : (
                  <ExpandMore />
                )}
              </ListItemButton>
            </ListItem>
            <Collapse
              in={isF1AdminPredictionsCollapseOpened}
              timeout="auto"
              unmountOnExit
            >
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1PredictionsAdmin"
                  link={buildLink(userId, "f1Predictions/admin")}
                  text="Результаты гонок"
                  nesting={2}
                />
              </RightsWrapper>
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1PredictionsBingoAdmin"
                  link={buildLink(userId, "f1Predictions/bingo/admin")}
                  text="Бинго"
                  nesting={2}
                />
              </RightsWrapper>
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1PredictionsTeamsAdmin"
                  link={buildLink(userId, "f1Predictions/teams")}
                  text="Изменение команд"
                  nesting={2}
                />
              </RightsWrapper>
            </Collapse>
            <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
              <UserPageSideBarItem
                sidebarKey="Settings"
                link={buildLink(userId, "settings")}
                text="Настройки"
              />
            </RightsWrapper>
          </List>
        </>
      )}
      {!isMyPage && currentLoggedInUserId && user && (
        <>
          <Divider />
          <List>
            <UserPageSideBarItem
              sidebarKey="ItemsTrade"
              link={buildLink(userId, "itemsTrade")}
              text="Обмен предметами"
            />
            <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
              <UserPageSideBarItem
                sidebarKey="F1PredictionsBingo"
                link={buildLink(userId, "f1Predictions/bingo")}
                text="Бинго"
              />
            </RightsWrapper>
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <Divider />
          <List>
            {!isMyPage && (
              <UserPageSideBarItem
                sidebarKey="BackToMyPage"
                link={buildLink(currentLoggedInUserId)}
                text="Вернуться на мою страницу"
              />
            )}
            <UserPageSideBarItem
              sidebarKey="Logout"
              link={""}
              text="Выход"
              onClick={() => {
                authStore.logOut();
                navigate(buildLink(userId));
              }}
            />
          </List>
        </>
      )}
      {!currentLoggedInUserId && (
        <>
          <Divider />
          <List>
            <UserPageSideBarItem sidebarKey="Login" link={"/auth"} text="Логин" />
          </List>
        </>
      )}
    </Stack>
  );
};

export default UserPageSideBar;
