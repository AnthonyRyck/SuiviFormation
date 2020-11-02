using AccessData.Models;
using FormationApp.Data;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface IDetailCompetence
	{
		bool CanDisplayFormationSearch { get; set; }

		CurrentUserService UserService { get; set; }
		string DescriptionCompetence { get; set; }
		List<int> IdDeleteFormation { get; set; }
		bool IsDisabled { get; set; }
		List<CatalogueFormations> NouvelleFormations { get; set; }
		List<FormationView> TempFormationViews { get; set; }
		string TitreCompetence { get; set; }

		void AddFormationOnClick(MouseEventArgs e);
		void CancelFormationOnClick(MouseEventArgs e);
		void CancelTitreOnClick(MouseEventArgs e);
		void UpdateTitreOnClick(MouseEventArgs e);
		Task ValidateFormationsOnClick(MouseEventArgs e);
		Task ValidateTitreOnClick(MouseEventArgs e);

		void DeleteRow(FormationView currentFormation);

		void GetFormation(CatalogueFormations formation);
	}
}