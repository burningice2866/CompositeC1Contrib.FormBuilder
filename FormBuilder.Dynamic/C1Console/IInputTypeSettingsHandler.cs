namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    public interface IInputTypeSettingsHandler
    {
        void Load(FormFieldModel field);
        void Save(FormFieldModel field);
    }
}
