using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessagingLambda
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {

            Console.WriteLine(JsonConvert.SerializeObject(request));

            byte[] bytes = Encoding.UTF8.GetBytes("Hello world");
            var response = await client.PostAsync($"https://oc9gdrsfcl.execute-api.eu-west-2.amazonaws.com/messaging-test/@connections/{request.RequestContext.ConnectionId}", new ByteArrayContent(bytes));

            Console.WriteLine(response.StatusCode);
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
