using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;

using Composite.C1Console.Forms;
using Composite.Core;
using Composite.Core.Types;
using Composite.Core.WebClient;
using Composite.Core.Xml;
using Composite.Functions;

using Composite.Plugins.Forms.WebChannel.UiControlFactories;

public partial class Composite_InstalledPackages_CompositeC1Contrib_Dynamic_FunctionSelector : UserControlBasedUiControl
{
    [BindableProperty]
    public string FunctionMarkup { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        btnDefaultValueFunctionMarkup.Attributes["label"] = GetCurrentlySelectedDefaultValueText();
        btnDefaultValueFunctionMarkup.Attributes["url"] =
            "${root}/content/dialogs/functions/editFunctionCall.aspx?zip_type="
            + UrlUtils.ZipContent(TypeManager.SerializeType(typeof(string)))
            + "&dialoglabel=" + HttpUtility.UrlEncodeUnicode("Select default value")
            + "&multimode=false&functionmarkup=";

        base.Render(writer);
    }

    public override void BindStateToControlProperties()
    {
        FunctionMarkup = btnDefaultValueFunctionMarkup.Value;
    }

    public override void InitializeViewState()
    {
        btnDefaultValueFunctionMarkup.Value = FunctionMarkup;
    }

    protected string GetCurrentlySelectedDefaultValueText()
    {
        if (!String.IsNullOrEmpty(btnDefaultValueFunctionMarkup.Value))
        {
            try
            {
                var functionElement = XElement.Parse(btnDefaultValueFunctionMarkup.Value);
                if (functionElement.Name.Namespace != Namespaces.Function10)
                {
                    functionElement = functionElement.Elements().First();
                }

                var widgetNode = (BaseFunctionRuntimeTreeNode)FunctionFacade.BuildTree(functionElement);

                return widgetNode.GetName();
            }
            catch (Exception ex)
            {
                Log.LogError("TypeFieldDesigner", String.Format("Widget settings reset. Existing widget failed to validate with the following message: '{0}'", ex.Message));
            }
        }

        return "(no default value)";
    }
}