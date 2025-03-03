using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.Strategy;
using GatiCarRental.Module.BusinessObjects;

namespace GatiCarRental.Win {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
    public partial class GatiCarRentalWindowsFormsApplication : WinApplication {
        private static Dictionary<string, bool> isCompatibilityChecked = new Dictionary<string, bool>();
        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
        static GatiCarRentalWindowsFormsApplication() {
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
			DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
        }
        private void InitializeDefaults() {
            LinkNewObjectToParentImmediately = false;
            OptimizedControllersCreation = true;
            UseLightStyle = true;
			SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
			ExecuteStartupLogicBeforeClosingLogonWindow = true;
        }
        #endregion
        public GatiCarRentalWindowsFormsApplication() {
            InitializeComponent();
			InitializeDefaults();
            ((SecurityStrategyComplex)Security).Authentication = new AuthenticationStandard<SecuritySystemUser, CustomLogonParametersForStandardAuthentication>();

        }

        protected override void OnLoggingOn(LogonEventArgs args)
        {
            base.OnLoggingOn(args);
            string targetDataBaseName = ((ICompanyNameParameter)args.LogonParameters).CompanyName;
            ObjectSpaceProvider.ConnectionString = MSSqlServerChangeDatabaseHelper.PatchConnectionString(targetDataBaseName, ConnectionString);
        }

        protected override bool IsCompatibilityChecked
        {
            get
            {
                return isCompatibilityChecked.ContainsKey(ConnectionString) ? isCompatibilityChecked[ConnectionString] : false;
            }

            set
            {
                isCompatibilityChecked[ConnectionString] = value;
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new SecuredObjectSpaceProvider((SecurityStrategyComplex)Security, XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true), false));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void GatiCarRentalWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
        private void GatiCarRentalWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if(System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
                e.Updater.Update();
                e.Handled = true;

    //            string message = "The application cannot connect to the specified database, " +
				//	"because the database doesn't exist, its version is older " +
				//	"than that of the application or its schema does not match " +
				//	"the ORM data model structure. To avoid this error, use one " +
				//	"of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

				//if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
				//	message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
				//}
				//throw new InvalidOperationException(message);
            }
#endif
        }
    }
}
