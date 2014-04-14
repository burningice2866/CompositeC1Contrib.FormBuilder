using System;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EditWorkflowAttribute : Attribute
    {
        public Type EditWorkflowType { get; set; }

        public EditWorkflowAttribute(Type type)
        {
            EditWorkflowType = type;
        }
    }
}
