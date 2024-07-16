﻿using System.Net;
using Microsoft.Extensions.Configuration;

namespace MailKit.Client;

/// <summary>
/// Provides the client configuration settings for connecting MailKit to an SMTP server.
/// </summary>
public sealed class MailKitClientSettings
{
    internal const string DefaultConfigSectionName = "MailKit:Client";

    /// <summary>
    /// Gets or sets the SMTP server <see cref="Uri"/>.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the network credentials that are optionally configurable for SMTP
    /// server's that require authentication.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public NetworkCredential? Credentials { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableTracing { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry metrics are disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableMetrics { get; set; }

    internal void ParseConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"""
                    ConnectionString is missing.
                    It should be provided in 'ConnectionStrings:<connectionName>'
                    or '{DefaultConfigSectionName}:Endpoint' key.'
                    configuration section.
                    """);
        }

        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) is false)
        {
            throw new InvalidOperationException($"""
                    The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                    '{DefaultConfigSectionName}') isn't a valid URI format.
                    """);
        }

        Endpoint = uri;
    }

    internal void ParseCredentials(IConfigurationSection credentialsSection)
    {
        if (credentialsSection is null or { Value: null })
        {
            return;
        }

        var username = credentialsSection["UserName"];
        var password = credentialsSection["Password"];

        if (username is null || password is null)
        {
            throw new InvalidOperationException($"""
                    The '{DefaultConfigSectionName}:Credentials' section cannot be empty.
                    Either remove Credentials altogether, or provide them.
                    """);
        }

        Credentials = new(username, password);
    }
}
