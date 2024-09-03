using Paiwise;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Events;
using Waher.IoTGateway;
using Waher.Persistence;
using Waher.Runtime.Inventory;

namespace TAG.Payments.Template
{
	/// <summary>
	/// Service template working as the interface with the payment infrastructure within the TAG Neuron(R), providing the actual
	/// interface with an actual payment service.
	/// </summary>
	/// <remarks>
	/// The <see cref="IBuyEDalerService"/> interface is used for service that allows users to buy eDaler(R) to their accounts. Likewise,
	/// the <see cref="ISellEDalerService"/> interface allows users to sell eDaler(R) available in their accounts, The 
	/// <see cref="IPaymentService"/> provides a means to create services on the Neuron(R) that can charge users
	/// for services, without using eDaler(R) or a connection to XMPP accounts. Implement the interfaces necessary for the service,
	/// and remove those that do not need to be implemented.
	/// </remarks>
	public class Service : IBuyEDalerService, ISellEDalerService, IPaymentService
	{
		private readonly ServiceProvider provider;

		/// <summary>
		/// ID of service.
		/// </summary>
		public static readonly string ServiceId = ServiceProvider.ServiceProviderId + ".Service";

		/// <summary>
		/// Service template working as the interface with the payment infrastructure within the TAG Neuron(R), providing the actual
		/// interface with an actual payment service.
		/// </summary>
		/// <param name="Provider">Reference to the service provider instantiating the service.</param>
		public Service(ServiceProvider Provider)
		{
			this.provider = Provider;
		}

		#region IServiceProvider

		/// <summary>
		/// ID of service
		/// </summary>
		public string Id => ServiceId;

		/// <summary>
		/// Name of service
		/// </summary>
		public string Name => "Payment Template";   // TODO: Change to a textual description of the service.

		/// <summary>
		/// Icon URL
		/// </summary>
		public string IconUrl => Gateway.GetUrl("/TemplatePayment/Images/Everaldo-Crystal-Clear-Mimetype-vector-gfx.128.png"); // TODO: Replace to a correct representation of the service.

		/// <summary>
		/// Width of icon, in pixels.
		/// </summary>
		public int IconWidth => 128;    // TODO: Change to Icon width, in pixels

		/// <summary>
		/// Height of icon, in pixels
		/// </summary>
		public int IconHeight => 128;   // TODO: Change to Icon height, in pixels

		#endregion

		#region IProcessingSupport<CaseInsensitiveString>

		/// <summary>
		/// How well a currency is supported
		/// </summary>
		/// <param name="Currency">Currency</param>
		/// <returns>Support</returns>
		public Grade Supports(CaseInsensitiveString Currency)
		{
			return Grade.Barely;	// TODO: Check for Currency support, and return a Grade on how well the currency is supported.
		}

		#endregion

		#region IBuyEDalerService

		/// <summary>
		/// Contract ID of Template, for buying e-Daler
		/// </summary>
		public string BuyEDalerTemplateContractId => string.Empty;		// TODO: For services allowing payments using smart contracts, provide the Contract ID of the required template here.

		/// <summary>
		/// Reference to the service provider.
		/// </summary>
		public IBuyEDalerServiceProvider BuyEDalerServiceProvider => this.provider;

		/// <summary>
		/// If the service can be used to process a request to buy eDaler for a given account.
		/// </summary>
		/// <param name="AccountName">Account Name</param>
		/// <returns>If service can be used.</returns>
		public Task<bool> CanBuyEDaler(CaseInsensitiveString AccountName)
		{
			return Task.FromResult(ServiceModule.Running);	// TODO: Check service properly configured.
		}

		/// <summary>
		/// Processes payment for buying eDaler.
		/// </summary>
		/// <param name="ContractParameters">Parameters available in the contract authorizing the payment.</param>
		/// <param name="IdentityProperties">Properties engraved into the legal identity signing the payment request.</param>
		/// <param name="Amount">Amount to be paid.</param>
		/// <param name="Currency">Currency</param>
		/// <param name="SuccessUrl">Optional Success URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="FailureUrl">Optional Failure URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="CancelUrl">Optional Cancel URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="ClientUrlCallback">Method to call if the payment service requests an URL to be displayed on the client.</param>
		/// <param name="State">State object to pass on the callback method.</param>
		/// <returns>Result of operation.</returns>
		public Task<PaymentResult> BuyEDaler(IDictionary<CaseInsensitiveString, object> ContractParameters,
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties,
			decimal Amount, string Currency, string SuccessUrl, string FailureUrl, string CancelUrl,
			ClientUrlEventHandler ClientUrlCallback, object State)
		{
			Log.Debug("User attempting to buy eDaler(R).",
				new KeyValuePair<string, object>("Amount", Amount),
				new KeyValuePair<string, object>("Currency", Currency));    // TODO: Remove when proper method implemented.

			// TODO: Perform payment

			return Task.FromResult(new PaymentResult("Mock service cannot perform payment requested."));    // TODO: Either return error message or actual amount processed.
		}

		/// <summary>
		/// Gets available payment options for buying eDaler.
		/// </summary>
		/// <param name="IdentityProperties">Properties engraved into the legal identity that will perform the request.</param>
		/// <param name="SuccessUrl">Optional Success URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="FailureUrl">Optional Failure URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="CancelUrl">Optional Cancel URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="ClientUrlCallback">Method to call if the payment service requests an URL to be displayed on the client.</param>
		/// <param name="State">State object to pass on the callback method.</param>
		/// <returns>Array of dictionaries, each dictionary representing a set of parameters that can be selected in the
		/// contract to sign.</returns>
		public Task<IDictionary<CaseInsensitiveString, object>[]> GetPaymentOptionsForBuyingEDaler(
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties,
			string SuccessUrl, string FailureUrl, string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			// TODO: Service can return a set of parameters that can be used to prefill the smart contract. Key names must match parameter
			//       names in the contract. Multiple dictionaries can be returned, each one representing one option, that the user can select
			//       to prefill the form.

			return Task.FromResult(new IDictionary<CaseInsensitiveString, object>[0]);
		}

		#endregion

		#region ISellEDalerService

		/// <summary>
		/// Contract ID of Template, for selling e-Daler
		/// </summary>
		public string SellEDalerTemplateContractId => string.Empty;      // TODO: For services allowing payments using smart contracts, provide the Contract ID of the required template here.

		/// <summary>
		/// Reference to the service provider.
		/// </summary>
		public ISellEDalerServiceProvider SellEDalerServiceProvider => this.provider;

		/// <summary>
		/// If the service can be used to process a request to sell eDaler for a given account.
		/// </summary>
		/// <param name="AccountName">Account Name</param>
		/// <returns>If service can be used.</returns>
		public Task<bool> CanSellEDaler(CaseInsensitiveString AccountName)
		{
			return Task.FromResult(ServiceModule.Running);  // TODO: Check service properly configured.
		}

		/// <summary>
		/// Processes payment for selling eDaler.
		/// </summary>
		/// <param name="ContractParameters">Parameters available in the contract authorizing the payment.</param>
		/// <param name="IdentityProperties">Properties engraved into the legal identity signing the payment request.</param>
		/// <param name="Amount">Amount to be paid.</param>
		/// <param name="Currency">Currency</param>
		/// <param name="SuccessUrl">Optional Success URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="FailureUrl">Optional Failure URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="CancelUrl">Optional Cancel URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="ClientUrlCallback">Method to call if the payment service requests an URL to be displayed on the client.</param>
		/// <param name="State">State object to pass on the callback method.</param>
		/// <returns>Result of operation.</returns>
		public Task<PaymentResult> SellEDaler(IDictionary<CaseInsensitiveString, object> ContractParameters,
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties,
			decimal Amount, string Currency, string SuccessUrl, string FailureUrl, string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			Log.Debug("User attempting to sell eDaler(R).",
				new KeyValuePair<string, object>("Amount", Amount),
				new KeyValuePair<string, object>("Currency", Currency));    // TODO: Remove when proper method implemented.

			// TODO: Perform payment

			return Task.FromResult(new PaymentResult("Mock service cannot perform payment requested."));	// TODO: Either return error message or actual amount processed.
		}

		/// <summary>
		/// Gets available payment options for selling eDaler.
		/// </summary>
		/// <param name="IdentityProperties">Properties engraved into the legal identity that will perform the request.</param>
		/// <param name="SuccessUrl">Optional Success URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="FailureUrl">Optional Failure URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="CancelUrl">Optional Cancel URL the service provider can open on the client from a client web page, if getting options has succeeded.</param>
		/// <param name="ClientUrlCallback">Method to call if the payment service requests an URL to be displayed on the client.</param>
		/// <param name="State">State object to pass on the callback method.</param>
		/// <returns>Array of dictionaries, each dictionary representing a set of parameters that can be selected in the contract to sign.</returns>
		public Task<IDictionary<CaseInsensitiveString, object>[]> GetPaymentOptionsForSellingEDaler(
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties,
			string SuccessUrl, string FailureUrl, string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			// TODO: Service can return a set of parameters that can be used to prefill the smart contract. Key names must match parameter
			//       names in the contract. Multiple dictionaries can be returned, each one representing one option, that the user can select
			//       to prefill the form.

			return Task.FromResult(new IDictionary<CaseInsensitiveString, object>[0]);
		}

		#endregion

		#region IPaymentService

		/// <summary>
		/// Reference to service provider.
		/// </summary>
		public IPaymentServiceProvider PaymentServiceProvider => this.provider;

		/// <summary>
		/// Processes a payment.
		/// </summary>
		/// <param name="Amount">Amount to be paid.</param>
		/// <param name="Currency">Currency</param>
		/// <param name="Description">Description of payment.</param>
		/// <param name="SuccessUrl">Optional Success URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="FailureUrl">Optional Failure URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="CancelUrl">Optional Cancel URL the service provider can open on the client from a client web page, if payment has succeeded.</param>
		/// <param name="ClientUrlCallback">Method to call if the payment service requests an URL to be displayed on the client.</param>
		/// <param name="State">State object to pass on the callback method.</param>
		/// <returns>Result of operation.</returns>
		public Task<PaymentResult> Pay(decimal Amount, string Currency, string Description, string SuccessUrl, string FailureUrl,
			string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			Log.Debug("User attempting to perform a payment.",
				new KeyValuePair<string, object>("Amount", Amount),
				new KeyValuePair<string, object>("Currency", Currency),
				new KeyValuePair<string, object>("Description", Description));    // TODO: Remove when proper method implemented.

			// TODO: Perform payment

			return Task.FromResult(new PaymentResult("Mock service cannot perform payment requested."));    // TODO: Either return error message or actual amount processed.
		}

		#endregion
	}
}
