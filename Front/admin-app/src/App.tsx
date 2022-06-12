import React from 'react';
import './App.css';
import AdminPanel from "./components/AdminPanel/AdminPanel";

interface State {
  showLogsComponent: boolean,
  showDiscordBotSettings: boolean
}

class App extends React.Component<{}, State> {

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
        ...modifiedState})
  }

  render(){
    return (
      <div className="App">
        <div className="header">Admin dashboard</div>
        <div className="App-content">
          <AdminPanel />
        </div>
      </div>
    )
  }
}

export default App;
