using System.Web.Mvc;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Objet permettant d'avoir une méthode de controleur associé à un bouton d'une vue
    /// </summary>
    public class SubmitButtonSelector : ActionNameSelectorAttribute
    {
        /// <summary>
        /// Le nom du bouton à associé
        /// </summary>
        public string Name { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, System.Reflection.MethodInfo methodInfo)
        {
            // Try to find out if the name exists in the data sent from form
            var value = controllerContext.Controller.ValueProvider.GetValue(Name);
            return value != null;
        }
    }
}