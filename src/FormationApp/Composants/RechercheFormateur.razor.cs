using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class RechercheFormateur : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		/// <summary>
		/// Pour pouvoir récupérer le formateur sélectionné
		/// </summary>
		[Parameter]
		public Action<FormateurView> GetFormateur { get; set; }

		/// <summary>
		/// Id formation recherché pour le formateur.
		/// </summary>
		[Parameter]
		public int IdFormation { get; set; }

		/// <summary>
		/// Nom de la formation recherchée.
		/// </summary>
		[Parameter]
		public string TitreFormation { get; set; }

		/// <summary>
		/// Collection contenant le résultat de la recherche.
		/// </summary>
		public IEnumerable<FormateurView> ResultFormateurs { get; set; }

		#endregion


		protected async override Task OnInitializedAsync()
		{
			ResultFormateurs = await SqlService.GetFormateurByFormationAsync(IdFormation);
		}


		#region Events

		/// <summary>
		/// Event levé lors de la sélection d'un formateur
		/// </summary>
		/// <param name="FormateurSelected"></param>
		public void OnFormateurDbClickedEvent(FormateurView FormateurSelected)
		{
			if (FormateurSelected != null)
			{
				GetFormateur(FormateurSelected);
			}
		}

		#endregion

	}
}
