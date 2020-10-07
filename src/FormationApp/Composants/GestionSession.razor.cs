using AccessData;
using AccessData.Models;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class GestionSession : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		[Inject]
		protected NavigationManager NavManager { get; set; }

		[Inject]
		protected CurrentUserService UserService { get; set; }

		/// <summary>
		/// Liste de toutes les sessions.
		/// </summary>
		public List<SessionView> AllSessions { get; set; }


		/// <summary>
		/// Indicateur d'un ajout de formation.
		/// </summary>
		public bool AjoutFormation { get; set; }

		/// <summary>
		/// Indicateur de l'ajout du formateur.
		/// </summary>
		public bool AjoutFormateur { get; set; }

		/// <summary>
		/// Indicateur de l'ajout de la salle
		/// </summary>
		public bool AjoutSalle { get; set; }


		#region Propriétés pour l'ajout d'une formation

		/// <summary>
		/// C'est la formation qui est sélectionné.
		/// </summary>
		public CatalogueFormations FormationSelected { get; set; }

		/// <summary>
		/// Date de début de la formation
		/// </summary>
		public DateTime DateFormation { get; set; }

		/// <summary>
		/// Nombre de place disponible
		/// </summary>
		public int PlaceDispo { get; set; }

		/// <summary>
		/// Salle sélectionné pour la session
		/// </summary>
		public Salle SalleSelected { get; set; }

		/// <summary>
		/// Formateur sélectionné pour la session
		/// </summary>
		public FormateurView FormateurSelected { get; set; }

		public bool AddSession { get; set; }

		#endregion

		#endregion

		#region Constructeur

		public GestionSession()
		{
			AddSession = false;
		}

		#endregion

		#region Protected Override methods

		protected async override Task OnInitializedAsync()
		{
			await LoadAllSessions();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Ajout une formation
		/// </summary>
		internal void AddFormation()
		{
			AddSession = true;

			AjoutFormateur = false;
			AjoutSalle = false;
			AjoutFormation = true;

			StateHasChanged();
		}

		/// <summary>
		/// Méthode pour récupérer la formation sélectionné.
		/// </summary>
		/// <param name="formation"></param>
		internal void GetFormationAction(CatalogueFormations formation)
		{
			FormationSelected = formation;

			StateHasChanged();
		}

		/// <summary>
		/// Méthode pour récupérer la salle sélectionné.
		/// </summary>
		/// <param name="salle"></param>
		internal void GetSalleAction(Salle salle)
		{
			SalleSelected = salle;
			StateHasChanged();
		}

		/// <summary>
		/// Ajout une formation
		/// </summary>
		internal void AddFormateur()
		{
			AjoutSalle = false;
			AjoutFormation = false;
			AjoutFormateur = true;

			StateHasChanged();
		}


		/// <summary>
		/// Ajout une formation
		/// </summary>
		internal void AddSalle()
		{
			AjoutFormateur = false;
			AjoutFormation = false;
			AjoutSalle = true;

			StateHasChanged();
		}

		/// <summary>
		/// Méthode pour récupérer la formation sélectionné.
		/// </summary>
		/// <param name="formateur"></param>
		internal void GetFormateurAction(FormateurView formateur)
		{
			FormateurSelected = formateur;

			StateHasChanged();
		}

		/// <summary>
		/// Valide la saisie d'une session.
		/// </summary>
		/// <returns></returns>
		internal async Task OnClickValider()
		{
			if (FormateurSelected == null)
			{
				// Message d'erreur.
				Toaster.Add("Il faut choisir un formateur.", MatToastType.Warning);
				return;
			}

			if (SalleSelected == null)
			{
				// Message d'erreur.
				Toaster.Add("Il faut choisir une salle.", MatToastType.Warning);
				return;
			}

			if (FormateurSelected == null)
			{
				// Message d'erreur.
				Toaster.Add("Il faut choisir un formateur.", MatToastType.Warning);
				return;
			}

			try
			{
				SqlService.AddSession(FormationSelected.IdFormation, FormateurSelected.IdPersonnel,
											SalleSelected.IdSalle, DateFormation, PlaceDispo);
			}
			catch (Exception)
			{
				Toaster.Add("Erreur sur la sauvegarde de la session.", MatToastType.Danger);
				return;
			}			

			AjoutFormateur = false;
			AjoutSalle = false;
			AjoutFormation = false;

			FormationSelected = null;
			FormateurSelected = null;
			SalleSelected = null;

			await LoadAllSessions();
			StateHasChanged();
		}



		/// <summary>
		/// 
		/// </summary>
		internal void OnClickAnnuler()
		{
			AddSession = false;

			AjoutFormateur = false;
			AjoutSalle = false;
			AjoutFormation = false;

			FormationSelected = null;
			FormateurSelected = null;
			SalleSelected = null;
		}


		internal void OpenSessionSetting(SessionView sessionSelected)
		{
			UserService.SessionView = sessionSelected;
			NavManager.NavigateTo($"detailsession/" + sessionSelected.IdSession);
		}



		#endregion

		#region Private methods

		/// <summary>
		/// Charge toutes les sessions.
		/// </summary>
		/// <returns></returns>
		private async Task LoadAllSessions()
		{
			try
			{
				AllSessions = await SqlService.GetAllSessionAsync();
			}
			catch (Exception)
			{
				Toaster.Add("Erreur sur le chargement de la table Session.", MatToastType.Danger);
			}
		}

		#endregion
	}
}
