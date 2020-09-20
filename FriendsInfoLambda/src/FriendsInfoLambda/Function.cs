using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.DependencyInjection;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FriendsInfoLambda
{
    public class Function
    {

        private ServiceCollection _serviceCollection;
        
        public Function()
        {
            ConfigureServices();
        }

        public void ConfigureServices()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddDefaultAWSOptions(new AWSOptions());
            _serviceCollection.AddAWSService<IAmazonDynamoDB>();
            _serviceCollection.AddTransient<Handler>();
        }
        
        /// <summary>
        /// Invoke the handler which will get all of the information to display a friend
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {

            await using (var serviceProvider = _serviceCollection.BuildServiceProvider())
            {
                return await serviceProvider.GetService<Handler>().Handle(request);
            }
        }
    }
}
