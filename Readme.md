## Spa hosting example

Added ClientApp folder with angular application

For development:

added src/proxy.conf file to angular
```
{
  "/api": {
    "target": "https://localhost:7169",
    "secure": false
  }
}
```

updated angular.json to include the proxy config
```
    "options": {
        "proxyConfig": "src/proxy.conf.json"
    },
```

For production:

disabled hashing in angular.json `"outputHashing": "none"`

MSBuild shitfuckery build the client app and include the dist folder in the output (and only that)
```
	<Target Name="BuildCLientApp" BeforeTargets="BeforePublish">
		<!-- Build the client app -->
		<Exec Command="pnpm install" WorkingDirectory="$(MSBuildProjectDirectory)\ClientApp" />
		<Exec Command="pnpm run build" WorkingDirectory="$(MSBuildProjectDirectory)\ClientApp" />
	</Target>

	<ItemGroup>
		<Content Remove="ClientApp\**\*" />
	</ItemGroup>
	
	<ItemGroup>
		<Content Include="ClientApp\dist\**\*" CopyToPublishDirectory="PreserveNewest" />
	</ItemGroup>
```