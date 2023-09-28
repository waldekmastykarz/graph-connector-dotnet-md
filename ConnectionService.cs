static class ConnectionService
{
  async static Task CreateConnection()
  {
    Console.Write("Creating connection...");

    await GraphClient.Client.External.Connections
      .PostAsync(ConnectionConfiguration.ExternalConnection);

    Console.WriteLine("DONE");
  }

  async static Task CreateSchema()
  {
    // workaround because there's no POST on schema
    var requestInfo = GraphClient.Client.External
      .Connections[ConnectionConfiguration.ExternalConnection.Id]
      .Schema
      .ToPatchRequestInformation(ConnectionConfiguration.Schema);
    var requestMessage = await GraphClient.Client.RequestAdapter
      .ConvertToNativeRequestAsync<HttpRequestMessage>(requestInfo);
    requestMessage!.Method = HttpMethod.Post;

    await GraphClient.HttpClient!.SendAsync(requestMessage);

    // await GraphClient.Client.External
    //   .Connections[ConnectionConfiguration.ExternalConnection.Id]
    //   .Schema
    //   .PostAsync(ConnectionConfiguration.Schema);
  }

  public static async Task ProvisionConnection()
  {
    try
    {
      await CreateConnection();
      await CreateSchema();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}