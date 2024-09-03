
namespace TAG.Networking.GatewayApi.Test
{
	[TestClass]
	public class ApiTestsBasicGlobal : ApiTestsTokenGlobal
	{
		public override AuthenticationMethod AuthenticationMethod => AuthenticationMethod.Basic;
	}
}