namespace TAG.Networking.GatewayApi.Test
{
	[TestClass]
	public class ApiTestsBasicEurope : ApiTestsTokenEurope
	{
		public override AuthenticationMethod AuthenticationMethod => AuthenticationMethod.Basic;
	}
}