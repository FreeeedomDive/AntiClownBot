import { useNavigate, useParams } from "react-router-dom";
import React, { useState } from "react";
import {
  Collapse,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
} from "@mui/material";
import {
  AccountBalanceWallet,
  AdminPanelSettings,
  ArrowBack,
  Assignment,
  Backpack,
  Casino,
  EmojiEvents,
  ExpandLess,
  ExpandMore,
  Flag,
  Groups,
  Leaderboard,
  Login,
  Logout,
  MenuBook,
  Person,
  Settings,
  Speed,
  Store,
  SwapHoriz,
} from "@mui/icons-material";
import { useStore } from "../../../Stores";
import { UserDto } from "../../../Dto/Users/UserDto";
import { RightsWrapper } from "../../../Components/RIghts/RightsWrapper";
import { RightsDto } from "../../../Dto/Rights/RightsDto";
import UserPageSideBarItem from "./UserPageSideBarItem";

const buildLink = (userId: string, subLink?: string): string => {
  return `/user/${userId}` + (subLink ? `/${subLink}` : "");
};

interface Props {
  user: UserDto | null | undefined;
}

const ICON_SX = { fontSize: 18 };
const TEXT_PROPS = { fontSize: "0.82rem" };

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
    <Stack direction="column" sx={{ paddingTop: "8px" }}>
      <List>
        <UserPageSideBarItem
          sidebarKey="Overview"
          link={buildLink(userId)}
          text="Профиль"
          icon={<Person sx={ICON_SX} />}
        />
      </List>
      {isMyPage && (
        <>
          <List>
            <UserPageSideBarItem
              sidebarKey="Economy"
              link={buildLink(userId, "economy")}
              text="Экономика"
              icon={<AccountBalanceWallet sx={ICON_SX} />}
            />
            <UserPageSideBarItem
              sidebarKey="Inventory"
              link={buildLink(userId, "inventory")}
              text="Инвентарь"
              icon={<Backpack sx={ICON_SX} />}
            />
            <UserPageSideBarItem
              sidebarKey="Shop"
              link={buildLink(userId, "shop")}
              text="Магазин"
              icon={<Store sx={ICON_SX} />}
            />
            <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
              <>
                <ListItem key={"F1Predictions"} disablePadding>
                  <ListItemButton
                    sx={{ mx: 1, borderRadius: 1.5 }}
                    onClick={() =>
                      setIsF1PredictionsCollapseOpened(
                        !isF1PredictionsCollapseOpened,
                      )
                    }
                  >
                    <ListItemIcon sx={{ minWidth: 32 }}>
                      <Speed sx={ICON_SX} />
                    </ListItemIcon>
                    <ListItemText
                      primary="Предсказания F1"
                      primaryTypographyProps={TEXT_PROPS}
                    />
                    {isF1PredictionsCollapseOpened ? (
                      <ExpandLess sx={ICON_SX} />
                    ) : (
                      <ExpandMore sx={ICON_SX} />
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
                      sidebarKey="F1PredictionsRulebook"
                      link={buildLink(userId, "f1Predictions/rulebook")}
                      text="Регламент"
                      icon={<MenuBook sx={ICON_SX} />}
                      nesting={2}
                      showBadge
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsStandings"
                      link={buildLink(userId, "f1Predictions/standings")}
                      text="Таблица"
                      icon={<Leaderboard sx={ICON_SX} />}
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsCurrent"
                      link={buildLink(userId, "f1Predictions/current")}
                      text="Текущие предсказания"
                      icon={<Assignment sx={ICON_SX} />}
                      nesting={2}
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsChampionship"
                      link={buildLink(userId, "f1Predictions/championship")}
                      text="Чемпионат"
                      icon={<EmojiEvents sx={ICON_SX} />}
                      nesting={2}
                      showBadge
                    />
                    <UserPageSideBarItem
                      sidebarKey="F1PredictionsBingo"
                      link={buildLink(userId, "f1Predictions/bingo")}
                      text="Бинго"
                      icon={<Casino sx={ICON_SX} />}
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
          <List>
            <ListItem key={"F1Admin"} disablePadding>
              <ListItemButton
                sx={{ mx: 1, borderRadius: 1.5 }}
                onClick={() =>
                  setIsF1AdminPredictionsCollapseOpened(
                    !isF1AdminPredictionsCollapseOpened,
                  )
                }
              >
                <ListItemIcon sx={{ minWidth: 32 }}>
                  <AdminPanelSettings sx={ICON_SX} />
                </ListItemIcon>
                <ListItemText
                  primary="Админка F1"
                  primaryTypographyProps={TEXT_PROPS}
                />
                {isF1AdminPredictionsCollapseOpened ? (
                  <ExpandLess sx={ICON_SX} />
                ) : (
                  <ExpandMore sx={ICON_SX} />
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
                  icon={<Flag sx={ICON_SX} />}
                  nesting={2}
                />
              </RightsWrapper>
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1ChampionshipPredictionsAdmin"
                  link={buildLink(userId, "f1Predictions/championship/admin")}
                  text="Чемпионат"
                  icon={<EmojiEvents sx={ICON_SX} />}
                  nesting={2}
                />
              </RightsWrapper>
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1PredictionsBingoAdmin"
                  link={buildLink(userId, "f1Predictions/bingo/admin")}
                  text="Бинго"
                  icon={<Casino sx={ICON_SX} />}
                  nesting={2}
                />
              </RightsWrapper>
              <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
                <UserPageSideBarItem
                  sidebarKey="F1PredictionsTeamsAdmin"
                  link={buildLink(userId, "f1Predictions/teams")}
                  text="Изменение команд"
                  icon={<Groups sx={ICON_SX} />}
                  nesting={2}
                />
              </RightsWrapper>
            </Collapse>
            <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
              <UserPageSideBarItem
                sidebarKey="Settings"
                link={buildLink(userId, "settings")}
                text="Настройки"
                icon={<Settings sx={ICON_SX} />}
              />
            </RightsWrapper>
          </List>
        </>
      )}
      {!isMyPage && currentLoggedInUserId && user && (
        <>
          <List>
            <UserPageSideBarItem
              sidebarKey="ItemsTrade"
              link={buildLink(userId, "itemsTrade")}
              text="Обмен предметами"
              icon={<SwapHoriz sx={ICON_SX} />}
            />
            <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
              <UserPageSideBarItem
                sidebarKey="F1PredictionsBingo"
                link={buildLink(userId, "f1Predictions/bingo")}
                text="Бинго"
                icon={<Casino sx={ICON_SX} />}
              />
            </RightsWrapper>
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <List>
            {!isMyPage && (
              <UserPageSideBarItem
                sidebarKey="BackToMyPage"
                link={buildLink(currentLoggedInUserId)}
                text="Вернуться на мою страницу"
                icon={<ArrowBack sx={ICON_SX} />}
              />
            )}
            <UserPageSideBarItem
              sidebarKey="Logout"
              link={""}
              text="Выход"
              icon={<Logout sx={ICON_SX} />}
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
          <List>
            <UserPageSideBarItem
              sidebarKey="Login"
              link={"/auth"}
              text="Логин"
              icon={<Login sx={ICON_SX} />}
            />
          </List>
        </>
      )}
    </Stack>
  );
};

export default UserPageSideBar;
