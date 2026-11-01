dotnet publish Flow.Launcher.Plugin.GlazeWM -c Release -r win-x64 --no-self-contained
Compress-Archive -Path "Flow.Launcher.Plugin.GlazeWM/bin/Release/win-x64/publish/*" -DestinationPath "Flow.Launcher.Plugin.GlazeWM/bin/GlazeWM.zip" -Force
