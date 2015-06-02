using Xero.Api.Core;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace XeroIntegrationSample.Xero.Applications.Private
{
    public class Core : XeroCoreApi
    {
        private static readonly DefaultMapper Mapper = new DefaultMapper();
        private static readonly string _baseUrl = "https://api.xero.com";
        private static readonly string _callbackUrl = "localhost";
        private static readonly string _signingCertificatePath = @"..\..\resources\cert\public_privatekey.pfx";

        private static readonly string _key = "{YOUR-KEY-HERE}";
        private static readonly string _secret = "{YOUR-SECRET-KEY-HERE}";
        private static readonly string _password = "{YOUR-CERTIFICATES-PASSWORD-HERE}";
        
        public Core() :
            base(_baseUrl,
                new PrivateAuthenticator(_signingCertificatePath, _password),
                new Consumer(_key, _secret),
                null,
                Mapper,
                Mapper)
        {
        }
    }
}
