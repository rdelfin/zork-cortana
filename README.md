# Zork Game Server

This is a small web service that allows you to play Zork through an HTTP API
using a JSON request format. This esencially works by running the
`gamebin/zork` executable with the appropriate save file inside `saves/`.
When a game request is sent with a given conversation ID, the web server first
checks if the file exists already. If so, it runs the executable with that
save file, runs `restore`, followed by the command, followed by `"save\nquit\ny"`
to ensure the game exits. If it does not already exist, it runs zork with a new
savefile, named `saves/${CONV_ID}.dat`, where `${CONV_ID}` is the conversation
ID mentioned above. It then saves the game an exits. In both cases, the game
grabs the console output, filters out the unnecessary text, and returns the
output as the HTTP response.

## Request Format

The request should be sent as a JSON with the following format:

```
POST /
```
Headers:
```
X-Password: <PASSWORD>
```

Body:
```
{
    "conversation_id": "<CONVERSATION_ID>",
    "command": "<COMMAND>"
}
```

* <PASSWORD>: The plaintext password used to authenticate the request. This is a required field.
* <CONVERSATION_ID>: A unique string identifier for the conversation. Will be used to pick up the correct game after consecutive requests. This field is required.
* <COMMAND>: The command/input to be sent to the Zork game. This field is optional (specially for the first request) and will default to the empty string.

## Response

There are two possible responses. In case of a successful request, it will return a JSON of the format:
```
{
    "message": "<MESSAGE>"
}
```

Where <MESSAGE> is the string returned by the game to display and read.

Alternatively, if there's an error, it will return the appropriate HTTP error code and the following JSON
format:

```
{
    "error": "<HTTP ERROR>: <ERROR DESCRIPTION>"
}
```

Here, <HTTP ERROR> is an HTTP error number and name, and <ERROR DESCRIPTION> is a long form description
of the cause of the error.
