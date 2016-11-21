using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Classe utilitaire pour le chiffrement
    /// </summary>
    public static class CryptUtils
    {
        private static readonly List<char> ListeLettreMinuscule = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k'
        , 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        private static readonly List<char> ListeLettreMajuscule = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'
        , 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
        private static readonly List<char> ListeChiffre = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        /// <summary>
        /// Liste des caractères spéciaux différentes  poru supprimer certains caractères pouvant poser problème dans la génération dans du code html
        /// </summary>
        private static readonly List<char> ListeCaractereSpeciauxGenerateur = new List<char> { '²', '&', 'é', '#', '{', '-', '|', 'è', '_'
        , 'ç', 'à', '@', ')', '(', '[', ']', '=', '+', '}', '£', '$', '¤', '%', 'ù', 'µ', '*', '?', ',', '.', ';'
        , ':', '§', '!', '€'};
        private static readonly List<char> ListeCaractereSpeciaux = new List<char> { '²', '&', 'é', '"', '#', '\'', '{', '-', '|', 'è', '_'
        , '\\', 'ç', 'à', '@', ')', '(', '[', ']', '=', '+', '}', '£', '$', '¤', '%', 'ù', 'µ', '*', '?', ',', '.', ';'
        , '/', ':', '§', '!', '€', '>', '<'};


        /// <summary>
        /// Méthode pour hasher en SHA256
        /// </summary>
        /// <param name="text">le texte à hasher</param>
        /// <returns>le hash</returns>
        private static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            var hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
        }

        /// <summary>
        /// Créer le Hash d'un mot de passe pour l'appli
        /// </summary>
        /// <param name="text">le mot de passe</param>
        /// <returns>le hash final</returns>
        public static string GenerateHashPassword(string text)
        {
            var hashA = "4af6y?B#5iiJgRM4e~]6^Ap_47Y;2RH(" + GetHashSha256(text)+ ".KRu4m){F2&5jB<8dC7T4G5U/+.fqv5z";
            return GetHashSha256(hashA);
        }


        /// <summary>
        /// Calcul la force approximative d'un mot de passe
        /// </summary>
        /// <param name="motDePasse">le mot de passe à calculer</param>
        /// <returns>la puissance en %</returns>
        public static int CalculForceMotDePasse(string motDePasse)
        {
            var somme = 0;// motDePasse.Length * 5;//chaque caractère vaut 5 %
            var nbTypePresent = 0;
            var minusculePresent = false;
            var chiffrePresent = false;
            var majusculePresent = false;
            var speciauxPresent = false;
            //ont vérifie les différents type de caractères présent
            if (motDePasse == null)
            {
                motDePasse = "";
            }
            foreach (var t in motDePasse)
            {
                if (ListeLettreMinuscule.Contains(t))
                {
                    minusculePresent = true;
                    somme += 4;
                }

                if (ListeLettreMajuscule.Contains(t))
                {
                    majusculePresent = true;
                    somme += 4;
                }

                if (ListeChiffre.Contains(t))
                {
                    chiffrePresent = true;
                    somme += 2;
                }

                if (ListeCaractereSpeciaux.Contains(t))
                {
                    speciauxPresent = true;
                    somme += 7;
                }
            }

            if (speciauxPresent) { nbTypePresent++; }
            if (minusculePresent) { nbTypePresent++; }
            if (majusculePresent) { nbTypePresent++; }
            if (chiffrePresent) { nbTypePresent++; }

            //ont multiple la force par un certains nombre en fonction du nombre de caractères.
            switch (nbTypePresent)
            {
                case 1: somme = ((int)(somme * 0.75)); break;
                case 2: somme = ((int)(somme * 1.3)); break;
                case 3: somme = ((int)(somme * 1.7)); break;
                case 4: somme = ((somme * 2)); break;
            }

            if (somme > 100) somme = 100;
            return somme;
        }

        /// <summary>
        /// Genere un mot de passe aléatoire composer de caractères majuscules, minuscules, de chiffres et de caractères spéciaux
        /// </summary>
        /// <param name="longueur">longueur du mot de passe souhaité, si 0 sera de 12 caractères</param>
        /// <param name="lettre">autorise les lettres minuscules et majuscules dans le mot de passe</param>
        /// <param name="chiffre">autorise les chiffres dans le mot de passe</param>
        /// <param name="caracSpeciaux">autorise les caractères spéciaux dans le mot de passe</param>
        /// <returns>le mot de passe généré</returns>
        public static string GeneratePassword(int longueur, bool lettre, bool chiffre, bool caracSpeciaux)
        {
            var length = (longueur == 0) ? 12 : longueur;
            var password = "";
            var rnd = new Random();
            for (var i = 0; i < length; i++)
            {
                var caracBienCree = false;
                do
                {
                    var typeTab = rnd.Next(4);
                    switch (typeTab)
                    {
                        case 0:
                            if (lettre)
                            {
                                password += ListeLettreMinuscule[rnd.Next(ListeLettreMinuscule.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 1:
                            if (lettre)
                            {
                                password += ListeLettreMajuscule[rnd.Next(ListeLettreMajuscule.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 2:
                            if (chiffre)
                            {
                                password += ListeChiffre[rnd.Next(ListeChiffre.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 3:
                            if (caracSpeciaux)
                            {
                                password += ListeCaractereSpeciaux[rnd.Next(ListeCaractereSpeciaux.Count)];
                                caracBienCree = true;
                            }
                            break;
                    }
                } while (!caracBienCree);
            }
            return password;
        }

        /// <summary>
        /// Vérifie si un mot de passe valde toute les conditions pour être sur
        /// </summary>
        /// <param name="motDePasse">le mot de passe</param>
        /// <returns>true si valide</returns>
        public static bool IsValidMotDePasse(string motDePasse)
        {
            return motDePasse.IndexOfAny(ListeChiffre.ToArray()) != -1 &&
                   motDePasse.IndexOfAny(ListeCaractereSpeciaux.ToArray()) != -1 &&
                   motDePasse.IndexOfAny(ListeLettreMinuscule.ToArray()) != -1 &&
                   motDePasse.IndexOfAny(ListeLettreMajuscule.ToArray()) != -1;
        }
    }
}