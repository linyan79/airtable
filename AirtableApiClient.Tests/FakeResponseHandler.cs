﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirtableApiClient.Tests
{
    internal class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<string, MethodAndResponse> _FakeResponses = new Dictionary<string, MethodAndResponse>();

        internal class MethodAndResponse
        {
            public HttpMethod Method { get; set; }
            public HttpResponseMessage Response { get; set; }
        }

        internal void AddFakeResponse(string uri, HttpMethod method, HttpResponseMessage response)
        {
            _FakeResponses.Add(uri, new MethodAndResponse { Method = method, Response = response });
        }

        protected async Task<HttpResponseMessage> Send(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_FakeResponses.ContainsKey(request.RequestUri.AbsoluteUri))
            {
                if (_FakeResponses[request.RequestUri.AbsoluteUri].Method == request.Method)
                {
                    return _FakeResponses[request.RequestUri.AbsoluteUri].Response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }
    }
}
