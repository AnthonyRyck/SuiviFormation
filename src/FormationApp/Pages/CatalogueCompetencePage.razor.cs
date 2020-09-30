using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class CatalogueCompetencePage
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		public List<Competence> AllCompetences { get; set; }

		#endregion

		#region Héritage

		protected override async Task OnInitializedAsync()
		{
			await LoadAllCompetences();
		}

		#endregion

		#region Private Methods

		private async Task LoadAllCompetences()
		{
			AllCompetences = await SqlService.GetAllCompetences();
		}

		private void ClickOnCompetence(Competence competence)
		{
			NavigationManager.NavigateTo("/competence/" + competence.IdCompetence);
		}

		#endregion
	}
}
