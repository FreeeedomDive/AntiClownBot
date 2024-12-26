import { useLocation, useNavigate, useParams } from "react-router-dom";
import React, { useState } from "react";
import {
  Badge,
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
  const location = useLocation();
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
        <ListItem key="Overview" disablePadding>
          <ListItemButton
            onClick={() => navigate(buildLink(userId))}
            selected={location.pathname === buildLink(userId)}
          >
            <ListItemText primary={"Профиль"} />
          </ListItemButton>
        </ListItem>
      </List>
      {isMyPage && (
        <>
          <Divider />
          <List>
            <ListItem key={"Economy"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "economy"))}
                selected={location.pathname === buildLink(userId, "economy")}
              >
                <Badge
                  anchorOrigin={{
                    vertical: "top",
                    horizontal: "left",
                  }}
                  variant="dot"
                  color="warning"
                >
                  <ListItemText primary={"Экономика"} />
                </Badge>
              </ListItemButton>
            </ListItem>
            <ListItem key={"Inventory"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "inventory"))}
                selected={location.pathname === buildLink(userId, "inventory")}
              >
                <ListItemText primary={"Инвентарь"} />
              </ListItemButton>
            </ListItem>
            <ListItem key={"Shop"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "shop"))}
                selected={location.pathname === buildLink(userId, "shop")}
              >
                <ListItemText primary={"Магазин"} />
              </ListItemButton>
            </ListItem>
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
                    <ListItem key={"F1PredictionsStandings"} disablePadding>
                      <ListItemButton
                        sx={{ pl: 4 }}
                        onClick={() =>
                          navigate(buildLink(userId, "f1Predictions/standings"))
                        }
                        selected={
                          location.pathname ===
                          buildLink(userId, "f1Predictions/standings")
                        }
                      >
                        <Badge
                          anchorOrigin={{
                            vertical: "top",
                            horizontal: "left",
                          }}
                          variant="dot"
                          color="warning"
                        >
                          <ListItemText primary="Таблица" />
                        </Badge>
                      </ListItemButton>
                    </ListItem>
                    <ListItem key={"F1PredictionsCurrent"} disablePadding>
                      <ListItemButton
                        sx={{ pl: 4 }}
                        onClick={() =>
                          navigate(buildLink(userId, "f1Predictions/current"))
                        }
                        selected={
                          location.pathname ===
                          buildLink(userId, "f1Predictions/current")
                        }
                      >
                        <ListItemText primary="Текущие предсказания" />
                      </ListItemButton>
                    </ListItem>
                    <RightsWrapper
                      requiredRights={[RightsDto.F1PredictionsAdmin]}
                    >
                      <ListItem key={"F1PredictionsAdmin"} disablePadding>
                        <ListItemButton
                          sx={{ pl: 4 }}
                          onClick={() =>
                            navigate(buildLink(userId, "f1Predictions/admin"))
                          }
                          selected={
                            location.pathname ===
                            buildLink(userId, "f1Predictions/admin")
                          }
                        >
                          <ListItemText primary={"Админка результатов"} />
                        </ListItemButton>
                      </ListItem>
                    </RightsWrapper>
                    <RightsWrapper
                      requiredRights={[RightsDto.F1PredictionsAdmin]}
                    >
                      <ListItem key={"F1PredictionsTeamsAdmin"} disablePadding>
                        <ListItemButton
                          sx={{ pl: 4 }}
                          onClick={() =>
                            navigate(buildLink(userId, "f1Predictions/teams"))
                          }
                          selected={
                            location.pathname ===
                            buildLink(userId, "f1Predictions/teams")
                          }
                        >
                          <ListItemText primary={"Команды"} />
                        </ListItemButton>
                      </ListItem>
                    </RightsWrapper>
                  </List>
                </Collapse>
              </>
            </RightsWrapper>
            <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
              <ListItem key={"Settings"} disablePadding>
                <ListItemButton
                  onClick={() => navigate(buildLink(userId, "settings"))}
                  selected={location.pathname === buildLink(userId, "settings")}
                >
                  <ListItemText primary={"Настройки"} />
                </ListItemButton>
              </ListItem>
            </RightsWrapper>
          </List>
        </>
      )}
      {!isMyPage && currentLoggedInUserId && user && (
        <>
          <Divider />
          <List>
            <ListItem key={"ItemsTrade"} disablePadding>
              <ListItemButton
                onClick={() => navigate(buildLink(userId, "itemsTrade"))}
                selected={location.pathname === buildLink(userId, "itemsTrade")}
              >
                <ListItemText primary={"Обмен предметами"} />
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
      {currentLoggedInUserId && (
        <>
          <Divider />
          <List>
            {!isMyPage && (
              <ListItem key="BackToMyPage" disablePadding>
                <ListItemButton
                  onClick={async () => {
                    navigate(buildLink(currentLoggedInUserId));
                  }}
                >
                  <ListItemText primary={"Вернуться на мою страницу"} />
                </ListItemButton>
              </ListItem>
            )}
            <ListItem key={"Logout"} disablePadding>
              <ListItemButton
                onClick={() => {
                  authStore.logOut();
                  navigate(buildLink(userId));
                }}
              >
                <ListItemText primary={"Выход"} />
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
              <ListItemButton onClick={() => navigate("/auth")}>
                <ListItemText primary={"Логин"} />
              </ListItemButton>
            </ListItem>
          </List>
        </>
      )}
    </Stack>
  );
};

export default UserPageSideBar;
