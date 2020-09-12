using System.Net.Http;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.DependencyInjection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessagingLambda
{
    public class Function
    {
        private ServiceCollection _serviceCollection;
        private static readonly HttpClient client = new HttpClient();

        public Function()
        {
            CreateServiceCollection();
        }

        public void CreateServiceCollection()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddDefaultAWSOptions(new AWSOptions());
            _serviceCollection.AddTransient<Handler>();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
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
