IF "%SteamAccountUsername%"=="" ECHO SteamAccountUsername Environment Variable is not defined
IF "%SteamAccountPassword%"=="" ECHO SteamAccountPassword Environment Variable is not defined

steamcmd.exe +login %SteamAccountUsername% %SteamAccountPassword% +run_app_build_http %~dp0/app.vdf +quit
