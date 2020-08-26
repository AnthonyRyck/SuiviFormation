using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AccessData.Models;
using MySql.Data.MySqlClient;

namespace AccessData
{ 
    public class SqlContext
	{
		public string ConnectionString { get; set; }

		public SqlContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

        #region Session

        public async Task<List<SessionView>> GetAllSessionAsync()
        {
            List<SessionView> listSession = new List<SessionView>();

			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, sesion.NombreDeJour, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            listSession.Add(new SessionView()
                            {
                                IdFormateur = await reader.GetFieldValueAsync<string>(0),
                                Nom = await reader.GetFieldValueAsync<string>(1),
                                Prenom = await reader.GetFieldValueAsync<string>(2),
                                EstExterne = reader.GetBoolean(3),
                                TitreFormation = await reader.GetFieldValueAsync<string>(4),
                                IdSession = await reader.GetFieldValueAsync<int>(5),
                                NombreDeJour = await reader.GetFieldValueAsync<int>(6),
                                DateDebutSession = await reader.GetFieldValueAsync<DateTime>(7),
                                IdSalle = await reader.GetFieldValueAsync<int>(8),
                                NomDeLaSalle = await reader.GetFieldValueAsync<string>(9),
                                NombreDePlaceDispo = await reader.GetFieldValueAsync<int>(10)
                            }) ;
                        }
                    }

                }
            }
			catch (Exception)
			{
				throw;
			}            

            return listSession;
        }

		/// <summary>
		/// Récupère toutes les sessions qui sont encore ouvertes
		/// </summary>
		/// <returns></returns>
		public async Task<List<SessionView>> GetAllOpenSessionAsync()
        {
            List<SessionView> listSession = new List<SessionView>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, sesion.NombreDeJour, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle"
                                        + " WHERE sesion.DateSession > NOW();";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            listSession.Add(new SessionView()
                            {
                                IdFormateur = await reader.GetFieldValueAsync<string>(0),
                                Nom = await reader.GetFieldValueAsync<string>(1),
                                Prenom = await reader.GetFieldValueAsync<string>(2),
                                EstExterne = reader.GetBoolean(3),
                                TitreFormation = await reader.GetFieldValueAsync<string>(4),
                                IdSession = await reader.GetFieldValueAsync<int>(5),
                                NombreDeJour = await reader.GetFieldValueAsync<int>(6),
                                DateDebutSession = await reader.GetFieldValueAsync<DateTime>(7),
                                IdSalle = await reader.GetFieldValueAsync<int>(8),
                                NomDeLaSalle = await reader.GetFieldValueAsync<string>(9),
                                NombreDePlaceDispo = await reader.GetFieldValueAsync<int>(10)
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return listSession;
        }

        /// <summary>
		/// Récupère toutes les sessions qui sont encore ouvertes
		/// </summary>
		/// <returns></returns>
		public async Task<List<DataSession>> GetAllOpenSessionWithDateAsync()
        {
            List<DataSession> listSession = new List<DataSession>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT catalogue.Titre, sesion.NombreDeJour, sesion.DateSession"
                                    + " FROM sessionformation sesion"
                                    + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                    + " WHERE sesion.DateSession > NOW();";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            int nbreJour = await reader.GetFieldValueAsync<int>(1);
                            DateTime dateStart = await reader.GetFieldValueAsync<DateTime>(2);
                            DateTime dateEnd = dateStart.AddDays(nbreJour);
                            
                            var temp = new DataSession()
                            {
                                Text = await reader.GetFieldValueAsync<string>(0),
                                Start = dateStart,
                                End = dateEnd
                            };

                            listSession.Add(temp);
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return listSession;
        }

        /// <summary>
        /// Ajout une session en base de donné.
        /// </summary>
        /// <param name="idFormation"></param>
        /// <param name="idFormateur"></param>
        /// <param name="idSalle"></param>
        /// <param name="dateFormation"></param>
        /// <param name="nombreJour"></param>
        /// <param name="nbrePlace"></param>
        /// <returns></returns>
        public void AddSession(int idFormation, string idFormateur, int idSalle, DateTime dateFormation, int nombreJour, int nbrePlace)
        {
			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO sessionformation (IdFormateur, IdFormation, IdSalle, DateSession, NombreDeJour, PlaceDispo)"
                                + " VALUES (@idFormateur, @idFormation, @idSalle, @dateSession, @nombreJour, @placeDispo);";


                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@idFormateur", idFormateur);
                        cmd.Parameters.AddWithValue("@idFormation", idFormation);
                        cmd.Parameters.AddWithValue("@idSalle", idSalle);
                        cmd.Parameters.AddWithValue("@dateSession", dateFormation);
                        cmd.Parameters.AddWithValue("@nombreJour", nombreJour);
                        cmd.Parameters.AddWithValue("@placeDispo", nbrePlace);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
			catch (Exception ex)
			{
                throw;
			}
        }

        /// <summary>
        /// Ajoute le fichier d'emargement pour la session.
        /// </summary>
        /// <param name="idSession"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task AddEmargementFile(int idSession, byte[] file, string fileName)
		{
            string cmdUpdate = @"UPDATE sessionformation SET emargement=@fichier, filename=@name"
                                + $" WHERE IdSession=@idSession";

            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand(cmdUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@idSession", idSession);
                    cmd.Parameters.AddWithValue("@fichier", file);
                    cmd.Parameters.AddWithValue("@name", fileName);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Récupère le nom du fichier d'émargement.
        /// Si null, c'est qu'il n'y a pas de fichier.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<Session> GetFileNameEmargementAsync(int idSession)
        {
            string commandText = $"SELECT IdSession, FileName FROM sessionformation WHERE IdSession={idSession}";

            Func<MySqlCommand, Session> funcCmd = (cmd) =>
            {
                Session session = new Session();
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        session.IdFormation = reader.GetInt32(0);
                        session.NomFichierEmargement = ConvertFromDBVal<string?>(reader.GetValue(1));
                    }
                }

                return session;
            };

            Session session = new Session();

            try
			{
                session = await GetCoreAsync(commandText, funcCmd);
            }
			catch (Exception ex)
			{

			}

            return session;
        }

        #endregion

        #region Formations

        /// <summary>
        /// Récupère toutes les formations.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CatalogueFormations>> GetAllFormationsAsync()
        {
            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, DateDeFin, NomDeFichier, EstInterne FROM catalogueformation;";

            Func<MySqlCommand, List<CatalogueFormations>> funcCmd = (cmd) =>
            {
                List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object tempDate = reader.GetValue(3);
                        DateTime? tempDateFin = ConvertFromDBVal<DateTime?>(tempDate);

                        listFormations.Add(new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateDeFin = tempDateFin,
                            NomDuFichier = reader.GetString(4),
                            EstInterne = reader.GetBoolean(5)
                        });
                    }
                }

                return listFormations;
            };

            List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

            try
            {
                listFormations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return listFormations;
        }

        /// <summary>
        /// Récupère les formations qui ne sont pas fermé à la date donnée.
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public async Task<List<CatalogueFormations>> GetAllFormationsEncoreValideAsync()
        {
            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, NomDeFichier, EstInterne FROM catalogueformation"
                            + " WHERE DateDeFin IS NULL"
                            + " OR DateDeFin > curdate();";

            Func<MySqlCommand, List<CatalogueFormations>> funcCmd = (cmd) =>
            {
                List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listFormations.Add(new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            NomDuFichier = reader.GetString(3),
                            EstInterne = reader.GetBoolean(4)
                        });
                    }
                }
                return listFormations;
            };

            List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

            try
            {
                listFormations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return listFormations;
        }

        /// <summary>
        /// Récupère une formation par rapport à son ID.
        /// </summary>
        /// <param name="idFormation"></param>
        /// <returns></returns>
		public async Task<CatalogueFormations> GetFormationAsync(int idFormation)
		{
            CatalogueFormations formation = new CatalogueFormations();

            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, DateDeFin, NomDeFichier, EstInterne FROM catalogueformation"
                                       + $" WHERE IdFormation={idFormation};";

            Func<MySqlCommand, CatalogueFormations> funcCmd = (cmd) =>
            {
                CatalogueFormations formation = new CatalogueFormations();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object tempDate = reader.GetValue(3);
                        DateTime? tempDateFin = ConvertFromDBVal<DateTime?>(tempDate);

                        formation = new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateDeFin = tempDateFin,
                            NomDuFichier = reader.GetString(4),
                            EstInterne = reader.GetBoolean(5)
                        };
                    }
                }

                return formation;
            };

            try
            {
                formation = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return formation;
        }

        /// <summary>
        /// Récupère les formations par rapport à un titre de formation.
        /// </summary>
        /// <param name="nomFormation"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CatalogueFormations>> GetFormationAsync(string nomFormation)
        {
            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, EstInterne FROM catalogueformation"
                            + $" WHERE Titre LIKE '%{nomFormation}%';";

            Func<MySqlCommand, List<CatalogueFormations>> funcCmd = (cmd) =>
            {
                List<CatalogueFormations> formations = new List<CatalogueFormations>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var formation = new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            EstInterne = reader.GetBoolean(3)
                        };

                        formations.Add(formation);
                    }
                }

                return formations;
            };

            List<CatalogueFormations> formations = new List<CatalogueFormations>();

            try
            {
                formations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return formations;
        }

        /// <summary>
        /// Supprime la formation du catalogue.
        /// </summary>
        /// <param name="currentFormation"></param>
        /// <returns></returns>
        public async Task DeleteFormation(CatalogueFormations currentFormation)
		{
            string commandDelete = $"DELETE FROM catalogueformation WHERE IdFormation={currentFormation.IdFormation}";

            await ExecuteCoreAsync(commandDelete);
        }

        /// <summary>
        /// Insert une nouvelle formation
        /// </summary>
        /// <param name="nouvelleFormation"></param>
        /// <returns></returns>
		public async Task InsertFormation(CatalogueFormations nouvelleFormation)
		{
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand("INSERT INTO catalogueformation (Titre, DescriptionFormation, DateDeFin, FichierContenu, NomDeFichier, EstInterne)"
                                                + " VALUES (@Titre, @Description, NULL, @ContenuFormationN, @NomDuFichier, @EstInterne);"
                , conn))
                {
                    cmd.Parameters.AddWithValue("@Titre", nouvelleFormation.Titre);
                    cmd.Parameters.AddWithValue("@Description", nouvelleFormation.Description);
                    cmd.Parameters.AddWithValue("@ContenuFormationN", nouvelleFormation.ContenuFormationN);
                    cmd.Parameters.AddWithValue("@NomDuFichier", nouvelleFormation.NomDuFichier);
                    cmd.Parameters.AddWithValue("@EstInterne", nouvelleFormation.EstInterne);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public async Task UpdateFormationAsync(CatalogueFormations currentFormation)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                string dateFin;
				if (currentFormation.DateDeFin.HasValue)
					dateFin = currentFormation.DateDeFin.Value.ToString("yyyy-MM-dd");
				else
					dateFin = null;


				var commandUpdateFormation = @$"UPDATE catalogueformation SET Titre=@Titre, DescriptionFormation=@Description, DateDeFin=@dateFin, FichierContenu=@ContenuFormationN, NomDeFichier=@NomDuFichier, EstInterne=@EstInterne"
                                         + $" WHERE IdFormation=@IdFormation;";

                using (var cmd = new MySqlCommand(commandUpdateFormation, conn))
                {
                    cmd.Parameters.AddWithValue("@Titre", currentFormation.Titre);
                    cmd.Parameters.AddWithValue("@Description", currentFormation.Description);
                    cmd.Parameters.AddWithValue("@ContenuFormationN", currentFormation.ContenuFormationN);
                    cmd.Parameters.AddWithValue("@NomDuFichier", currentFormation.NomDuFichier);
                    cmd.Parameters.AddWithValue("@IdFormation", currentFormation.IdFormation);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);
                    cmd.Parameters.AddWithValue("@EstInterne", currentFormation.EstInterne);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public async Task UpdateFormatioWithoutFilenAsync(CatalogueFormations currentFormation)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                string dateFin;
                if (currentFormation.DateDeFin.HasValue)
                    dateFin = currentFormation.DateDeFin.Value.ToString("yyyy-MM-dd");
                else
                    dateFin = null;


                var commandUpdateFormation = @$"UPDATE catalogueformation SET Titre=@Titre, DescriptionFormation=@Description, DateDeFin=@dateFin, NomDeFichier=@NomDuFichier, EstInterne=@EstInterne"
                                         + $" WHERE IdFormation=@IdFormation;";

                using (var cmd = new MySqlCommand(commandUpdateFormation, conn))
                {
                    cmd.Parameters.AddWithValue("@Titre", currentFormation.Titre);
                    cmd.Parameters.AddWithValue("@Description", currentFormation.Description);
                    cmd.Parameters.AddWithValue("@NomDuFichier", currentFormation.NomDuFichier);
                    cmd.Parameters.AddWithValue("@IdFormation", currentFormation.IdFormation);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);
                    cmd.Parameters.AddWithValue("@EstInterne", currentFormation.EstInterne);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Récupère le fichier en BDD pour l'ID donnée.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<byte[]> GetFormationFileAsync(int id)
        {
            var commandText = @"SELECT FichierContenu FROM catalogueformation"
                                        + $" WHERE IdFormation={id};";

            return await GetBytesCore(commandText);
        }

        /// <summary>
        /// Récupère le fichier d'émergement pour une session donnée.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<byte[]> GetEmargementFileAsync(int idSession)
        {
            var commandText = @"SELECT Emargement FROM sessionformation"
                                        + $" WHERE IdSession={idSession};";
            return await GetBytesCore(commandText);
        }

        #endregion

        #region Salle

        /// <summary>
        /// Récupère toutes les salles.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Salle>> GetAllSalleAsync()
        {
            List<Salle> listSalle = new List<Salle>();

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var commandText = @"SELECT IdSalle, NomSalle, Description, NombrePlace FROM Salle;";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        listSalle.Add(new Salle()
                        {
                            IdSalle = await reader.GetFieldValueAsync<int>(0),
                            NomSalle = await reader.GetFieldValueAsync<string>(1),
                            Description = await reader.GetFieldValueAsync<string>(2),
                            NombreDePlace = await reader.GetFieldValueAsync<int>(3)
                        });
                    }
                }

            }

            return listSalle;
        }

        /// <summary>
        /// Supprime une salle
        /// </summary>
        /// <param name="currentSalle"></param>
        /// <returns></returns>
		public async Task DeleteSalle(Salle currentSalle)
		{
            string commandDelete = $"DELETE FROM salle WHERE IdSalle={currentSalle.IdSalle}";
            
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandDelete, conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Ajoute une nouvelle salle
        /// </summary>
        /// <param name="nouvelleSalle"></param>
        /// <returns></returns>
		public async Task InsertSalle(Salle salle)
		{
            string commandInsert = "INSERT INTO salle (NomSalle, Description, NombrePlace) " 
                + $" VALUES ('{salle.NomSalle}', '{salle.Description}', {salle.NombreDePlace}); ";

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Met à jour les informations d'une salle.
        /// </summary>
        /// <param name="currentSalle"></param>
        /// <returns></returns>
        public async Task UpdateSalleAsync(Salle currentSalle)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var commandUpdateSalle = @$"UPDATE salle SET NomSalle='{currentSalle.NomSalle}', Description='{currentSalle.Description}', NombrePlace={currentSalle.NombreDePlace}"
                                         + $" WHERE IdSalle={currentSalle.IdSalle};";
                MySqlCommand cmd = new MySqlCommand(commandUpdateSalle, conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Récupère une salle par rapport à son ID.
        /// </summary>
        /// <param name="idSalle"></param>
        /// <returns></returns>
        public async Task<Salle> GetSalle(int idSalle)
        {
            Salle salle = null;

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var commandText = $@"SELECT IdSalle, NomSalle, Description, NombrePlace FROM Salle WHERE IdSalle={idSalle};";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        salle = new Salle()
                        {
                            IdSalle = await reader.GetFieldValueAsync<int>(0),
                            NomSalle = await reader.GetFieldValueAsync<string>(1),
                            Description = await reader.GetFieldValueAsync<string>(2),
                            NombreDePlace = await reader.GetFieldValueAsync<int>(3)
                        };
                    }
                }
            }

            return salle;
        }

        /// <summary>
        /// Récupère une salle par rapport à son nom.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Salle>> GetSalle(string name)
        {
            List<Salle> salles = new List<Salle>();

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var commandText = $@"SELECT IdSalle, NomSalle, Description, NombrePlace "
                    + "FROM salle "
                    + $"WHERE NomSalle LIKE '%{name}%';";

                MySqlCommand cmd = new MySqlCommand(commandText, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        Salle salle = new Salle()
                        {
                            IdSalle = await reader.GetFieldValueAsync<int>(0),
                            NomSalle = await reader.GetFieldValueAsync<string>(1),
                            Description = await reader.GetFieldValueAsync<string>(2),
                            NombreDePlace = await reader.GetFieldValueAsync<int>(3)
                        };
                        
                        salles.Add(salle);
                    }
                }
            }

            return salles;
        }

        #endregion

        #region Personnel

        /// <summary>
        /// Ajout un nouveau Personnel dans la table.
        /// </summary>
        /// <param name="personnel"></param>
        /// <returns></returns>
        public async Task AddPersonnel(Personnel personnel)
        {
            try
            {
                string commandInsert = "INSERT INTO personnel (IdPersonnel, Nom, Prenom, Service, IsActif, Login, IsExterne) "
                + $"VALUES ('{personnel.IdPersonnel}', '{personnel.Nom}','{personnel.Prenom}','{personnel.Service}',{personnel.IsActif},'{personnel.Login}',{personnel.IsExterne});";

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Retourne la liste de tous les personnels
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Personnel>> GetAllPersonnel()
		{
            try
            {
                string commandInsert = "SELECT IdPersonnel, Nom, Prenom, Service, IsActif, Login, IsExterne "
                + "FROM personnel;";

                List<Personnel> personnels = new List<Personnel>();

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            personnels.Add(new Personnel()
                            {
                                IdPersonnel = reader.GetString(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Service = reader.GetString(3),
                                IsActif = reader.GetBoolean(4),
                                Login = reader.GetString(5),
                                IsExterne = reader.GetBoolean(6)
                            });
                        }
                    }
                }

                return personnels;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Retourne une liste de personnel avec le nom donnée.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Personnel>> GetPersonnel(string name)
        {
            try
            {
                string commandInsert = "SELECT IdPersonnel, Nom, Prenom, Service, IsActif, Login, IsExterne "
                + "FROM personnel"
                + $" WHERE Nom like '%{name}%';";

                List<Personnel> personnels = new List<Personnel>();

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            personnels.Add(new Personnel()
                            {
                                IdPersonnel = reader.GetString(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Service = reader.GetString(3),
                                IsActif = reader.GetBoolean(4),
                                Login = reader.GetString(5),
                                IsExterne = reader.GetBoolean(6)
                            });
                        }
                    }
                }

                return personnels;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Formateurs

        public async Task<List<FormateurView>> GetAllFormateurAsync()
        {
            List<FormateurView> listFormateurs = new List<FormateurView>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    var commandText = "SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.Service, pers.IsExterne, catalogue.IdFormation, catalogue.Titre, pers.Login"
                        + " FROM formateur forma"
                        + " INNER JOIN personnel pers ON pers.IdPersonnel = forma.IdPersonnel"
                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = forma.IdFormation;";

                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            var formateur = new FormateurView()
                            {
                                IdPersonnel = reader.GetString(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Service = reader.GetString(3),
                                EstExterne = reader.GetBoolean(4),
                                Login = reader.GetString(7)
                            };

                            // Si contient déjà le formateur, il faut ajouter la formation.
                            if(listFormateurs.Any(x => x.IdPersonnel == formateur.IdPersonnel))
							{
                                var formation = new FormationView()
                                {
                                    IdFormation = reader.GetInt32(5),
                                    TitreFormation = reader.GetString(6)
                                };

                                // Récupération du formateur dans la liste pour ajouter cette formation dans sa liste.
                                FormateurView refFormateur = listFormateurs.FirstOrDefault(x => x.IdPersonnel == formateur.IdPersonnel);
                                refFormateur.Formations.Add(new FormationView()
                                    {
                                       IdFormation = reader.GetInt32(5),
                                       TitreFormation = reader.GetString(6)
                                   });
                            }
							else
							{
                                formateur.Formations = new List<FormationView>()
                                {
                                   new FormationView()
								   {
                                       IdFormation = reader.GetInt32(5),
                                       TitreFormation = reader.GetString(6)
								   }
                                };

                                listFormateurs.Add(formateur);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return listFormateurs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formateur"></param>
        /// <param name="formations"></param>
        /// <returns></returns>
		public async Task AddFormateur(Personnel formateur, List<CatalogueFormations> formations)
        {
            try
            {
                string commandInsert = "INSERT INTO formateur (IdPersonnel, IdFormation, EstEncoreFormateur) VALUES ";

                int maxLine = formations.Count();
                for (int i = 0; i < maxLine; i++)
				{
                    commandInsert += $"('{formateur.IdPersonnel}', {formations[i].IdFormation}, 1)";

                    if (i < (maxLine - 1))
                        commandInsert += ", ";
                }

                commandInsert += ";";
                

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Récupère une liste de formateurs qui peuvent faire cette formation en paramètre.
        /// </summary>
        /// <param name="idFormation"></param>
        /// <returns></returns>
        public async Task<List<FormateurView>> GetFormateurByFormationAsync(int idFormation)
        {
            List<FormateurView> listFormateurs = new List<FormateurView>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    var commandText = "SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.Service, pers.IsExterne"
                        + " FROM formateur forma"
                        + " INNER JOIN personnel pers ON pers.IdPersonnel = forma.IdPersonnel"
                       + $" WHERE forma.IdFormation = {idFormation};";

                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            var formateur = new FormateurView()
                            {
                                IdPersonnel = reader.GetString(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Service = reader.GetString(3),
                                EstExterne = reader.GetBoolean(4)
                            };

                            listFormateurs.Add(formateur);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return listFormateurs;
        }

		#endregion

		#region Inscription

        /// <summary>
        /// Récupère la liste des inscrits pour une session donnée.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<IEnumerable<InscriptionSession>> GetInscriptionAsync(int idSession)
		{
            List<InscriptionSession> listInscription = new List<InscriptionSession>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT IdSession, IdPersonnel, IsSessionValidate" 
                                    + " FROM inscriptionformation"
                                    + $" WHERE IdSession={idSession}";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            listInscription.Add(new InscriptionSession()
                            {
                                IdSession = await reader.GetFieldValueAsync<int>(0),
                                IdPersonnel = await reader.GetFieldValueAsync<string>(1),
                                IsSessionValidate = reader.GetBoolean(2)
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return listInscription;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSession"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task InsertInscriptionAsync(int idSession, string userId)
		{
            string commandInsert = "INSERT INTO inscriptionformation (IdSession, IdPersonnel, IsSessionValidate) "
                + $" VALUES ('{idSession}', '{userId}', 0); ";

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandInsert, conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Commande pour supprimer un utilisateur d'une inscription.
        /// </summary>
        /// <param name="idSession"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task DeleteInscriptionAsync(int idSession, string userId)
        {
            string commandDelete = $"DELETE FROM inscriptionformation"
                                   + $" WHERE IdSession={idSession} AND IdPersonnel='{userId}';";

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandDelete, conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Récupère toutes les sessions de l'utilisateur en paramètre qui sont encore ouverte
        /// </summary>
        /// <param name="idUser">ID de l'utilisateur.</param>
        /// <returns></returns>
        public async Task<List<SessionView>> GetInscriptionSessionUserAsync(string idUser)
        {
            List<SessionView> listSession = new List<SessionView>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, sesion.NombreDeJour, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle"
                                        + " INNER JOIN inscriptionformation inscrit ON inscrit.IdSession = sesion.IdSession"
                                        + " WHERE sesion.DateSession > NOW()"
                                        + $" AND inscrit.IdPersonnel = '{idUser}';";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            listSession.Add(new SessionView()
                            {
                                IdFormateur = await reader.GetFieldValueAsync<string>(0),
                                Nom = await reader.GetFieldValueAsync<string>(1),
                                Prenom = await reader.GetFieldValueAsync<string>(2),
                                EstExterne = reader.GetBoolean(3),
                                TitreFormation = await reader.GetFieldValueAsync<string>(4),
                                IdSession = await reader.GetFieldValueAsync<int>(5),
                                NombreDeJour = await reader.GetFieldValueAsync<int>(6),
                                DateDebutSession = await reader.GetFieldValueAsync<DateTime>(7),
                                IdSalle = await reader.GetFieldValueAsync<int>(8),
                                NomDeLaSalle = await reader.GetFieldValueAsync<string>(9),
                                NombreDePlaceDispo = await reader.GetFieldValueAsync<int>(10)
                            });
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return listSession;
        }

        /// <summary>
        /// Récupère toutes les sessions que l'utilisateur s'est inscrit, validé ou non
        /// </summary>
        /// <param name="idUser">ID de l'utilisateur.</param>
        /// <returns></returns>
        public async Task<List<SessionInscritUserView>> GetInscriptionSessionUserFinishAsync(string idUser)
        {
            List<SessionInscritUserView> listSession = new List<SessionInscritUserView>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = "SELECT inscrit.IdSession, formateur.Nom, formateur.Prenom, formateur.IsExterne,"
                                            + " catalogue.Titre, sesion.NombreDeJour, sesion.DateSession, inscrit.IsSessionValidate," 
                                            + " inscrit.Note, inscrit.Commentaire"
                                    + " FROM inscriptionformation inscrit"
                                    + " INNER JOIN sessionformation sesion ON inscrit.IdSession = sesion.IdSession"
                                    + " INNER JOIN personnel formateur ON formateur.IdPersonnel = sesion.IdFormateur"
                                    + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                    + " WHERE sesion.DateSession < NOW()"
                                    + $" AND inscrit.IdPersonnel = '{idUser}';";

                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            listSession.Add(new SessionInscritUserView()
                            {
                                IdSession = await reader.GetFieldValueAsync<int>(0),
                                NomFormateur = await reader.GetFieldValueAsync<string>(1),
                                PrenomFormateur = await reader.GetFieldValueAsync<string>(2),
                                IsExterne = reader.GetBoolean(3),
                                TitreFormation = await reader.GetFieldValueAsync<string>(4),
                                NombreJourFormation = await reader.GetFieldValueAsync<int>(5),
                                DateDeLaFormation = await reader.GetFieldValueAsync<DateTime>(6),
                                IsFormationValide = reader.GetBoolean(7),
                                Note = ConvertFromDBVal<int?>(reader.GetValue(8)),
                                Commentaire = ConvertFromDBVal<string?>(reader.GetValue(9))
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return listSession;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSessionNotation"></param>
        /// <param name="userId"></param>
        /// <param name="notePourFormation"></param>
        /// <param name="commentairePourFormation"></param>
        /// <returns></returns>
        public async Task SaveNotationFormation(int idSessionNotation, string userId, int notePourFormation, string commentairePourFormation)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
				try
				{
                    var commandUpdateInscrition = @$"UPDATE inscriptionformation"
                                        + " SET Note=@notePourFormation, Commentaire=@commentairePourFormation"
                                         + $" WHERE IdSession=@idSessionNotation"
                                         + " AND IdPersonnel=@userId;";

                    using (var cmd = new MySqlCommand(commandUpdateInscrition, conn))
                    {
                        cmd.Parameters.AddWithValue("@notePourFormation", notePourFormation);
                        cmd.Parameters.AddWithValue("@commentairePourFormation", commentairePourFormation);

                        cmd.Parameters.AddWithValue("@idSessionNotation", idSessionNotation);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
				catch (Exception ex)
				{
					throw;
				}
            }
        }

        #endregion

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }


        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSql"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<List<T>> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, List<T>> func)
            where T : new()
        {
            return await Task.Run(() =>
            {
                List<T> result = new List<T>();

                try
                {
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                        conn.Open();
                        result = func.Invoke(cmd);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                return result;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSql"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<T> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, T> func)
            where T : new()
        {
            return await Task.Run(() =>
            {
                T result = new T();

                try
                {
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                        conn.Open();
                        result = func.Invoke(cmd);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                return result;
            });
        }

        private async Task ExecuteCoreAsync(string commandSql)
		{
            using (var conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(commandSql, conn);

                conn.Open();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Permet la récupération d'un BLOB uniquement !
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private async Task<byte[]> GetBytesCore(string commandText)
        {
            byte[] file = null;

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            file = (byte[])reader[0];

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return file;
        }

        #endregion
    }

}
