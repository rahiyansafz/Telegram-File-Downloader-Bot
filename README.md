# TelegramFileDownloaderBot

A bot to download files from internet and send them to Telegram

Send the bot the link that want to be downloaded. The bot send you the file.

## Building
Build with .Net 7. After building, copy config.json file to the output directory.

### Config File
There are 4 values you can edit at config.json
* **Token**(Required): The token of you bot in string format.
* **MaxFileSize**(Required): The maximum file size that bot can download in bytes.
* **Admins**: The user IDs of users that can use this bot. This is an int array. Pass empty array or null to remove limits.
* **DownloadPath**: The path that the temp files will be downloaded. (All downloads will be deleted after they are sent)
