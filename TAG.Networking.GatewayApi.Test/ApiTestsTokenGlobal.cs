using System.Text;
using Waher.Content;
using Waher.Events;
using Waher.Events.Console;
using Waher.Networking.Sniffers;
using Waher.Persistence;
using Waher.Persistence.Files;
using Waher.Persistence.Serialization;
using Waher.Runtime.Inventory;
using Waher.Runtime.Settings;

namespace TAG.Networking.GatewayApi.Test
{
	[TestClass]
	public class ApiTestsTokenGlobal
	{
		private const string PhoneNr1 = "";	// TODO: Enter number to use for unit tests.
		private const string PhoneNr2 = "";	// TODO: Enter number to use for unit tests.

		private static FilesProvider? filesProvider = null;
		private static ConsoleEventSink? consoleEventSink = null;

		private GatewayApiClient? client;

		[AssemblyInitialize]
		public static async Task AssemblyInitialize(TestContext _)
		{
			Types.Initialize(
				typeof(ApiTestsTokenGlobal).Assembly,
				typeof(InternetContent).Assembly,
				typeof(Database).Assembly,
				typeof(FilesProvider).Assembly,
				typeof(ObjectSerializer).Assembly,
				typeof(RuntimeSettings).Assembly);

			Log.Register(consoleEventSink = new ConsoleEventSink(false));

			if (!Database.HasProvider)
			{
				filesProvider = await FilesProvider.CreateAsync("Data", "Default", 8192, 1000, 8192, Encoding.UTF8, 10000, true);
				Database.Register(filesProvider);
			}

			// Before running tests for the first time, configure the following properties:
			// NOTE: Do not check in keys.
			//
			// await RuntimeSettings.SetAsync("GatewayAPI.Key", "");	// Enter Value here
			// await RuntimeSettings.SetAsync("GatewayAPI.Secret", "");	// Enter Value here
			// await RuntimeSettings.SetAsync("GatewayAPI.Token", "");	// Enter Value here

			Assert.IsTrue(await Types.StartAllModules(60000));
		}

		[AssemblyCleanup]
		public static async Task AssemblyCleanup()
		{
			await Types.StopAllModules();

			if (filesProvider is not null)
			{
				await filesProvider.DisposeAsync();
				filesProvider = null;
			}

			if (consoleEventSink is not null)
			{
				Log.Unregister(consoleEventSink);
				consoleEventSink = null;
			}
		}

		[TestInitialize]
		public async Task TestInitialize()
		{
			string Key = await RuntimeSettings.GetAsync("GatewayAPI.Key", string.Empty);
			string Secret = await RuntimeSettings.GetAsync("GatewayAPI.Secret", string.Empty);
			string Token = await RuntimeSettings.GetAsync("GatewayAPI.Token", string.Empty);

			this.client = new GatewayApiClient(Key, Secret, Token, this.Europe, this.AuthenticationMethod,
				new ConsoleOutSniffer(BinaryPresentationMethod.Base64, LineEnding.NewLine));
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.client?.Dispose();
			this.client = null;
		}

		public virtual AuthenticationMethod AuthenticationMethod => AuthenticationMethod.Token;
		public virtual bool Europe => false;

		[TestMethod]
		public async Task Test_01_SendSimpleMessage()
		{
			Assert.IsNotNull(this.client);
			await this.client.SendSimpleMessage("Unit Test", "Testing", PhoneNr1, PhoneNr2);
		}

		[TestMethod]
		public async Task Test_02_SendMessage()
		{
			Assert.IsNotNull(this.client);
			await this.client.SendMessage("Unit Test", "Testing", PhoneNr1, PhoneNr2);
		}

		[TestMethod]
		public async Task Test_03_SendSimpleMessage_NonAsciiCharacters()
		{
			Assert.IsNotNull(this.client);
			await this.client.SendSimpleMessage("Unit Test", "åäö ÅÄÖ éñã ÉÑÃ", PhoneNr1, PhoneNr2);
		}

		[TestMethod]
		public async Task Test_04_SendMessage_NonAsciiCharacters()
		{
			Assert.IsNotNull(this.client);
			await this.client.SendMessage("Unit Test", "åäö ÅÄÖ éñã ÉÑÃ", PhoneNr1, PhoneNr2);
		}

	}
}