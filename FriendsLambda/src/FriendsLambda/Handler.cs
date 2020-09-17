using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace FriendsLambda
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
            
            var friendGetRequest = new GetItemRequest
            {
                TableName = "MessagingUserTable",
                Key = new Dictionary<string, AttributeValue>
                {
                    ["UserId"] = new AttributeValue{S = userId}
                }
            };

            var friendItemResponse = await _dynamoDb.GetItemAsync(friendGetRequest);
            if (!friendItemResponse.IsItemSet)
                return forbiddenApiGatewayResponse;

            var friends = friendItemResponse.Item["Connections"].SS;
            Console.WriteLine(JsonConvert.SerializeObject(friends));
            
            //
            // var client = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
            // {
            //     ServiceURL = "https://oc9gdrsfcl.execute-api.eu-west-2.amazonaws.com/messaging-test",
            // });
            //
            // var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(friends));
            // var messageStream = new MemoryStream(messageBytes);
            //
            // Console.WriteLine("Hello");
            // var response = await client.PostToConnectionAsync(new PostToConnectionRequest
            // {
            //     ConnectionId = request.RequestContext.ConnectionId,
            //     Data = messageStream,
            // }, CancellationToken.None);
            //
            // Console.WriteLine(JsonConvert.SerializeObject(response));
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(friends),
            };
        }
    }
}