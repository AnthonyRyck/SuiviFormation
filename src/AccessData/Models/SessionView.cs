using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	/// <summary>
	/// Pour la vue GestionSession
	/// </summary>
	public class SessionView
	{
		public int IdSession { get; set; }

		public DateTime DateDebutSession { get; set; }

		/// <summary>
		/// Nombre de place totale pour cette session
		/// </summary>
		public int NombreDePlaceDispo { get; set; }

		public int NombreDeJour { get; set; }

		#region Lié au catalogue de formation

		public int IdFormation { get; set; }

		public string TitreFormation { get; set; }

		#endregion

		#region Lié à la salle

		public int IdSalle { get; set; }

		public string NomDeLaSalle { get; set; }

		#endregion

		#region Lié au formateur

		public string IdFormateur { get; set; }

		public string Nom { get; set; }

		public string Prenom { get; set; }

		public bool EstExterne { get; set; }

		#endregion

		public SessionView()
		{
			UsersInscrits = new List<string>();
		}

		public List<string> UsersInscrits { get; set; }
	}
}
