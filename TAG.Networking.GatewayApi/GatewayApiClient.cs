using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Waher.Content;
using Waher.Content.Getters;
using Waher.Content.Json;
using Waher.Networking;
using Waher.Networking.Sniffers;
using Waher.Security;

namespace TAG.Networking.GatewayApi
{
	/// <summary>
	/// Authentication mechanism to use when communicating with the Gateway API.
	/// </summary>
	public enum AuthenticationMethod
	{
		/// <summary>
		/// Token, in clear text
		/// </summary>
		Token,

		/// <summary>
		/// BASIC authentication
		/// </summary>
		Basic,

		/// <summary>
		/// OAUTH v1.0
		/// 
		/// Ref:
		/// https://datatracker.ietf.org/doc/html/rfc5849
		/// </summary>
		OAuth1
	}

	/// <summary>
	/// Client managing communication with GatewayAPI.com.
	/// </summary>
	public class GatewayApiClient : CommunicationLayer, IDisposable
	{
		private const string apiEndpoint = "https://gatewayapi.com/";
		private const string apiEndpointEurope = "https://gatewayapi.eu/";

		private readonly string key;
		private readonly string secret;
		private readonly string token;
		private readonly string endpoint;
		private readonly AuthenticationMethod authMechanism;

		/// <summary>
		/// Client managing communication with GatewayAPI.com.
		/// </summary>
		/// <param name="Key">Key used to identify the account.</param>
		/// <param name="Secret">Secret used to authenticate yourself.</param>
		/// <param name="Token">Token secret.</param>
		/// <param name="Sniffers">Any sniffers associated with the client.</param>
		public GatewayApiClient(string Key, string Secret, string Token,
			AuthenticationMethod AuthMechanism, params ISniffer[] Sniffers)
			: this(Key, Secret, Token, false, AuthMechanism, Sniffers)
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
		public GatewayApiClient(string Key, string Secret, string Token, bool Europe,
			AuthenticationMethod AuthMechanism, params ISniffer[] Sniffers)
			: this(Key, Secret, Token, Europe ? apiEndpointEurope : apiEndpoint, AuthMechanism, Sniffers)
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
		public GatewayApiClient(string Key, string Secret, string Token, string Endpoint,
			AuthenticationMethod AuthMechanism, params ISniffer[] Sniffers)
			: base(false, Sniffers)
		{
			this.key = Key;
			this.secret = Secret;
			this.token = Token;
			this.endpoint = Endpoint;
			this.authMechanism = AuthMechanism;
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

		/// <summary>
		/// Generates an OAUTH 1.0 signaure.
		/// </summary>
		/// <param name="Method">Method used in the call.</param>
		/// <param name="Url">URL</param>
		/// <param name="Arguments">Arguments</param>
		/// <returns>Signature</returns>
		public string GetAuthorization(string Method, string Url, Dictionary<string, object> Request)
		{
			StringBuilder sb = new StringBuilder();

			switch (this.authMechanism)
			{
				case AuthenticationMethod.OAuth1:
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

					AddOuthParameters(Sorted, string.Empty, Request);

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

					BStr.Append(Method);
					BStr.Append('&');
					BStr.Append(OAuthEncode(Url));
					BStr.Append('&');
					BStr.Append(OAuthEncode(PStr.ToString()));

					byte[] Key = Encoding.ASCII.GetBytes(OAuthEncode(this.secret) + "&" + OAuthEncode(this.token));
					byte[] Hash = Hashes.ComputeHMACSHA1Hash(Key, Encoding.ASCII.GetBytes(BStr.ToString()));
					string Signature = Convert.ToBase64String(Hash);

					sb.Append("OAuth oauth_consumer_key=\"");
					sb.Append(JSON.Encode(this.key));
					sb.Append("\", oauth_nonce=\"");
					sb.Append(JSON.Encode(Nonce));
					sb.Append("\", oauth_timestamp=\"");
					sb.Append(JSON.Encode(TotalSeconds));
					sb.Append("\", oauth_version=\"1.0");
					sb.Append("\", oauth_signature_method=\"HMAC-SHA1");
					sb.Append("\", oauth_signature=\"");
					sb.Append(JSON.Encode(Signature));
					sb.Append('"');

					return sb.ToString();

				case AuthenticationMethod.Token:
					sb.Append("Token ");
					sb.Append(this.token);

					return sb.ToString();

				case AuthenticationMethod.Basic:
					sb.Append(this.token);
					sb.Append(':');

					byte[] Bin = Encoding.UTF8.GetBytes(sb.ToString());

					sb.Clear();
					sb.Append("Basic ");
					sb.Append(Convert.ToBase64String(Bin));

					return sb.ToString();

				default:
					throw new NotImplementedException("Authentication mechanism not implemented: " + this.authMechanism.ToString());
			}
		}

		private static void AddOuthParameters(SortedDictionary<string, string> Sorted, string Prefix, 
			Dictionary<string, object> Request)
		{
			foreach (KeyValuePair<string, object> P in Request)
				AddOuthParameter(Sorted, Prefix + P.Key, P.Value);
		}

		private static void AddOuthParameter(SortedDictionary<string, string> Sorted, string Name, object Value)
		{
			if (Value is string s)
				Sorted[Name] = s;
			else if (Value is Dictionary<string, object> Obj)
				AddOuthParameters(Sorted, Name + ".", Obj);
			else if (Value is Array A)
			{
				int i = 0;

				foreach (object Item in A)
				{
					AddOuthParameter(Sorted, Name + "." + i.ToString(), Item);
					i++;
				}
			}
			else
				Sorted[Name] = JSON.Encode(Value, false);
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

		public async Task<object> SendSimpleMessage(string Sender, string Message, params string[] Recipients)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.endpoint);
			sb.Append("rest/mtsms?message=");
			sb.Append(HttpUtility.UrlEncode(Message));
			sb.Append("&sender=" + HttpUtility.UrlEncode(Sender));
			sb.Append("&encoding=UCS2");

			int i = 0;
			List<object> Recipients2 = new List<object>();

			foreach (string Recipient in Recipients)
			{
				if (string.IsNullOrEmpty(Recipient))
					continue;

				long Nr = PhoneNumberToLong(Recipient);

				sb.Append("&recipients.");
				sb.Append(i++);
				sb.Append(".msisdn=");
				sb.Append(Nr.ToString());

				Recipients2.Add(new Dictionary<string, object>()
				{
					{ "msisdn", Nr }
				});
			}

			Dictionary<string, object> Request = new Dictionary<string, object>()
			{
				{ "sender", Sender },
				{ "message", Message },
				{ "recipients", Recipients2.ToArray() }
			};

			string Url = sb.ToString();
			string Authorization = this.GetAuthorization("GET", Url, Request);

			if (this.HasSniffers)
			{
				sb.Clear();

				sb.Append("GET ");
				sb.Append(Url);
				sb.Append("\r\nAuthorization: ");
				sb.Append(Authorization);
				sb.Append("\r\nAccept: ");
				sb.Append(JsonCodec.DefaultContentType);

				this.TransmitText(sb.ToString());
			}

			object Result = null;

			try
			{
				ContentResponse Content = await InternetContent.GetAsync(new Uri(Url),
					new KeyValuePair<string, string>("Authorization", Authorization),
					new KeyValuePair<string, string>("Accept", JsonCodec.DefaultContentType));

				Content.AssertOk();

				Result = Content.Decoded;

				if (this.HasSniffers)
					this.ReceiveText(JSON.Encode(Result, true));
			}
			catch (WebException ex)
			{
				this.ProcessError(ex);
			}
			catch (Exception ex)
			{
				this.ProcessError(ex);
			}

			return Result;
		}

		public async Task<object> SendMessage(string Sender, string Message, params string[] Recipients)
		{
			string Url = this.endpoint + "rest/mtsms";
			List<object> Recipients2 = new List<object>();

			foreach (string Recipient in Recipients)
			{
				if (string.IsNullOrEmpty(Recipient))
					continue;

				Recipients2.Add(new Dictionary<string, object>()
				{
					{ "msisdn", PhoneNumberToLong(Recipient) }
				});
			}

			Dictionary<string, object> Request = new Dictionary<string, object>()
			{
				{ "sender", Sender },
				{ "encoding", "UCS2" },
				{ "message", Message },
				{ "recipients", Recipients2.ToArray() }
			};

			string Authorization = this.GetAuthorization("POST", Url, Request);

			if (this.HasSniffers)
			{
				this.TransmitText("POST " + Url + "\r\nAuthorization: " + Authorization +
					"\r\nAccept: " + JsonCodec.DefaultContentType + "\r\nContent-Type: " +
					JsonCodec.DefaultContentType + "\r\n\r\n" + JSON.Encode(Request, true));
			}

			object Result = null;

			try
			{
				Uri Uri = new Uri(Url);
				ContentBinaryResponse BinaryContent = await InternetContent.PostAsync(Uri,
					Encoding.UTF8.GetBytes(JSON.Encode(Request, false)),
					JsonCodec.DefaultContentType + "; charset=utf-8",
					new KeyValuePair<string, string>("Authorization", Authorization),
					new KeyValuePair<string, string>("Accept", JsonCodec.DefaultContentType));
				BinaryContent.AssertOk();

				ContentResponse Content = await InternetContent.DecodeAsync(BinaryContent.ContentType,
					BinaryContent.Encoded, Uri);
				Content.AssertOk();

				Result = Content.Decoded;

				if (this.HasSniffers)
					this.ReceiveText(JSON.Encode(Result, true));
			}
			catch (WebException ex)
			{
				this.ProcessError(ex);
			}
			catch (Exception ex)
			{
				this.ProcessError(ex);
			}

			return Result;
		}

		private void ProcessError(WebException ex)
		{
			if (this.HasSniffers)
			{
				this.ReceiveText(((int)ex.StatusCode).ToString() + " " +
					ex.StatusCode.ToString() + "\r\n\r\n" + JSON.Encode(ex.Content, true));

				this.Error(ex.Message);
			}

			if (ex.Content is Dictionary<string, object> Error &&
				Error.TryGetValue("code", out object Obj) && Obj is string Code &&
				Error.TryGetValue("incident_uuid", out Obj) && Obj is string IncidentId &&
				Error.TryGetValue("message", out Obj) && Obj is string ErrorMessage)
			{
				throw new GatewayApiException(Code, IncidentId, ErrorMessage);
			}

			ExceptionDispatchInfo.Capture(ex).Throw();
		}


		private void ProcessError(Exception ex)
		{
			if (this.HasSniffers)
				this.Error(ex.Message);

			ExceptionDispatchInfo.Capture(ex).Throw();
		}

		private static long PhoneNumberToLong(string PhoneNumber)
		{
			long Result = 0;
			long Limit = (long.MaxValue - 9) / 10;

			foreach (char ch in PhoneNumber)
			{
				if (ch >= '0' && ch <= '9')
				{
					if (Result > Limit)
						throw new ArgumentOutOfRangeException(nameof(PhoneNumber));

					Result *= 10;
					Result += (ch - '0');
				}
				else if (!char.IsWhiteSpace(ch) && ch != '+')
					throw new ArgumentException("Phone number contains invalid characters.", nameof(PhoneNumber));
			}

			return Result;
		}
	}
}
