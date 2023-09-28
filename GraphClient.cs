using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Net;

class GraphClient
{
  static GraphServiceClient? _client;
  static HttpClient? _httpClient;

  public static GraphServiceClient Client
  {
    get
    {
      if (_client is null)
      {
        var builder = new ConfigurationBuilder().AddUserSecrets<GraphClient>();
        var config = builder.Build();

        var clientId = config["AzureAd:ClientId"];
        var clientSecret = config["AzureAd:ClientSecret"];
        var tenantId = config["AzureAd:TenantId"];

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var handlers = GraphClientFactory.CreateDefaultHandlers();
        // add as a second middleware to get access to the access token
        handlers.Insert(1, new CompleteJobWithDelayHandler(1000));
        _httpClient = GraphClientFactory.Create(handlers, proxy: new WebProxy("http://localhost:8000"));

        _client = new GraphServiceClient(_httpClient, credential);
      }

      return _client;
    }
  }

  // needed as a workaround because there's no POST on schema
  public static HttpClient? HttpClient
  {
    get
    {
      return _httpClient;
    }
  }
}