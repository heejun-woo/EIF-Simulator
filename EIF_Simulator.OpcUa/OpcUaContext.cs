using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;


namespace EIF_Simulator.OpcUa
{
    using Microsoft.Extensions.Logging;
    using Opc.Ua;
    using Opc.Ua.Configuration;

    namespace EIF_Simulator.OpcUa
    {
        public class OpcUaContext
        {
            private ApplicationInstance? _application;
            private OpcUaServer? _server;
            private ITelemetryContext? _telemetry;

            public OpcUaBindingManager BindingManager { get; }

            public OpcUaServer? Server => _server;

            public bool IsRunning => _server != null;

            public OpcUaContext()
            {
                BindingManager =
                    new OpcUaBindingManager();
            }

            public async Task StartAsync(
                int port = 4840)
            {
                if (_server != null)
                    return;

                _telemetry =
                    DefaultTelemetry.Create(
                        builder =>
                        {
                            builder.SetMinimumLevel(
                                LogLevel.Warning);
                        });

                string applicationUri =
                    "urn:localhost:EIFSimulator";

                string productUri =
                    "urn:EIFSimulator";

                string endpoint =
                    $"opc.tcp://localhost:{port}/EIFSimulator";

                var serverConfiguration =
                    new ServerConfiguration
                    {
                        BaseAddresses =
                            new[]
                            {
                            endpoint
                            }.ToArrayOf(),

                        SecurityPolicies =
                            new[]
                            {
                            new ServerSecurityPolicy
                            {
                                SecurityMode =
                                    MessageSecurityMode.None,

                                SecurityPolicyUri =
                                    SecurityPolicies.None
                            }
                            }.ToArrayOf()
                    };

                var securityConfiguration =
                    new SecurityConfiguration
                    {
                        AutoAcceptUntrustedCertificates =
                            true,

                        AddAppCertToTrustedStore =
                            true,

                        ApplicationCertificate =
                            new CertificateIdentifier
                            {
                                StoreType =
                                    "Directory",

                                StorePath =
                                    "%LocalApplicationData%/OPC Foundation/CertificateStores/MachineDefault",

                                SubjectName =
                                    "CN=EIF Simulator OPC UA"
                            },

                        TrustedPeerCertificates =
                            new CertificateTrustList
                            {
                                StoreType =
                                    "Directory",

                                StorePath =
                                    "%LocalApplicationData%/OPC Foundation/CertificateStores/UA Applications"
                            },

                        TrustedIssuerCertificates =
                        new CertificateTrustList
                        {
                            StoreType =
                            "Directory",
                            
                            StorePath =
                            "%LocalApplicationData%/OPC Foundation/CertificateStores/UA Certificate Authorities"
                        },

                        RejectedCertificateStore =
                            new CertificateTrustList
                            {
                                StoreType =
                                    "Directory",

                                StorePath =
                                    "%LocalApplicationData%/OPC Foundation/CertificateStores/RejectedCertificates"
                            }
                    };

                var configuration =
                    new ApplicationConfiguration(
                        _telemetry)
                    {
                        ApplicationName =
                            "EIF Simulator OPC UA",

                        ApplicationUri =
                            applicationUri,

                        ProductUri =
                            productUri,

                        ApplicationType =
                            ApplicationType.Server,

                        ServerConfiguration =
                            serverConfiguration,

                        SecurityConfiguration =
                            securityConfiguration,

                        TransportQuotas =
                            new TransportQuotas
                            {
                                OperationTimeout =
                                    15000
                            },

                        TraceConfiguration =
                            new TraceConfiguration()
                    };

                await configuration.ValidateAsync(
                    ApplicationType.Server);

                _application =
                    new ApplicationInstance(
                        configuration,
                        _telemetry)
                    {
                        ApplicationName =
                            "EIF Simulator OPC UA",

                        ApplicationType =
                            ApplicationType.Server
                    };

                await _application
                    .CheckApplicationInstanceCertificatesAsync(
                        true);

                _server =
                    new OpcUaServer(
                        _telemetry,
                        BindingManager);

                await _application
                    .StartAsync(
                        _server);
            }

            public async Task StopAsync()
            {
                if (_application != null)
                {
                    await _application
                        .StopAsync();
                }

                if (_server != null)
                {
                    await _server
                        .DisposeAsync();
                }

                _server = null;
                _application = null;
                _telemetry = null;
            }
        }
    }
}