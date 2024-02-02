# luraph-dotnet

[![Version](https://img.shields.io/nuget/v/luraph.api.svg)](https://www.nuget.org/packages/luraph.api)
[![Downloads](https://img.shields.io/nuget/dt/luraph.api.svg)](https://www.nuget.org/packages/luraph.api)
[![Build Status](https://github.com/Luraph/luraph-dotnet/actions/workflows/release.yml/badge.svg)](https://github.com/Luraph/luraph-dotnet/actions/workflows/release.yml)
[![License](https://img.shields.io/github/license/Luraph/luraph-dotnet)](LICENSE)

This repository hosts the official SDK for interacting with the Luraph API from .NET environments.

**Luraph API access is only available for accounts under certain plans. For more information, please check out the pricing plans on the [Luraph website](https://lura.ph/#pricing).**

## Installation

*See more information on the [NuGet Gallery Page](https://www.nuget.org/packages/luraph.api).*

Installation via [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/):
```sh
dotnet add package Luraph.API
```

Installation via [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):
```powershell
Install-Package Luraph.API
```



## Usage

*The official [Luraph API documentation](https://lura.ph/dashboard/documents/apidoc) contains the most up-to-date and complete information and instructions for integrating with the Luraph API.*

*Coming soon! See the [test file](LuraphTests/Program.cs) for an example integration.*

## Documentation

*The official [Luraph API documentation](https://lura.ph/dashboard/documents/apidoc) contains the most up-to-date and complete information and instructions for integrating with the Luraph API.*

### class LuraphAPI

#### new LuraphAPI(string apiKey)

Description: Creates an instance of the SDK.

Parameters:
- **apiKey** - API key to authenticate your requests. You can fetch your API key from your [account page](https://lura.ph/dashboard/account).

Returns: [*LuraphAPI*](#class-luraphapi)

---

#### GetNodes()

Description: Obtains a list of available obfuscation nodes.

Parameters: *None!*

Returns: [*NodesResponse*](#struct-nodesresponse)

---

#### CreateNewJob(string node, string script, string fileName, Dictionary<string, object> options, bool useTokens = false, bool enforceSettings = false)

Description: Queues a new obfuscation task.

Parameters:
- **node**: The node to assign the obfuscation job to.
- **script**: The script to be obfuscated.
- **fileName**: A file name to associate with this script. The maximum file name is 255 characters.
- **options**: An object containing keys that represent the option identifiers, and values that represent the desired settings. Unless `enforceSettings` is set to false, all options supported by the node must have a value specified, else the endpoint will error.
- **useTokens** - A boolean on whether you'd like to use tokens regardless of your active subscription.
- **enforceSettings** - A boolean on whether you'd like the `options` field to require *every* option requested by the server to be present with a valid value. If this is false, your integration will not break when invalid options are provided; however, updates that change Luraph's options will result in your integration using default settings when invalid values are specified. By default, this is set to `true`.

Returns: [*CreateJobResponse*](#struct-createjobresponse)

---

#### GetJobStatus(string jobId)

Description: This endpoint does not return until the referenced obfuscation job is complete. The maximum timeout is 60 seconds, and this endpoint can be called a **maximum** of 3 times per job.

Parameters:
- **jobId** - The job ID of the obfuscation to wait for.

Returns: [*ObfuscateStatusResponse*](#struct-obfuscatestatusresponse)

---

#### DownloadResult(string jobId)

Description: Downloads the resulting file associated with an obfuscation job.

Parameters:
- **jobId** - The job ID of the obfuscation to download.

Returns: [*ObfuscateDownloadResponse*](#struct-obfuscatedownloadresponse)

---

### struct LuraphOption

Fields:
- string **Name** - The human readable name associated with an option.
- string **Description** - The markdown formatted description for an option.
- **Tier** - One of `CUSTOMER_ONLY`, `PREMIUM_ONLY`, `ADMIN_ONLY`.
- **Type** - One of `CHECKBOX`, `DROPDOWN`, `TEXT`.
- string[] **Choices** - An array of acceptable option values when `type == DROPDOWN`.

---

### struct LuraphNode

Fields:
- double **CpuUsage** - The current CPU usage of the node.
- Dictionary&lt;string, [*LuraphOption*](#struct-luraphoption)&gt; **Options** - An object with option identifiers as keys and `LuraphOption` as values.

---

### struct LuraphError

Fields:
- string? **Param** - The parameter associated with the cause of the error.
- string **Message** - A human readable error message.

---

### struct NodesResponse

Fields:
- string? **RecommendedId** - The most suitable node to perform an obfuscation based on current service load and other possible factors.
- Dictionary&lt;string, [*LuraphNode*](#struct-luraphnode)&gt; **Nodes** - A list of all available nodes to submit obfuscation jobs to.

---

### struct CreateJobResponse

Fields:
- string **JobId** - A unique identifier for the queued obfuscation job.

---

### struct ObfuscateStatusResponse

Fields:
- bool **Success** - A boolean indicating whether the job was successful.
- string? **Error** - An error message if the job failed, or null if the job succeeded.

---

### struct ObfuscateDownloadResponse

Fields:
- string **FileName** - A sanitized version of the initial filename, including a suffix to differentiate it from the original filename.
- Stream **Data** - A stream containing the obfuscated script.

---

### class LuraphException : Exception

Fields:
- bool **IsHttpError** - Whether the exception was caused by an unhandled error.
- **Errors** - An array of [*LuraphError*](#struct-lurapherror).
- **Message** - A human readable collection of error messages returned by a request.

## Useful Links
- [Visit the Luraph Website](https://lura.ph/ "Luraph - Online Lua Obfuscation")
- [Join the Luraph Discord](https://discord.lura.ph/ "Luraph Discord Server")
- [Read the Luraph Documentation](https://lura.ph/dashboard/documents "Luraph Documentation")
- [Read the Luraph FAQs](https://lura.ph/dashboard/faq "Luraph Frequently Asked Questions")