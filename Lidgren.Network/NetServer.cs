﻿using System;
using System.Collections.Generic;

namespace Lidgren.Network
{
	/// <summary>
	/// Specialized version of NetPeer used for "server" peers
	/// </summary>
	public class NetServer : NetPeer, INetServer
    {
		/// <summary>
		/// NetServer constructor
		/// </summary>
		public NetServer(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = true;
		}

		/// <summary>
		/// Send a message to all connections
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="method">How to deliver the message</param>
		public void SendToAll(INetOutgoingMessage msg, NetDeliveryMethod method)
		{
            NetOutgoingMessage _msg = (NetOutgoingMessage)msg;
            var all = this.Connections;
			if (all.Count <= 0) {
				if (_msg.m_isSent == false)
					Recycle(_msg);
				return;
			}

			SendMessage(msg, all, method, 0);
		}

		/// <summary>
		/// Send a message to all connections except one
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="method">How to deliver the message</param>
		/// <param name="except">Don't send to this particular connection</param>
		/// <param name="sequenceChannel">Which sequence channel to use for the message</param>
		public void SendToAll(INetOutgoingMessage msg, INetConnection except, NetDeliveryMethod method, int sequenceChannel)
		{
            NetOutgoingMessage _msg = (NetOutgoingMessage)msg;
            var all = this.Connections;
			if (all.Count <= 0) {
				if (_msg.m_isSent == false)
					Recycle(_msg);
				return;
			}

			if (except == null)
			{
				SendMessage(msg, all, method, sequenceChannel);
				return;
			}

			List<INetConnection> recipients = new List<INetConnection>(all.Count - 1);
			foreach (var conn in all)
				if (conn != except)
					recipients.Add(conn);

			if (recipients.Count > 0)
				SendMessage(msg, recipients, method, sequenceChannel);
		}

		/// <summary>
		/// Returns a string that represents this object
		/// </summary>
		public override string ToString()
		{
			return "[NetServer " + ConnectionsCount + " connections]";
		}
	}
}
