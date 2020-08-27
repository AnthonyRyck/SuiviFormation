using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	public class PersonnelInscritView
	{
		public string IdPersonnel { get; set; }

		public string Nom
		{
			get { return _nom; }
			set { _nom = value.Replace("'", "’"); }
		}
		private string _nom;

		public string Prenom
		{
			get { return _prenom; }
			set { _prenom = value.Replace("'", "’"); }
		}
		private string _prenom;


		public string Service
		{
			get { return _service; }
			set { _service = value.Replace("'", "’"); }
		}
		private string _service;

		public bool IsSessionValidate { get; set; }

		public int? Note { get; set; }

		public string? Commentaire { get; set; }
	}
}
