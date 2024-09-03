using System.IO;
using System.Threading.Tasks;
using TAG.Networking.GatewayApi;
using Waher.IoTGateway;
using Waher.Networking.HTTP;
using Waher.Networking.Sniffers;

namespace TAG.Service.GatewayApi
{
	/// <summary>
	/// Service module for the GatewayAPI service integration.
	/// </summary>
	public class GatewayApiService : IConfigurableModule
	{
		/// <summary>
		/// Reference to client sniffer for Transbank communication.
		/// </summary>
		private static XmlFileSniffer xmlFileSniffer = null;

		/// <summary>
		/// Sniffable object that can be sniffed on dynamically.
		/// </summary>
		private static readonly Sniffable sniffable = new Sniffable();

		/// <summary>
		/// Sniffer proxy, forwarding sniffer events to <see cref="sniffable"/>.
		/// </summary>
		private static readonly SnifferProxy snifferProxy = new SnifferProxy(sniffable);

		/// <summary>
		/// Service module for the GatewayAPI service integration.
		/// </summary>
		public GatewayApiService()
		{
		}

		/// <summary>
		/// Method called when module has been loaded, and will be started.
		/// </summary>
		public Task Start()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Method called before unloading the module and stopping the broker.
		/// </summary>
		public Task Stop()
		{
			xmlFileSniffer?.Dispose();
			xmlFileSniffer = null;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Pages displayable on the administration page.
		/// </summary>
		public Task<IConfigurablePage[]> GetConfigurablePages()
		{
			return Task.FromResult(new IConfigurablePage[]
			{
				new ConfigurablePage("GatewayAPI (SMS)", "/GatewayAPI/Settings.md", "Admin.SMS.GatewayAPI")
			});
		}

		internal static GatewayApiClient CreateClient(ServiceConfiguration Configuration)
		{
			if (!Configuration.IsWellDefined)
				return null;

			if (xmlFileSniffer is null)
			{
				xmlFileSniffer = new XmlFileSniffer(Gateway.AppDataFolder + "GatewayApi" + Path.DirectorySeparatorChar +
					"Log %YEAR%-%MONTH%-%DAY%T%HOUR%.xml",
					Gateway.AppDataFolder + "Transforms" + Path.DirectorySeparatorChar + "SnifferXmlToHtml.xslt",
					7, BinaryPresentationMethod.Base64);
			}

			return new GatewayApiClient(Configuration.ApiKey, Configuration.ApiSecret, Configuration.ApiToken, Configuration.ApiEurope,
				AuthenticationMethod.Token, snifferProxy);
		}

		internal static void Dispose(GatewayApiClient Client)
		{
			Client?.Remove(xmlFileSniffer);
			Client?.Remove(snifferProxy);
			Client?.Dispose();
		}

		/// <summary>
		/// Sends an SMS message
		/// </summary>
		/// <param name="Sender">Sender of message.</param>
		/// <param name="Message">Message text.</param>
		/// <param name="Recipients">Recipients to receive the message.</param>
		/// <exception cref="ServiceUnavailableException">If service has not been configured properly.</exception>
		public static async Task SendSMS(string Sender, string Message, params string[] Recipients)
		{
			ServiceConfiguration Configuration = await ServiceConfiguration.GetCurrent();
			GatewayApiClient Client = CreateClient(Configuration);
			
			if (Configuration is null)
				throw new ServiceUnavailableException("GatewayAPI not properly configured.");

			try
			{
				await Client.SendSimpleMessage(Sender, Message, Recipients);
			}
			finally
			{
				Dispose(Client);
			}
		}

		/// <summary>
		/// Registers a web sniffer on the GatewayAPI client.
		/// </summary>
		/// <param name="SnifferId">Sniffer ID</param>
		/// <param name="Request">HTTP Request for sniffer page.</param>
		/// <param name="UserVariable">Name of user variable.</param>
		/// <param name="Privileges">Privileges required to view content.</param>
		/// <returns>Code to embed into page.</returns>
		public static string RegisterSniffer(string SnifferId, HttpRequest Request,
			string UserVariable, params string[] Privileges)
		{
			return Gateway.AddWebSniffer(SnifferId, Request, sniffable, UserVariable, Privileges);
		}

	}
}
