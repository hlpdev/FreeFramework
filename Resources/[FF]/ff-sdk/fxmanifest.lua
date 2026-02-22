fx_version('cerulean')
game('gta5')

files {
    "*.dll"
}

server_scripts {
    'FreeFramework.SDK.Server.net.dll',
    '*Server.net.dll'
}

client_scripts {
    'FreeFramework.SDK.Client.net.dll',
    '*Client.net.dll'
}