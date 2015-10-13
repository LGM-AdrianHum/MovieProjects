﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using RestSharp;
using TMDbLib.Objects.Authentication;

namespace TMDbLib.Client
{
    public partial class TMDbClient
    {
        public async Task<Token> AuthenticationRequestAutenticationToken()
        {
            RestRequest request = new RestRequest("authentication/token/new")
            {
                DateFormat = "yyyy-MM-dd HH:mm:ss UTC"
            };

            IRestResponse<Token> response = await _client.ExecuteGetTaskAsync<Token>(request).ConfigureAwait(false);
            Token token = response.Data;

            token.AuthenticationCallback = response.Headers.First(h => h.Name.Equals("Authentication-Callback")).Value.ToString();

            return token;
        }

        public async Task AuthenticationValidateUserToken(string initialRequestToken, string username, string password)
        {
            RestRequest request = new RestRequest("authentication/token/validate_with_login");
            request.AddParameter("request_token", initialRequestToken);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            IRestResponse response;
            try
            {
                response = await _client.ExecuteGetTaskAsync(request).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

	        if (response.StatusCode == HttpStatusCode.Unauthorized)
	        {
		        throw new UnauthorizedAccessException("Call to TMDb returned unauthorized. Most likely the provided user credentials are invalid.");
	        }
        }

        public async Task<UserSession> AuthenticationGetUserSession(string initialRequestToken)
        {
            RestRequest request = new RestRequest("authentication/session/new");
            request.AddParameter("request_token", initialRequestToken);

            IRestResponse<UserSession> response = await _client.ExecuteGetTaskAsync<UserSession>(request).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Conveniance method combining 'AuthenticationRequestAutenticationToken', 'AuthenticationValidateUserToken' and 'AuthenticationGetUserSession'
        /// </summary>
        /// <param name="username">A valid TMDb username</param>
        /// <param name="password">The passoword for the provided login</param>
        public async Task<UserSession> AuthenticationGetUserSession(string username, string password)
        {
            Token token = await AuthenticationRequestAutenticationToken();
            await AuthenticationValidateUserToken(token.RequestToken, username, password);
            return await AuthenticationGetUserSession(token.RequestToken);
        }

        public async Task<GuestSession> AuthenticationCreateGuestSession()
        {
            RestRequest request = new RestRequest("authentication/guest_session/new")
            {
                DateFormat = "yyyy-MM-dd HH:mm:ss UTC"
            };

            IRestResponse<GuestSession> response = await _client.ExecuteGetTaskAsync<GuestSession>(request).ConfigureAwait(false);

            return response.Data;
        }
    }
}
