using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

class GraphService
{
  static GraphServiceClient? _client;
  static HttpClient? _httpClient;

  public static GraphServiceClient Client
  {
    get
    {
      if (_client is null)
      {
        var builder = new ConfigurationBuilder().AddUserSecrets<GraphService>();
        var config = builder.Build();

        var clientId = config["AzureAd:ClientId"];
        var clientSecret = config["AzureAd:ClientSecret"];
        var tenantId = config["AzureAd:TenantId"];

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var handlers = GraphClientFactory.CreateDefaultHandlers();
        handlers.Insert(0, new CompleteJobWithDelayHandler(60000));
        // add at the end to get all information about the request
        // handlers.Add(new DebugRequestHandler());
        // add at the beginning to get all information about the response
        // and also have the response body decompressed
        // handlers.Insert(0, new DebugResponseHandler());
        // _httpClient = GraphClientFactory.Create(handlers, proxy: new WebProxy("http://localhost:8000"));
        _httpClient = GraphClientFactory.Create(handlers);

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