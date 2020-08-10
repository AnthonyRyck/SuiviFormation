using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class RecherchePersonnel : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		/// <summary>
		/// Pour pouvoir récupérer le personnel qui est sélectionné
		/// </summary>
		[Parameter]
		public Action<Personnel> GetPersonnel { get; set; }

		/// <summary>
		/// Résultat de la recherche sur le nom du personnel.
		/// </summary>
		public IEnumerable<Personnel> AllPersonnels { get; set; }

		/// <summary>
		/// Nom de la personne recherché.
		/// </summary>
		public string Nom
		{
			get { return _nom; }
			set 
			{
				_nom = value;
				GetName(value);
			}
		}
		private string _nom;

		#endregion

		#region Contructeur

		public RecherchePersonnel()
		{
			AllPersonnels = new List<Personnel>();
		}

		#endregion

		#region Internal methods



		#endregion

		#region Events

		/// <summary>
		/// Event levé lors de la sélection d'un personnel
		/// </summary>
		/// <param name="persoSelected"></param>
		public void OnPersonnelDbClickedEvent(Personnel persoSelected)
		{
			if (persoSelected != null)
			{
				GetPersonnel(persoSelected);
			}			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Récupére la liste des Personnels par rapport à un bout de nom.
		/// </summary>
		/// <param name="name"></param>
		private async void GetName(string name)
		{
			AllPersonnels = await SqlService.GetPersonnel(name);
			StateHasChanged();
		}

		#endregion

	}
}
