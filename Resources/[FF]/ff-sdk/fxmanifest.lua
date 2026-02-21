fx_version('cerulean')
game('gta5')

files {
    "FreeFramework.SDK.dll",
    "JetBrains.Annotations.dll",
    "Microsoft.Bcl.AsyncInterfaces.dll",
    "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
    "Microsoft.Extensions.DependencyInjection.dll",
    "Microsoft.Extensions.Logging.Abstractions.dll",
    "System.Threading.Tasks.Extensions.dll"
}

dependencies {
    'baseevents',
    'spawnmanager'
}

server_script('FreeFramework.SDK.Server.net.dll')
client_script('FreeFramework.SDK.Client.net.dll')