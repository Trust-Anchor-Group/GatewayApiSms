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
	public class ApiTests
	{
		private static FilesProvider? filesProvider = null;
		private static ConsoleEventSink? consoleEventSink = null;

		private GatewayApiClient? client;

		[AssemblyInitialize]
		public static async Task AssemblyInitialize(TestContext _)
		{
			Types.Initialize(
				typeof(ApiTests).Assembly,
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

			Assert.IsTrue(await Types.StartAllModules(60000));
		}

		[AssemblyCleanup]
		public static async Task AssemblyCleanup()
		{
			await Types.StopAllModules();

			filesProvider?.Dispose();
			filesProvider = null;

			if (!(consoleEventSink is null))
			{
				Log.Unregister(consoleEventSink);
				consoleEventSink = null;
			}
		}

		[TestInitialize]
		public async Task TestInitialize()
		{
			string Key = await RuntimeSettings.GetAsync("GatewayAPI.Key", string.Empty);
			string Secret =  await RuntimeSettings.GetAsync("GatewayAPI.Secret", string.Empty);

			this.client = new GatewayApiClient(Key, Secret, this.Europe,
				new ConsoleOutSniffer(BinaryPresentationMethod.Base64, LineEnding.NewLine));
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.client?.Dispose();
			this.client = null;
		}

		public virtual bool Europe => false;

		[TestMethod]
		public void TestMethod1()
		{
		}

	}
}