using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.DirectoryService;
using Amazon.CDK.AWS.SecretsManager;
using System.Text.Json;

namespace CdkRdgwAdSample
{

    public class SecretStack : Stack
    {
        public object PasswordObject { get; private set; }
        public Secret Secret { get; private set; }
        public string ClearTextSecret => Secret.SecretValueFromJson("Password").ToString();

        internal SecretStack(Construct scope, string id, object passwordObject, string secretName, IStackProps props = null) : base(scope, id, props)
        {
            PasswordObject = passwordObject;
            Secret = new Secret(this, id, new SecretProps{
                GenerateSecretString = new SecretStringGenerator {
                    SecretStringTemplate = JsonSerializer.Serialize(passwordObject),
                    GenerateStringKey = "Password",
                    ExcludePunctuation = true
                },
                SecretName = secretName
            });
        }
    }

    public class MadStack : Stack
    {
        public CfnMicrosoftAD AD { get; private set; }

        internal MadStack(Construct scope, string id, Vpc vpc, string domainName, string edition, SecretStack secret, IStackProps props = null) : base(scope, id, props)
        {
            AD = new CfnMicrosoftAD(this, "MAD", new CfnMicrosoftADProps
            {
                VpcSettings = new CfnMicrosoftAD.VpcSettingsProperty
                {
                    SubnetIds = vpc.SelectSubnets(new SubnetSelection { SubnetType = SubnetType.PRIVATE }).SubnetIds,
                    VpcId = vpc.VpcId
                },
                Name = domainName,
                Password = secret.ClearTextSecret,
                Edition = edition
            });

            var mad_dns_ip1 = Fn.Select(0, AD.AttrDnsIpAddresses);
            var mad_dns_ip2 = Fn.Select(1, AD.AttrDnsIpAddresses);

            new CfnOutput(this, "mad-dns1", new CfnOutputProps{
                Value = mad_dns_ip1,
                ExportName = "mad-dns1"
            });

            new CfnOutput(this, "mad-dns2", new CfnOutputProps{
                Value = mad_dns_ip2,
                ExportName = "mad-dns2"
            });
        }
    }
}
