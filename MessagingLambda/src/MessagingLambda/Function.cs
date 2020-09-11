using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessagingLambda
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {

            Console.WriteLine(JsonConvert.SerializeObject(request));
            
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
