using Google.Cloud.SecretManager.V1;
using Google.Protobuf;

namespace authentication_service.GCloud
{
	public class GCSecretManager
	{
		public string GetSecret(string projectId, string secretId)
		{
			SecretManagerServiceClient client = SecretManagerServiceClient.Create();
			SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, "latest");
			AccessSecretVersionResponse response = client.AccessSecretVersion(secretVersionName);
			return response.Payload.Data.ToStringUtf8();
		}
	}
}
