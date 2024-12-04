/* Copyright (c) 1996-2022 The OPC Foundation. All rights reserved.
   The source code in this file is covered under a dual-license scenario:
     - RCL: for OPC Foundation Corporate Members in good-standing
     - GPL V2: everybody else
   RCL license terms accompanied with this source code. See http://opcfoundation.org/License/RCL/1.00/
   GNU General Public License as published by the Free Software Foundation;
   version 2 of the License are accompanied with this source code. See http://opcfoundation.org/License/GPLv2
   This source code is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/

using System;
using System.Security.Cryptography.X509Certificates;

namespace Opc.Ua
{
    /// <summary>
    /// An object used by clients to access a UA discovery service.
    /// </summary>
    public partial class RegistrationClient
    {
        #region Constructors

        /// <summary>
        /// Creates a binding for to use for discovering servers.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="description">The description.</param>
        /// <param name="endpointConfiguration">The endpoint configuration.</param>
        /// <param name="instanceCertificate">The instance certificate.</param>
        /// <param name="transportMode">The transport mode to use with thic client.</param>
        /// <returns>the registration client</returns>
        public static RegistrationClient Create(ApplicationConfiguration configuration,
            EndpointDescription description,
            EndpointConfiguration endpointConfiguration,
            X509Certificate2 instanceCertificate,
            MessageTransportMode transportMode)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (description == null) throw new ArgumentNullException(nameof(description));

            ITransportChannel channel = RegistrationChannel.Create(
                configuration,
                description,
                endpointConfiguration,
                instanceCertificate,
                transportMode,
                new ServiceMessageContext());

            return new RegistrationClient(channel);
        }

        #endregion
    }

    /// <summary>
    /// A channel object used by clients to access a UA discovery service.
    /// </summary>
    public partial class RegistrationChannel
    {
        #region Constructors

        /// <summary>
        /// Creates a new transport channel that supports the IRegistrationChannel service contract.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="description">The description for the endpoint.</param>
        /// <param name="endpointConfiguration">The configuration to use with the endpoint.</param>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="transportMode"></param>
        /// <param name="messageContext">The message context to use when serializing the messages.</param>
        /// <returns></returns>
        public static ITransportChannel Create(ApplicationConfiguration configuration,
            EndpointDescription description,
            EndpointConfiguration endpointConfiguration,
            X509Certificate2 clientCertificate,
            MessageTransportMode transportMode,
            IServiceMessageContext messageContext)
        {
            // create a UA binary channel.
            ITransportChannel channel = CreateUaBinaryChannel(
                configuration,
                description,
                endpointConfiguration,
                clientCertificate,
                transportMode,
                messageContext);

            // create a registration channel.
            if (channel == null)
            {
                Uri endpointUrl = new Uri(description.EndpointUrl);
                channel = new RegistrationChannel();

                var settings = new TransportChannelSettings {
                    Configuration = endpointConfiguration,
                    Description = description,
                    ClientCertificate = clientCertificate,
                    TransportMode = transportMode
                };
                channel.Initialize(endpointUrl, settings);
            }

            return channel;
        }

        #endregion
    }
}
