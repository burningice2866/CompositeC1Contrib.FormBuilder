using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Composite.C1Console.Forms.CoreUiControls;
using Composite.Core.Linq;
using Composite.Core.Types;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.GeneratedTypes;
using Composite.Data.Types;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class CompositeFormRendererConverter
    {
        private readonly StringBuilder _log;

        public CompositeFormRendererConverter(StringBuilder log)
        {
            _log = log;
        }

        public void ConvertForms(bool replaceforms)
        {
            var formdefinitions = new Dictionary<string, DynamicFormDefinition>();

            foreach (var publicationScope in new[] { PublicationScope.Unpublished, PublicationScope.Published })
            {
                using (var data = new DataConnection(publicationScope))
                {
                    foreach (var ph in data.Get<IPagePlaceholderContent>().ToList())
                    {
                        if (String.IsNullOrEmpty(ph.Content))
                        {
                            continue;
                        }

                        var content = XElement.Parse(ph.Content);
                        var forms = content.Descendants(Namespaces.Function10 + "function").Where(f => f.Attribute("name").Value == "Composite.Forms.Renderer").ToList();

                        foreach (var formular in forms)
                        {
                            var pageNode = SiteMap.Provider.FindSiteMapNodeFromKey(ph.PageId.ToString());
                            var dataTypeName = GetFunctionParamValue(formular, "DataType");

                            var dataType = TypeManager.TryGetType(dataTypeName);
                            if (dataType == null)
                            {
                                continue;
                            }

                            var dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(dataType);
                            if (dataTypeDescriptor == null)
                            {
                                continue;
                            }

                            dataTypeName = dataTypeName.Replace("DynamicType:", String.Empty);

                            _log.AppendLine("datatype = " + dataType.FullName + " PageId = " + ph.PageId + (pageNode != null ? " Url = " + pageNode.Url : String.Empty));

                            var generatedTypesHelper = new GeneratedTypesHelper(dataTypeDescriptor);
                            var formHelper = new DataTypeDescriptorFormsHelper(dataTypeDescriptor);

                            formHelper.AddReadOnlyFields(generatedTypesHelper.NotEditableDataFieldDescriptorNames);

                            var formXml = XElement.Parse(formHelper.GetForm());
                            var formdefinitionKey = formular.ToString(SaveOptions.OmitDuplicateNamespaces);

                            if (formdefinitions.ContainsKey(formdefinitionKey))
                            {
                                formular.ReplaceWith(new XElement(Namespaces.Function10 + "function",
                                    new XAttribute("name", formdefinitions[formdefinitionKey].Name)));

                                continue;
                            }

                            var variant = 0;

                            while (
                                formdefinitions.Values.Any(
                                    f => f.Name == (dataTypeName + (variant == 0 ? String.Empty : variant.ToString(CultureInfo.InvariantCulture)))))
                            {
                                variant++;
                            }

                            dataTypeName = dataTypeName + (variant == 0 ? String.Empty : variant.ToString(CultureInfo.InvariantCulture));

                            var dfd = new DynamicFormDefinition(dataTypeName);
                            formdefinitions.Add(formdefinitionKey, dfd);

                            var introText = GetFunctionOptionalParamValue(formular, "IntroText");
                            if (!String.IsNullOrEmpty(introText))
                            {
                                dfd.IntroText = XhtmlDocument.Parse(introText);
                            }

                            var responseText = GetFunctionOptionalParamValue(formular, "ResponseText");
                            if (!String.IsNullOrEmpty(responseText))
                            {
                                dfd.SuccessResponse = XhtmlDocument.Parse(responseText);
                            }

                            var responseUrl = GetFunctionOptionalParamValue(formular, "ResponseUrl");
                            if (!String.IsNullOrEmpty(responseUrl))
                            {
                                dfd.SuccessResponse = new XhtmlDocument();

                                dfd.SuccessResponse.Body.Add(
                                    XElement.Parse(
                                        String.Format(
                                            "<f:function name=\"Composite.Web.Response.Redirect\" xmlns:f=\"http://www.composite.net/ns/function/1.0\"><f:param name=\"Url\" value=\"{0}\" /></f:function>")),
                                    responseUrl);
                            }

                            var sendButtonLabel = GetFunctionOptionalParamValue(formular, "sendButtonLabel");
                            if (!String.IsNullOrEmpty(sendButtonLabel))
                            {
                                dfd.Model.Attributes.Add(new SubmitButtonLabelAttribute(sendButtonLabel));
                            }

                            var resetButtonLabel = GetFunctionOptionalParamValue(formular, "ResetButtonLabel");
                            if (!String.IsNullOrEmpty(resetButtonLabel))
                            {
                                //TODO formbuilder do not support reset button
                                //throw new NotImplementedException();
                            }

                            ResolveFields(dataTypeDescriptor, formXml, dfd);

                            var importFormMails = ImportMailHandlers(formular, dfd);
                            if (!importFormMails)
                            {
                                var now = DateTime.Now;

                                dfd.SubmitHandlers.Add(new SaveFormSubmitHandler
                                {
                                    Name = "Save",
                                    IncludeAttachments = false
                                });

                                var instances = DataFacade.GetData(dataType).ToListOfObjects();
                                for (int i = 0; i < instances.Count; i++)
                                {
                                    var instance = instances[i];

                                    foreach (var field in dfd.Model.Fields)
                                    {
                                        var prop = instance.GetType().GetProperty(field.Name);
                                        if (prop == null)
                                        {
                                            continue;
                                        }

                                        field.Value = prop.GetValue(instance, null);
                                    }

                                    SaveFormSubmitFacade.SaveSubmit(dfd.Model, false, now.AddSeconds(i));
                                }
                            }

                            DynamicFormsFacade.SaveForm(dfd);

                            formular.ReplaceWith(new XElement(Namespaces.Function10 + "function", new XAttribute("name", dfd.Name)));

                            if (!replaceforms)
                            {
                                continue;
                            }

                            ph.Content = content.ToString();
                            data.Update(ph);
                        }
                    }
                }
            }

            _log.AppendLine(formdefinitions.Count + " forms created");
        }

        private static void ResolveFields(DataTypeDescriptor dataTypeDescriptor, XElement formXml, DynamicFormDefinition dfd)
        {
            foreach (var element in formXml.Descendants())
            {
                FormField field = null;

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "TextBox")
                {
                    var fieldName = GetFormBindingFieldName(element);

                    if (String.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    var fieldType = element.Attribute("Type") != null ? element.Attribute("Type").Value : null;

                    if (fieldType == TextBoxType.Password.ToString())
                    {
                        field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                        field.Attributes.Add(new PasswordInputElementAttribute());
                    }
                    else if (fieldType == TextBoxType.Integer.ToString())
                    {
                        field = new FormField(dfd.Model, fieldName, typeof(int), new List<Attribute>());

                        field.Attributes.Add(new TextboxInputElementAttribute());
                        field.Attributes.Add(new IntegerFieldValidatorAttribute("skal være et tal"));
                    }
                    else if (fieldType == TextBoxType.Decimal.ToString())
                    {
                        field = new FormField(dfd.Model, fieldName, typeof(decimal), new List<Attribute>());

                        field.Attributes.Add(new TextboxInputElementAttribute());
                        field.Attributes.Add(new DecimalFieldValidatorAttribute("skal være et decimal tal"));
                    }
                    else if (fieldType == TextBoxType.ReadOnly.ToString())
                    {
                        field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                        field.Attributes.Add(new TextboxInputElementAttribute());
                        field.IsReadOnly = true;
                    }
                    else
                    {
                        field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                        field.Attributes.Add(new TextboxInputElementAttribute());
                    }
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "TextArea" ||
                    element.Name == Namespaces.BindingFormsStdUiControls10 + "InlineXhtmlEditor")
                {
                    var fieldName = GetFormBindingFieldName(element);
                    if (String.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                    field.Attributes.Add(new TextAreaInputElementAttribute());
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "DateSelector")
                {
                    var fieldName = GetFormBindingFieldName(element);
                    if (String.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    field = new FormField(dfd.Model, fieldName, typeof(DateTime), new List<Attribute>());

                    field.Attributes.Add(new TextboxInputElementAttribute());
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "BoolSelector")
                {
                    var fieldName = GetFormBindingFieldName(element);
                    if (String.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                    field.Attributes.Add(new RadioButtonInputElementAttribute());
                    field.Attributes.Add(new StringBasedDataSourceAttribute(element.Attribute("TrueLabel").Value, element.Attribute("FalseLabel").Value));
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "CheckBox")
                {
                    var fieldName = GetFormBindingFieldName(element);
                    if (String.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    field = new FormField(dfd.Model, fieldName, typeof(string), new List<Attribute>());

                    field.Attributes.Add(new CheckboxInputElementAttribute());

                    if (element.Attribute("ItemLabel") != null && !String.IsNullOrEmpty(element.Attribute("ItemLabel").Value))
                    {
                        field.Attributes.Add(new StringBasedDataSourceAttribute(element.Attribute("ItemLabel").Value));
                    }
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "MultiKeySelector")
                {
                    var fieldName = GetFormBindingFieldName(element);

                    //TODO not in use on dhf
                    throw new NotImplementedException();
                }

                if (element.Name == Namespaces.BindingFormsStdUiControls10 + "Selector")
                {
                    var fieldName = GetFormBindingFieldName(element);

                    //TODO not in use on dhf
                    throw new NotImplementedException();
                }

                if (field == null)
                {
                    continue;
                }

                var datatypeField = dataTypeDescriptor.Fields[field.Name];

                var label = element.Attributes("Label").Select(f => f.Value).FirstOrDefault();
                if (!String.IsNullOrEmpty(label))
                {
                    field.Attributes.Add(new FieldLabelAttribute(label));
                    field.Attributes.Add(new PlaceholderTextAttribute(label));
                }

                var help = element.Attributes("Help").Select(f => f.Value).FirstOrDefault();
                if (!String.IsNullOrEmpty(help))
                {
                    field.Attributes.Add(new FieldHelpAttribute(help));
                }

                if (!datatypeField.IsNullable)
                {
                    field.Attributes.Add(new RequiredFieldAttribute("Skal udfyldes"));
                }

                ImportDefaultValue(dfd, field, datatypeField);

                dfd.Model.Fields.Add(field);
            }
        }

        private static void ImportDefaultValue(DynamicFormDefinition dfd, FormField field, DataFieldDescriptor datatypeField)
        {
            XElement defaultvalue = null;

            if (datatypeField.DefaultValue != null)
            {
                switch (datatypeField.DefaultValue.ValueType)
                {
                    case DefaultValueType.Boolean:

                        var boolValue = (bool)datatypeField.DefaultValue.Value;
                        if (boolValue)
                        {
                            defaultvalue = new XElement(Namespaces.Function10 + "function",
                                new XAttribute("name", "Composite.Constant.Boolean"),
                                new XElement(Namespaces.Function10 + "param",
                                    new XAttribute("name", "Constant"),
                                    new XAttribute("value", datatypeField.DefaultValue.Value)));
                        }

                        break;

                    case DefaultValueType.DateTime:

                        defaultvalue = new XElement(Namespaces.Function10 + "function",
                            new XAttribute("name", "Composite.Constant.DateTime"),
                            new XElement(Namespaces.Function10 + "param",
                                new XAttribute("name", "Constant"),
                                new XAttribute("value", datatypeField.DefaultValue.Value)));

                        break;

                    case DefaultValueType.DateTimeNow:

                        defaultvalue = new XElement(Namespaces.Function10 + "function",
                            new XAttribute("name", "Composite.Utils.Date.Now"));

                        break;

                    case DefaultValueType.Decimal:

                        var decimalValue = (Decimal)datatypeField.DefaultValue.Value;
                        if (!decimalValue.Equals(Decimal.Zero))
                        {
                            defaultvalue = new XElement(Namespaces.Function10 + "function",
                                new XAttribute("name", "Composite.Constant.Decimal"),
                                new XElement(Namespaces.Function10 + "param",
                                    new XAttribute("name", "Constant"),
                                    new XAttribute("value", datatypeField.DefaultValue.Value)));
                        }

                        break;

                    case DefaultValueType.Guid:

                        defaultvalue = new XElement(Namespaces.Function10 + "function",
                            new XAttribute("name", "Composite.Constant.Guid"),
                            new XElement(Namespaces.Function10 + "param",
                                new XAttribute("name", "Constant"),
                                new XAttribute("value", datatypeField.DefaultValue.Value)));

                        break;

                    case DefaultValueType.Integer:

                        var intValue = (int)datatypeField.DefaultValue.Value;
                        if (!intValue.Equals(0))
                        {
                            defaultvalue = new XElement(Namespaces.Function10 + "function",
                                new XAttribute("name", "Composite.Constant.Integer"),
                                new XElement(Namespaces.Function10 + "param",
                                    new XAttribute("name", "Constant"),
                                    new XAttribute("value", datatypeField.DefaultValue.Value)));
                        }

                        break;

                    case DefaultValueType.NewGuid:

                        defaultvalue = new XElement(Namespaces.Function10 + "function",
                            new XAttribute("name", "Composite.Utils.Guid.NewGuid"));

                        break;

                    case DefaultValueType.String:

                        var stringValue = (string)datatypeField.DefaultValue.Value;
                        if (!String.IsNullOrEmpty(stringValue))
                        {
                            defaultvalue = new XElement(Namespaces.Function10 + "function",
                                new XAttribute("name", "Composite.Constant.String"),
                                new XElement(Namespaces.Function10 + "param",
                                    new XAttribute("name", "Constant"),
                                    new XAttribute("value", datatypeField.DefaultValue.Value)));
                        }

                        break;
                }
            }

            if (datatypeField.NewInstanceDefaultFieldValue != null)
            {
                defaultvalue = XElement.Parse(datatypeField.NewInstanceDefaultFieldValue);
            }

            if (defaultvalue != null && !dfd.DefaultValues.ContainsKey(field.Name))
            {
                dfd.DefaultValues.Add(field.Name, defaultvalue);
            }
        }

        private static bool ImportMailHandlers(XElement formular, DynamicFormDefinition dfd)
        {
            var fromEmails = formular.Descendants(Namespaces.Function10 + "function")
                    .Where(f => f.Attribute("name").Value == "Composite.Forms.FormEmail")
                    .ToArray();

            for (int i = 0; i < fromEmails.Length; i++)
            {
                var fromEmail = fromEmails[i];

                var from = GetFunctionParamValue(fromEmail, "From");
                var to = fromEmail.Elements(Namespaces.Function10 + "param")
                        .Where(f => f.Attribute("name").Value == "To")
                        .Select(f => f.Attribute("value") != null
                            ? f.Attribute("value").Value
                            : f.Descendants()
                                .Where(j => j.Attribute("name").Value == "PropertyName")
                                .Select(j => "%" + j.Attribute("value").Value + "%")
                                .First()).FirstOrDefault();

                var cc = GetFunctionOptionalParamValue(fromEmail, "IntroText");
                var subject = GetFunctionParamValue(fromEmail, "Subject");
                var body = fromEmail.Elements(Namespaces.Function10 + "param")
                        .Where(f => f.Attribute("name").Value == "Body")
                        .Elements()
                        .FirstOrDefault();

                var bodyDocument = body != null ? new XhtmlDocument(body) : new XhtmlDocument();

                var appendFormData = Convert.ToBoolean(GetFunctionOptionalParamValue(fromEmail, "AppendFormData") ?? "true");
                if (appendFormData)
                {
                    bodyDocument.Body.Add(
                        new XElement(Namespaces.Function10 + "function",
                            new XAttribute("name", "FormBuilder.DumpSubmittedFormValues"))
                        );
                }

                dfd.SubmitHandlers.Add(new EmailSubmitHandler
                {
                    Name = "Email" + (i + 1),
                    IncludeAttachments = appendFormData,
                });

                using (var data = new DataConnection())
                {
                    var template = data.CreateNew<IMailTemplate>();

                    template.Key = dfd.Name + "." + "Email" + (i + 1);
                    template.From = from;
                    template.To = to;
                    template.Cc = cc;
                    template.Subject = subject;
                    template.Body = body.ToString();
                    template.EncryptMessage = false;
                    template.EncryptPassword = String.Empty;

                    data.Add(template);
                }
            }

            return fromEmails.Length > 0;
        }

        private static string GetFormBindingFieldName(XElement element)
        {
            return element.Descendants(Namespaces.BindingForms10 + "bind")
                .Select(f => f.Attribute("source").Value)
                .FirstOrDefault() ??
                   element.Descendants(Namespaces.BindingForms10 + "read")
                       .Select(f => f.Attribute("source").Value)
                       .FirstOrDefault();
        }

        private static string GetFunctionOptionalParamValue(XElement function, string name)
        {
            return function.Elements(Namespaces.Function10 + "param")
                .Where(f => f.Attribute("name").Value == name)
                .Select(f => f.Attribute("value").Value)
                .FirstOrDefault();
        }

        private static string GetFunctionParamValue(XElement function, string name)
        {
            return function.Elements(Namespaces.Function10 + "param")
                .First(f => f.Attribute("name").Value == name)
                .Attribute("value")
                .Value;
        }
    }
}
