2.3

- More code refactory
- Add EFCore support of WithSerialization
- Performance moniting interface
	- Report http request time to interface
	- Report database operation time to interface

3.0

- Dynamic plugin reload support
	- On .NET Framework: Multiple AppDomain
	- On .NET Core: Collectible Assembly (3.0+)

Future

- Update dapper context since query transaction is supported in dommel
	- It's in 2.0 beta, not stabel yet
- APM Integration
	- https://github.com/ButterflyAPM/butterfly-csharp
	- https://github.com/OpenSkywalking/skywalking-netcore