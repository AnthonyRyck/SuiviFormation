using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class RechercheSalle : ComponentBase
	{

		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		/// <summary>
		/// Pour pouvoir récupérer la salle qui est sélectionné
		/// </summary>
		[Parameter]
		public Action<Salle> GetSalle { get; set; }

		/// <summary>
		/// Liste de toutes les salles
		/// </summary>
		public IEnumerable<Salle> AllSalles { get; set; }

		/// <summary>
		/// Nom de la salle recherché.
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

		#region Constructeur



		#endregion

		#region Events

		/// <summary>
		/// Event levé lors de la sélection d'une salle
		/// </summary>
		/// <param name="salleSelected"></param>
		public void OnSalleDbClickedEvent(Salle salleSelected)
		{
			if (salleSelected != null)
			{
				GetSalle(salleSelected);
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Récupére la liste des salles par rapport à un bout de nom.
		/// </summary>
		/// <param name="name"></param>
		private async void GetName(string name)
		{
			AllSalles = await SqlService.GetSalle(name);
			StateHasChanged();
		}

		#endregion
	}
}
