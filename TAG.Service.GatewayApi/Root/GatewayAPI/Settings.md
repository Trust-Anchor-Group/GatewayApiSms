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

<form action="Settings.md" method="post">
<fieldset>
<legend>GatewayAPI settings</legend>

The following settings are required by the integration of the Neuron(R) with the [GatewayAPI SMS transmission service](https://gatewayapi.com/). 
By providing such an integration, SMS messages can be sent from the Neuron(R).

{{
if exists(Posted) then
(
	SetSetting("TAG.Service.GatewayApi.ApiKey",Str(Posted.ApiKey));
	SetSetting("TAG.Service.GatewayApi.ApiSecret",Str(Posted.ApiSecret));
	SetSetting("TAG.Service.GatewayApi.ApiToken",Str(Posted.ApiToken));
	SetSetting("TAG.Service.GatewayApi.ApiEurope",Bool(Posted.ApiEurope));

	TAG.Service.GatewayApi.ServiceConfiguration.InvalidateCurrent();

	SeeOther("Settings.md");
);
}}

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