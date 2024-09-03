
namespace TAG.Networking.GatewayApi.Test
{
	[TestClass]
	public class ApiTestsOAuthGlobal : ApiTestsTokenGlobal
	{
		public override AuthenticationMethod AuthenticationMethod => AuthenticationMethod.OAuth1;
	}
}