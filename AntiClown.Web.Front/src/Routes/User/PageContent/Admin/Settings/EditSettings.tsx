import React, {useEffect, useState} from "react";
import {SettingsCategory} from "../../../../../Dto/Settings/SettingsCategory";
import SettingsApi from "../../../../../Api/SettingsApi";
import {SettingDto} from "../../../../../Dto/Settings/SettingDto";
import {RightsDto} from "../../../../../Dto/Rights/RightsDto";
import {RightsWrapper} from "../../../../../Components/RIghts/RightsWrapper";
import {FormControl, MenuItem, Select, Stack, Table, TableBody, TableContainer} from "@mui/material";
import EditSettingRow from "./EditSettingRow";

const EditSettings = () => {
    const [currentCategory, setCurrentCategory] =
        React.useState<SettingsCategory>("DiscordGuild")
    const [settings, setSettings] = useState<SettingDto[]>([])

    async function loadSettings(category: SettingsCategory) {
        const result = await SettingsApi.find(category);
        setSettings(result)
    }

    useEffect(() => {
        loadSettings(currentCategory);
    }, []);

    return (<RightsWrapper requiredRights={[RightsDto.EditSettings]}>
        <Stack direction={"column"} spacing={4}>
            <FormControl fullWidth>
                <Select
                    labelId="category-select"
                    id="category-select"
                    value={currentCategory}
                    onChange={async (event) => {
                        const value = event.target.value as SettingsCategory;
                        setCurrentCategory(value)
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
            <TableContainer>
                <Table>
                    <TableBody>
                        {
                            settings.map(
                                (setting) =>
                                    <EditSettingRow
                                        key={`${setting.category} ${setting.name}`}
                                        setting={setting}
                                    />)
                        }
                    </TableBody>
                </Table>
            </TableContainer>
        </Stack>
    </RightsWrapper>)
}

export default EditSettings;