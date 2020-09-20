using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;

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
            
            var getUserIdRequest = new GetItemRequest
            {
                TableName = "EmailAddressToUserMappingTable", 
                Key = new Dictionary<string, AttributeValue>
                {
                    ["Email"] = new AttributeValue{S = "joshuaquarryhouse@gmail.com"}
                },
            };

            var userIdResponse = await _dynamoDb.GetItemAsync(getUserIdRequest);

            var forbiddenApiGatewayResponse = new APIGatewayProxyResponse
            {
                StatusCode = 403
            };
            
            if (!userIdResponse.IsItemSet)
                return forbiddenApiGatewayResponse;

            var userId = userIdResponse.Item["UserId"].S;
            
            var putItemRequest = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["UserId"] = new AttributeValue {S = userId},
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