import React, { useEffect, useState } from "react";
import { SettingsCategory } from "../../../../../Dto/Settings/SettingsCategory";
import SettingsApi from "../../../../../Api/SettingsApi";
import { SettingDto } from "../../../../../Dto/Settings/SettingDto";
import { RightsDto } from "../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../Components/RIghts/RightsWrapper";
import {
  Box,
  Button,
  FormControl,
  MenuItem,
  Modal,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import EditSettingRow from "./EditSettingRow";
import { Add } from "@mui/icons-material";
import AddSettings from "./AddSettings";
import { Loader } from "../../../../../Components/Loader/Loader";

const HEADER_SX = {
  color: "text.secondary",
  fontWeight: 600,
  fontSize: "0.72rem",
  textTransform: "uppercase",
  letterSpacing: "0.06em",
  py: 1.5,
  px: 2,
} as const;

const modalStyle = {
  position: "absolute" as "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 440,
  bgcolor: "background.paper",
  borderRadius: 2,
  boxShadow: 24,
  p: 3,
};

const EditSettings = () => {
  const [currentCategory, setCurrentCategory] =
    React.useState<SettingsCategory>("DiscordGuild");
  const [settings, setSettings] = useState<SettingDto[]>([]);
  const [isCreateSettingsModalOpen, setIsCreateSettingsModalOpen] =
    React.useState(false);
  const [isLoading, setIsLoading] = React.useState(true);

  async function loadSettings(category: SettingsCategory) {
    setIsLoading(true);
    const result = await SettingsApi.find(category);
    setSettings(result);
    setIsLoading(false);
  }

  useEffect(() => {
    loadSettings(currentCategory).catch(console.error);
  }, []);

  return isLoading ? (
    <Loader />
  ) : (
    <RightsWrapper requiredRights={[RightsDto.EditSettings]}>
      <Stack direction="column" spacing={2}>
        <Stack direction="row" spacing={1}>
          <FormControl fullWidth size="small">
            <Select
              labelId="category-select"
              id="category-select"
              value={currentCategory}
              onChange={async (event) => {
                const value = event.target.value as SettingsCategory;
                setCurrentCategory(value);
                await loadSettings(value);
              }}
            >
              {Object.values(SettingsCategory).map((category) => (
                <MenuItem key={category} value={category}>
                  {category}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <Button
            color="primary"
            size="small"
            variant="outlined"
            startIcon={<Add />}
            onClick={() => setIsCreateSettingsModalOpen(true)}
            sx={{ whiteSpace: "nowrap" }}
          >
            Добавить
          </Button>
        </Stack>
        <Paper variant="outlined" sx={{ borderRadius: 2, overflow: "hidden" }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={HEADER_SX}>Параметр</TableCell>
                  <TableCell sx={HEADER_SX}>Значение</TableCell>
                  <TableCell sx={{ ...HEADER_SX, width: 56 }} />
                </TableRow>
              </TableHead>
              <TableBody>
                {settings.map((setting) => (
                  <EditSettingRow
                    key={`${setting.category} ${setting.name}`}
                    setting={setting}
                  />
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
        <Modal
          open={isCreateSettingsModalOpen}
          onClose={() => setIsCreateSettingsModalOpen(false)}
        >
          <Box sx={modalStyle}>
            <AddSettings
              category={currentCategory}
              onSave={async () => {
                setIsCreateSettingsModalOpen(false);
                await loadSettings(currentCategory);
              }}
            />
          </Box>
        </Modal>
      </Stack>
    </RightsWrapper>
  );
};

export default EditSettings;
