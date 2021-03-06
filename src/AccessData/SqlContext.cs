﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessData.Models;
using AccessData.Views;
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

        /// <summary>
        /// Récupère toutes les sessions qui ne sont pas encore archivées.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SessionView>> GetAllSessionAsync()
        {
            List<SessionView> listSession = new List<SessionView>();

			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.IdFormation, catalogue.Titre, sesion.IdSession, catalogue.Duree, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo, sesion.IsArchive"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle";
                                        //+ " WHERE sesion.IsArchive = 0";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listSession.Add(new SessionView()
                            {
                                IdFormateur = await reader.GetFieldValueAsync<string>(0),
                                Nom = await reader.GetFieldValueAsync<string>(1),
                                Prenom = await reader.GetFieldValueAsync<string>(2),
                                EstExterne = reader.GetBoolean(3),
                                IdFormation = await reader.GetFieldValueAsync<int>(4),
                                TitreFormation = await reader.GetFieldValueAsync<string>(5),
                                IdSession = await reader.GetFieldValueAsync<int>(6),
                                NombreDeJour = await reader.GetFieldValueAsync<double>(7),
                                DateDebutSession = await reader.GetFieldValueAsync<DateTime>(8),
                                IdSalle = await reader.GetFieldValueAsync<int>(9),
                                NomDeLaSalle = await reader.GetFieldValueAsync<string>(10),
                                NombreDePlaceDispo = await reader.GetFieldValueAsync<int>(11),
                                IsArchive = reader.GetBoolean(12)
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
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, catalogue.Duree, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle"
                                        + " WHERE sesion.DateSession > NOW();";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
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
                                NombreDeJour = await reader.GetFieldValueAsync<double>(6),
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
        /// Récupère les informations de la session.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<SessionView> GetSessionAsync(int idSession)
        {
            SessionView sessionView = new SessionView();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, catalogue.Duree, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo, sesion.IdFormation, sesion.IsArchive"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle"
                                        + $" WHERE sesion.IdSession = {idSession};";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sessionView = new SessionView()
                            {
                                IdFormateur = await reader.GetFieldValueAsync<string>(0),
                                Nom = await reader.GetFieldValueAsync<string>(1),
                                Prenom = await reader.GetFieldValueAsync<string>(2),
                                EstExterne = reader.GetBoolean(3),
                                TitreFormation = await reader.GetFieldValueAsync<string>(4),
                                IdSession = await reader.GetFieldValueAsync<int>(5),
                                NombreDeJour = await reader.GetFieldValueAsync<double>(6),
                                DateDebutSession = await reader.GetFieldValueAsync<DateTime>(7),
                                IdSalle = await reader.GetFieldValueAsync<int>(8),
                                NomDeLaSalle = await reader.GetFieldValueAsync<string>(9),
                                NombreDePlaceDispo = await reader.GetFieldValueAsync<int>(10),
                                IdFormation = await reader.GetFieldValueAsync<int>(11),
                                IsArchive = reader.GetBoolean(12)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return sessionView;
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
                    var commandText = @"SELECT catalogue.Titre, catalogue.Duree, sesion.DateSession"
                                    + " FROM sessionformation sesion"
                                    + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                    + " WHERE sesion.DateSession > NOW();";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            double nbreJour = await reader.GetFieldValueAsync<double>(1);
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
		/// <param name="nbrePlace"></param>
		/// <returns></returns>
		public async Task AddSession(int idFormation, string idFormateur, int idSalle, DateTime dateFormation, int nbrePlace)
        {
			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO sessionformation (IdFormateur, IdFormation, IdSalle, DateSession, PlaceDispo, IsArchive)"
                                + " VALUES (@idFormateur, @idFormation, @idSalle, @dateSession, @placeDispo, @archive);";


                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@idFormateur", idFormateur);
                        cmd.Parameters.AddWithValue("@idFormation", idFormation);
                        cmd.Parameters.AddWithValue("@idSalle", idSalle);
                        cmd.Parameters.AddWithValue("@dateSession", dateFormation);
                        cmd.Parameters.AddWithValue("@placeDispo", nbrePlace);
                        cmd.Parameters.AddWithValue("@archive", false);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
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
		/// Ajout une session en base de donné.
		/// </summary>
		/// <param name="idFormation"></param>
		/// <param name="idFormateur"></param>
		/// <param name="idSalle"></param>
		/// <param name="dateFormation"></param>
		/// <param name="nbrePlace"></param>
		/// <returns></returns>
		public async Task<int> CreateSessionHistorique(int idFormation, string idFormateur, int idSalle, DateTime dateFormation)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO sessionformation (IdFormateur, IdFormation, IdSalle, DateSession, PlaceDispo, IsArchive)"
                             + " VALUES (@idFormateur, @idFormation, @idSalle, @dateSession, @placeDispo, @archive);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@idFormateur", idFormateur);
                        cmd.Parameters.AddWithValue("@idFormation", idFormation);
                        cmd.Parameters.AddWithValue("@idSalle", idSalle);
                        cmd.Parameters.AddWithValue("@dateSession", dateFormation);
                        cmd.Parameters.AddWithValue("@placeDispo", 0);
                        cmd.Parameters.AddWithValue("@archive", false);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }

                string commandId = " SELECT LAST_INSERT_ID();";
                int idSession = await GetIntCore(commandId);

                return idSession;
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

            Func<MySqlCommand, Task<Session>> funcCmd = async(cmd) =>
            {
                Session session = new Session();
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
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
			catch (Exception)
			{
                throw;
			}

            return session;
        }

        /// <summary>
        /// Permet d'archiver la session.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task ArchiverSession(int idSession)
        {
            string cmdUpdate = @"UPDATE sessionformation SET IsArchive=@archive"
                                + $" WHERE IdSession={idSession}";

            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand(cmdUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@archive", true);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
        }

        #endregion

        #region TypeDeFormation

        /// <summary>
        /// Récupère la liste des types de formation
        /// </summary>
        /// <returns></returns>
        public async Task<List<TypeFormation>> GetAllTypeFormations()
        {
            var commandText = @"SELECT IdTypeFormation, TitreType"
                                + " FROM typeformation;";                                

            Func<MySqlCommand, Task<List<TypeFormation>>> funcCmd = async (cmd) =>
            {
                List<TypeFormation> listTypes = new List<TypeFormation>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        listTypes.Add(new TypeFormation()
                        {
                            IdTypeFormation = reader.GetInt32(0),
                            TitreTypeFormation = reader.GetString(1)
                        });
                    }
                }

                return listTypes;
            };

            List<TypeFormation> listTypesFormations = new List<TypeFormation>();

            try
            {
                listTypesFormations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return listTypesFormations;
        }

        #endregion

        #region Formations

        /// <summary>
        /// Récupère toutes les formations.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CatalogueFormations>> GetAllFormationsAsync()
        {
            var commandText = @"SELECT catalogue.IdFormation, catalogue.Titre, catalogue.DescriptionFormation, catalogue.DateDeFin, catalogue.NomDeFichier, catalogue.EstInterne, catalogue.Duree, typeform.TitreType, typeform.IdTypeFormation"
                                + " FROM catalogueformation catalogue"
                                + " INNER JOIN typeformation typeform ON typeform.IdTypeFormation = catalogue.IdTypeFormation;";

            Func<MySqlCommand, Task<List<CatalogueFormations>>> funcCmdAsync = async (cmd) =>
            {
                List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        object tempDate = reader.GetValue(3);
                        DateTime? tempDateFin = ConvertFromDBVal<DateTime?>(tempDate);

                        listFormations.Add(new CatalogueFormations()
                        {
                            IdFormation = await reader.GetFieldValueAsync<int>(0),
                            Titre = await reader.GetFieldValueAsync<string>(1),
                            Description = await reader.GetFieldValueAsync<string>(2),
                            DateDeFin = tempDateFin,
                            NomDuFichier = await reader.GetFieldValueAsync<string>(4),
                            EstInterne = reader.GetBoolean(5),
                            Duree = await reader.GetFieldValueAsync<double>(6),
                            TypeFormation = await reader.GetFieldValueAsync<string>(7),
                            TypeFormationId = await reader.GetFieldValueAsync<int>(8)
                        });
                    }
                }

                return listFormations;
            };

            List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

            try
            {
                listFormations = await GetCoreAsync(commandText, funcCmdAsync);
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
            var commandText = @"SELECT catalogue.IdFormation, catalogue.Titre, catalogue.DescriptionFormation, catalogue.NomDeFichier, catalogue.EstInterne, catalogue.Duree, typeform.TitreType"
                            + " FROM catalogueformation catalogue"
                            + " INNER JOIN typeformation typeform ON typeform.IdTypeFormation = catalogue.IdTypeFormation"
                            + " WHERE catalogue.DateDeFin IS NULL"
                            + " OR catalogue.DateDeFin > curdate();";

            Func<MySqlCommand, Task<List<CatalogueFormations>>> funcCmd = async (cmd) =>
            {
                List<CatalogueFormations> listFormations = new List<CatalogueFormations>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        listFormations.Add(new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            NomDuFichier = reader.GetString(3),
                            EstInterne = reader.GetBoolean(4),
                            Duree = reader.GetDouble(5),
                            TypeFormation = reader.GetString(6)
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

            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, DateDeFin, NomDeFichier, EstInterne, Duree FROM catalogueformation"
                                       + $" WHERE IdFormation={idFormation};";

            Func<MySqlCommand, Task<CatalogueFormations>> funcCmd = async(cmd) =>
            {
                CatalogueFormations formation = new CatalogueFormations();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
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
                            EstInterne = reader.GetBoolean(5),
                            Duree = reader.GetDouble(6)
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
            var commandText = @"SELECT IdFormation, Titre, DescriptionFormation, EstInterne, Duree FROM catalogueformation"
                            + $" WHERE Titre LIKE '%{nomFormation}%';";

            Func<MySqlCommand, Task<List<CatalogueFormations>>> funcCmd = async (cmd) =>
            {
                List<CatalogueFormations> formations = new List<CatalogueFormations>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var formation = new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2),
                            EstInterne = reader.GetBoolean(3),
                            Duree = reader.GetDouble(4)
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
			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand("INSERT INTO catalogueformation (Titre, DescriptionFormation, DateDeFin, FichierContenu, NomDeFichier, EstInterne, Duree, IdTypeFormation)"
                                                    + " VALUES (@Titre, @Description, NULL, @ContenuFormationN, @NomDuFichier, @EstInterne, @Duree, @IdTypeFormation);"
                    , conn))
                    {
                        cmd.Parameters.AddWithValue("@Titre", nouvelleFormation.Titre);
                        cmd.Parameters.AddWithValue("@Description", nouvelleFormation.Description);
                        cmd.Parameters.AddWithValue("@ContenuFormationN", nouvelleFormation.ContenuFormationN);
                        cmd.Parameters.AddWithValue("@NomDuFichier", nouvelleFormation.NomDuFichier);
                        cmd.Parameters.AddWithValue("@EstInterne", nouvelleFormation.EstInterne);
                        cmd.Parameters.AddWithValue("@Duree", nouvelleFormation.Duree);
                        cmd.Parameters.AddWithValue("@IdTypeFormation", nouvelleFormation.TypeFormationId);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }
            }
			catch (Exception ex)
			{
				throw;
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


				var commandUpdateFormation = @$"UPDATE catalogueformation SET Titre=@Titre, DescriptionFormation=@Description, DateDeFin=@dateFin, FichierContenu=@ContenuFormationN, NomDeFichier=@NomDuFichier, EstInterne=@EstInterne, Duree=@Duree, IdTypeFormation=@IdType"
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
                    cmd.Parameters.AddWithValue("@Duree", currentFormation.Duree);
                    cmd.Parameters.AddWithValue("@IdType", currentFormation.TypeFormationId);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
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


                var commandUpdateFormation = @$"UPDATE catalogueformation SET Titre=@Titre, DescriptionFormation=@Description, DateDeFin=@dateFin, NomDeFichier=@NomDuFichier, EstInterne=@EstInterne, Duree=@Duree, IdTypeFormation=@IdType"
                                         + $" WHERE IdFormation=@IdFormation;";

                using (var cmd = new MySqlCommand(commandUpdateFormation, conn))
                {
                    cmd.Parameters.AddWithValue("@Titre", currentFormation.Titre);
                    cmd.Parameters.AddWithValue("@Description", currentFormation.Description);
                    cmd.Parameters.AddWithValue("@NomDuFichier", currentFormation.NomDuFichier);
                    cmd.Parameters.AddWithValue("@IdFormation", currentFormation.IdFormation);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);
                    cmd.Parameters.AddWithValue("@EstInterne", currentFormation.EstInterne);
                    cmd.Parameters.AddWithValue("@Duree", currentFormation.Duree);
                    cmd.Parameters.AddWithValue("@IdType", currentFormation.TypeFormationId);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
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

                using (var reader = await cmd.ExecuteReaderAsync())
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

                using (var reader = await cmd.ExecuteReaderAsync())
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

                using (var reader = await cmd.ExecuteReaderAsync())
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

                    using (var reader = await cmd.ExecuteReaderAsync())
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

                    using (var reader = await cmd.ExecuteReaderAsync())
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

                    using (var reader = await cmd.ExecuteReaderAsync())
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

                    using (var reader = await cmd.ExecuteReaderAsync())
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

                    using (var reader = await cmd.ExecuteReaderAsync())
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
                    var commandText = @"SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.IsExterne, catalogue.Titre, sesion.IdSession, catalogue.Duree, sesion.DateSession, sal.IdSalle, sal.NomSalle, sesion.PlaceDispo"
                                        + " FROM sessionformation sesion"
                                        + " INNER JOIN personnel pers ON pers.IdPersonnel = sesion.IdFormateur"
                                        + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                        + " INNER JOIN salle sal ON sal.IdSalle = sesion.IdSalle"
                                        + " INNER JOIN inscriptionformation inscrit ON inscrit.IdSession = sesion.IdSession"
                                        + " WHERE sesion.DateSession > NOW()"
                                        + $" AND inscrit.IdPersonnel = '{idUser}';";
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
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
                                NombreDeJour = await reader.GetFieldValueAsync<double>(6),
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
                                            + " catalogue.Titre, catalogue.Duree, sesion.DateSession, inscrit.IsSessionValidate,"
                                            + " inscrit.Note, inscrit.Commentaire"
                                    + " FROM inscriptionformation inscrit"
                                    + " INNER JOIN sessionformation sesion ON inscrit.IdSession = sesion.IdSession"
                                    + " INNER JOIN personnel formateur ON formateur.IdPersonnel = sesion.IdFormateur"
                                    + " INNER JOIN catalogueformation catalogue ON catalogue.IdFormation = sesion.IdFormation"
                                    + " WHERE sesion.DateSession < NOW()"
                                    + $" AND inscrit.IdPersonnel = '{idUser}';";

                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
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
                                NombreJourFormation = await reader.GetFieldValueAsync<double>(5),
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
        /// Récupère la liste des utilisateurs inscrits.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<List<PersonnelInscritView>> GetPersonnelsInscritSession(int idSession)
        {
            List<PersonnelInscritView> personnelInscrit = new List<PersonnelInscritView>();

            var commandText = "SELECT pers.IdPersonnel, pers.Nom, pers.Prenom, pers.Service, inscription.IsSessionValidate, inscription.Note, inscription.Commentaire"
                            + " FROM personnel pers"
                            + " INNER JOIN inscriptionformation inscription ON pers.IdPersonnel = inscription.IdPersonnel"
                            + $" WHERE inscription.IdSession = {idSession};";

            Func<MySqlCommand, Task<List<PersonnelInscritView>>> funcCmd = async (cmd) =>
            {
                List<PersonnelInscritView> personnelInscrits = new List<PersonnelInscritView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PersonnelInscritView personnel = new PersonnelInscritView()
                        {
                            IdPersonnel = reader.GetString(0),
                            Nom = reader.GetString(1),
                            Prenom = reader.GetString(2),
                            Service = reader.GetString(3),
                            IsSessionValidate = reader.GetBoolean(4),
                            Note = ConvertFromDBVal<int?>(reader.GetValue(5)),
                            Commentaire = ConvertFromDBVal<string?>(reader.GetValue(6))
                        };

                        personnelInscrits.Add(personnel);
                    }
                }

                return personnelInscrits;
            };

            try
            {
                personnelInscrit = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return personnelInscrit;

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
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }
				catch (Exception ex)
				{
					throw;
				}
            }
        }

        /// <summary>
        /// Met à jour pour un utilisateur, si la session est validé ou non.
        /// </summary>
        /// <param name="isValidate"></param>
        /// <param name="idSession"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public async Task UpdateValidationUser(bool isValidate, int idSession, string idUser)
		{
				try
				{
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        string commandUpdateSession = @"UPDATE inscriptionformation"
                                                            + $" SET IsSessionValidate=@valueValidate"
                                                            + $" WHERE IdSession = {idSession}  AND IdPersonnel = '{idUser}';";

                        using (var cmd = new MySqlCommand(commandUpdateSession, conn))
                        {
                            cmd.Parameters.AddWithValue("@valueValidate", isValidate);

                            conn.Open();
                            int result = await cmd.ExecuteNonQueryAsync();
                            conn.Close();
                        }
                    }
                }
                catch (Exception)
				{
					throw;
				}       
        }

        #endregion

        #region Competences

        /// <summary>
        /// Ajoute une nouvelle compétence
        /// </summary>
        /// <param name="nouvelleCompetence"></param>
        /// <returns></returns>
        public async Task<int> InsertCompetence(Competence nouvelleCompetence)
        {
            // Pour insérer
            using (var conn = new MySqlConnection(ConnectionString))
            {
                string commandInsert = "INSERT INTO competences(Titre, DescriptionCompetence) "
                                                + "VALUES(@titre, @descriptionCompetence);";

                using (var cmd = new MySqlCommand(commandInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@titre", nouvelleCompetence.Titre);
                    cmd.Parameters.AddWithValue("@descriptionCompetence", nouvelleCompetence.Description);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Pour récupérer l'ID de la ligne.
            string idCmd = "SELECT LAST_INSERT_ID();";
            return await GetIntCore(idCmd);
        }

        /// <summary>
        /// Ajout les formations pour une compétence
        /// </summary>
        /// <param name="idCompetence"></param>
        /// <param name="formations"></param>
        /// <returns></returns>
        public async Task InsertCompetenceFormation(int idCompetence, List<CatalogueFormations> formations)
        {
            // Pour insérer
            try
            {
                if (formations.Count > 0)
                {
                    string commandInsert = "INSERT INTO competenceformation(IdCompetence, IdFormation) VALUES ";

                    int maxLine = formations.Count();
                    for (int i = 0; i < maxLine; i++)
                    {
                        commandInsert += $"({idCompetence}, {formations[i].IdFormation})";

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
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Récupère toutes les compétences, avec les formations liés.
        /// </summary>
        /// <returns></returns>
        public async Task<CompetenceDetailView> GetCompetenceView(int idCompetence)
        {
            var commandText = @"SELECT comp.IdCompetence, comp.Titre, comp.DescriptionCompetence, formation.IdFormation, formation.Titre, formation.DescriptionFormation, formation.EstInterne, formation.Duree, formation.NomDeFichier "
                                + "FROM competences comp "
                                + "INNER JOIN competenceformation compform ON compform.IdCompetence = comp.IdCompetence "
                                + "INNER JOIN catalogueformation formation ON formation.IdFormation = compform.IdFormation "
                                + $"WHERE comp.IdCompetence={idCompetence}";

            Func<MySqlCommand, Task<CompetenceDetailView>> funcCmd = async(cmd) =>
            {
                CompetenceDetailView competenceView = new CompetenceDetailView();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if(competenceView.Competence == null)
						{
                            Competence competence = new Competence()
                            {
                                IdCompetence = reader.GetInt32(0),
                                Titre = reader.GetString(1),
                                Description = reader.GetString(2)
                            };

                            competenceView.Competence = competence;

                            competenceView.Formations = new List<CatalogueFormations>();
                        }

                        CatalogueFormations formation = new CatalogueFormations()
                        {
                            IdFormation = reader.GetInt32(3),
                            Titre  = reader.GetString(4),
                            Description = reader.GetString(5),
                            EstInterne = reader.GetBoolean(6),
                            Duree = reader.GetDouble(7),
                            NomDuFichier = reader.GetString(8)
                        };

                        competenceView.Formations.Add(formation);
                    }
                }

                return competenceView;
            };

            CompetenceDetailView competenceView = new CompetenceDetailView();

            try
            {
                competenceView = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return competenceView;
        }


        /// <summary>
        /// Récupère toutes les compétences, avec les formations liés.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompetenceView>> GetAllCompetence()
		{
            var commandText = @"SELECT comp.IdCompetence, comp.Titre, comp.DescriptionCompetence, formation.IdFormation, formation.Titre "
                                + "FROM competences comp "
                                + "INNER JOIN competenceformation compform ON compform.IdCompetence = comp.IdCompetence "
                                + "INNER JOIN catalogueformation formation ON formation.IdFormation = compform.IdFormation";

            Func<MySqlCommand, Task<List<CompetenceView>>> funcCmd = async(cmd) =>
            {
                List<CompetenceView> competences = new List<CompetenceView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Competence competence = new Competence()
                        {
                            IdCompetence = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2)
                        };

                        FormationView formationView = new FormationView()
                        {
                            IdFormation = reader.GetInt32(3),
                            TitreFormation = reader.GetString(4)
                        };

                        // Si la liste contient la compétence, ajouter la formation
                        var tempCompetence = competences.FirstOrDefault(x => x.Competence.IdCompetence == competence.IdCompetence);

                        if (tempCompetence != null)
						{
                            tempCompetence.FormationViews.Add(formationView);
						}
						else
						{
                            var tempFormations = new List<FormationView>();
                            tempFormations.Add(formationView);

                            competences.Add(new CompetenceView()
                            {
                                Competence = competence,
                                FormationViews = tempFormations
                            });
                        }
                    }
                }

                return competences;
            };

            List<CompetenceView> competences = new List<CompetenceView>();

            try
            {
                competences = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return competences;
        }

        /// <summary>
        /// Récupère toutes les compétences
        /// </summary>
        /// <returns></returns>
        public async Task<List<Competence>> GetAllCompetences()
        {
            var commandText = @"SELECT comp.IdCompetence, comp.Titre, comp.DescriptionCompetence "
                                + "FROM competences comp;";

            Func<MySqlCommand, Task<List<Competence>>> funcCmd = async(cmd) =>
            {
                List<Competence> competences = new List<Competence>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Competence competence = new Competence()
                        {
                            IdCompetence = reader.GetInt32(0),
                            Titre = reader.GetString(1),
                            Description = reader.GetString(2)
                        };

                        competences.Add(competence);
                    }
                }

                return competences;
            };

            List<Competence> competences = new List<Competence>();

            try
            {
                competences = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return competences;
        }

        /// <summary>
        /// Met à jour les valeurs du titre et de la description pour une compétence.
        /// </summary>
        /// <param name="titreCompetence"></param>
        /// <param name="descriptionCompetence"></param>
        /// <returns></returns>
        public async Task UpdateCompetenceTitreDescription(int idCompetence, string titreCompetence, string descriptionCompetence)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                var commandUpdateCompetence = @$"UPDATE competences SET Titre=@titre, DescriptionCompetence=@description"
                                      + $" WHERE IdCompetence={idCompetence};";

                using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
                {
                    cmd.Parameters.AddWithValue("@titre", titreCompetence);
                    cmd.Parameters.AddWithValue("@description", descriptionCompetence);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Supprime une formation pour cette compétence.
        /// </summary>
        /// <param name="idCompetence"></param>
        /// <param name="idFormation"></param>
        /// <returns></returns>
        public async Task DeleteCompetenceFormation(int idCompetence, int idFormation)
        {
            string commandDelete = $"DELETE FROM competenceformation"
                                   + $" WHERE IdCompetence={idCompetence} AND IdFormation='{idFormation}';";

            await ExecuteCoreAsync(commandDelete);
        }

        /// <summary>
        /// Retourne la liste des ID de compétence pour cette formation.
        /// </summary>
        /// <param name="idFormation"></param>
        /// <returns></returns>
        public async Task<List<int>> GetCompetencesIdByFormation(int idFormation)
        {
            var commandText = @"SELECT comp.IdCompetence "
                            + "FROM competenceformation comp "
                            + $"WHERE comp.IdFormation = {idFormation};";

            Func<MySqlCommand, Task<List<int>>> funcCmd = async(cmd) =>
            {
                List<int> competences = new List<int>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int idCompetence = reader.GetInt32(0);
                        competences.Add(idCompetence);
                    }
                }

                return competences;
            };

            List<int> competences = new List<int>();

            try
            {
                competences = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return competences;
        }

        /// <summary>
        /// Retourne la liste des IDs formations pour une compétence
        /// </summary>
        /// <param name="idCompetence"></param>
        /// <returns></returns>
        public async Task<List<int>> GetFormationsByCompetence(int idCompetence)
        {
            var commandText = @"SELECT comp.IdFormation "
                            + "FROM competenceformation comp "
                            + $"WHERE comp.IdCompetence = {idCompetence};";

            Func<MySqlCommand, Task<List<int>>> funcCmd = async(cmd) =>
            {
                List<int> idFormations = new List<int>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int idFormation = reader.GetInt32(0);
                        idFormations.Add(idFormation);
                    }
                }

                return idFormations;
            };

            List<int> formations = new List<int>();

            try
            {
                formations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return formations;
        }

        /// <summary>
        /// Récupère la liste des utilisateurs de la session donnée, qui sont validés.
        /// </summary>
        /// <param name="idSession"></param>
        /// <returns></returns>
        public async Task<List<string>> GetUsersValidateOnThisSession(int idSession)
        {
            var commandText = @"SELECT pers.IdPersonnel "
                                + "FROM inscriptionformation pers "
                                + $"WHERE pers.IdSession = {idSession} "
                                + "AND pers.IsSessionValidate = true;";

            Func<MySqlCommand, Task<List<string>>> funcCmd = async(cmd) =>
            {
                List<string> users = new List<string>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string idUser = reader.GetString(0);
                        users.Add(idUser);
                    }
                }

                return users;
            };

            List<string> usersId = new List<string>();

            try
            {
                usersId = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return usersId;
        }

        /// <summary>
        /// Récupère les formations validés pour cet utilisateur.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<int>> GetFormationsValideByUser(string user)
        {
            var commandText = @"SELECT sesion.IdFormation "
                             + "FROM inscriptionformation inscrit "
                             + "INNER JOIN sessionformation sesion "
                             + "ON sesion.IdSession = inscrit.IdSession "
                             + $"WHERE inscrit.IdPersonnel = '{user}' "
                             + "AND inscrit.IsSessionValidate = true;";

            Func<MySqlCommand, Task<List<int>>> funcCmd = async(cmd) =>
            {
                List<int> allFormationsValidate = new List<int>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int idFormation = reader.GetInt32(0);
                        allFormationsValidate.Add(idFormation);
                    }
                }

                return allFormationsValidate;
            };

            List<int> formations = new List<int>();

            try
            {
                formations = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return formations;
        }

        /// <summary>
        /// Récupère les compétences que l'utilisateur à validé.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<CompetenceUserView>> GetCompetencesUser(string userId)
        {
            string commandText = @"SELECT competence.IdCompetence, competence.Titre, competence.DescriptionCompetence, suivi.IdSame, sam.Nom, suivi.DateObtention"
                                + " FROM suivisame suivi"
                                + " INNER JOIN same sam"
                                + " ON suivi.IdSame = sam.IdSame"
                                + " INNER JOIN competences competence"
                                + " ON suivi.IdCompetence = competence.IdCompetence"
                                + $" WHERE suivi.IdPersonnel = '{userId}';";

            Func<MySqlCommand, Task<List<CompetenceUserView>>> funcCmd = async(cmd) =>
            {
                List<CompetenceUserView> allCompetencesValidate = new List<CompetenceUserView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int idCompetence = reader.GetInt32(0);

                        var tempCompetence = allCompetencesValidate.FirstOrDefault(x => x.IdCompetence == idCompetence);

                        // Si contient une ligne avec cette compétence, mettre à jour les lignes SAME
                        //if (allCompetencesValidate.Any(x => x.IdCompetence == idCompetence))
                        if(tempCompetence != null)
						{
                            SameView sameView = new SameView();

                            sameView.IdSame = reader.GetInt32(3);
                            sameView.TitreSame = reader.GetString(4);
                            sameView.DateObtention = reader.GetDateTime(5);

                            tempCompetence.Same.Add(sameView);
                        }
						else
						{
                            CompetenceUserView competenceUserView = new CompetenceUserView();
                            competenceUserView.Same = new List<SameView>();
                            SameView sameView = new SameView();

                            competenceUserView.IdCompetence = idCompetence;
                            competenceUserView.Titre = reader.GetString(1);
                            competenceUserView.Description = reader.GetString(2);
                            
                            sameView.IdSame = reader.GetInt32(3);
                            sameView.TitreSame  = reader.GetString(4);
                            sameView.DateObtention = reader.GetDateTime(5);

                            competenceUserView.Same.Add(sameView);
                            allCompetencesValidate.Add(competenceUserView);
                        }
                    }
                }

                return allCompetencesValidate;
            };

            List<CompetenceUserView> competenceUsers = new List<CompetenceUserView>();

            try
            {
                competenceUsers = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
                throw;
            }

            return competenceUsers;
        }

        #endregion

        #region SAME

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> IsCompetenceValidate(string user, int idCompetence)
        {
            var commandText = @"SELECT IdSame "
                           + "FROM suivisame "
                           + $"WHERE IdPersonnel = '{user}' "
                           + $"AND IdCompetence = {idCompetence};";
           
            int id = await GetIntCore(commandText);
            return (id > 0);
        }

        /// <summary>
        /// Ajoute dans la table SuiviSAME, la compétence avec l'ID SAME pour un utilisateur.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="idCompetence"></param>
        /// <param name="idSame"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public async Task AddCompetenceToUser(string user, int idCompetence, int idSame, DateTime now)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand("INSERT INTO suivisame (IdPersonnel, IdCompetence, IdSame, DateObtention)"
                                                + " VALUES (@user, @idCompetence, @same, @date);"
                , conn))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@idCompetence", idCompetence);
                    cmd.Parameters.AddWithValue("@same", idSame);
                    cmd.Parameters.AddWithValue("@date", now);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task<List<T>> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<List<T>>> func)
            where T : new()
        {
            List<T> result = new List<T>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                    conn.Open();
                    result = await func.Invoke(cmd);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSql"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<T> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<T>> func)
            where T : new()
        {
            T result = new T();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                    conn.Open();
                    result = await func.Invoke(cmd);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Execute une commande qui n'attend pas de retour.
        /// </summary>
        /// <param name="commandSql"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Permet la récupération d'un ID type int uniquement !
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private async Task<int> GetIntCore(string commandText)
        {
            int id = 0;

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    UInt64 idTemp = 0;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            idTemp = (UInt64)reader[0];
                        }
                    }

                    id = Convert.ToInt32(idTemp);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return id;
        }

        /// <summary>
        /// Permet de gérer les retours de valeur null de la BDD
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T ConvertFromDBVal<T>(object obj)
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

        #endregion

    }
}
