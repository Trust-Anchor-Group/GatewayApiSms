using Paiwise;
using System.Threading.Tasks;
using Waher.IoTGateway;
using Waher.Persistence;

namespace TAG.Payments.Template
{
	/// <summary>
	/// Service Provider template working as the principal interface with the payment infrastructure within the TAG Neuron(R).
	/// </summary>
	/// <remarks>
	/// A Service Provider acts as a host of one or more services implemented in the module. The <see cref="IBuyEDalerServiceProvider"/>
	/// interface is used for service providers that allows users to buy eDaler(R) to their accounts. Likewise, the
	/// <see cref="ISellEDalerServiceProvider"/> interface allows users to sell eDaler(R) available in their accounts.
	/// The <see cref="IPaymentServiceProvider"/> provides a means to create services on the Neuron(R) that can charge users
	/// for services, without using eDaler(R) or a connection to XMPP accounts. Implement the interfaces necessary for the service provider,
	/// and remove those that do not need to be implemented.
	/// 
	/// Classes implementing these interfaces, containing a default constructor, will be found and instantiated using the
	/// <see cref="Waher.Runtime.Inventory.Types"/> static class.
	/// </remarks>
	public class ServiceProvider : IBuyEDalerServiceProvider, ISellEDalerServiceProvider, IPaymentServiceProvider
	{
		/// <summary>
		/// ID of service provider.
		/// </summary>
		public static readonly string ServiceProviderId = typeof(ServiceProvider).Namespace;

		/// <summary>
		/// Service Provider template working as the principal interface with the payment infrastructure within the TAG Neuron(R).
		/// </summary>
		/// <remarks>
		/// Default constructor necessary, for the Neuron(R) to roperly instantiate the service provider when needed.
		/// </remarks>
		public ServiceProvider()
		{
		}

		#region IServiceProvider

		/// <summary>
		/// ID of service provider
		/// </summary>
		public string Id => ServiceProviderId;

		/// <summary>
		/// Name of service provider
		/// </summary>
		public string Name => "Payment Provider Template";   // TODO: Change to a textual description of the service provider.

		/// <summary>
		/// Icon URL
		/// </summary>
		public string IconUrl => Gateway.GetUrl("/TemplatePayment/Images/Everaldo-Crystal-Clear-Mimetype-vector-gfx.128.png"); // TODO: Replace to a correct representation of the service provider.

		/// <summary>
		/// Width of icon, in pixels.
		/// </summary>
		public int IconWidth => 128;    // TODO: Change to Icon width, in pixels

		/// <summary>
		/// Height of icon, in pixels
		/// </summary>
		public int IconHeight => 128;   // TODO: Change to Icon height, in pixels

		#endregion

		#region IBuyEDalerServiceProvider

		/// <summary>
		/// Gets available payment services.
		/// </summary>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Available payment services.</returns>
		public Task<IBuyEDalerService[]> GetServicesForBuyingEDaler(CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (!ServiceModule.Running)
				return Task.FromResult(new IBuyEDalerService[0]);

			// TODO: Check Currency and Country support

			return Task.FromResult(new IBuyEDalerService[]
				{
					new Service(this)
				}); // TODO: Return proper set of service references.
		}

		/// <summary>
		/// Gets a payment service.
		/// </summary>
		/// <param name="ServiceId">Service ID</param>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Service, if found, null otherwise.</returns>
		public Task<IBuyEDalerService> GetServiceForBuyingEDaler(string ServiceId, CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (ServiceModule.Running && ServiceId == Service.ServiceId)     // TODO: Check against actual set of services.
				return Task.FromResult<IBuyEDalerService>(new Service(this));
			else
				return null;
		}

		#endregion

		#region ISellEDalerServiceProvider

		/// <summary>
		/// Gets available payment services.
		/// </summary>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Available payment services.</returns>
		public Task<ISellEDalerService[]> GetServicesForSellingEDaler(CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (!ServiceModule.Running)
				return Task.FromResult(new ISellEDalerService[0]);

			// TODO: Check Currency and Country support

			return Task.FromResult(new ISellEDalerService[]
				{
					new Service(this)
				});  // TODO:Return proper set of service references.
		}

		/// <summary>
		/// Gets a payment service.
		/// </summary>
		/// <param name="ServiceId">Service ID</param>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Service, if found, null otherwise.</returns>
		public Task<ISellEDalerService> GetServiceForSellingEDaler(string ServiceId, CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (ServiceModule.Running && ServiceId == Service.ServiceId)		// TODO: Check against actual set of services.
				return Task.FromResult<ISellEDalerService>(new Service(this));
			else
				return null;
		}

		#endregion

		#region IPaymentServiceProvider

		/// <summary>
		/// Gets available payment services.
		/// </summary>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Available payment services.</returns>
		public Task<IPaymentService[]> GetServicesForPayment(CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (!ServiceModule.Running)
				return Task.FromResult(new IPaymentService[0]);

			// TODO: Check Currency and Country support

			return Task.FromResult(new IPaymentService[]
				{
					new Service(this)
				});  // TODO:Return proper set of service references.
		}

		/// <summary>
		/// Gets a payment service.
		/// </summary>
		/// <param name="ServiceId">Service ID</param>
		/// <param name="Currency">Currency to use.</param>
		/// <param name="Country">Country where service is to be used.</param>
		/// <returns>Service, if found, null otherwise.</returns>
		public Task<IPaymentService> GetServiceForPayment(string ServiceId, CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			if (ServiceModule.Running && ServiceId == Service.ServiceId)     // TODO: Check against actual set of services.
				return Task.FromResult<IPaymentService>(new Service(this));
			else
				return null;
		}

		#endregion

	}
}
