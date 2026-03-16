import { useLocation, useNavigate, useParams } from "react-router-dom";
import React from "react";
import { observer } from "mobx-react-lite";
import { List, Stack } from "@mui/material";
import {
  AccountBalanceWallet,
  AdminPanelSettings,
  ArrowBack,
  Backpack,
  Casino,
  Login,
  Logout,
  Person,
  Settings,
  Speed,
  Store,
  SwapHoriz,
} from "@mui/icons-material";
import { useStore } from "../../../Stores";
import { UserDto } from "../../../Dto/Users/UserDto";
import { RightsWrapper } from "../../../Components/UserRights/RightsWrapper";
import { RightsDto } from "../../../Dto/Rights/RightsDto";
import UserPageSideBarItem from "./UserPageSideBarItem";

const buildLink = (userId: string, subLink?: string): string => {
  return `/user/${userId}` + (subLink ? `/${subLink}` : "");
};

interface Props {
  user: UserDto | null | undefined;
}

const ICON_SX = { fontSize: 18 };

const F1_USER_PATHS = [
  "standings",
  "rulebook",
  "races",
  "championship",
  "bingo",
].map((p) => `f1Predictions/${p}`);
const F1_ADMIN_PATHS = [
  "admin/f1Predictions/results",
  "admin/f1Predictions/championship",
  "admin/f1Predictions/bingo",
  "admin/f1Predictions/teams",
];

const UserPageSideBar = observer(({ user }: Props) => {
  const { authStore } = useStore();
  const { rightsStore } = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const { userId = "" } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;
  const navigate = useNavigate();
  const location = useLocation();

  const isF1AdminSelected = F1_ADMIN_PATHS.some((p) =>
    location.pathname.endsWith(p),
  );
  const isF1PredictionsSelected =
    !isF1AdminSelected &&
    F1_USER_PATHS.some((p) => location.pathname.endsWith(p));
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
              <UserPageSideBarItem
                sidebarKey="F1Predictions"
                link={buildLink(userId, "f1Predictions/standings")}
                text="Предсказания F1"
                icon={<Speed sx={ICON_SX} />}
                isSelected={isF1PredictionsSelected}
              />
            </RightsWrapper>
          </List>
        </>
      )}
      {isMyPage && userHasAnyAdminRights && (
        <>
          <List>
            <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
              <UserPageSideBarItem
                sidebarKey="F1Admin"
                link={buildLink(userId, "admin/f1Predictions/results")}
                text="Админка F1"
                icon={<AdminPanelSettings sx={ICON_SX} />}
                isSelected={isF1AdminSelected}
              />
            </RightsWrapper>
            <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
              <UserPageSideBarItem
                sidebarKey="Settings"
                link={buildLink(userId, "admin/settings")}
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
});

export default UserPageSideBar;
