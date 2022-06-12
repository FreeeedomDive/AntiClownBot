import React from "react";
import AdminPanelElement from "../AdminPanelElement/AdminPanelElement";
import DiscordBotSettingsEditor from "../DiscordBotSettingsEditor/DiscordBotSettingsEditor";
import LogsComponent from "../LogsComponent/LogsComponent";

interface State {
  showLogsComponent: boolean,
  showDiscordBotSettings: boolean
}

class AdminPanel extends React.Component<{}, State> {
  public state: State = {
    showLogsComponent: false,
    showDiscordBotSettings: false
  }

  patchSelectedSettings = (modifiedState: {}) => {
    this.setState({
      ...{
        showLogsComponent: false,
        showDiscordBotSettings: false
      },
      ...modifiedState
    })
  }

  render() {
    return (
      <div className="wrapper">
        <div className="settingsRoutes">
          <AdminPanelElement name={"Просмотр логов"} handleOnClick={() => {
            this.patchSelectedSettings({showLogsComponent: true});
          }}/>
          <AdminPanelElement name={"Настройки дискорд бота"} handleOnClick={() => {
            this.patchSelectedSettings({showDiscordBotSettings: true});
          }}/>
        </div>
        <div className="settingsEditor">
          {
            this.state.showLogsComponent && <DiscordBotSettingsEditor />
          }
          {
            this.state.showDiscordBotSettings && <LogsComponent />
          }
        </div>
      </div>
    )
  }
}

export default AdminPanel;