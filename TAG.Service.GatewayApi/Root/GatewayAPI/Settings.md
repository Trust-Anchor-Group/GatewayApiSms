Title: GatewayAPI settings
Description: Configures integration with the GatewayAPI SMS transmission service.
Date: 2024-09-03
Author: Peter Waher
Master: /Master.md
Cache-Control: max-age=0, no-cache, no-store
JavaScript: /Sniffers/Sniffer.js
UserVariable: User
Privilege: Admin.SMS.GatewayAPI
Login: /Login.md

========================================================================

{{
if exists(Posted) then
(
	({
	"ApiKey":Required(Str(PApiKey)),
	"ApiSecret":Required(Str(PApiSecret)),
	"ApiToken":Required(Str(PApiToken)),
	"ApiEurope":Optional(Boolean(PApiEurope)),
	"TestSender":Optional(Str(PTestSender)),
	"TestRecipient":Optional(Str(PTestRecipient)),
	"TestMessage":Optional(Str(PTestMessage))
	}:=Posted) ??? BadRequest("Invalid Payload");

	SetSetting("TAG.Service.GatewayApi.ApiKey",PApiKey);
	SetSetting("TAG.Service.GatewayApi.ApiSecret",PApiSecret);
	SetSetting("TAG.Service.GatewayApi.ApiToken",PApiToken);
	SetSetting("TAG.Service.GatewayApi.ApiEurope",PApiEurope ?? false);
	SetSetting("TAG.Service.GatewayApi.TestSender",PTestSender ?? "");
	SetSetting("TAG.Service.GatewayApi.TestRecipient",PTestRecipient ?? "");
	SetSetting("TAG.Service.GatewayApi.TestMessage",PTestMessage ?? "");

	if !exists(PGatewayApiError) then PGatewayApiError:="";

	TAG.Service.GatewayApi.ServiceConfiguration.InvalidateCurrent();

	if !empty(PTestRecipient) and !empty(PTestMessage) then
	(
		PGatewayApiError:="";
		TAG.Service.GatewayApi.GatewayApiService.SendSMS(PTestSender,PTestMessage,[PTestRecipient]) ??? PGatewayApiError:=Exception.Message;
	);

	SeeOther("Settings.md");
);
}}

<form action="Settings.md" method="post">
<fieldset>
<legend>GatewayAPI settings</legend>

The following settings are required by the integration of the Neuron(R) with the [GatewayAPI SMS transmission service](https://gatewayapi.com/). 
By providing such an integration, SMS messages can be sent from the Neuron(R).

<p>
<label for="ApiKey">API Key:</label>  
<input type="text" id="ApiKey" name="ApiKey" value='{{GetSetting("TAG.Service.GatewayApi.ApiKey","")}}' autofocus required title="API Key."/>
</p>

<p>
<label for="ApiSecret">API Secret:</label>  
<input type="text" id="ApiSecret" name="ApiSecret" value='{{GetSetting("TAG.Service.GatewayApi.ApiSecret","")}}' required title="API Secret."/>
</p>


<p>
<label for="ApiToken">API Token:</label>  
<input type="text" id="ApiToken" name="ApiToken" value='{{GetSetting("TAG.Service.GatewayApi.ApiToken","")}}' required title="API Token."/>
</p>

<p>
<input id="ApiEurope" name="ApiEurope" type="checkbox" {{GetSetting("TAG.Service.GatewayApi.ApiEurope",false) ? "checked" : ""}}/>
<label for="ApiEurope">European Server</label>
</p>

<fieldset>
<legend>Test Parameters</legend>

<p>
<label for="TestSender">Sender:</label>  
<input type="text" id="TestSender" name="TestSender" value='{{GetSetting("TAG.Service.GatewayApi.TestSender","")}}' title="Sender used for testing parameters."/>
</p>

<p>
<label for="TestRecipient">Recipient:</label>  
<input type="text" id="TestRecipient" name="TestRecipient" value='{{GetSetting("TAG.Service.GatewayApi.TestRecipient","")}}' title="Recipient used for testing parameters."/>
</p>

<p>
<label for="TestMessage">Message:</label>  
<input type="text" id="TestMessage" name="TestMessage" value='{{GetSetting("TAG.Service.GatewayApi.TestMessage","")}}' title="Message used for testing parameters."/>
</p>

</fieldset>

{{
if exists(PGatewayApiError) && !empty(PGatewayApiError) then ]]
<div class="error">
((MarkdownEncode(PGatewayApiError) ))
</div>
[[
}}

<button type="submit" class="posButton">Apply</button>
</fieldset>

<fieldset>
<legend>Tools</legend>
<button type="button" class="posButton"{{
if User.HasPrivilege("Admin.Communication.GatewayAPI") and User.HasPrivilege("Admin.Communication.Sniffer") then
	" onclick=\"OpenSniffer('Sniffer.md')\""
else
	" disabled"
}}>Sniffer</button>
</fieldset>
</form>