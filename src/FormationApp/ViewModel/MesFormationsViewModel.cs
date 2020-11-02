using AccessData;
using AccessData.Models;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class MesFormationsViewModel : IMesFormations
	{
		#region Properties

		SqlContext SqlService { get; set; }

		public CurrentUserService UserService { get; set; }


		IMatToaster Toaster { get; set; }

		/// <summary>
		/// Liste des sessions inscrits.
		/// </summary>
		public List<SessionView> AllSessions { get; set; }

		/// <summary>
		/// Liste des sessions que l'utilisateur s'est inscrit et qui sont terminés.
		/// </summary>
		public List<SessionInscritUserView> SessionInscritUserViews { get; set; }

		/// <summary>
		/// Liste des compétences acquis par l'utilisateur
		/// </summary>
		public List<CompetenceUserView> CompetencesUser { get; set; }

		public string? CommentairePourFormation { get; set; }

		public int NotePourFormation { get; set; }


		#endregion

		#region Constructeur

		public MesFormationsViewModel(SqlContext sqlContext, CurrentUserService currentUserService, IMatToaster toaster)
		{
			SqlService = sqlContext;
			UserService = currentUserService;
			Toaster = toaster;

			AllSessions = SqlService.GetInscriptionSessionUserAsync(UserService.UserId).GetAwaiter().GetResult();
			SessionInscritUserViews = SqlService.GetInscriptionSessionUserFinishAsync(UserService.UserId).GetAwaiter().GetResult();
			CompetencesUser = SqlService.GetCompetencesUser(UserService.UserId).GetAwaiter().GetResult();
		}

		#endregion

		#region MatDialog pour ajouter une note

		public bool DialogIsOpenNewNote { get; set; } = false;

		public int IdSessionNotation { get; set; }

		public void OpenDialogNewNote(int idSession)
		{
			IdSessionNotation = idSession;
			DialogIsOpenNewNote = true;
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		public async void OkClickNewNote()
		{
			// Sauvegarde en BDD
			await SqlService.SaveNotationFormation(IdSessionNotation, UserService.UserId, NotePourFormation, CommentairePourFormation);

			var tempSession = SessionInscritUserViews.First(x => x.IdSession == IdSessionNotation);
			tempSession.Note = NotePourFormation;
			tempSession.Commentaire = CommentairePourFormation;

			// Remise à zéro, dans le cas d'une nouvelle notation
			NotePourFormation = 0;
			CommentairePourFormation = null;

			DialogIsOpenNewNote = false;
		}

		/// <summary>
		/// Annule l'ajout
		/// </summary>
		public void AnnulerClickNewNote()
		{
			// Remise à zéro, dans le cas d'une nouvelle notation
			NotePourFormation = 0;
			CommentairePourFormation = null;

			DialogIsOpenNewNote = false;
		}

		#endregion



		#region Public methods

		/// <summary>
		/// Event sur un click pour se désinscrire à une session
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idSession">ID de la Session.</param>
		public async void DesinscriptionOnClick(MouseEventArgs e, int idSession)
		{
			try
			{
				await SqlService.DeleteInscriptionAsync(idSession, UserService.UserId);

				AllSessions.RemoveAll(x => x.IdSession == idSession);

				Toaster.Add("Désinscription effectuée.", MatToastType.Success);
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur votre désinscription à la session.", MatToastType.Danger);
			}
		}

		#endregion

	}
}
