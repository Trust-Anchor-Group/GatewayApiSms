using System;

namespace TAG.Networking.GatewayApi
{
	/// <summary>
	/// Error returned from the Gateway API
	/// </summary>
	public class GatewayApiException : Exception
	{
		/// <summary>
		/// Error returned from the Gateway API
		/// </summary>
		/// <param name="Code">Error code.</param>
		/// <param name="IncidentId">Incident ID</param>
		/// <param name="Message">Error message</param>
		public GatewayApiException(string Code, string IncidentId, string Message)
			: base("Error Code: " + Code +
				  "\r\nIncident ID: " + IncidentId +
				  "\r\nMessage: " + Message)
		{
			this.Code = Code;
			this.IncidentId = IncidentId;
			this.MessageString = Message;
		}

		/// <summary>
		/// Error code
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// Incident ID
		/// </summary>
		public string IncidentId { get; }

		/// <summary>
		/// Error message
		/// </summary>
		public string MessageString { get; }
	}
}
