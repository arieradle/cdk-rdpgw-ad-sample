using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;

namespace CdkRdgwAdSample
{
    public class BastionStack : Stack
    {
        public Role Role { get; private set; }
        public Instance_ Bastion { get; private set; }

        internal BastionStack(Construct scope, string id, Vpc vpc, string keyPairName, IStackProps props = null) : base(scope, id, props)
        {

            Role  = new Role(this, "ec2-bastion-role", new RoleProps{
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com")
            });

            Role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("SecretsManagerReadWrite"));

            Bastion = new Instance_(this, id, new InstanceProps
            {
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                MachineImage = new WindowsImage(WindowsVersion.WINDOWS_SERVER_2019_ENGLISH_FULL_BASE),
                Vpc = vpc,
                UserData = UserData.Custom(Utils.GetResource("bastion_user_data.ps1")),
                KeyName = keyPairName,
                Role = Role,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC }
            });

             Bastion.Connections.AllowFromAnyIpv4(Port.Tcp(3389), "Internet access RDP");

             new CfnOutput(this, "Bastion Host", new CfnOutputProps{
                 Value = Bastion.InstancePublicDnsName
             });
        }
    }
}