
 
 
 
 

 
 
 
 
 
 


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
		 /// <summary>You need to fill in some fields</summary> 
 public static string Validation_ErrorNotification_Header { get { return T("Validation.ErrorNotification.Header"); } } 
 /// <summary>Please fill in the fields below and resubmit</summary> 
 public static string Validation_ErrorNotification_Footer { get { return T("Validation.ErrorNotification.Footer"); } } 
 /// <summary>Choose</summary> 
 public static string Widgets_Dropdown_SelectLabel { get { return T("Widgets.Dropdown.SelectLabel"); } } 
 /// <summary>Captcha</summary> 
 public static string Captcha_Label { get { return T("Captcha.Label"); } } 
 /// <summary>Write the text from the image</summary> 
 public static string Captcha_Help { get { return T("Captcha.Help"); } } 
 /// <summary>Text has to match the image</summary> 
 public static string Captcha_Error { get { return T("Captcha.Error"); } } 
     private static string T(string key) 
       { 
            return StringResourceSystemFacade.GetString("CompositeC1Contrib.FormBuilder", key);
        }

	}
}


