import { SettingDto } from "../../../../../Dto/Settings/SettingDto";
import { TableCell, TableRow, TextField, Typography } from "@mui/material";
import React, { useCallback } from "react";
import { Save } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import SettingsApi from "../../../../../Api/SettingsApi";

interface Props {
  setting: SettingDto;
}

const EditSettingRow = ({ setting }: Props) => {
  const [settingValue, setSettingValue] = React.useState<string>(setting.value);
  const [isSaving, setIsSaving] = React.useState(false);

  const saveSettings = useCallback(async () => {
    setIsSaving(true);
    await SettingsApi.save({
      category: setting.category,
      name: setting.name,
      value: settingValue,
    });
    setIsSaving(false);
  }, [setting.category, setting.name, settingValue]);

  return (
    <TableRow sx={{ "&:hover": { bgcolor: "action.hover" } }}>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <Typography variant="body2" fontFamily="monospace">
          {setting.name}
        </Typography>
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <TextField
          size="small"
          fullWidth
          value={settingValue}
          onChange={(e) => setSettingValue(e.target.value)}
        />
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2, width: 56 }}>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="primary"
          size="small"
          variant="text"
          startIcon={<Save />}
          onClick={saveSettings}
        />
      </TableCell>
    </TableRow>
  );
};

export default EditSettingRow;
