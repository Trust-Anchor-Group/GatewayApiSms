namespace TAG.Networking.GatewayApi.Test
{
	[TestClass]
	public class ApiTestsOAuthEurope : ApiTestsTokenEurope
	{
		public override AuthenticationMethod AuthenticationMethod => AuthenticationMethod.OAuth1;
	}
}