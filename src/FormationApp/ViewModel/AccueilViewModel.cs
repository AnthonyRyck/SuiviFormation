using AccessData;
using AccessData.Models;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class AccueilViewModel : IAccueil
	{
		#region Properties

		public DialogService DialogService { get; set; }

		SqlContext SqlService { get; set; }

		/// <summary>
		/// Liste des prochaines sessions.
		/// </summary>
		public IList<DataSession> ProchaineSessions { get; set; }

		#endregion

		#region Override methods

		public AccueilViewModel(SqlContext sqlContext, DialogService dialogService)
		{
			SqlService = sqlContext;
			DialogService = dialogService;

			ProchaineSessions = SqlService.GetAllOpenSessionWithDateAsync().GetAwaiter().GetResult();
		}

		#endregion
	}
}
