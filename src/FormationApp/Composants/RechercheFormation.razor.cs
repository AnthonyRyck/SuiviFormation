using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class RechercheFormation : ComponentBase
	{

		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		/// <summary>
		/// Pour pouvoir récupérer la formation qui est sélectionné
		/// </summary>
		[Parameter]
		public EventCallback<CatalogueFormations> OnSelectFormationClick { get; set; }

		/// <summary>
		/// Résultat de la recherche sur le nom de la formation.
		/// </summary>
		public IEnumerable<CatalogueFormations> AllFormations { get; set; }

		/// <summary>
		/// Nom de la personne recherché.
		/// </summary>
		public string NomFormation
		{
			get { return _nomFormation; }
			set
			{
				_nomFormation = value;
				GetFormationByName(value);
			}
		}
		private string _nomFormation;

		#endregion

		#region Constructeur



		#endregion

		#region Internal methods



		#endregion

		#region Private methods

		/// <summary>
		/// Récupère la liste des formations par rapport à un nom de formation.
		/// </summary>
		/// <param name="nomFormation"></param>
		private async void GetFormationByName(string nomFormation)
		{
			AllFormations = await SqlService.GetFormationAsync(nomFormation);
			StateHasChanged();
		}

		#endregion
	}
}
