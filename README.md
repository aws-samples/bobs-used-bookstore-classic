# Bob's Used Books Classic

## Overview
Bob's Used Books Classic is a backport of the [Bob's Used Books Sample Application](https://github.com/aws-samples/bobs-used-bookstore-sample). Bob's Used Books Classic is an ASP.NET MVC application that targets .NET Framework 4.8.

## Prerequisites
To run and debug the application locally you need the following:
* A Windows-based development environment
* The [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-developer-pack-offline-installer)
* A modern IDE, for example [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

To deploy the application to AWS you need the following:
* An AWS IAM User with an attached _AdministratorAccess_ policy
* The [AWS Cloud Development Kit (CDK)](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html)
* [Bootstrap](https://docs.aws.amazon.com/cdk/v2/guide/bootstrapping.html) your AWS environment for the AWS CDK by executing `cdk bootstrap` in a terminal window
* [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows-install/) running Windows containers

## Getting started
Clone the repository or download the source code and open the solution in your preferred IDE. Set _Bookstore.Web_ as the startup project then press **F5** to debug the application.

By default, Bob's Used Books Classic is completely disconnected from AWS. When you are ready you can configure the application to use AWS services by deploying the **BobsUsedBooksClassicCore** CDK stack and updating the service setting in the application's `web.config` file. The application service settings are found within the `<appSettings>` section and are prefixed with "Services/":

```
<appSettings>
    ...
    <add key="Services/Authentication" value="local" />
    <add key="Services/Database" value="local" />
    <add key="Services/FileService" value="local" />
    <add key="Services/ImageValidationService" value="local" />
    <add key="Services/LoggingService" value="local" />
    ...
</appSettings>
```
Change the value of a service from "local" to "aws" to use the AWS implementation of that service.

With the exception of the database service*, you can use any combination of AWS services you like from within your development environment.

> \* The RDS for SQL Server database is deployed to a private subnet and is not accessible outside of the application's VPC

## Amazon Cognito first run
Use the following credentials the first time you authenticate using the AWS implementation of the authentication service:

* Username: **Admin**
* Password: **P@ssword1**

## Deployment
Bob's Used Books Classic includes a CDK stack called **BobsUsedBooksClassicECS** that automatically containerizes the application and deploys it to Amazon ECS on Fargate. You need to have Docker Desktop configured to use Windows containers before deploying the CDK stack. To deploy the application, open a terminal window and execute the following command:
```
cdk deploy BobsUsedBooksClassicECS
```
When the application is deployed to ECS on Fargate it is automatically configured to integrate with all supported AWS services except for Amazon Cognito. The Amazon Cognito Hosted UI requires login redirects to use HTTPS (except for http://localhost). ECS on Fargate is configured to use HTTP which means the application has to use the local authentication service.

## Deleting the resources

When you have completed working with the sample applications we recommend deleting the resources to avoid possible charges. To do this, either:

* Navigate to the CloudFormation dashboard in the AWS Management Console and delete all **BobsUsedBooksClassic** stacks, or

* In a terminal window navigate to the solution folder and run the command `cdk destroy BobsUsedBooksClassic*`. 