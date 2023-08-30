namespace pwnctl.infra.Aws;

using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

public static class AwsConfigProvider
{
    public static CredentialProfile TryGetAWSProfile(string profileName)
    {
        try
        {
            var credentialProfileStoreChain = new CredentialProfileStoreChain();

            if (!credentialProfileStoreChain.TryGetProfile(profileName, out CredentialProfile profile))
                throw new AmazonClientException($"Unable to find profile {profileName} in CredentialProfileStoreChain.");

            return profile;
        }
        catch
        {
            return null;
        }
    }

    public static AWSCredentials TryGetAWSProfileCredentials(string profileName)
    {
        try
        {
            var credentialProfileStoreChain = new CredentialProfileStoreChain();

            if (!credentialProfileStoreChain.TryGetAWSCredentials(profileName, out AWSCredentials credentials))
                throw new AmazonClientException($"Unable to find credentials {profileName} in CredentialProfileStoreChain.");

            return credentials;
        }
        catch
        {
            return null;
        }
    }
}