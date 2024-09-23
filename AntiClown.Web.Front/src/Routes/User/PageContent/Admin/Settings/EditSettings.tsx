import React, {useEffect, useState} from "react";
import {SettingsCategory} from "../../../../../Dto/Settings/SettingsCategory";
import SettingsApi from "../../../../../Api/SettingsApi";
import {SettingDto} from "../../../../../Dto/Settings/SettingDto";
import {RightsDto} from "../../../../../Dto/Rights/RightsDto";
import {RightsWrapper} from "../../../../../Components/RIghts/RightsWrapper";
import {
    Box,
    Button,
    FormControl,
    MenuItem, Modal,
    Select,
    Stack,
    Table,
    TableBody,
    TableContainer
} from "@mui/material";
import EditSettingRow from "./EditSettingRow";
import {Add} from "@mui/icons-material";
import AddSettings from "./AddSettings";

const modalStyle = {
    position: 'absolute' as 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    bgcolor: 'background.paper',
    border: '2px solid #000',
    boxShadow: 24,
    pt: 2,
    px: 4,
    pb: 3,
};

const EditSettings = () => {
    const [currentCategory, setCurrentCategory] =
        React.useState<SettingsCategory>("DiscordGuild")
    const [settings, setSettings] = useState<SettingDto[]>([])
    const [isCreateSettingsModalOpen, setIsCreateSettingsModalOpen] = React.useState(false);

    async function loadSettings(category: SettingsCategory) {
        const result = await SettingsApi.find(category);
        setSettings(result)
    }

    useEffect(() => {
        loadSettings(currentCategory);
    }, []);

    return (<RightsWrapper requiredRights={[RightsDto.EditSettings]}>
        <Stack direction={"column"} spacing={4}>
            <Stack direction={"row"} spacing={1}>
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
                <Button
                    color="primary"
                    size="large"
                    variant="outlined"
                    startIcon={<Add/>}
                    onClick={() => setIsCreateSettingsModalOpen(true)}
                >
                    Добавить
                </Button>
            </Stack>
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
            <Modal
                open={isCreateSettingsModalOpen}
                onClose={() => setIsCreateSettingsModalOpen(false)}
                aria-labelledby="parent-modal-title"
                aria-describedby="parent-modal-description"
            >
                <Box sx={{ ...modalStyle, width: 400 }}>
                    <AddSettings category={currentCategory} onSave={async () => {
                        setIsCreateSettingsModalOpen(false)
                        await loadSettings(currentCategory)
                    }}/>
                </Box>
            </Modal>
        </Stack>
    </RightsWrapper>)
}

export default EditSettings;