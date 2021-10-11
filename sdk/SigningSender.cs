using System.Threading.Tasks;

namespace SmartyStreets
{
	public class SigningSender : ISender
	{
		private readonly ICredentials signer;
		private readonly ISender inner;

		public SigningSender(ICredentials signer, ISender inner)
		{
			this.signer = signer;
			this.inner = inner;
		}

        public Task<Response> SendAsync(Request request)
        {
			this.signer.Sign(request);
			return this.inner.SendAsync(request);
		}
    }
}