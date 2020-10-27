using AccessData.Models;
using System.Collections.Generic;

namespace FormationApp.ViewModel
{
	public interface ICatalogueCompetence
	{
		/// <summary>
		/// Liste de toutes les compétences
		/// </summary>
		List<Competence> AllCompetences { get; set; }

		/// <summary>
		/// Lors d'une sélection d'une compétence.
		/// </summary>
		/// <param name="competence"></param>
		void ClickOnCompetence(Competence competence);
	}
}