import {observer} from "mobx-react-lite";
import {useLocation, useNavigate, useParams} from "react-router-dom";
import React from "react";
import {Divider, List, ListItem, ListItemButton, ListItemText, Stack,} from "@mui/material";
import {useStore} from "../../../Stores";

const buildLink = (userId: string, subLink?: string): string => {
  return `/user/${userId}` + (subLink ? `/${subLink}` : "");
}

const UserPageSideBar = observer(() => {
  const {authStore} = useStore();
  const currentLoggedInUserId = authStore.userId;
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
            <ListItemText primary={"User"}/>
          </ListItemButton>
        </ListItem>
      </List>
      {isMyPage && (
        <>
          <Divider />
          <List>
            <ListItem key={"Inventory"} disablePadding>
              <ListItem key={"Economy"} disablePadding>
                <ListItemButton
                  onClick={() => navigate(buildLink(userId, "economy"))}
                  selected={location.pathname === buildLink(userId, "economy")}
                >
                  <ListItemText primary={"Economy"}/>
                </ListItemButton>
              </ListItem>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "inventory"))}
                selected={location.pathname === buildLink(userId, "inventory")}
              >
                <ListItemText primary={"Inventory"}/>
              </ListItemButton>
            </ListItem>
            <ListItem key={"Shop"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "shop"))}
                selected={location.pathname === buildLink(userId, "shop")}
              >
                <ListItemText primary={"Shop"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {!isMyPage && currentLoggedInUserId && (
        <>
          <Divider/>
          <List>
            <ListItem key={"Items trade"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "itemsTrade"))}
                selected={location.pathname === buildLink(userId, "itemsTrade")}
              >
                <ListItemText primary={"Items trade"}/>
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <Divider/>
          <List>
            <ListItem key={"Logout"} disablePadding>
              <ListItemButton
                onClick={() => {
                  authStore.logOut();
                  navigate(buildLink(userId));
                }}
              >
                <ListItemText primary={"Log out"} />
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {!currentLoggedInUserId && (
        <>
          <Divider />
          <List>
            <ListItem key={"Login"} disablePadding>
              <ListItemButton
                onClick={() => navigate("/auth")}
              >
                <ListItemText primary={"Log in"} />
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
    </Stack>
  );
});

export default UserPageSideBar;