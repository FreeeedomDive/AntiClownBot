import {SettingDto} from "../../../../../Dto/Settings/SettingDto";
import {FormControl, OutlinedInput, TableCell, TableRow, Typography} from "@mui/material";
import React, {useCallback} from "react";
import {Save} from "@mui/icons-material";
import {LoadingButton} from "@mui/lab";
import SettingsApi from "../../../../../Api/SettingsApi";

interface Props {
    setting: SettingDto;
}

const EditSettingRow = ({setting}: Props) => {
    const [settingValue, setSettingValue] = React.useState<string>(setting.value);
    const [isSaving, setIsSaving] = React.useState(false);

    const saveSettings = useCallback(async () => {
        setIsSaving(true)
        await SettingsApi.save({
            category: setting.category,
            name: setting.name,
            value: settingValue
        })
        setIsSaving(false)
    }, [setting.category, setting.name, settingValue]);

    return (
        <TableRow>
            <TableCell sx={{padding: '1px'}}>
                <Typography>{setting.name}</Typography>
            </TableCell>
            <TableCell sx={{padding: '1px'}}>
                <FormControl>
                    <OutlinedInput
                        id="outlined-adornment-weight"
                        aria-describedby="outlined-weight-helper-text"
                        value={settingValue}
                        onChange={(event) => {
                            setSettingValue(event.target.value)
                        }}
                    />
                </FormControl>
            </TableCell>
            <TableCell sx={{padding: '1px'}}>
                <LoadingButton
                    loading={isSaving}
                    disabled={isSaving}
                    color="primary"
                    size="large"
                    variant="text"
                    startIcon={<Save/>}
                    onClick={saveSettings}
                />
            </TableCell>
        </TableRow>
    )
}

export default EditSettingRow;