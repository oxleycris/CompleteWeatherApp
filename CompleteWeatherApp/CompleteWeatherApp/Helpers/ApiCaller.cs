﻿using System.Net.Http;
using System.Threading.Tasks;

namespace CompleteWeatherApp.Helpers
{
    public class ApiCaller
    {
        public static async Task<ApiResponse> GetAsync(string url, string authId = null)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(authId))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authId);
                }

                var request = await client.GetAsync(url);

                if (request.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        Response = await request.Content.ReadAsStringAsync()
                    };
                };

                return new ApiResponse
                {
                    ErrorMessage = request.ReasonPhrase
                };
            }
        }
    }

    public class ApiResponse
    {
        public bool Successful => ErrorMessage == null;

        public string ErrorMessage { get; set; }

        public string Response { get; set; }
    }
}
