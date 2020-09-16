using AccessData;
using AccessData.Models;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class GestionCompetence : ComponentBase
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
		/// Indicateur pour changer la vue pour ajouter une compétence.
		/// </summary>
		public bool IsAddCompetence { get; set; }

		public List<CompetenceView> CompetencesCollection { get; set; }

		public CompetenceModel competenceModel { get; set; }


		public string StyleCollectionFormation { get; set; }


		#endregion

		#region Constructeur

		public GestionCompetence()
		{
			StyleCollectionFormation = "hidden";
			CompetencesCollection = new List<CompetenceView>();
			IsAddCompetence = false;
		}

		#endregion

		#region Protected Methods

		protected override async Task OnInitializedAsync()
		{
			CompetencesCollection = await SqlService.GetAllCompetence();
		}

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		private void OpenCompetenceSetting(CompetenceView competenceSelected)
		{
			UserService.CompetenceView = competenceSelected;
			NavManager.NavigateTo($"detailcompetence");
		}

		private void CreateNewCompetence()
		{
			competenceModel = new CompetenceModel();
			IsAddCompetence = true;
		}

		/// <summary>
		/// Méthode levé quand le model est validé.
		/// </summary>
		private async void HandleValidSubmit()
		{
			if (competenceModel.ListFormations.Count == 0)
				return;

			try
			{
				// Ajoute une ligne dans Competence
				Competence nouvelleCompetence = competenceModel.ToCompetence();

				int idCompetence = await SqlService.InsertCompetence(nouvelleCompetence);
				nouvelleCompetence.IdCompetence = idCompetence;

				// Ajoute les formations pour cette compétence.
				List<CatalogueFormations> formations = competenceModel.ToFormation();
				await SqlService.InsertCompetenceFormation(idCompetence, formations);

				CompetenceView competenceView = new CompetenceView();
				competenceView.Competence = nouvelleCompetence;
				competenceView.FormationViews = competenceModel.ToFormationView();

				// Ajoute dans la collection.
				CompetencesCollection.Add(competenceView);

				// Remise à zéro de l'objet.
				competenceModel = new CompetenceModel();
			}
			catch (Exception ex)
			{
				// Message d'erreur.
				Toaster.Add("Erreur sur la sauvegarde de la compétence.", MatToastType.Danger);
			}

			IsAddCompetence = false;
			StateHasChanged();
		}

		/// <summary>
		/// Annule l'ajout
		/// </summary>
		private void AnnulationNewCompetence()
		{
			// Remise à zéro de l'objet.
			competenceModel = new CompetenceModel();
			IsAddCompetence = false;
		}


		/// <summary>
		/// Méthode pour récupérer la formation sélectionné.
		/// </summary>
		/// <param name="formation"></param>
		private void GetFormationAction(CatalogueFormations formation)
		{
			competenceModel.ListFormations.Add(formation);
			StateHasChanged();
		}

		#endregion

	}


	public class CompetenceModel
	{
		[Required(ErrorMessage = "La compétence doit avoir un titre.")]
		[StringLength(50, ErrorMessage = "Le titre est trop long, 50 caractères max")]
		public string Titre { get; set; }

		[Required(ErrorMessage = "Manque une description.")]
		public string Description { get; set; }

		public List<CatalogueFormations> ListFormations { get; set; }

		public CompetenceModel()
		{
			ListFormations = new List<CatalogueFormations>();
		}

		public Competence ToCompetence()
		{
			return new Competence()
			{
				Titre = this.Titre,
				Description = this.Description
			};
		}

		internal List<CatalogueFormations> ToFormation()
		{
			return ListFormations;
		}

		internal List<FormationView> ToFormationView()
		{
			//List<FormationView> result = new List<FormationView>(ListFormations.Count);
			return ListFormations.Select(x => new FormationView() { IdFormation = x.IdFormation, TitreFormation = x.Titre }).ToList();

			//return result;
		}
	}
}
