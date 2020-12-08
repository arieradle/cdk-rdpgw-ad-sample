using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.DirectoryService;
using System.Reflection;

namespace CdkRdgwAdSample
{
    public class VpcStack : Stack
    {
        public Vpc Vpc { get; private set; }
        internal VpcStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Vpc = new Vpc(this, "MainVPC", new VpcProps
            {
                Cidr = "10.0.0.0/16",
                SubnetConfiguration = new[] { new SubnetConfiguration {
                        CidrMask = 24,
                        Name = "pub-subnet-1",
                        SubnetType = SubnetType.PUBLIC
                    }, new SubnetConfiguration {
                        CidrMask = 24,
                        Name = "priv-subnet-1",
                        SubnetType = SubnetType.PRIVATE
                    }}
            });
        }
    }

    public class DHCPOption : Stack
    {
        public CfnDHCPOptions DhcpOptions { get; set; }
        internal DHCPOption(Construct scope, string id, CfnMicrosoftAD directory, Vpc vpc, IStackProps props = null) : base(scope, id, props)
        {   
             DhcpOptions = new CfnDHCPOptions(this, id, new CfnDHCPOptionsProps{
                DomainName = directory.Name,
                DomainNameServers = new string[] {Fn.ImportValue("mad-dns1"), Fn.ImportValue("mad-dns2")},
                NtpServers = new[] {"169.254.169.123"}
            });
        }
    }

    public class SetDHCPOption : Stack
    {
        internal SetDHCPOption(Construct scope, string id, Vpc vpc, DHCPOption dhcpOption, IStackProps props = null) : base(scope, id, props)
        {   
            var dhcpOptionsAssociation = new CfnVPCDHCPOptionsAssociation(this, id, new CfnVPCDHCPOptionsAssociationProps{
                VpcId = vpc.VpcId,
                DhcpOptionsId = dhcpOption.DhcpOptions.Ref
            });
        }
    }
}