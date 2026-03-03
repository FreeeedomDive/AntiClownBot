import React, { useEffect, useState } from "react";
import UsersApi from "../Api/UsersApi";
import DiscordMembersApi from "../Api/DiscordMembersApi";
import { useStore } from "../Stores";
import UserOverview from "./UserOverview";
import { Loader } from "../Components/Loader/Loader";
import { UserDto } from "../Dto/Users/UserDto";
import RightsApi from "../Api/RightsApi";
import {
  Collapse,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import { RightsWrapper } from "../Components/RIghts/RightsWrapper";
import { RightsDto } from "../Dto/Rights/RightsDto";
import { ChakraProvider, createSystem, defineConfig } from "@chakra-ui/react";
import F1PredictionsList from "./F1Predictions/F1PredictionsList";

const config = defineConfig({
  theme: {
    tokens: {
      colors: {},
    },
  },
});

const system = createSystem(config);

export default function MobileMainPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [isOverviewOpened, setIsOverviewOpened] = useState(true);
  const [isF1PredictionsOpened, setIsF1PredictionsOpened] = useState(false);

  const { authStore, mobileUserContextStore, rightsStore } = useStore();

  useEffect(() => {
    async function loadTelegramUser(): Promise<WebAppUser | null> {
      window.Telegram.WebApp.ready();
      const telegramUser = window.Telegram.WebApp.initDataUnsafe.user;
      mobileUserContextStore.addTelegramUser(telegramUser);
      authStore.logInViaTelegram(telegramUser?.id);

      return telegramUser;
    }

    async function loadApiUser(
      telegramUserId: number,
    ): Promise<UserDto | null> {
      const apiUser = await UsersApi.find({
        telegramId: telegramUserId,
      });

      if (!!apiUser) {
        mobileUserContextStore.addUser(apiUser);
      }

      return apiUser;
    }

    async function loadDiscordMember(userId: string) {
      const discordMember = await DiscordMembersApi.getMember(userId);
      if (!!discordMember) {
        mobileUserContextStore.addDiscordMember(discordMember);
      }
    }

    async function getUserRights(userId: string): Promise<void> {
      const rights = await RightsApi.getUserRights(userId);
      rightsStore.setRights(rights);
    }

    async function load() {
      setIsLoading(true);
      const telegramUser = await loadTelegramUser();
      if (!telegramUser) {
        return;
      }

      const apiUser = await loadApiUser(telegramUser.id);
      if (!apiUser) {
        return;
      }

      await loadDiscordMember(apiUser.id);
      await getUserRights(apiUser.id);
    }

    load().finally(() => setIsLoading(false));
  }, []);

  return (
    <ChakraProvider value={system}>
      <>
        {isLoading && <Loader />}
        {!isLoading && (
          <List>
            <ListItem key={"UserOverview"} disablePadding>
              <ListItemButton
                onClick={() => {
                  setIsOverviewOpened(!isOverviewOpened);
                  setIsF1PredictionsOpened(false);
                }}
              >
                <ListItemText primary={"Профиль"} />
                {isOverviewOpened ? <ExpandLess /> : <ExpandMore />}
              </ListItemButton>
            </ListItem>
            <Collapse in={isOverviewOpened} timeout="auto" unmountOnExit>
              <UserOverview />
            </Collapse>
            <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
              <>
                {
                  <ListItem key={"F1Predictions"} disablePadding>
                    <ListItemButton
                      onClick={() => {
                        setIsOverviewOpened(false);
                        setIsF1PredictionsOpened(!isF1PredictionsOpened);
                      }}
                    >
                      <ListItemText primary={"Предсказания F1"} />
                      {isF1PredictionsOpened ? <ExpandLess /> : <ExpandMore />}
                    </ListItemButton>
                  </ListItem>
                }
                <Collapse
                  in={isF1PredictionsOpened}
                  timeout="auto"
                  unmountOnExit
                >
                  <F1PredictionsList />
                </Collapse>
              </>
            </RightsWrapper>
          </List>
        )}
      </>
    </ChakraProvider>
  );
}
