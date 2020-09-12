using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;

namespace OnConnectionLambda
{
    public class Handler
    {
        IAmazonDynamoDB _dynamoDb;
        const string TableName = "MessagingConnectionsTable";

        public Handler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }
        
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            Console.WriteLine("Connected");

            Console.WriteLine(request.RequestContext.ConnectionId);

            var connectionId = request.RequestContext.ConnectionId;

            var putItemRequest = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["UserId"] = new AttributeValue {S = Guid.NewGuid().ToString()},
                    ["ConnectionId"] = new AttributeValue {S = connectionId},
                },
            };

            await _dynamoDb.PutItemAsync(putItemRequest);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "",
            };
        }
    }
}