using AccessData.Models;
using FormationApp.Data;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace FormationApp.ViewModel
{
	public interface IMesFormations
	{

		bool DialogIsOpenNewNote { get; set; }
		List<SessionView> AllSessions { get; set; }
		string CommentairePourFormation { get; set; }
		List<CompetenceUserView> CompetencesUser { get; set; }
		int IdSessionNotation { get; set; }
		int NotePourFormation { get; set; }
		List<SessionInscritUserView> SessionInscritUserViews { get; set; }
		CurrentUserService UserService { get; set; }

		void AnnulerClickNewNote();
		void DesinscriptionOnClick(MouseEventArgs e, int idSession);
		void OkClickNewNote();

		void OpenDialogNewNote(int idSession);
	}
}