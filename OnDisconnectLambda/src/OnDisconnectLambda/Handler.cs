using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;

namespace OnDisconnectLambda
{
    public class Handler
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "MessagingConnectionsTable";
        
        public Handler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            Console.WriteLine("Disconnected");

            var queryRequest = new QueryRequest()
            {
                TableName = TableName,
                IndexName = "ConnectionId",
                ProjectionExpression = "UserId",
                KeyConditionExpression = "ConnectionId = :cid",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":cid"] = new AttributeValue{S = request.RequestContext.ConnectionId}
                }
            };

            var response = await _dynamoDb.QueryAsync(queryRequest);

            if (response.Items.Count == 0)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 403,
                    Body = "",
                };
            }
            
            var userId = response.Items[0]["UserId"].S;
            
            Console.WriteLine(userId);
            
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["UserId"] = new AttributeValue {S = userId}
                }
            };
            
            await _dynamoDb.DeleteItemAsync(deleteItemRequest);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Disconnected!",
            };
        } 
    }
}