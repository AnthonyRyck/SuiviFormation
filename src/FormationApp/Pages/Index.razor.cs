using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class Index : ComponentBase
	{
		#region Properties

		[Inject]
		protected DialogService DialogService { get; set; }

		[Inject]
		public SqlContext SqlService { get; set; }

		/// <summary>
		/// Liste des prochaines sessions.
		/// </summary>
		protected IList<DataSession> ProchaineSessions { get; set; }

		#endregion

		#region Override methods

		protected async override Task OnInitializedAsync()
		{
			ProchaineSessions = await SqlService.GetAllOpenSessionWithDateAsync();
		}

		#endregion


	}
}
