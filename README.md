[![Build and Test](https://github.com/trakx/fireblocks-api-client/actions/workflows/test.yml/badge.svg)](https://github.com/trakx/fireblocks-api-client/actions/workflows/test.yml)

# fireblocks-api-client
C# implementation of a Fireblocks api client

## How to regenerate C# API clients

* If you work with external API, you probably need to update OpenAPI definition added to the project. It's usually openApi3.yaml file.
* Do right click on the project and select Edit Project File. In the file change value of `GenerateApiClient` property to true.
* Rebuild the project. `NSwag` target will be executed as post action.
* The last thing to be done is to run integration test `OpenApiGeneratedCodeModifier` that will rewrite auto generated C# classes to use C# 9 features like records and init keyword.

## AWS Parameters
In order to be able to run some integration tests, you should ensure that you have access to the AWS parameters starting in `/CiCd`. In order for the applications in this solution to run correctly on AWS, please ensure that variables starting in `/[environment]` are defined for all 3 environments \( _Production_, _Staging_, _Development_ \) :
```awsParams
# REPOSITORY SECRETS
/[environment]/Trakx/Fireblocks/ApiClient/FireblocksApiConfiguration/ApiPrivateKey
/[environment]/Trakx/Fireblocks/ApiClient/FireblocksApiConfiguration/ApiPubKey
/[environment]/Trakx/Fireblocks/ApiClient/FireblocksApiConfiguration/BaseUrl

# GLOBAL SECRETS
# Instead of creating a specific repository secret, can use the global one with the same [Key]
/[environment]/Global/FireblocksApiConfiguration/BaseUrl
```
