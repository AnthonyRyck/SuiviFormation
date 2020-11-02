using AccessData.Models;
using Radzen;
using System.Collections.Generic;

namespace FormationApp.ViewModel
{
	public interface IAccueil
	{
		DialogService DialogService { get; set; }

		IList<DataSession> ProchaineSessions { get; set; }
	}
}