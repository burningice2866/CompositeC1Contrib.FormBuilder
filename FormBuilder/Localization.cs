
 
 
 
 

 
 
 
 
 
 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.FormBuilder
{
	/// <summary>    
	/// Class generated from localization files  
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
	internal static class Localization
	{
		 /// <summary>You need to fill in some fields:</summary> 
 public static string Validation_ErrorNotificationTop { get { return T("Validation.ErrorNotificationTop"); } } 
 /// <summary>Please fill in the fields below and resubmit.</summary> 
 public static string Validation_ErrorNotificationBottom { get { return T("Validation.ErrorNotificationBottom"); } } 
 /// <summary>Choose</summary> 
 public static string Widgets_Dropdown_SelectLabel { get { return T("Widgets.Dropdown.SelectLabel"); } } 
     private static string T(string key) 
       { 
            return StringResourceSystemFacade.GetString("CompositeC1Contrib.FormBuilder", key);
        }

	}
}


