using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace MessagingLambda
{
    public class Handler
    {
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));

            var client = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = "https://oc9gdrsfcl.execute-api.eu-west-2.amazonaws.com/messaging-test",
            });

            var bytes = Encoding.UTF8.GetBytes("Hello world");
            var stream = new MemoryStream(bytes);
            
            Console.WriteLine(JsonConvert.SerializeObject(client));
            
            var response = await client.PostToConnectionAsync(new PostToConnectionRequest
            {
                ConnectionId = request.RequestContext.ConnectionId,
                Data = stream,
            }, CancellationToken.None);

            Console.WriteLine(JsonConvert.SerializeObject(response));

            if (request.Body != null)
            {
                var body = JsonConvert.DeserializeObject<MessageBody>(request.Body);
                var message = new MessageData(body.Message, request.RequestContext.ConnectionId);
             
                Console.WriteLine(message.Message);
            }
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Connected"
            };
        }
    }
}