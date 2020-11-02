using AccessData.Models;
using AccessData.Views;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface ICompetence
	{
		CompetenceDetailView CompetenceView { get; set; }
		bool DisplayDetail { get; set; }

		//int IdCompetence { get; set; }
		CatalogueFormations SelectedFormation { get; set; }

		Task LoadCompetence(int idCompetence);

		void ClickOnFormation(CatalogueFormations formation);
		void CloseDetail(MouseEventArgs e);
		void DownloadOnClick(MouseEventArgs e, int id, string fileName);
	}
}