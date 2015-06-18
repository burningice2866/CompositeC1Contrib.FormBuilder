
 
 
 
 

 
 
 
 
 
 


using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.FormBuilder
{
	/// <summary>    
	/// Class generated from localization files  
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
	public static class Localization
	{
		 /// <summary>&quot;You need to fill in some fields&quot;</summary> 
 public static string Validation_ErrorNotification_Header { get { return T("Validation.ErrorNotification.Header"); } } 
 /// <summary>&quot;Please fill in the fields below and resubmit&quot;</summary> 
 public static string Validation_ErrorNotification_Footer { get { return T("Validation.ErrorNotification.Footer"); } } 
 /// <summary>&quot;Choose&quot;</summary> 
 public static string Widgets_Dropdown_SelectLabel { get { return T("Widgets.Dropdown.SelectLabel"); } } 
 /// <summary>&quot;Write the text from the image&quot;</summary> 
 public static string Captcha_CompositeC1_Label { get { return T("Captcha.CompositeC1.Label"); } } 
 /// <summary>&quot;Write the text from the image&quot;</summary> 
 public static string Captcha_CompositeC1_Help { get { return T("Captcha.CompositeC1.Help"); } } 
 /// <summary>&quot;Text has to match the image&quot;</summary> 
 public static string Captcha_CompositeC1_Error { get { return T("Captcha.CompositeC1.Error"); } } 
 /// <summary>&quot;Please tick here&quot;</summary> 
 public static string Captcha_GoogleReCAPTCHA_Label { get { return T("Captcha.GoogleReCAPTCHA.Label"); } } 
 /// <summary>&quot;To prevent abuse of this form, please tick the box. Possibly you will be prompted to enter a text that appears as an image.&quot;</summary> 
 public static string Captcha_GoogleReCAPTCHA_Help { get { return T("Captcha.GoogleReCAPTCHA.Help"); } } 
 /// <summary>&quot;Please mark&quot;</summary> 
 public static string Captcha_GoogleReCAPTCHA_Error { get { return T("Captcha.GoogleReCAPTCHA.Error"); } } 
     private static string T(string key) 
       { 
            return StringResourceSystemFacade.GetString("CompositeC1Contrib.FormBuilder", key);
        }

	}
}


