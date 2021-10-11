namespace SmartyStreets.USReverseGeoApi
{
	using System;
	using System.IO;
    using System.Threading.Tasks;

    public class Client : IUSReverseGeoClient
	{
		private readonly ISender sender;
		private readonly ISerializer serializer;

		public Client(ISender sender, ISerializer serializer)
		{
			this.sender = sender;
			this.serializer = serializer;
		}

		public async Task SendAsync(Lookup lookup)
		{
			if (lookup == null)
				throw new ArgumentNullException("lookup");

			var request = BuildRequest(lookup);

			var response = await this.sender.SendAsync(request);

			using (var payloadStream = new MemoryStream(response.Payload))
			{
				var smartyResponse = this.serializer.Deserialize<SmartyResponse>(payloadStream) ?? new SmartyResponse();
				lookup.SmartyResponse = smartyResponse;
			}
		}

		private static Request BuildRequest(Lookup lookup)
		{
			var request = new Request();

			request.SetParameter("latitude", lookup.Latitude);
			request.SetParameter("longitude", lookup.Longitude);

			return request;
		}
	}
}