// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General actions for the app itself.
/// </summary>
public class LumosApp
{
	/// <summary>
	/// Notifies the server that the player is playing.
	/// </summary>
	///
	public static void Ping ()
	{
		Ping(false);
	}

	/// <summary>
	/// Notifies the server that the player is playing.
	/// </summary>
	/// <param name="sendPlayerInfo">Whether to send player info.</param>
	public static void Ping (bool sendPlayerInfo)
	{
		var parameters = new Dictionary<string, object>() {
			{ "player_id", Lumos.playerId },
			{ "lumos_version", Lumos.version.ToString() }
		};

		if (sendPlayerInfo) {
			// Attach extra information about the player.
			var playerInfo = new Dictionary<string, object>() {
				{ "language", Application.systemLanguage.ToString() }
			};

			// Report the domain if the game is deployed on the web.
			if (Application.isWebPlayer) {
				playerInfo.Add("domain", Application.absoluteURL);
			}

			parameters.Add("player_info", playerInfo);
		}

		LumosWWW.Send("app.ping", parameters, delegate {
			// Parse which services are available.
			var response = LumosWWW.lastResponse;

			if (!response.Contains("services_available")) {
				Lumos.Remove("Available services unknown.");
				return;
			}

			var servicesAvailable = response["services_available"] as IList;

			if (servicesAvailable.Contains("none")) {
				Lumos.Remove("No services available. " +
				             "The game is either over quota or nonexistant.");
				return;
			}

			foreach (string service in servicesAvailable) {
				if (service == "system-info") {
					Lumos.servicesAvailable.Add(Lumos.service.SystemInfo);
				} else if (service == "events") {
					Lumos.servicesAvailable.Add(Lumos.service.Events);
				} else if (service == "logs") {
					Lumos.servicesAvailable.Add(Lumos.service.Logs);
				} else if (service == "feedback") {
					Lumos.servicesAvailable.Add(Lumos.service.Feedback);
				} else if (service == "realtime") {
					Lumos.servicesAvailable.Add(Lumos.service.Realtime);
				}
			}

			if (sendPlayerInfo) {
				LumosSystemInfo.Record();
			}
		});
	}
}

