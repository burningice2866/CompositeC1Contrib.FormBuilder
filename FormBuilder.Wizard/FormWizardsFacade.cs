using System;
using System.Collections.Generic;
using System.IO;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizardsFacade
    {
        private static readonly IList<Action> FormWizardChangeNotifications = new List<Action>();

        public static readonly string WizardsPath = Path.Combine(FormModelsFacade.RootPath, "Wizards");

        public static void SubscribeToFormWizardChanges(Action notify)
        {
            FormWizardChangeNotifications.Add(notify);
        }

        public static void NotifyChanges()
        {
            foreach (var action in FormWizardChangeNotifications)
            {
                action();
            }
        }
    }
}
