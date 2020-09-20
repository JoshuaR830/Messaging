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

            var body = JsonConvert.DeserializeObject<MessageBody>(request.Body);

            var messageRecipients = body.Users ?? new List<string>();
            
            var messageBytes = Encoding.UTF8.GetBytes(body.Message);
            var messageStream = new MemoryStream(messageBytes);
            
            // var bytes = Encoding.UTF8.GetBytes($"Hello from {request.RequestContext.ConnectionId}");
            // var stream = new MemoryStream(bytes);
            
            var connectionIds = new List<string>();

            var keys = new List<Dictionary<string, AttributeValue>>();

            Console.WriteLine(JsonConvert.SerializeObject(body.Users));
            Console.WriteLine(JsonConvert.SerializeObject(messageRecipients));
            
            foreach (var recipientId in messageRecipients)
            {
                keys.Add(new Dictionary<string, AttributeValue>
                {
                    ["UserId"] = new AttributeValue {S = recipientId}
                });
            }

            if (keys.Count > 0)
            {
                var batchGetConnections = new BatchGetItemRequest
                {
                    RequestItems = new Dictionary<string, KeysAndAttributes>
                    {
                        {
                            "MessagingConnectionsTable",
                            new KeysAndAttributes
                            {
                                Keys = keys,
                            }
                        }
                    }
                };

                var connectionIdsResponse = await _dynamoDb.BatchGetItemAsync(batchGetConnections);

                connectionIds
                    .AddRange(connectionIdsResponse.Responses["MessagingConnectionsTable"]
                    .Select(connectionIdResponse => connectionIdResponse["ConnectionId"].S));
            }

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
                    messageStream.Position = 0;
                    var response = await client.PostToConnectionAsync(new PostToConnectionRequest
                    {
                        ConnectionId = connectionId,
                        Data = messageStream,
                    }, CancellationToken.None);

                    Console.WriteLine(JsonConvert.SerializeObject(response));
                }
                catch (Exception e)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(e));
                }
            }
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Connected"
            };
        }
    }
}