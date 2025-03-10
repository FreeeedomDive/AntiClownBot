import { useLocation, useNavigate, useParams } from "react-router-dom";
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
  const currentLoggedInUserId = authStore.loggedInUserId;
  const { userId = "" } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;
  const navigate = useNavigate();
  const [isF1PredictionsCollapseOpened, setIsF1PredictionsCollapseOpened] =
    useState(false);

  return (
    <Stack
      direction="column"
      sx={{
        paddingTop: "8px",
      }}
    >
      <List>
        <UserPageSideBarItem
          key="Overview"
          link={buildLink(userId)}
          text="Профиль"
        />
      </List>
      {isMyPage && (
        <>
          <Divider />
          <List>
            <UserPageSideBarItem
              key="Economy"
              link={buildLink(userId, "economy")}
              text="Экономика"
            />
            <UserPageSideBarItem
              key="Inventory"
              link={buildLink(userId, "inventory")}
              text="Инвентарь"
            />
            <UserPageSideBarItem
              key="Shop"
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
                      key="F1PredictionsStandings"
                      link={buildLink(userId, "f1Predictions/standings")}
                      text="Таблица"
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      key="F1PredictionsCurrent"
                      link={buildLink(userId, "f1Predictions/current")}
                      text="Текущие предсказания"
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      key="F1PredictionsBingo"
                      link={buildLink(userId, "f1Predictions/bingo")}
                      text="Бинго"
                      nesting={2}
                      showBadge
                    />
                    <RightsWrapper
                      requiredRights={[RightsDto.F1PredictionsAdmin]}
                    >
                      <UserPageSideBarItem
                        key="F1PredictionsAdmin"
                        link={buildLink(userId, "f1Predictions/admin")}
                        text="Админка результатов"
                        nesting={2}
                      />
                    </RightsWrapper>
                    <RightsWrapper
                      requiredRights={[RightsDto.F1PredictionsAdmin]}
                    >
                      <UserPageSideBarItem
                        key="F1PredictionsBingoAdmin"
                        link={buildLink(userId, "f1Predictions/bingo/admin")}
                        text="Админка бинго"
                        nesting={2}
                      />
                    </RightsWrapper>
                    <RightsWrapper
                      requiredRights={[RightsDto.F1PredictionsAdmin]}
                    >
                      <UserPageSideBarItem
                        key="F1PredictionsTeamsAdmin"
                        link={buildLink(userId, "f1Predictions/teams")}
                        text="Админка команд"
                        nesting={2}
                      />
                    </RightsWrapper>
                  </List>
                </Collapse>
              </>
            </RightsWrapper>
            <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
              <UserPageSideBarItem
                key="Settings"
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
              key="ItemsTrade"
              link={buildLink(userId, "itemsTrade")}
              text="Обмен предметами"
            />
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <Divider />
          <List>
            {!isMyPage && (
              <UserPageSideBarItem
                key="BackToMyPage"
                link={buildLink(currentLoggedInUserId)}
                text="Вернуться на мою страницу"
              />
            )}
            <UserPageSideBarItem
              key="Logout"
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
            <UserPageSideBarItem
              key="Login"
              link={"/auth"}
              text="Логин"
            />
          </List>
        </>
      )}
    </Stack>
  );
};

export default UserPageSideBar;
