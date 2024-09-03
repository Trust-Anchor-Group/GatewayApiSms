using System.Threading.Tasks;
using Waher.Runtime.Settings;

namespace TAG.Service.GatewayApi
{
	/// <summary>
	/// Contains the service configuration.
	/// </summary>
	public class ServiceConfiguration
	{
		private static ServiceConfiguration current = null;

		/// <summary>
		/// Contains the service configuration.
		/// </summary>
		public ServiceConfiguration()
		{
		}

		/// <summary>
		/// API Key
		/// </summary>
		public string ApiKey
		{
			get;
			private set;
		}

		/// <summary>
		/// API Secret
		/// </summary>
		public string ApiSecret
		{
			get;
			private set;
		}

		/// <summary>
		/// API Token
		/// </summary>
		public string ApiToken
		{
			get;
			private set;
		}

		/// <summary>
		/// Europe
		/// </summary>
		public bool ApiEurope
		{
			get;
			private set;
		}

		/// <summary>
		/// If configuration is well-defined.
		/// </summary>
		public bool IsWellDefined
		{
			get
			{
				return !string.IsNullOrEmpty(this.ApiKey) &&
					!string.IsNullOrEmpty(this.ApiSecret) &&
					!string.IsNullOrEmpty(this.ApiToken);
			}
		}

		/// <summary>
		/// Loads configuration settings.
		/// </summary>
		/// <returns>Configuration settings.</returns>
		public static async Task<ServiceConfiguration> Load()
		{
			ServiceConfiguration Result = new ServiceConfiguration();
			string Prefix = typeof(GatewayApiService).Namespace;

			Result.ApiKey = await RuntimeSettings.GetAsync(Prefix + "." + nameof(ApiKey), string.Empty);
			Result.ApiSecret = await RuntimeSettings.GetAsync(Prefix + "." + nameof(ApiSecret), string.Empty);
			Result.ApiToken = await RuntimeSettings.GetAsync(Prefix + "." + nameof(ApiToken), string.Empty);
			Result.ApiEurope = await RuntimeSettings.GetAsync(Prefix + "." + nameof(ApiEurope), false);

			return Result;
		}

		/// <summary>
		/// Gets the current configuration.
		/// </summary>
		/// <returns>Configuration</returns>
		public static async Task<ServiceConfiguration> GetCurrent()
		{
			if (current is null)
				current = await Load();

			return current;
		}

		/// <summary>
		/// Invalidates the current configuration, triggering a reload of the
		/// configuration for the next operation.
		/// </summary>
		public static void InvalidateCurrent()
		{
			current = null;
		}
	}
}
