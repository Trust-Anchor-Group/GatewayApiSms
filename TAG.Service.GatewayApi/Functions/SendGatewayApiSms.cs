using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Script;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Exceptions;
using Waher.Script.Model;
using Waher.Script.Objects;

namespace TAG.Service.GatewayApi.Functions
{
	/// <summary>
	/// Sends an SMS message to one or multiple recipients using GatewayAPI (if configured correctly).
	/// </summary>
	public class SendGatewayApiSms : FunctionMultiVariate
	{
		/// <summary>
		/// Sends an SMS message to one or multiple recipients using GatewayAPI (if configured correctly).
		/// </summary>
		/// <param name="Sender">Sender of message.</param>
		/// <param name="Message">Message text.</param>
		/// <param name="Recipients">One recipient or a vector of recipients.</param>
		/// <param name="Start">Start position in script expression.</param>
		/// <param name="Length">Length of expression covered by node.</param>
		/// <param name="Expression">Expression.</param>
		public SendGatewayApiSms(ScriptNode Sender, ScriptNode Message, ScriptNode Recipients,
			int Start, int Length, Expression Expression)
			: base(new ScriptNode[] { Sender, Message, Recipients },
				  new ArgumentType[] { ArgumentType.Scalar, ArgumentType.Scalar, ArgumentType.Normal },
				  Start, Length, Expression)
		{
		}

		/// <summary>
		/// Name of the function
		/// </summary>
		public override string FunctionName => nameof(SendGatewayApiSms);

		/// <summary>
		/// Default Argument names
		/// </summary>
		public override string[] DefaultArgumentNames => new string[] { "Sender", "Message", "Recipients" };

		/// <summary>
		/// If the node (or its decendants) include asynchronous evaluation. Asynchronous
		/// nodes should be evaluated using <see cref="FunctionMultiVariate.EvaluateAsync(Variables)"/>.
		/// </summary>
		public override bool IsAsynchronous => true;

		/// <summary>
		/// Evaluates the node, using the variables provided in the Variables collection.
		/// </summary>
		/// <param name="Arguments">Function arguments.</param>
		/// <param name="Variables">Variables collection.</param>
		/// <returns>Function result.</returns>
		public override IElement Evaluate(IElement[] Arguments, Variables Variables)
		{
			return this.EvaluateAsync(Arguments, Variables).Result;
		}

		/// <summary>
		/// Evaluates the node, using the variables provided in the Variables collection.
		/// </summary>
		/// <param name="Arguments">Function arguments.</param>
		/// <param name="Variables">Variables collection.</param>
		/// <returns>Function result.</returns>
		public override async Task<IElement> EvaluateAsync(IElement[] Arguments, Variables Variables)
		{
			string Sender = Arguments[0].AssociatedObjectValue?.ToString() ?? string.Empty;
			string Message = Arguments[1].AssociatedObjectValue?.ToString() ?? string.Empty;
			object Obj = Arguments[2].AssociatedObjectValue ?? string.Empty;
			string[] Recipients;

			if (Obj is string s)
				Recipients = new string[] { s };
			else if (Obj is string[] s2)
				Recipients = s2;
			else if (Obj is Array A)
			{
				int i, c = A.Length;

				Recipients = new string[c];

				for (i = 0; i < c; i++)
					Recipients[i] = A.GetValue(i)?.ToString() ?? string.Empty;
			}
			else
				throw new ScriptRuntimeException("Invalid recipients.", this);

			object Result = await GatewayApiService.SendSMS(Sender, Message, Recipients);

			return new ObjectValue(Expression.Encapsulate(Result));
		}
	}
}
