using AccessData.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace FormationApp.ViewModel
{
	public interface ICatalogueFormation
	{
		List<CatalogueFormations> AllFormations { get; set; }
		bool DisplayDetail { get; set; }
		CatalogueFormations SelectedFormation { get; set; }

		void ClickOnFormation(CatalogueFormations formation);
		void CloseDetail(MouseEventArgs e);
		void DownloadOnClick(MouseEventArgs e, int id, string fileName);
	}
}