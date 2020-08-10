
namespace AccessData.Models
{
	public class Personnel
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

		public bool IsActif { get; set; }

		
		public string Login
		{
			get { return _login; }
			set { _login = value.Replace("'", "’"); }
		}
		private string _login;


		public bool IsExterne { get; set; }

	}
}
