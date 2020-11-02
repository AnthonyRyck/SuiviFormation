using AccessData;
using AccessData.Models;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormationApp.Codes.Extensions;

namespace FormationApp.ViewModel
{
	public class DetailCompetenceViewModel : IDetailCompetence
	{
		#region Properties

		SqlContext SqlService { get; set; }

		protected IMatToaster Toaster { get; set; }

		public CurrentUserService UserService { get; set; }


		public bool IsDisabled { get; set; }

		public string TitreCompetence { get; set; }

		public string DescriptionCompetence { get; set; }

		public List<FormationView> TempFormationViews { get; set; }
		public List<CatalogueFormations> NouvelleFormations { get; set; }
		public List<int> IdDeleteFormation { get; set; }

		public bool CanDisplayFormationSearch { get; set; }

		#endregion

		#region Contructeur

		public DetailCompetenceViewModel(SqlContext sqlContext, IMatToaster toaster, CurrentUserService userService)
		{
			IsDisabled = true;
			CanDisplayFormationSearch = false;

			NouvelleFormations = new List<CatalogueFormations>();
			IdDeleteFormation = new List<int>();

			SqlService = sqlContext;
			Toaster = toaster;
			UserService = userService;


			TitreCompetence = UserService.CompetenceView.Competence.Titre;
			DescriptionCompetence = UserService.CompetenceView.Competence.Description;
			TempFormationViews = UserService.CompetenceView.FormationViews.ToList();
		}


		//protected override void OnInitialized()
		//{
		//	TitreCompetence = UserService.CompetenceView.Competence.Titre;
		//	DescriptionCompetence = UserService.CompetenceView.Competence.Description;
		//	TempFormationViews = UserService.CompetenceView.FormationViews.ToList();
		//}

		#endregion

		#region Public Methods

		#region Modif sur Competence

		/// <summary>
		/// Donne le droit de modifier.
		/// </summary>
		/// <param name="e"></param>
		public void UpdateTitreOnClick(MouseEventArgs e)
		{
			IsDisabled = false;
		}

		/// <summary>
		/// Cancel la modification
		/// </summary>
		/// <param name="e"></param>
		public void CancelTitreOnClick(MouseEventArgs e)
		{
			// Remise de la valeur.
			IsDisabled = true;
			TitreCompetence = UserService.CompetenceView.Competence.Titre;
			DescriptionCompetence = UserService.CompetenceView.Competence.Description;
		}

		/// <summary>
		/// Valide les modifications sur Titre et decription
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public async Task ValidateTitreOnClick(MouseEventArgs e)
		{
			try
			{
				await SqlService.UpdateCompetenceTitreDescription(UserService.CompetenceView.Competence.IdCompetence, TitreCompetence, DescriptionCompetence);
				Toaster.Add("Sauvegarde OK.", MatToastType.Success);

				UserService.CompetenceView.Competence.Titre = TitreCompetence;
				UserService.CompetenceView.Competence.Description = DescriptionCompetence;
			}
			catch (Exception)
			{
				TitreCompetence = UserService.CompetenceView.Competence.Titre;
				DescriptionCompetence = UserService.CompetenceView.Competence.Description;

				// Message d'erreur.
				Toaster.Add("Erreur sur la sauvegarde.", MatToastType.Danger);
			}

			IsDisabled = true;
		}

		#endregion

		#region Modif sur formations

		/// <summary>
		/// Affiche le menu de recherche de formation.
		/// </summary>
		/// <param name="e"></param>
		public void AddFormationOnClick(MouseEventArgs e)
		{
			CanDisplayFormationSearch = true;
		}

		public void GetFormation(CatalogueFormations formation)
		{
			// Vérifier pour ne pas ajouter 2 fois la même formation.
			if (TempFormationViews.Any(x => x.IdFormation == formation.IdFormation))
			{
				Toaster.Add("Cette formation est déjà dans la liste.", MatToastType.Warning);
			}
			else
			{
				TempFormationViews.Add(formation.ToFormationView());
				NouvelleFormations.Add(formation);
			}
		}

		/// <summary>
		/// Valide les modifications sur la liste de formation
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public async Task ValidateFormationsOnClick(MouseEventArgs e)
		{
			try
			{
				await SqlService.InsertCompetenceFormation(UserService.CompetenceView.Competence.IdCompetence, NouvelleFormations);
				UserService.CompetenceView.FormationViews.AddRange(NouvelleFormations.Select(x => x.ToFormationView()));

				Toaster.Add("Sauvegarde OK.", MatToastType.Success);
			}
			catch (Exception)
			{
				TempFormationViews = UserService.CompetenceView.FormationViews.ToList();
				Toaster.Add("Erreur sur la sauvegarde.", MatToastType.Danger);
			}

			CanDisplayFormationSearch = false;
		}

		/// <summary>
		/// Supprime une ligne
		/// </summary>
		/// <param name="currentFormation"></param>
		public async void DeleteRow(FormationView currentFormation)
		{
			try
			{
				if (TempFormationViews.Count == 1)
				{
					Toaster.Add("Il doit avoir au moins 1 compétence.", MatToastType.Danger);
				}
				else
				{
					await SqlService.DeleteCompetenceFormation(UserService.CompetenceView.Competence.IdCompetence, currentFormation.IdFormation);
					TempFormationViews.Remove(currentFormation);
					NouvelleFormations.RemoveAll(x => x.IdFormation == currentFormation.IdFormation);
					UserService.CompetenceView.FormationViews.Remove(currentFormation);
				}
			}
			catch (Exception ex)
			{
				TempFormationViews = UserService.CompetenceView.FormationViews.ToList();
				Toaster.Add("Erreur sur la sauvegarde.", MatToastType.Danger);
			}

			//SallesViewGrid.Reload();
		}

		/// <summary>
		/// Cancel la modification
		/// </summary>
		/// <param name="e"></param>
		public void CancelFormationOnClick(MouseEventArgs e)
		{
			// Remise de la valeur.
			CanDisplayFormationSearch = false;
			TempFormationViews = UserService.CompetenceView.FormationViews.ToList();
		}

		#endregion

		#endregion
	}
}
