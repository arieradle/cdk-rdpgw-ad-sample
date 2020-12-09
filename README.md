# CDK example demonstrating creation of VPC, RDP gateway (bastion), Managed AD and a target windows host.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET Core CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Important Notes

* Please make sure you create keypair named `inst-key-pair` prior to running this example.
* In this example, the stacks are deployed into `eu-west-1` region.
* In order to get the domain `Admin` user password to test the RDP login, please exprole the `Secrets Manager` console.
* This example was built using CDK v1.76.0

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template