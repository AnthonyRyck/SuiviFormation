using AccessData;
using AccessData.Models;
using FormationApp.ModelsValidation;
using MatBlazor;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class GestionHistoriqueViewModel : IGestionHistoriqueViewModel
	{
		public HistoriqueModelValidation HistoriqueModelValidation { get; set; }

		public EditContext EditContextValidation { get; set; }

		public bool DisplayRechercheFormation { get; set; }

		public bool DisplayRecherchePersonnel { get; set; }

		public bool DisplayRechercheSalle { get; set; }

		public bool DisplayRechercheFormateur { get; set; }


		private IMatToaster Toaster;
		private SqlContext SqlContext;

		public GestionHistoriqueViewModel(IMatToaster toaster, SqlContext sqlContext)
		{
			HistoriqueModelValidation = new HistoriqueModelValidation();
			EditContextValidation = new EditContext(HistoriqueModelValidation);

			Toaster = toaster;
			SqlContext = sqlContext;
		}

		public Action StateHasChangedDelegate { get; set; }


		public async Task CreateHistoriqueSubmit()
		{
			try
			{
				if (EditContextValidation.Validate())
				{
					// Création de la session
					int idSession = await SqlContext.CreateSessionHistorique(HistoriqueModelValidation.Formation.IdFormation,
												HistoriqueModelValidation.Formateur.IdPersonnel,
												HistoriqueModelValidation.Salle.IdSalle,
												HistoriqueModelValidation.DateDeSession.Value);
					// Nombre de place mis à zéro car historique.

					// Ajout des personnels dans la session.
					foreach (var personnel in HistoriqueModelValidation.PersonnelsInscrit)
					{
						await SqlContext.InsertInscriptionAsync(idSession, personnel.IdPersonnel);
					}

					HistoriqueModelValidation = new HistoriqueModelValidation();

					Toaster.Add("Création de la session historique avec succés", MatToastType.Success);
				}
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur sur la création de la session historique", MatToastType.Danger);
			}
		}

		public void CanDisplayRechercheFormation()
		{
			DisplayRecherchePersonnel = false;
			DisplayRechercheFormateur = false;
			DisplayRechercheSalle = false;
			DisplayRechercheFormation = true;
		}

		public void CanDisplayRecherchePersonnel()
		{
			DisplayRechercheFormation = false;
			DisplayRechercheFormateur = false;
			DisplayRechercheSalle = false;
			DisplayRecherchePersonnel = true;
		}

		public void CanDisplayRechercheFormateur()
		{
			DisplayRechercheFormation = false;
			DisplayRechercheSalle = false;
			DisplayRecherchePersonnel = false;
			DisplayRechercheFormateur = true;
		}

		public void CanDisplayRechercheSalle()
		{
			DisplayRechercheFormation = false;
			DisplayRecherchePersonnel = false;
			DisplayRechercheFormateur = false;
			DisplayRechercheSalle = true;
		}


		public void GetFormationAction(CatalogueFormations formation)
		{
			HistoriqueModelValidation.Formation = formation;
		}


		public void GetPersonnelAction(Personnel personnel)
		{
			if(HistoriqueModelValidation.PersonnelsInscrit == null)
			{
				HistoriqueModelValidation.PersonnelsInscrit = new List<Personnel>();
			}
			
			if(!HistoriqueModelValidation.PersonnelsInscrit.Contains(personnel))
			{
				HistoriqueModelValidation.PersonnelsInscrit.Add(personnel);
				StateHasChangedDelegate.Invoke();
			}
			else
			{
				Toaster.Add("Personnel déjà dans la liste.", MatToastType.Warning);
			}
		}


		public void GetSalle(Salle salle)
		{
			HistoriqueModelValidation.Salle = salle;
			StateHasChangedDelegate.Invoke();
		}

		public void GetFormateur(FormateurView formateur)
		{
			HistoriqueModelValidation.Formateur = formateur;
			StateHasChangedDelegate.Invoke();
		}
	}
}
