import React from "react";
import styles from './AdminPanelElement.module.css';

interface Props {
    name: string,
    handleOnClick(): void
}

class AdminPanelElement extends React.Component<Props, { }>{

    render(){
        return (
            <div className={styles.settingsBlock} onClick={() => this.props.handleOnClick()}>
                {this.props.name}
            </div>
        );
    }

}

export default AdminPanelElement;