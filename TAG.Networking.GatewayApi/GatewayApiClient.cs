using System;
using Waher.Networking.Sniffers;

namespace TAG.Networking.GatewayApi
{
	/// <summary>
	/// Client managing communication with GatewayAPI.com.
	/// </summary>
	public class GatewayApiClient : Sniffable, IDisposable
	{
		private const string apiEndpoint = "https://gatewayapi.com/";
		private const string apiEndpointEurope = "https://gatewayapi.eu/";

		private readonly string key;
		private readonly string secret;
		private readonly string endpoint;

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, params ISniffer[] Sniffers)
			: this(Key, Secret, false, Sniffers)
		{
		}

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Europe">If operation is to be performed for the European market.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, bool Europe, params ISniffer[] Sniffers)
			: this(Key, Secret, Europe ? apiEndpointEurope : apiEndpoint, Sniffers)
		{
		}

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Endpoint">API Endpoint to use.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, string Endpoint, params ISniffer[] Sniffers)
			: base(Sniffers)
		{
			this.key = Key;
			this.secret = Secret;
			this.endpoint = Endpoint;
		}

		/// <summary>
		/// <see cref="IDisposable"/>
		/// </summary>
		public void Dispose()
		{
			if (this.HasSniffers)
			{
				foreach (ISniffer Sniffer in this.Sniffers)
				{
					this.Remove(Sniffer);
					if (Sniffer is IDisposable Disposable)
						Disposable.Dispose();
				}
			}
		}
	}
}
