using AccessData;
using AccessData.Models;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class GestionFormateur : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		public List<FormateurView> AllFormateurs { get; set; }

		/// <summary>
		/// C'est le personnel qui est sélectionné pour être nouveau Formateur
		/// </summary>
		public Personnel PersonnelSelectedNouveauFormateur { get; set; }

		/// <summary>
		/// Liste des formations que le nouveau formateur peut faire.
		/// </summary>
		public List<CatalogueFormations> ListFormationsNouveauFormateur { get; set; }

		public bool AjoutPersonnel { get; set; }

		public bool AjoutFormation { get; set; }

		#endregion

		#region Constructeur

		public GestionFormateur()
		{
			ListFormationsNouveauFormateur = new List<CatalogueFormations>();
		}

		#endregion

		#region Héritage

		protected override async Task OnInitializedAsync()
		{
			await LoadAllFormateurs();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// 
		/// </summary>
		internal void AddFormateur()
		{
			if (AjoutFormation)
				AjoutFormation = false;

			AjoutPersonnel = true;
			StateHasChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		internal void AddFormations()
		{
			if(AjoutPersonnel)
				AjoutPersonnel = false;

			AjoutFormation = true;
			StateHasChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal async Task OnClickValider()
		{
			if (ListFormationsNouveauFormateur.Count == 0)
			{
				// Message d'erreur.
				Toaster.Add("Il faut une formation avant de valider.", MatToastType.Warning);
				
				return;
			}

			await SqlService.AddFormateur(PersonnelSelectedNouveauFormateur, ListFormationsNouveauFormateur);
			OnClickAnnuler();

			AjoutFormation = false;
			AjoutPersonnel = false;

			await LoadAllFormateurs();
			StateHasChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		internal void OnClickAnnuler()
		{
			PersonnelSelectedNouveauFormateur = null;
			ListFormationsNouveauFormateur = new List<CatalogueFormations>();
		}

		/// <summary>
		/// Méthode pour récupérer le personnel qui à été sélectionné.
		/// </summary>
		/// <param name="personnel"></param>
		internal void GetPersonnelAction(Personnel personnel)
		{
			if(PersonnelSelectedNouveauFormateur != null)
			{
				ListFormationsNouveauFormateur = new List<CatalogueFormations>();
			}

			PersonnelSelectedNouveauFormateur = personnel;
			StateHasChanged();
		}

		/// <summary>
		/// Méthode pour récupérer la formation sélectionné.
		/// </summary>
		/// <param name="formation"></param>
		internal void GetFormationAction(CatalogueFormations formation)
		{
			// Si formateur déjà présent dans la liste de formateur.
			if(AllFormateurs.Any(x => x.IdPersonnel == PersonnelSelectedNouveauFormateur.IdPersonnel))
			{
				FormateurView perso = AllFormateurs.FirstOrDefault(x => x.IdPersonnel == PersonnelSelectedNouveauFormateur.IdPersonnel);

				if(!perso.Formations.Any(x => x.IdFormation == formation.IdFormation))
				{
					ListFormationsNouveauFormateur.Add(formation);
				}
				else
				{
					// Message d'erreur.
					Toaster.Add("Formation déjà présente", MatToastType.Warning);
				}
			}
			else // Si non présent.
			{
				if (!ListFormationsNouveauFormateur.Any(x => x.IdFormation == formation.IdFormation))
				{
					ListFormationsNouveauFormateur.Add(formation);
				}
				else
				{
					// Message d'erreur.
					Toaster.Add("Formation déjà présente", MatToastType.Warning);
				}
			}

			StateHasChanged();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Charge tous les formateurs, avec leurs formations
		/// </summary>
		/// <returns></returns>
		private async Task LoadAllFormateurs()
		{
			AllFormateurs = await SqlService.GetAllFormateurAsync();
		}

		#endregion
	}

}
