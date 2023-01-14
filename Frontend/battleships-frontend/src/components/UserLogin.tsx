import { Card, CardBody, CardHeader } from "@chakra-ui/card";
import { Button, FormLabel, Input } from "@chakra-ui/react";
import { useState } from "react";

function RandomUsername() {
    const usernames = [
        'Jose',
        'Arthur',
        'Dimitri',
        'Dinesh'
    ];

    return usernames[
        Math.floor(Math.random() * usernames.length)
    ];
}

interface UserLoginProps {
    onLogin: (username: string) => {}
}

function UserLogin ({ onLogin }: UserLoginProps) {
    const [username, setUsername] = useState(RandomUsername());

    return (
        <Card height={'fit-content'}>
            <CardHeader>
                Welcome to Battleships
            </CardHeader>
            <CardBody>
            <FormLabel htmlFor="username">Choose a Username</FormLabel>
            <Input id="username" value={username} onChange={(event) => setUsername(event.target.value)}/>
            </CardBody>
            <Button onClick={() => onLogin(username)}> Login </Button>
        </Card>
    );
}

export default UserLogin;