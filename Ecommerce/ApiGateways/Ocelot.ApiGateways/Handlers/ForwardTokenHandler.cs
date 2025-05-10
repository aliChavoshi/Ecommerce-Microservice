using System.Net.Http.Headers;

namespace Ocelot.ApiGateways.Handlers;

public class ForwardTokenHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Contains("Authorization"))
        {
            var token = request.Headers.GetValues("Authorization").First();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        }
        return await base.SendAsync(request, cancellationToken);
    }
}