// Author: Hum, Adrian
// Project: Movies/TMDbLib/TMDbRestClient.cs
//
// Created  Date: 2015-10-09  2:40 PM
// Modified Date: 2015-10-13  10:21 AM

#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TMDbLib.Objects.Exceptions;

#endregion

namespace TMDbLib
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class TMDbRestClient : RestClient
    {
        public TMDbRestClient()
        {
            InitializeDefaults();
        }

        public TMDbRestClient(string baseUrl) : base(baseUrl)
        {
            InitializeDefaults();
        }

        public int MaxRetryCount { get; set; }
        public int RetryWaitTimeInSeconds { get; set; }
        public bool ThrowErrorOnExcedingMaxCalls { get; set; }

        /// <summary>
        ///     Executes the specified request and deserializes the response content using the appropriate content handler
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to execute</param>
        /// <returns>RestResponse[[T]] with deserialized data in Data property</returns>
        /// <exception cref="UnauthorizedAccessException">
        ///     Can be thrown if either to provided API key is invalid or when relavant
        ///     the provided session id does not grant to required access
        /// </exception>
        /// <exception cref="Exception">Condition.</exception>
        /// <exception cref="RequestLimitExceededException">Condition.</exception>
        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            var response = base.Execute<T>(request);

            if (response.ErrorException != null) {
                if (MaxRetryCount >= request.Attempts && response.ErrorException.GetType() == typeof (WebException)) {
//                    var webException = (WebException) response.ErrorException;

                    // Retry the call after waiting the configured ammount of time, it gets progressively longer every retry
                    Thread.Sleep(request.Attempts*RetryWaitTimeInSeconds*1000);
                    return Execute<T>(request);
                }

                throw response.ErrorException;
            }

            switch (response.StatusCode) {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException("Call to TMDb returned unauthorized. Most likely the provided API key is invalid.");
                case (HttpStatusCode) 429:
                    if (ThrowErrorOnExcedingMaxCalls) throw new RequestLimitExceededException();
                    var retryAfterParam = response.Headers.FirstOrDefault(header => header.Name.Equals("retry-after", StringComparison.OrdinalIgnoreCase));
                    if (retryAfterParam == null) throw new RequestLimitExceededException();
                    int retryAfter;
                    if (!int.TryParse(retryAfterParam.Value.ToString().Trim(), out retryAfter)) throw new RequestLimitExceededException();
                    Thread.Sleep(retryAfter*1000);
                    return Execute<T>(request);

                // We don't wish to wait or no valid retry-after header was present
            }

            return response;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     The time-out value is negative and is not equal to
        ///     <see cref="F:System.Threading.Timeout.Infinite" />.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     Call to TMDb returned unauthorized. Most likely the provided API key is
        ///     invalid.
        /// </exception>
        /// <exception cref="Exception">Condition.</exception>
        /// <exception cref="RequestLimitExceededException">Condition.</exception>
        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            var response = await base.ExecuteTaskAsync<T>(request, token).ConfigureAwait(false);

            if (response.ErrorException != null) {
                if (MaxRetryCount >= request.Attempts && response.ErrorException.GetType() == typeof (WebException)) {
                    //var webException = (WebException) response.ErrorException;

                    // Retry the call after waiting the configured ammount of time, it gets progressively longer every retry
                    Thread.Sleep(request.Attempts*RetryWaitTimeInSeconds*1000);
                    return await ExecuteTaskAsync<T>(request, token).ConfigureAwait(false);
                }

                throw response.ErrorException;
            }

            switch (response.StatusCode) {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException("Call to TMDb returned unauthorized. Most likely the provided API key is invalid.");
                case (HttpStatusCode) 429:
                    if (ThrowErrorOnExcedingMaxCalls) throw new RequestLimitExceededException();
                    var retryAfterParam = response.Headers.FirstOrDefault(header => header.Name.Equals("retry-after", StringComparison.OrdinalIgnoreCase));
                    if (retryAfterParam == null) throw new RequestLimitExceededException();
                    int retryAfter;
                    if (!int.TryParse(retryAfterParam.Value.ToString().Trim(), out retryAfter)) throw new RequestLimitExceededException();
                    Thread.Sleep(retryAfter*1000);
                    return await ExecuteTaskAsync<T>(request, token).ConfigureAwait(false);

                // We don't wish to wait or no valid retry-after header was present
            }

            return response;
        }

        private void InitializeDefaults()
        {
            MaxRetryCount = 0;
            RetryWaitTimeInSeconds = 10;
            ThrowErrorOnExcedingMaxCalls = false;
        }
    }
}