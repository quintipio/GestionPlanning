using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Utilitaire pour les dates
    /// </summary>
    public static class DateUtils
    {
        //liste des jour férié statique
        private static readonly List<DateTime> ListeJourFerie = new List<DateTime> { new DateTime(2000,01,01),new DateTime(2000,05,01),new DateTime(2000,05,08),new DateTime(2000,07,14), new DateTime(2000,11,11)
        ,new DateTime(2000,08,15),new DateTime(2000,11,01),new DateTime(2000,12,25)};


        /// <summary>
        /// retourne l'objet DateTime à une heure précise
        /// </summary>
        /// <returns>La date précise</returns>
        public static DateTime GetMaintenant()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Retourne la date du jour (ann
        /// </summary>
        /// <returns></returns>
        public static DateTime GetAujourdhui()
        {
            return DateTime.Today;
        }

        /// <summary>
        /// Converti uen chaine de caractère ex : "01/08/2008" en DateTime
        /// </summary>
        /// <param name="date">La date à convertir</param>
        /// <returns>La DateTime</returns>
        public static DateTime StringEnDate(string date)
        {
            return Convert.ToDateTime(date);
        }

        /// <summary>
        /// retourne l'objet DateTime en String à une heure précise
        /// </summary>
        /// <returns>La date précise en String</returns>
        public static string GetMaintenantString()
        {
            return DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// retourne la date du jour
        /// </summary>
        /// <returns>La date du jour formatté en dd-MM-YYYY</returns>
        public static string GetDateJour()
        {
            return GetDateFormat(GetMaintenant(), "dd-MM-yyyy");
        }

        /// <summary>
        /// Permet de formatter une date
        /// y : année, M : mois, d : jour
        /// h : heure/12, H : heure/24
        /// m : minute, s : seconde
        /// f : fraction de secondes , F : sans zéros
        /// t : AM ou PM, z : time zone
        /// </summary>
        /// <param name="d">La date à formatter</param>
        /// <param name="format">le format voulu</param>
        /// <returns>La date formatter</returns>
        public static string GetDateFormat(DateTime d, string format)
        {
            return string.Format("{0:" + format + "}", d);
        }


        /// <summary>
        /// Permet d'obtenir l'intervalle de jours entre deux dates
        /// </summary>
        /// <param name="oldDate"> la plus veille date</param>
        /// <param name="newDate">date la plus récente</param>
        /// <returns>Le nombre de jours</returns>
        public static int IntervalleEntreDeuxDates(DateTime oldDate, DateTime newDate)
        {
            var ts = newDate - oldDate;

            return ts.Days;
        }


        /// <summary>
        /// Retourne le diminutif d'un jour de la semaine
        /// </summary>
        /// <param name="date">La date dont ont souhaite le jour</param>
        /// <returns>son diminutif</returns>
        public static string GetJourDiminutif(DateTime date)
        {
            string jour = null;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    jour = "L";
                    break;

                case DayOfWeek.Tuesday:
                    jour = "Ma";
                    break;

                case DayOfWeek.Wednesday:
                    jour = "Me";
                    break;

                case DayOfWeek.Thursday:
                    jour = "J";
                    break;

                case DayOfWeek.Friday:
                    jour = "V";
                    break;

                case DayOfWeek.Saturday:
                    jour = "S";
                    break;

                case DayOfWeek.Sunday:
                    jour = "D";
                    break;
            }

            return jour;
        }

        /// <summary>
        /// Retourne le nom complet du mois dont la date est en paramètre
        /// </summary>
        /// <param name="date">La date dont ont cherche le mois en francais</param>
        /// <returns>Le nom du mois</returns>
        public static string GetNomMois(DateTime date)
        {
            string jour = null;
            switch (date.Month)
            {
                case 1: jour = "Janvier"; break;
                case 2: jour = "Février"; break;
                case 3: jour = "Mars"; break;
                case 4: jour = "Avril"; break;
                case 5: jour = "Mai"; break;
                case 6: jour = "Juin"; break;
                case 7: jour = "Juillet"; break;
                case 8: jour = "Aout"; break;
                case 9: jour = "Septembre"; break;
                case 10: jour = "Octobre"; break;
                case 11: jour = "Novembre"; break;
                case 12: jour = "Décembre"; break;
            }
            return jour;
        }

        /// <summary>
        /// retourne la date de fin de mois d'une date donnée
        /// </summary>
        /// <param name="dateDebut"></param>
        /// <returns></returns>
        public static DateTime GetEndMoisDate(DateTime dateDebut)
        {
            return new DateTime(dateDebut.Year, dateDebut.Month, DateTime.DaysInMonth(dateDebut.Year, dateDebut.Month));
        }

        public static int ConvertiHeureMinute(string heure)
        {
            if (heure.ToLower().Contains('h'))
            {
                var l = StringUtils.Split(heure, "h");
                var h = (!string.IsNullOrWhiteSpace(l[0])) ? int.Parse(l[0]) * 60 : 0;
                var m = (!string.IsNullOrWhiteSpace(l[1])) ? int.Parse(l[1]) : 0;
                return +h + m;
            }
            return 0;
        }

        /// <summary>
        /// Converti un nombre de minutes en heure format "xxhxx"
        /// </summary>
        /// <param name="minutes">le nombre de minutes</param>
        /// <returns>l'heure en format string</returns>
        public static string ConvertirMinutesHeure(int minutes)
        {
            var m = "";
            if ((minutes % 60) > 0)
            { m = (minutes % 60).ToString(); }
            return (minutes / 60) + "h" + m;
        }

        /// <summary>
        /// permet de connaitre le numéro de semaine de l'année d'une journée
        /// </summary>
        /// <param name="date">La date</param>
        /// <returns>le numéro de semaine</returns>
        public static int GetWeekOfYear(DateTime date)
        {
            var cultInfo = CultureInfo.CreateSpecificCulture("no");
            var cal = cultInfo.Calendar;
            var weekCount = cal.GetWeekOfYear(date, cultInfo.DateTimeFormat.CalendarWeekRule, cultInfo.DateTimeFormat.FirstDayOfWeek);
            return weekCount;
        }

        /// <summary>
        /// Calcul la date de paque
        /// </summary>
        /// <param name="year">L'année dont ont cherche à connaitre paque</param>
        /// <returns>la date de paque</returns>
        private static DateTime DateDePaque(int year)
        {
            int g, c, x, z, d, e, n, month, day;
            g = (year % 19) + 1;
            c = (year / 100) + 1;
            x = ((3 * c) / 4) - 12;
            z = ((8 * c + 5) / 25) - 5;
            d = ((5 * year) / 4) - x - 10;
            e = (11 * g + 20 + z - x) % 30;
            if (e < 0) e += 30;
            if (((e == 25) && (g > 11)) || (e == 24)) e++;
            n = 44 - e;
            if (n < 21) n += 30;
            n = n + 7 - ((d + n) % 7);
            if (n > 31)
            {
                month = 4;
                day = n - 31;
            }
            else
            {
                month = 3;
                day = n;
            }
            return new DateTime(year, month, day);
        }


        /// <summary>
        /// Recherche dans la liste des jours férié si le jour en paramètre est présent.
        /// </summary>
        /// <param name="d">le jour dont ont souhaite connaitre l'état</param>
        /// <returns>true si férié</returns>
        public static bool IsFerie(DateTime d)
        {
            var paque = DateDePaque(d.Year);
            if (paque.AddDays(1).Equals(d) || paque.AddDays(39).Equals(d) || paque.AddDays(50).Equals(d))
            { return true; }

            return ListeJourFerie.Any(ferie => ferie.Month == d.Month && ferie.Day == d.Day) ;
        }

        /// <summary>
        /// Indique si un jour est un jour de week end ou non
        /// </summary>
        /// <param name="d">la date du jour</param>
        /// <returns>true si week end</returns>
        public static bool IsWeekEnd(DateTime d)
        {
            var lettre = GetJourDiminutif(d);
            
            return (lettre.Equals("D")|| lettre.Equals("S"));
        }

        /// <summary>
        /// Retourne la plus petite des trois dates
        /// </summary>
        /// <param name="A">la première date obligatoire</param>
        /// <param name="B">la deuxième date nullable</param>
        /// <param name="C">la troisième date nullable</param>
        /// <returns>la plus petite date</returns>
        public static DateTime GetPlusGrandeDate(DateTime A, DateTime? B, DateTime? C)
        {
            if (B == null && C == null)
            {
                return A;
            }

            if (B != null && C == null)
            {
                return A < B ? B.Value : A;
            }

            if (B == null && C != null)
            {
                return A < C ? C.Value : A;
            }

            if (B != null && C != null)
            {
                var grande = B < C ? C.Value : B.Value;
                return A < grande ? grande : A;
            }
            return A;
        }
    }
}