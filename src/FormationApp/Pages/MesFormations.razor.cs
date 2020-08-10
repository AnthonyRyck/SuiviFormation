using AccessData;
using AccessData.Models;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class MesFormations
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected SignInManager<IdentityUser> SignInManager { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		/// <summary>
		/// Liste des sessions inscrits.
		/// </summary>
		public List<SessionView> AllSessions { get; set; }

		/// <summary>
		/// Liste des sessions que l'utilisateur s'est inscrit et qui sont terminés.
		/// </summary>
		public List<SessionInscritUserView> SessionInscritUserViews { get; set; }

		public string UserId { get; private set; }

		#endregion

		#region MatDialog pour ajouter une note

		public bool DialogIsOpenNewNote = false;

		public int IdSessionNotation { get; set; }

		internal void OpenDialogNewNote(int idSession)
		{
			IdSessionNotation = idSession;
			DialogIsOpenNewNote = true;
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		internal async void OkClickNewNote()
		{
			// Sauvegarde en BDD
			await SqlService.SaveNotationFormation(IdSessionNotation, UserId, NotePourFormation, CommentairePourFormation);

			var tempSession = SessionInscritUserViews.First(x => x.IdSession == IdSessionNotation);
			tempSession.Note = NotePourFormation;
			tempSession.Commentaire = CommentairePourFormation;

			// Remise à zéro, dans le cas d'une nouvelle notation
			NotePourFormation = 0;
			CommentairePourFormation = null;

			StateHasChanged();
			DialogIsOpenNewNote = false;
		}

		/// <summary>
		/// Annule l'ajout
		/// </summary>
		internal async void AnnulerClickNewNote()
		{
			// Remise à zéro, dans le cas d'une nouvelle notation
			NotePourFormation = 0;
			CommentairePourFormation = null;

			StateHasChanged();
			DialogIsOpenNewNote = false;
		}

		public string? CommentairePourFormation { get; set; }

		public int NotePourFormation { get; set; }

		#endregion

		#region Constructeur



		#endregion

		#region Override methods

		protected async override Task OnInitializedAsync()
		{
			UserId = SignInManager.UserManager.GetUserId(SignInManager.Context.User);

			AllSessions = await SqlService.GetInscriptionSessionUserAsync(UserId);
			SessionInscritUserViews = await SqlService.GetInscriptionSessionUserFinishAsync(UserId);
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Event sur un click pour se désinscrire à une session
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idSession">ID de la Session.</param>
		public async void DesinscriptionOnClick(MouseEventArgs e, int idSession)
		{
			try
			{
				await SqlService.DeleteInscriptionAsync(idSession, UserId);

				AllSessions.RemoveAll(x => x.IdSession == idSession);

				Toaster.Add("Désinscription effectuée.", MatToastType.Success);
				StateHasChanged();
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur votre désinscription à la session.", MatToastType.Danger);
			}
		}

		#endregion

		#region Private methods



		#endregion
	}
}
