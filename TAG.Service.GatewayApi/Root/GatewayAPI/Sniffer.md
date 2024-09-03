Title: GatewayAPI Sniffer
Description: Allows the user to view GatewayAPI communication.
Date: 2024-09-03
Author: Peter Waher
Master: /Master.md
JavaScript: /Events.js
JavaScript: /Sniffers/Sniffer.js
CSS: /Sniffers/Sniffer.css
UserVariable: User
Privilege: Admin.Communication.Sniffer
Privilege: Admin.Communication.GatewayAPI
Login: /Login.md
Parameter: SnifferId

========================================================================

GatewayAPI Communication
===========================

On this page, you can follow the [GatewayAPI](https://gatewayapi.com/) communication made from the machine to the 
the API back-end. The sniffer will automatically be terminated after some time to avoid performance degradation and leaks. 
Sniffers should only be used as a tool for troubleshooting.

{{
TAG.Service.GatewayApi.GatewayApiService.RegisterSniffer(SnifferId,Request,"User",["Admin.Communication.Sniffer","Admin.Communication.GatewayAPI"])
}}
