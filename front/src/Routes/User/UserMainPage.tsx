import React from "react";
import {useParams} from "react-router-dom";

export default function UserMainPage(): React.ReactElement {
    const { userId } = useParams<"userId">();
    return <div>This is user {userId}</div>
};