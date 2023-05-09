using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EdjCase.ICP.Agent.Agents.Http
{
	/// <summary>
	/// The default http client to use with the built in `HttpClient`
	/// </summary>
	public class DefaultHttpClient : IHttpClient
	{
		private const string CBOR_CONTENT_TYPE = "application/cbor";

		private readonly HttpClient httpClient;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="client">HTTP client to use</param>
		public DefaultHttpClient(HttpClient client)
		{
			this.httpClient = client;
		}

		/// <inheritdoc />
		public async Task<HttpResponse> GetAsync(string url)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			return await this.SendAsync(request);
		}

		/// <inheritdoc />
		public async Task<HttpResponse> PostAsync(string url, byte[] cborBody)
		{
			var content = new ByteArrayContent(cborBody);
			content.Headers.Remove("Content-Type");
			content.Headers.Add("Content-Type", CBOR_CONTENT_TYPE);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = content
			};
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(CBOR_CONTENT_TYPE));

			return await this.SendAsync(request);
		}

		private async Task<HttpResponse> SendAsync(HttpRequestMessage message)
		{
			HttpResponseMessage response = await this.httpClient.SendAsync(message);

			return new HttpResponse(response.StatusCode, response.Content.ReadAsByteArrayAsync);
		}
	}
}
