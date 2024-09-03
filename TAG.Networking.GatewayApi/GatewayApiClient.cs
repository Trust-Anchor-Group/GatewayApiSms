using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Networking.Sniffers;
using Waher.Security;

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
		private readonly string token;
		private readonly string endpoint;

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Token">Token secret.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, string Token, params ISniffer[] Sniffers)
			: this(Key, Secret, Token, false, Sniffers)
		{
		}

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Token">Token secret.</param>
		/// <param name="Europe">If operation is to be performed for the European market.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, string Token, bool Europe, params ISniffer[] Sniffers)
			: this(Key, Secret, Token, Europe ? apiEndpointEurope : apiEndpoint, Sniffers)
		{
		}

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Token">Token secret.</param>
		/// <param name="Endpoint">API Endpoint to use.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, string Token, string Endpoint, params ISniffer[] Sniffers)
			: base(Sniffers)
		{
			this.key = Key;
			this.secret = Secret;
			this.token = Token;
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

		public string GetAuthorization(string Url, params KeyValuePair<string, string>[] Arguments)
		{
			StringBuilder sb = new StringBuilder();
			DateTime Timestamp = DateTime.UtcNow;
			string TotalSeconds = ((int)Timestamp.Subtract(JSON.UnixEpoch).TotalSeconds).ToString();
			string Nonce = Guid.NewGuid().ToString();

			SortedDictionary<string, string> Sorted = new SortedDictionary<string, string>()
			{
				{ "oauth_consumer_key", this.key },
				{ "oauth_timestamp", TotalSeconds },
				{ "oauth_nonce", Nonce },
				{ "oauth_version", "1.0" },
				{ "oauth_signature_method", "HMAC-SHA1" }
			};

			foreach (KeyValuePair<string, string> P in Arguments)
				Sorted[P.Key] = P.Value;

			StringBuilder PStr = new StringBuilder();
			bool First = true;

			foreach (KeyValuePair<string, string> Pair in Sorted)
			{
				if (First)
					First = false;
				else
					PStr.Append("&");

				PStr.Append(OAuthEncode(Pair.Key));
				PStr.Append("=");
				PStr.Append(OAuthEncode(Pair.Value));
			}

			StringBuilder BStr = new StringBuilder();

			BStr.Append("POST&");
			BStr.Append(OAuthEncode(Url));
			BStr.Append('&');
			BStr.Append(OAuthEncode(PStr.ToString()));

			byte[] Key = Encoding.ASCII.GetBytes(OAuthEncode(this.secret) + "&" + OAuthEncode(this.token));
			byte[] Hash = Hashes.ComputeHMACSHA1Hash(Key, Encoding.ASCII.GetBytes(BStr.ToString()));
			string Signature = Convert.ToBase64String(Hash);

			sb.Append("OAuth oauth_consumer_key=");
			sb.Append(JSON.Encode(this.key));
			sb.Append(", oauth_nonce=");
			sb.Append(JSON.Encode(Nonce));
			sb.Append(", oauth_timestamp=");
			sb.Append(JSON.Encode(TotalSeconds));
			sb.Append(", oauth_version=\"1.0\"");
			sb.Append(", oauth_signature_method=\"HMAC-SHA1\"");
			sb.Append(", oauth_signature=");
			sb.Append(JSON.Encode(Signature));

			return sb.ToString();
		}

		private static string OAuthEncode(string s)
		{
			StringBuilder Result = new StringBuilder();

			foreach (char ch in s)
			{
				if (OAuthReserved.IndexOf(ch) < 0)
				{
					Result.Append("%");
					Result.Append(((int)ch).ToString("X2"));
				}
				else
					Result.Append(ch);
			}

			return Result.ToString();
		}

		private const string OAuthReserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";

		public async Task SendMessage(string Recipient, string Message)
		{

		}
	}
}
