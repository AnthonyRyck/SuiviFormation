using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class CatalogueCompetenceViewModel : ICatalogueCompetence
	{
		#region Properties

		private readonly SqlContext SqlService;

		public List<Competence> AllCompetences { get; set; }

		private readonly NavigationManager NavigationManager;

		#endregion

		#region Constructeur

		public CatalogueCompetenceViewModel(SqlContext sqlContext, NavigationManager navigationManager)
		{
			SqlService = sqlContext;
			NavigationManager = navigationManager;

			LoadAllCompetences().GetAwaiter().GetResult();
		}

		#endregion

		#region Public Methods

		public void ClickOnCompetence(Competence competence)
		{
			NavigationManager.NavigateTo("/competence/" + competence.IdCompetence);
		}

		#endregion

		#region Private Methods

		private async Task LoadAllCompetences()
		{
			AllCompetences = await SqlService.GetAllCompetences();
		}

		#endregion
	}
}
