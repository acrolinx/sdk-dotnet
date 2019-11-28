# Acrolinx .NET SDK

This library is meant to be used to interact with the Acrolinx Platform API in automated integrations.
It does NOT offer an interface to work with the Acrolinx Sidebar (see [Sidebar .NET SDK](https://github.com/acrolinx/sidebar-sdk-dotnet)).

## Get Started with Your Integration

### Prerequisites

Please contact [Acrolinx SDK support](https://github.com/acrolinx/acrolinx-coding-guidance/blob/master/topics/sdk-support.md)
for consulting and getting your integration certified.
The tests in this SDK work with a test license on an internal Acrolinx URL.
This license is only meant for demonstration and developing purposes.
Once you finished your integration, you'll have to get a license for your integration from Acrolinx.
  
Acrolinx offers different other SDKs, and examples for developing integrations.

Before you start developing your own integration, you might benefit from looking into:

* [Getting Started with Custom Integrations](https://docs.acrolinx.com/customintegrations),
* the [Guidance for the Development of Acrolinx Integrations](https://github.com/acrolinx/acrolinx-coding-guidance),
* the [Acrolinx Platform API](https://github.com/acrolinx/platform-api)
* the [Rendered Version of Acrolinx Platform API](https://acrolinxapi.docs.apiary.io/#)
* the [Acrolinx SDKs](https://github.com/acrolinx?q=sdk), and
* the [Acrolinx Demo Projects](https://github.com/acrolinx?q=demo).

### Start Developing

1. Set the environment variable `ACROLINX_API_SSO_TOKEN` to a valid SSO token for the Acrolinx Platform test instance you’re using.
2. Set a valid user in the environment variable `ACROLINX_API_USERNAME`. User custom fields must be filled out for this user.
3. Open [`Acrolinx.Net\Acrolinx.Net.sln`](Acrolinx.Net\Acrolinx.Net.sln) with Visual Studio 2019.
4. Compile and run all tests.

#### Installation

#### NuGet

Reference the [`Acrolinx.Net` NuGet package](https://www.nuget.org/packages/Acrolinx.Net/) using the NuGet package manager.

![Click on `Mange NuGet Packages...`](doc/nuget.png)

![Add the latest `Acrolinx.Net` NuGet package.](doc/nuget2.png)

#### First Steps

Create instance of `AcrolinxEndpoint` to begin.

`AcrolinxEndpoint` offers a single entry point to avail features provided by the SDK.

See [Acrolinx .NET SDK Demo](https://github.com/acrolinx/sdk-demo-dotnet/blob/master/Acrolinx.Net.Demo/Program.cs) for a quickstart example.

See [EndpointTest.java](Acrolinx.Net/Acrolinx.Net.Tests/EndpointTest.cs) for more examples.

## SDK Features

1. **Authenticate via SSO or API token** -
   See: [Authentication in Coding Guidance](https://github.com/acrolinx/acrolinx-coding-guidance/blob/master/topics/configuration.md#authentication)
2. **Check** - Check documents, set all relevant parameters, and submit document content, access high-level results like Acrolinx Score - See: [Text Extraction](https://github.com/acrolinx/acrolinx-coding-guidance/blob/master/topics/text-extraction.md)
3. **Content Analysis Dashboard** - Aggregate all results to one dashboard

## License

Copyright 2019-present Acrolinx GmbH

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at:

[http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
