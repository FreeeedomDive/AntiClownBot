import {Button, FormControl, MenuItem, OutlinedInput, Select, Stack, Typography} from "@mui/material";
import {SettingsCategory} from "../../../../../Dto/Settings/SettingsCategory";
import React, {useCallback} from "react";
import {Add} from "@mui/icons-material";
import {LoadingButton} from "@mui/lab";
import SettingsApi from "../../../../../Api/SettingsApi";

interface Props {
    category: SettingsCategory;
    onSave: () => Promise<void>;
}

const AddSettings = ({category, onSave}: Props) => {
    const [selectedCategory, setCategory] = React.useState(category);
    const [newSettingName, setNewSettingName] = React.useState("")
    const [newSettingValue, setNewSettingValue] = React.useState("")
    const [isSaving, setIsSaving] = React.useState(false);

    const createNewSetting = useCallback(async () => {
        setIsSaving(true)
        await SettingsApi.save({
            category: selectedCategory,
            name: newSettingName,
            value: newSettingValue
        })
        setIsSaving(false)
        await onSave()
    }, [newSettingName, newSettingValue, selectedCategory]);

    return (
        <Stack spacing={2} direction="column">
            <Typography variant={"h6"}>Добавить новую настройку</Typography>
            <FormControl fullWidth>
                <Select
                    labelId="category-select"
                    id="category-select"
                    value={selectedCategory}
                    onChange={async (event) => {
                        const value = event.target.value as SettingsCategory;
                        setCategory(value)
                    }}
                >
                    {Object.values(SettingsCategory).map((category) => (
                        <MenuItem key={category} value={category}>
                            {category}
                        </MenuItem>
                    ))}
                </Select>
            </FormControl>
            <FormControl>
                <OutlinedInput
                    id="outlined-adornment-weight"
                    aria-describedby="outlined-weight-helper-text"
                    placeholder={"Название"}
                    value={newSettingName}
                    onChange={(event) => {
                        setNewSettingName(event.target.value)
                    }}
                />
            </FormControl>
            <FormControl>
                <OutlinedInput
                    id="outlined-adornment-weight"
                    aria-describedby="outlined-weight-helper-text"
                    placeholder={"Значение"}
                    value={newSettingValue}
                    onChange={(event) => {
                        setNewSettingValue(event.target.value)
                    }}
                />
            </FormControl>
            <LoadingButton
                loading={isSaving}
                disabled={isSaving}
                color="primary"
                size="large"
                variant="outlined"
                startIcon={<Add/>}
                onClick={createNewSetting}
            >
                Добавить
            </LoadingButton>
        </Stack>
    )
}

export default AddSettings