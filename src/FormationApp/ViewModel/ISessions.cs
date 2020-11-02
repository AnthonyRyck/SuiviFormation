using AccessData.Models;
using FormationApp.Data;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace FormationApp.ViewModel
{
	public interface ISessions
	{
		List<SessionView> AllSessions { get; set; }
		CurrentUserService UserService { get; set; }

		void DesinscriptionOnClick(MouseEventArgs e, int idSession);
		void InscriptionOnClick(MouseEventArgs e, int idSession);
	}
}