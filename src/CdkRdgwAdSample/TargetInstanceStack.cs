using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;

namespace CdkRdgwAdSample
{
    public class TargetInstanceStack : Stack
    {
        public Role Role { get; private set; }
        public Instance_ TargetInstance { get; private set; }

        public SecurityGroup SecurityGroup { get; private set; }

        internal TargetInstanceStack(Construct scope, string id, Vpc vpc, string keyPairName, IStackProps props = null) : base(scope, id, props)
        {

            SecurityGroup = new SecurityGroup(this, "TargetInstance-Security-Group", new SecurityGroupProps
            {
                Vpc = vpc,
                AllowAllOutbound = true,
                Description = "TargetInstance-Security-Group",
                SecurityGroupName = "secgroup-" + id
            });


            Role = new Role(this, "ec2-targetinstance-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com")
            });

            Role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("SecretsManagerReadWrite"));

            TargetInstance = new Instance_(this, id, new InstanceProps
            {
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                MachineImage = new WindowsImage(WindowsVersion.WINDOWS_SERVER_2019_ENGLISH_FULL_BASE),
                Vpc = vpc,
                UserData = UserData.Custom(Utils.GetResource("target_instance_user_data.ps1")),
                KeyName = keyPairName,
                Role = Role,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE },
                SecurityGroup = SecurityGroup
            });

            SecurityGroup.AddIngressRule(Peer.AnyIpv4(), Port.AllTraffic(), "Allow all trafic in. In production - change this!");

            new CfnOutput(this, "target-instance", new CfnOutputProps{
                Value = TargetInstance.InstancePrivateIp
            });
        }
    }
}