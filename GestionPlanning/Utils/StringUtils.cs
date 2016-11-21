using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Classe utilitaire pour la getion des chaines de caractères
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Copie une chaine de caractère
        /// </summary>
        /// <param name="chaine">la chaine à copier</param>
        /// <returns>la nouvelle chaine</returns>
        public static string CopyString(string chaine)
        {
            return chaine.ToCharArray().Aggregate("", (current, carac) => current + carac);
        }

        /// <summary>
        /// Met en majuscule la première lettre d'une chaine de caractère
        /// </summary>
        /// <param name="chaine">la chaine à modifier</param>
        /// <returns>le résultat</returns>
        public static string FirstLetterUpper(string chaine)
        {
            chaine = chaine.Trim();
            if (chaine.Length > 0)
            {
                if (chaine.Length > 1)
                {
                    return chaine.Substring(0, 1).ToUpper() + chaine.Substring(1, chaine.Length - 1).ToLower();
                }
                if(chaine.Length == 1)
                {
                    return chaine.Substring(0, 1).ToUpper();
                }
            }
            return "";
        }

        /// <summary>
        /// Permet de donner l'extension d'un fichier
        /// </summary>
        /// <param name="fichier">le chemin ou le nom complet du fichier</param>
        /// <returns>l'extension</returns>
        public static string GetExtension(string fichier)
        {
            return fichier.Substring(fichier.LastIndexOf('.') + 1, fichier.Length - (fichier.LastIndexOf('.') + 1));
        }

        /// <summary>
        /// Sépare une chaine de caractère en liste de string à partir d'une chaine de caractère
        /// </summary>
        /// <param name="chaine">La chaine à couper</param>
        /// <param name="separateur">la chaine séparatrice</param>
        /// <returns>la liste de String</returns>
        public static List<string> Split(string chaine, string separateur)
        {
            return Regex.Split(chaine, separateur).ToList();
        }

        /// <summary>
        /// Vérifie si une lsite de string ne contient que des nombres
        /// </summary>
        /// <param name="list">la liste à vérifier</param>
        /// <returns>true si ok</returns>
        public static bool CheckListStringIsInt(IEnumerable<string> list)
        {
            if (list != null)
            {
                var test = true;

                foreach (var chiffre in list)
                {
                    int i;
                    if (!int.TryParse(chiffre, out i))
                    {
                        test = false;
                        break;
                    }
                }
                return test;
            }
            return false;
        }

        /// <summary>
        /// Vérifie si un tableau de string contient des chiffres
        /// </summary>
        /// <param name="array">le tableau à controler</param>
        /// <returns>true si il en contient</returns>
        public static bool CheckStringArrayContainsInt(string[] array)
        {
            if (array != null)
            {
                int val;
                return array.Any(element => int.TryParse(element, out val));
            }
            return false;
        }
    }
}