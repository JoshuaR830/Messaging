using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;

namespace FriendsInfoLambda
{
    public class Handler
    {
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "",
            };
        }
    }
}