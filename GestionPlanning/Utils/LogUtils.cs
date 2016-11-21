using System;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Utilitaire pour gérer les logs
    /// </summary>
    public class LogUtils
    {
        /// <summary>
        /// La connexion à la base de donnée pour enregistrer les logs
        /// </summary>
        private readonly ContexteDb _db;

        /// <summary>
        /// Constructeur
        /// </summary>
        public LogUtils()
        {
            _db = new ContexteDb();
        }

        /// <summary>
        /// Enregistre une erreur en base
        /// </summary>
        /// <param name="message">le message à enregistrer</param>
        public void Error(string message)
        {
            LogMessage(TypeLogEnum.ERROR, message);
        }

        /// <summary>
        /// Enregistre un message informatif en base
        /// </summary>
        /// <param name="message">le message à enregistrer</param>
        public void Info(string message)
        {
            LogMessage(TypeLogEnum.INFO, message);
        }

        /// <summary>
        /// Enregistre un avertissement base
        /// </summary>
        /// <param name="message">le message à enregistrer</param>
        public void Avertissement(string message)
        {
            LogMessage(TypeLogEnum.AVERTISSEMENT, message);
        }

        /// <summary>
        /// Enregistre un message si l'appli est en debug
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            #if DEBUG
            LogMessage(TypeLogEnum.DEBUG, message);
            #endif
        }


        /// <summary>
        /// Enregistre un log en base de donnée en ajoutant les informaions complémentaires
        /// </summary>
        /// <param name="type">le niveau du log</param>
        /// <param name="message">le message</param>
        private void LogMessage(TypeLogEnum type, string message)
        {
            try
            {
                var log = new Logs
                {
                    Date = DateUtils.GetMaintenant(),
                    Type = type,
                    Texte = message
                };

                _db.Logs.Add(log);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        

    }
}