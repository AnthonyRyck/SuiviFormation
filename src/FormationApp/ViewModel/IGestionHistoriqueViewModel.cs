using AccessData.Models;
using FormationApp.ModelsValidation;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	interface IGestionHistoriqueViewModel
	{
		Action StateHasChangedDelegate { get; set; }

		EditContext EditContextValidation { get; set; }

		bool DisplayRecherchePersonnel { get; set; }
		bool DisplayRechercheFormation { get; set; }
		bool DisplayRechercheFormateur { get; set; }

		bool DisplayRechercheSalle { get; set; }

		HistoriqueModelValidation HistoriqueModelValidation { get; set; }

		Task CreateHistoriqueSubmit();

		void CanDisplayRechercheFormation();

		void CanDisplayRecherchePersonnel();

		void CanDisplayRechercheFormateur();

		void CanDisplayRechercheSalle();


		void GetFormationAction(CatalogueFormations formation);


		void GetPersonnelAction(Personnel personnel);


		void GetSalle(Salle salle);

		void GetFormateur(FormateurView formateur);
	}
}
