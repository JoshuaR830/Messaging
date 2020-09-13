using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System.Linq;

namespace MessagingLambda
{
    public class Handler
    {
        private IAmazonDynamoDB _dynamoDb;
        private const string TableName = "MessagingConnectionsTable";

        public Handler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));

            var bytes = Encoding.UTF8.GetBytes("Hello world");
            var stream = new MemoryStream(bytes);
            
            var connectionIds = new List<string>();

            ScanResponse dynamoResponse = null;

            do
            {
                var scanRequest = new ScanRequest
                {
                    TableName = TableName,
                    ExclusiveStartKey = dynamoResponse?.LastEvaluatedKey,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":cId"] = new AttributeValue {S = request.RequestContext.ConnectionId}
                    },
                    FilterExpression = "ConnectionId <> :cId",
                    ConsistentRead = true
                };

                dynamoResponse = await _dynamoDb.ScanAsync(scanRequest);

                var tempConnectionIds = dynamoResponse.Items.Select(x => x["ConnectionId"].S).ToList();

                connectionIds.AddRange(tempConnectionIds);
            } while (dynamoResponse.LastEvaluatedKey.Values.Count > 0);

            Console.WriteLine(JsonConvert.SerializeObject(connectionIds));

            var client = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = "https://oc9gdrsfcl.execute-api.eu-west-2.amazonaws.com/messaging-test",
            });

            foreach (var connectionId in connectionIds)
            {
                Console.WriteLine(connectionId);

                try
                {
                    stream.Position = 0;
                    var response = await client.PostToConnectionAsync(new PostToConnectionRequest
                    {
                        ConnectionId = connectionId,
                        Data = stream,
                    }, CancellationToken.None);

                    Console.WriteLine(JsonConvert.SerializeObject(response));
                }
                catch (Exception e)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(e));
                }
            }

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