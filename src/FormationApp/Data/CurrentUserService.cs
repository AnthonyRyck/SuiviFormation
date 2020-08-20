using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Data
{
	public class CurrentUserService
	{
		/// <summary>
		/// ID de l'utilisateur
		/// </summary>
		public string UserId {
			get
			{
				if (string.IsNullOrEmpty(_userId))
				{
					_userId = GetId().Result;
				}
				
				return _userId;
			}
		}
		private string _userId;


		private AuthenticationStateProvider AuthenticationStateProvider;

		public CurrentUserService(AuthenticationStateProvider stateProvider)
		{
			AuthenticationStateProvider = stateProvider;
		}

		public async Task<string> GetId()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			string userId;

			if (user.Identity.IsAuthenticated)
			{
				var claims = user.Claims;
				userId = claims.Where(x => x.Type.Contains("nameidentifier")).FirstOrDefault().Value;
			}
			else
			{
				userId = null;
			}

			return userId;
		}

	}
}
