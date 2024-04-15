#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Base
{
    #region StiAnimationType
    public enum StiAnimationType
    {
        Opacity,
        Scale,
        Translation,
        Rotation,
        Column,
        Points,
        PieSegment
    }
    #endregion

    #region StiOutputType
    /// <summary>
    /// Types of the result to compiling.
    /// </summary>
    public enum StiOutputType
    {
        /// <summary>
        /// Class library.
        /// </summary>
        ClassLibrary,

        /// <summary>
        /// Console application.
        /// </summary>
        ConsoleApplication,

        /// <summary>
        /// Windows application.
        /// </summary>
        WindowsApplication
    }
    #endregion

    #region StiLexerError
    /// <summary>
    /// Defines identifiers that indicate the type of lexical analysis error.
    /// </summary>
    public enum StiLexerError
    {
        /// <summary>
        /// Left paren not found.
        /// </summary>
        LParenNotFound,
        /// <summary>
        /// Comma not found.
        /// </summary>
        CommaNotFound,
        /// <summary>
        /// Assing not found.
        /// </summary>
        AssignNotFound,
        /// <summary>
        /// Right paren not found.
        /// </summary>
        RParenNotFound,
        /// <summary>
        /// Left brace not found.
        /// </summary>
        LBraceNotFound,
        /// <summary>
        /// Semicolon not found.
        /// </summary>
        SemicolonNotFound,
        /// <summary>
        /// Right brace not found.
        /// </summary>
        RBraceNotFound
    }
    #endregion

    #region StiTokenType
    /// <summary>
    /// Types of token.
    /// </summary>
    public enum StiTokenType
    {
        /// <summary>
        /// None token.
        /// </summary>
        None = 0,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// ;
        /// </summary>
        SemiColon,
        /// <summary>
        /// Shift to the left Token.
        /// </summary>
        Shl,
        /// <summary>
        /// Shift to the right Token.
        /// </summary>
        Shr,
        /// <summary>
        /// Assign Token.
        /// </summary>
        Assign,
        /// <summary>
        /// Equal Token.
        /// </summary>
        Equal,
        /// <summary>
        /// NotEqual Token.
        /// </summary>
        NotEqual,
        /// <summary>
        /// LeftEqual Token.
        /// </summary>
        LeftEqual,
        /// <summary>
        /// Left Token.
        /// </summary>
        Left,
        /// <summary>
        /// RightEqual Token.
        /// </summary>
        RightEqual,
        /// <summary>
        /// Right Token.
        /// </summary>
        Right,
        /// <summary>
        /// Logical OR Token.
        /// </summary>
        Or,
        /// <summary>
        /// Logical AND Token.
        /// </summary>
        And,
        /// <summary>
        /// Logical NOT Token.
        /// </summary>
        Not,
        /// <summary>
        /// Double logical OR Token.
        /// </summary>
        DoubleOr,
        /// <summary>
        /// Double logical AND Token.
        /// </summary>
        DoubleAnd,
        /// <summary>
        /// Copyright
        /// </summary>
        Copyright,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// *
        /// </summary>
        Mult,
        /// <summary>
        /// /
        /// </summary>
        Div,
        /// <summary>
        /// \
        /// </summary>
        Splash,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// @
        /// </summary>
        Ampersand,
        /// <summary>
        /// #
        /// </summary>
        Sharp,
        /// <summary>
        /// $
        /// </summary>
        Dollar,
        /// <summary>
        /// €
        /// </summary>
        Euro,
        /// <summary>
        /// ++
        /// </summary>
        DoublePlus,
        /// <summary>
        /// --
        /// </summary>
        DoubleMinus,
        /// <summary>
        /// (
        /// </summary>
        LPar,
        /// <summary>
        /// )
        /// </summary>
        RPar,
        /// <summary>
        /// {
        /// </summary>
        LBrace,
        /// <summary>
        /// }
        /// </summary>
        RBrace,
        /// <summary>
        /// [
        /// </summary>
        LBracket,
        /// <summary>
        /// ]
        /// </summary>
        RBracket,
        /// <summary>
        /// Token contains value.
        /// </summary>
        Value,
        /// <summary>
        /// Token contains identifier.
        /// </summary>
        Ident,
        /// <summary>
        /// 
        /// </summary>
        Unknown,
        /// <summary>
        /// EOF Token.
        /// </summary>
        EOF
    }
    #endregion

    #region StiSqlParserType
    public enum StiSqlParserType
    {
        Number,
        Date,
        String
    }
    #endregion

    #region StiGuiMode
    public enum StiGuiMode
    {
        Gdi,
        Wpf,
        Cloud
    }
    #endregion

    #region StiLevel
    /// <summary>
    /// Enums provides levels of access to property.
    /// </summary>
    public enum StiLevel
    {
        /// <summary>
        /// Minimal level of properties access. Only principal properties of components.
        /// </summary>
        Basic,
        /// <summary>
        /// Standard level of properties access. All properties available except rarely used properties.
        /// </summary>
        Standard,
        /// <summary>
        /// Professional level of properties access. All properties available.
        /// </summary>
        Professional
    }
    #endregion

    #region StiImageSize
    public enum StiImageSize
    {
        Normal,
        OneHalf,
        Double
    }
    #endregion

    #region SystemScaleID
    public enum SystemScaleID
    {
        x1 = 0,
        x2 = 1,
        x3 = 2,
        x4 = 3
    }
    #endregion

    #region SystemScaleIconID
    public enum SystemScaleIconID
    {
        x100,
        x125,
        x150,
        x175,
        x200,
        x225,
        x250,
        x275,
        x300,
        x325,
        x350,
        x375,
        x400,
    }
    #endregion

    #region StiJsonSaveMode
    public enum StiJsonSaveMode
    {
        Report,
        Document
    }
    #endregion

    #region StiJsonConverterVersion
    public enum StiJsonConverterVersion
    {
        ConverterV1,
        ConverterV2
    }
    #endregion    

    #region StiAutoBool
    public enum StiAutoBool
    {
        Auto,
        True,
        False
    }
    #endregion

    #region StiNoticeIdent
    /// <summary>
    /// Enumeration contains all possible server notice idents.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiNoticeIdent
    {
        ActivationMaxActivationsReached = 1,
        ActivationExpiriedBeforeFirstRelease,
        ActivationLicenseIsNotCorrect,
        ActivationLockedAccount,
        ActivationTrialExpired,
        ActivationServerVersionNotAllowed,
        ActivationServerIsNotAvailableNow,
        ActivationSomeTroublesOccurred,
        ActivationUserNameOrPasswordIsWrong,
        ActivationWrongAccountType,

        Alert,

        AuthAccountCantBeUsedNow,
        AuthAccountIsNotActivated,
        AuthCantChangeSystemRole,
        AuthCantChangeRoleBecauseLastAdministratorUser,
        AuthCantChangeRoleBecauseLastSupervisorUser,
        AuthCantDeleteHimselfUser,
        AuthCantDeleteLastAdministratorUser,
        AuthCantDeleteLastSupervisorUser,
        AuthCantDeleteSystemRole,
        AuthCantDisableUserBecauseLastAdministratorUser,
        AuthCantDisableUserBecauseLastSupervisorUser,
        AuthOAuthIdNotSpecified,
        AuthPasswordIsTooShort,
        AuthPasswordIsNotSpecified,
        AuthPasswordIsNotCorrect,
        AuthRequestsLimitIsExceeded,
        AuthRoleCantBeDeletedBecauseUsedByUsers,
        AuthRoleNameAlreadyExists,
        AuthRoleNameIsSystemRole,
        AuthTokenIsNotCorrect,
        AuthUserNameNotAssociatedWithYourAccount,

        AuthUserHasLoggedOut,
        AuthUserNameAlreadyExists,
        AuthUserNameIsNotSpecified,
        AuthUserNameOrPasswordIsNotCorrect,
        AuthUserNameShouldLookLikeAnEmailAddress,
        AuthUserNameEmailIsBlocked,
        AuthWorkspaceNameAlreadyInUse,

        CommandTimeOut,
        CustomMessage,

        ExecutionError,

        IsNotAuthorized,
        IsNotDeleted,
        IsNotCorrect,
        IsNotEqual,
        IsNotFound,
        IsNotRecognized,
        IsNotSpecified,

        ItemCantBeDeletedBecauseItemIsAttachedToOtherItems,
        ItemCantBeMovedToSpecifiedPlace,
        ItemDoesNotSupport,
        KeyAndToKeyAreEqual,

        NotificationFailed,

        NotificationFileUploading,
        NotificationFilesUploadingComplete,
        NotificationItemDelete,
        NotificationItemDeleteComplete,
        NotificationItemRestore,
        NotificationItemRestoreComplete,
        NotificationItemTransfer,
        NotificationItemTransferComplete,
        NotificationItemWaitingProcessing,
        NotificationMailing,
        NotificationMailingComplete,
        NotificationMailingWaitingProcessing,
        NotificationOperationAborted,
        NotificationRecycleBinCleaning,
        NotificationRecycleBinCleaningComplete,
        NotificationRecycleBinWaitingProcessing,
        NotificationReportCompiling,
        NotificationReportDataProcessing,
        NotificationReportExporting,
        NotificationReportExportingComplete,
        NotificationReportRendering,
        NotificationReportRenderingComplete,
        NotificationReportSaving,
        NotificationReportWaitingProcessing,
        NotificationSchedulerRunning,
        NotificationSchedulerRunningComplete,
        NotificationSchedulerWaitingProcessing,
        NotificationTransferring,
        NotificationTransferringComplete,
        NotificationTitleFilesUploading,
        NotificationTitleItemRefreshing,
        NotificationTitleItemTransferring,
        NotificationTitleMailing,
        NotificationTitleReportExporting,
        NotificationTitleReportRendering,
        NotificationTitleSchedulerRunning,

        QuotaMaximumComputingCyclesCountExceeded,
        QuotaMaximumFileSizeExceeded,
        QuotaMaximumItemsCountExceeded,
        QuotaMaximumReportPagesCountExceeded,
        QuotaMaximumUsersCountExceeded,
        QuotaMaximumWorkspacesCountExceeded,

        AccessDenied,
        OutOfRange,
        ParsingCommandException,
        SchedulerCantRunItSelf,
        SessionTimeOut,
        SnapshotAlreadyProcessed,
        SpecifiedItemIsNot,
        WithSpecifiedKeyIsNotFound,
        TinyKeyIsNotCorrect,
        NoAccessToShareItem,

        VersionCopyFromItem,
        VersionCreatedFromFile,
        VersionCreatedFromItem,
        VersionNewItemCreation,
        VersionLoadedFromFile,

        Congratulations,
        SuccessfullyRenewed,
        NewProduct,
        NewVersionsAvailable,
        TrialToLicense,
        SubscriptionExpired,
        SubscriptionExpiredDate,
        SubscriptionsOut10,
        SubscriptionsOut20,
        Warning,
        WouldYouLikeToUpdateNow
    }
    #endregion

    #region StiRelationDirection
    public enum StiRelationDirection
    {
        ParentToChild,
        ChildToParent
    }
    #endregion

    #region StiRelationDirection
    public enum StiGisDataType
    {
        Wkt,
        GeoJSON
    }
    #endregion

    #region StiTableColumnVisibility
    public enum StiTableColumnVisibility
    {
        True,
        False,
        Expression
    }
    #endregion

    #region StiCardsColumnVisibility
    public enum StiCardsColumnVisibility
    {
        True,
        False,
        Expression
    }
    #endregion

    #region StiSummaryColumnType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiSummaryColumnType
    {
        //
        // Summary:
        //     The sum of all values in a column.
        Sum,
        //
        // Summary:
        //     The minimum value in a column.
        Min,
        //
        // Summary:
        //     The maximum value in a column.
        Max,
        //
        // Summary:
        //     The record count.
        Count,
        //
        // Summary:
        //     The average value of a column.
        Average,
    }
    #endregion

    #region StiDesignerSpecification
    /// <summary>
    /// A specification of a customer of the designer.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDesignerSpecification
    {
        /// <summary>
        /// The designer automatically determines a kind of the designer's customer.
        /// </summary>
        Auto,
        /// <summary>
        /// UI of the designer specified for the developers and programmers.
        /// </summary>
        Developer,
        /// <summary>
        /// UI of the designer specified for the BI creators.
        /// </summary>
        BICreator,
        /// <summary>
        /// UI of the designer specified for the beginner.
        /// </summary>
        Beginner
    }
    #endregion

    #region StiVisualStatePermissionKind
    public enum StiVisualStatePermissionKind
    {
        AllowDefaultAndStyle,
        AllowStyle,
        Deny
    }
    #endregion
}