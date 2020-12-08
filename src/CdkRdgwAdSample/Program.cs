using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CdkRdgwAdSample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            string domainName = "mydomain.aws";
            var env = new Amazon.CDK.Environment { Region = "eu-west-1" };

            var app = new App();
            var vpcStack = new VpcStack(app, "VpcStack", new StackProps { Env = env });

            var secretStack = new SecretStack(app, "MAD-Secret", passwordObject: new { Domain = domainName, UserID = "Admin"}, secretName: "ManagedAD-Admin-Password", new StackProps
            {
                Description = "Managed AD Aut-Generated Password",
                Env = env
            });

            var madStack = new MadStack(app, "Managed-AD", vpc: vpcStack.Vpc, domainName: domainName, edition: "Standard", secret: secretStack, new StackProps
            {
                Env = env
            });

            var dhcpOption = new DHCPOption(app, "VPC-DHCP-Options-with-MAD", directory: madStack.AD, vpc: vpcStack.Vpc, new StackProps
            {
                Env = env
            });

            var setDhcpOptionToVpc = new SetDHCPOption(app, "Apply-DHCP-Options", vpc:vpcStack.Vpc, dhcpOption: dhcpOption, new StackProps
            {
                Env = env
            });

            var bastionStack = new BastionStack(app, "Bastion-Host", vpc: vpcStack.Vpc, new StackProps
            {
                Env = env
            });

            var targetInstanceStack = new TargetInstanceStack(app, "Target-Instance", vpc: vpcStack.Vpc, new StackProps
            {
                Env = env
            });


            // Defining the order of the CDK Deployment
            secretStack.AddDependency(vpcStack);
            madStack.AddDependency(secretStack);
            dhcpOption.AddDependency(madStack);
            setDhcpOptionToVpc.AddDependency(dhcpOption);

            targetInstanceStack.AddDependency(setDhcpOptionToVpc);
            bastionStack.AddDependency(targetInstanceStack);

            app.Synth();
        }
    }
}
