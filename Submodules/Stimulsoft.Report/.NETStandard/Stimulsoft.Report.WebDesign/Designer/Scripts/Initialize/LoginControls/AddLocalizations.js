
StiMobileDesigner.prototype.AddLocalizations = function () {
    this.options.defaultLocalization = this.GetDefaultLocalization();

    this.loc.Navigator = {
        ButtonSettings: this.loc.Export.Settings,
        ButtonChangePassword: this.loc.Cloud.ButtonChangePassword,
        ButtonAbout: this.loc.MainMenu.menuHelpAboutProgramm.replace("&", "").replace("...", ""),
        ButtonLogout: this.loc.Cloud.ButtonLogout,
        TextProfile: this.loc.Cloud.TextProfile
    }

    this.loc.Administration = {
        LabelNewPassword: this.loc.Cloud.LabelNewPassword.replace(":", ""),
        LabelCurrentPassword: this.loc.Cloud.LabelCurrentPassword.replace(":", ""),
        LabelPicture: this.loc.Cloud.LabelPicture.replace(":", ""),
        LabelRole: this.loc.Cloud.TextRole,
        TextRole: this.loc.Cloud.TextRole,
        TextUser: this.loc.Cloud.TextUser,
        TextDate: this.loc.FormFormatEditor.Date,
        LabelFirstName: this.loc.Cloud.TextFirstName,
        LabelLastName: this.loc.Cloud.TextLastName,
        LabelUserName: this.loc.Cloud.TextUserName,
        LabelLastLogin: this.loc.Cloud.LabelLastLogin
    }

    this.loc.File = {
        MessageFailedAddFollowingFiles: "Failed to add the following files. Exceeded the size {0} MB."
    }

    this.loc.Common = {
        ButtonBack: this.loc.Wizards.ButtonBack.replace("&", ""),
        LabelDescription: this.loc.PropertyMain.Description,
        LabelCreated: this.loc.Cloud.LabelCreated.replace(":", ""),
        LabelModified: this.loc.Cloud.LabelModified.replace(":", ""),
        ButtonOK: this.loc.Buttons.Ok.replace("&", ""),
        ButtonCancel: this.loc.Buttons.Cancel.replace("&", "")
    }

    this.loc.Authorization = {
        ButtonLogin: this.loc.Cloud.Login,
        ButtonResendEmail: this.loc.Cloud.ButtonResendEmail,
        ButtonResetPassword: this.loc.Cloud.ButtonResetPassword,
        ButtonSignUp: this.loc.Cloud.ButtonSignUp,
        ButtonSendEmail: this.loc.FormViewer.SendEMail.replace("...", ""),

        CheckBoxEnabled: this.loc.PropertyMain.Enabled,
        CheckBoxRememberMe: this.loc.Cloud.CheckBoxRememberMe,

        HyperlinkBack: this.loc.Wizards.ButtonBack.replace("&", ""),
        HyperlinkAgreeToTerms: this.loc.Cloud.HyperlinkAgreeToTerms,
        HyperlinkAlreadyHaveAccount: this.loc.Cloud.HyperlinkAlreadyHaveAccount,
        HyperlinkForgotPassword: this.loc.Cloud.HyperlinkForgotPassword,
        HyperlinkHavePassword: this.loc.Cloud.HyperlinkHavePassword,

        TextCreate: this.loc.Cloud.Create,
        TextView: this.loc.Cloud.ButtonView,
        TextModify: this.loc.Cloud.TextModify,
        TextDelete: this.loc.Buttons.Delete,
        TextRegistrationSuccessfully: this.loc.Messages.TextRegistrationSuccessfully,
        TextRun: this.loc.Cloud.ButtonRun,
        TextVerifyEmail: "Verify your email address at {0} to ensure your account can be recovered.",

        TextFirstName: this.loc.Cloud.TextFirstName,
        TextLastName: this.loc.Cloud.TextLastName,
        TextPassword: this.loc.Password.StiSavePasswordForm,
        TextUserName: this.loc.Cloud.TextUserName,

        LabelFirstName: this.loc.Cloud.TextFirstName,
        LabelLastName: this.loc.Cloud.TextLastName,
        LabelUserName: this.loc.Cloud.TextUserName,

        WindowTitleForgotPassword: this.loc.Cloud.WindowTitleForgotPassword,
        WindowTitleLogin: this.loc.Cloud.WindowTitleLogin,
        WindowTitleSignUp: this.loc.Cloud.WindowTitleSignUp
    };

    this.loc.Options = {
        TabInterface: "Interface",
        TabLanguage: this.loc.PropertyMain.Language,
        LabelBackground: this.loc.Report.LabelBackground.replace(":", ""),
        LabelForeground: this.loc.Cloud.LabelForeground.replace(":", "")
    }

    this.loc.License = {
        WindowTitleLicense: this.loc.NuGet.License.replace(":", "")
    }

    this.loc.Update = {
        ButtonIAgree: "I Agree"
    }
}