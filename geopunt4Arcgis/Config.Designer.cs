//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace geopunt4Arcgis {
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.ArcMapUI;
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.Desktop.AddIns;
    
    
    /// <summary>
    /// A class for looking up declarative information in the associated configuration xml file (.esriaddinx).
    /// </summary>
    internal static class ThisAddIn {
        
        internal static string Name {
            get {
                return "geopunt4Arcgis";
            }
        }
        
        internal static string AddInID {
            get {
                return "{9473f72a-33ce-402e-85c7-170c29b08419}";
            }
        }
        
        internal static string Company {
            get {
                return "AGIV";
            }
        }
        
        internal static string Version {
            get {
                return "1.4";
            }
        }
        
        internal static string Description {
            get {
                return "Het Vlaamse Geoportaal Geopunt biedt een aantal geografische diensten (web-servic" +
                    "es) aan, \ndie met deze add-in geïntegreed worden in Arcgis.";
            }
        }
        
        internal static string Author {
            get {
                return "Kay Warrie";
            }
        }
        
        internal static string Date {
            get {
                return "11/02/2015";
            }
        }
        
        internal static ESRI.ArcGIS.esriSystem.UID ToUID(this System.String id) {
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = id;
            return uid;
        }
        
        /// <summary>
        /// A class for looking up Add-in id strings declared in the associated configuration xml file (.esriaddinx).
        /// </summary>
        internal class IDs {
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntAddressCmd', the id declared for Add-in Button class 'geopuntAddressCmd'
            /// </summary>
            internal static string geopuntAddressCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntAddressCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntReverse', the id declared for Add-in Tool class 'geopuntReverseTool'
            /// </summary>
            internal static string geopuntReverseTool {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntReverse";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntBatchGeocodingCmd', the id declared for Add-in Button class 'geopuntBatchGeocodingCmd'
            /// </summary>
            internal static string geopuntBatchGeocodingCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntBatchGeocodingCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntGipodCmd', the id declared for Add-in Button class 'geopuntGipodCmd'
            /// </summary>
            internal static string geopuntGipodCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntGipodCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntPoiCmd', the id declared for Add-in Button class 'geopuntPoiCmd'
            /// </summary>
            internal static string geopuntPoiCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntPoiCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_capakeyCmd', the id declared for Add-in Button class 'geopuntCapakeyCmd'
            /// </summary>
            internal static string geopuntCapakeyCmd {
                get {
                    return "KGIS_geopunt4Arcgis_capakeyCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntElevation', the id declared for Add-in Button class 'geopuntElevationCmd'
            /// </summary>
            internal static string geopuntElevationCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntElevation";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntMetaCatalog', the id declared for Add-in Button class 'geopuntMetaCatalogCmd'
            /// </summary>
            internal static string geopuntMetaCatalogCmd {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntMetaCatalog";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_aboutCmd', the id declared for Add-in Button class 'aboutCmd'
            /// </summary>
            internal static string aboutCmd {
                get {
                    return "KGIS_geopunt4Arcgis_aboutCmd";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntBatchGeocodingTool', the id declared for Add-in Tool class 'geopuntBatchGeocodingTool'
            /// </summary>
            internal static string geopuntBatchGeocodingTool {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntBatchGeocodingTool";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopuntElevationTool', the id declared for Add-in Tool class 'geopuntElevationTool'
            /// </summary>
            internal static string geopuntElevationTool {
                get {
                    return "KGIS_geopunt4Arcgis_geopuntElevationTool";
                }
            }
            
            /// <summary>
            /// Returns 'KGIS_geopunt4Arcgis_geopunt4arcgisExtension', the id declared for Add-in Extension class 'geopunt4arcgisExtension'
            /// </summary>
            internal static string geopunt4arcgisExtension {
                get {
                    return "KGIS_geopunt4Arcgis_geopunt4arcgisExtension";
                }
            }
        }
    }
    
internal static class ArcMap
{
  private static IApplication s_app = null;
  private static IDocumentEvents_Event s_docEvent;

  public static IApplication Application
  {
    get
    {
      if (s_app == null)
        s_app = Internal.AddInStartupObject.GetHook<IMxApplication>() as IApplication;

      return s_app;
    }
  }

  public static IMxDocument Document
  {
    get
    {
      if (Application != null)
        return Application.Document as IMxDocument;

      return null;
    }
  }
  public static IMxApplication ThisApplication
  {
    get { return Application as IMxApplication; }
  }
  public static IDockableWindowManager DockableWindowManager
  {
    get { return Application as IDockableWindowManager; }
  }
  public static IDocumentEvents_Event Events
  {
    get
    {
      s_docEvent = Document as IDocumentEvents_Event;
      return s_docEvent;
    }
  }
}

namespace Internal
{
  [StartupObjectAttribute()]
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public sealed partial class AddInStartupObject : AddInEntryPoint
  {
    private static AddInStartupObject _sAddInHostManager;
    private List<object> m_addinHooks = null;

    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    public AddInStartupObject()
    {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool Initialize(object hook)
    {
      bool createSingleton = _sAddInHostManager == null;
      if (createSingleton)
      {
        _sAddInHostManager = this;
        m_addinHooks = new List<object>();
        m_addinHooks.Add(hook);
      }
      else if (!_sAddInHostManager.m_addinHooks.Contains(hook))
        _sAddInHostManager.m_addinHooks.Add(hook);

      return createSingleton;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void Shutdown()
    {
      _sAddInHostManager = null;
      m_addinHooks = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal static T GetHook<T>() where T : class
    {
      if (_sAddInHostManager != null)
      {
        foreach (object o in _sAddInHostManager.m_addinHooks)
        {
          if (o is T)
            return o as T;
        }
      }

      return null;
    }

    // Expose this instance of Add-in class externally
    public static AddInStartupObject GetThis()
    {
      return _sAddInHostManager;
    }
  }
}
}
