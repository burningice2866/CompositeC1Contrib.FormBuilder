using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    [ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
    public class FormBuilderActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup _actionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);
        private static readonly ActionLocation _actionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = _actionGroup };

        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            return Enumerable.Empty<ElementAction>();
        }
    }
}