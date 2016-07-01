﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bit.App.Abstractions;
using Bit.App.Models.Api;
using Newtonsoft.Json;
using Plugin.Connectivity.Abstractions;

namespace Bit.App.Repositories
{
    public class FolderApiRepository : ApiRepository<FolderRequest, FolderResponse, string>, IFolderApiRepository
    {
        public FolderApiRepository(IConnectivity connectivity)
            : base(connectivity)
        { }

        protected override string ApiRoute => "folders";

        public virtual async Task<ApiResult<ListResponse<FolderResponse>>> GetByRevisionDateAsync(DateTime since)
        {
            if(!Connectivity.IsConnected)
            {
                return HandledNotConnected<ListResponse<FolderResponse>>();
            }

            using(var client = new ApiHttpClient())
            {
                var requestMessage = new TokenHttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(client.BaseAddress, string.Concat(ApiRoute, "?since=", since)),
                };

                var response = await client.SendAsync(requestMessage);
                if(!response.IsSuccessStatusCode)
                {
                    return await HandleErrorAsync<ListResponse<FolderResponse>>(response);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JsonConvert.DeserializeObject<ListResponse<FolderResponse>>(responseContent);
                return ApiResult<ListResponse<FolderResponse>>.Success(responseObj, response.StatusCode);
            }
        }
    }
}
