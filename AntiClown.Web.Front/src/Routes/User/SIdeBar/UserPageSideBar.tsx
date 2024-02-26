import {useLocation, useNavigate, useParams} from "react-router-dom";
import React from "react";
import {Divider, List, ListItem, ListItemButton, ListItemText, Stack,} from "@mui/material";
import {useStore} from "../../../Stores";
import {UserDto} from "../../../Dto/Users/UserDto";

const buildLink = (userId: string, subLink?: string): string => {
  return `/user/${userId}` + (subLink ? `/${subLink}` : "");
}

interface Props {
  user: UserDto | undefined;
  updateViewedUser: (userId: string) => Promise<void>;
}

const UserPageSideBar = ({user, updateViewedUser}: Props) => {
  const {authStore} = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const {userId = ""} = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Stack direction="column" sx={{
      paddingTop: "8px"
    }}>
      <List>
        <ListItem key="Overview" disablePadding>
          <ListItemButton
            onClick={() => navigate(buildLink(userId))}
            selected={location.pathname === buildLink(userId)}
          >
            <ListItemText primary={"Профиль"}/>
          </ListItemButton>
        </ListItem>
      </List>
      {isMyPage && (
        <>
          <Divider/>
          <List>
            <ListItem key={"Economy"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "economy"))}
                selected={location.pathname === buildLink(userId, "economy")}
              >
                <ListItemText primary={"Экономика"}/>
              </ListItemButton>
            </ListItem>
            <ListItem key={"Inventory"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "inventory"))}
                selected={location.pathname === buildLink(userId, "inventory")}
              >
                <ListItemText primary={"Инвентарь"}/>
              </ListItemButton>
            </ListItem>
            <ListItem key={"Shop"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "shop"))}
                selected={location.pathname === buildLink(userId, "shop")}
              >
                <ListItemText primary={"Магазин"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {!isMyPage && currentLoggedInUserId && user && (
        <>
          <Divider/>
          <List>
            <ListItem key={"ItemsTrade"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "itemsTrade"))}
                selected={location.pathname === buildLink(userId, "itemsTrade")}
              >
                <ListItemText primary={"Обмен предметами"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <Divider/>
          <List>
            {
              !isMyPage && (
                <ListItem key="BackToMyPage" disablePadding>
                  <ListItemButton onClick={async () => {
                    navigate(buildLink(currentLoggedInUserId))
                    await updateViewedUser(currentLoggedInUserId);
                  }}>
                    <ListItemText primary={"Вернуться на мою страницу"}/>
                  </ListItemButton>
                </ListItem>
              )
            }
            <ListItem key={"Logout"} disablePadding>
              <ListItemButton
                onClick={() => {
                  authStore.logOut();
                  navigate(buildLink(userId));
                }}
              >
                <ListItemText primary={"Выход"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {!currentLoggedInUserId && (
        <>
          <Divider/>
          <List>
            <ListItem key={"Login"} disablePadding>
              <ListItemButton
                onClick={() => navigate("/auth")}
              >
                <ListItemText primary={"Логин"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
    </Stack>
  );
};

export default UserPageSideBar;