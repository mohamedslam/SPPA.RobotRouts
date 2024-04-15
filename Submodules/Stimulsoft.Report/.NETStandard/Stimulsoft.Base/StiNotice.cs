#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stimulsoft.Base
{
    public class StiNotice : StiObject
    {
        #region Properties
        public StiNoticeIdent Ident { get; set; }

        public List<string> Arguments { get; set; }

        public string CustomMessage { get; set; }
        #endregion

        #region Methods
        private static bool Equals(List<string> list1, List<string> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;

            return list1.SequenceEqual(list2);
        }

        protected bool Equals(StiNotice other)
        {
            return 
                other != null &&
                Ident == other.Ident &&
                Equals(this.Arguments, other.Arguments) &&
                string.Equals(CustomMessage, other.CustomMessage);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StiNotice);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Ident;
                hashCode = (hashCode * 397) ^ (Arguments != null ? Arguments.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomMessage != null ? CustomMessage.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (this.Ident == StiNoticeIdent.CustomMessage)
                return CustomMessage;

            if (this.Ident == StiNoticeIdent.AccessDenied && !string.IsNullOrWhiteSpace(this.CustomMessage))
                return $"Access Denied - {CustomMessage}";

            if (this.Arguments == null || this.Arguments.Count == 0)
            {
                var value = StiLocalization.Get("Notices", this.Ident.ToString(), false);
                if (value != null)
                    return value;

                return this.Ident.ToString();
            }

            return $"{this.Ident}, Arguments: {string.Join(",", this.Arguments)}";
        }

        public static StiNotice Create(StiNoticeIdent ident)
        {
            return new StiNotice { Ident = ident }; 
        }

        public static StiNotice Create(StiNoticeIdent ident, string argument)
        {
            return new StiNotice { Ident = ident, Arguments = new List<string> { argument } }; 
        }

        public static StiNotice Create(StiNoticeIdent ident, string argument1, string argument2)
        {
            return new StiNotice { Ident = ident, Arguments = new List<string> { argument1, argument2 } };
        }

        public static StiNotice Create(StiNoticeIdent ident, string argument1, string argument2, string argument3)
        {
            return new StiNotice { Ident = ident, Arguments = new List<string> { argument1, argument2, argument3 } };
        }

        public static StiNotice Create(StiNoticeIdent ident, params string[] arguments)
        {
            return new StiNotice { Ident = ident, Arguments = arguments != null ? arguments.ToList() : null };
        }

        public static StiNotice Create(string customMessage)
        {
            return new StiNotice { Ident = StiNoticeIdent.CustomMessage, CustomMessage = customMessage };
        }

        public static StiNotice Create(Exception exception)
        {
            var serverException = exception as StiServerException;
            if (serverException != null)
                return serverException.Notice;
            
            var message = exception.InnerException != null ? $"{exception.Message} ({exception.InnerException.Message})" : exception.Message;
            return new StiNotice { Ident = StiNoticeIdent.CustomMessage, CustomMessage = message };
        }
        #endregion

        #region Methods.Helpers
        public static StiNotice ActivationMaxActivationsReached()
        {
            return new StiNotice { Ident = StiNoticeIdent.ActivationMaxActivationsReached };
        }

        public static StiNotice ActivationLockedAccount()
        {
            return new StiNotice { Ident = StiNoticeIdent.ActivationLockedAccount };
        }

        public static StiNotice ActivationTrialExpired()
        {
            return new StiNotice { Ident = StiNoticeIdent.ActivationTrialExpired };
        }

        public static StiNotice AccessDenied(string message = null)
        {
            return new StiNotice { Ident = StiNoticeIdent.AccessDenied, CustomMessage = message };
        }

        public static StiNotice AuthAccountCantBeUsedNow()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthAccountCantBeUsedNow };
        }

        public static StiNotice AuthAccountIsNotActivated()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthAccountIsNotActivated };
        }

        public static StiNotice AuthCantChangeRoleBecauseLastAdministratorUser()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantChangeRoleBecauseLastAdministratorUser };
        }

        public static StiNotice AuthCantChangeSystemRole()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantChangeSystemRole };
        }

        public static StiNotice AuthCantDeleteHimselfUser()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantDeleteHimselfUser };
        }

        public static StiNotice AuthCantDeleteLastAdministratorUser()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantDeleteLastAdministratorUser };
        }

        public static StiNotice AuthCantDeleteLastSupervisorUser()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantDeleteLastSupervisorUser };
        }

        public static StiNotice AuthCantDeleteSystemRole()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantDeleteSystemRole };
        }

        public static StiNotice AuthCantDisableUserBecauseLastAdministratorUser()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthCantDisableUserBecauseLastAdministratorUser };
        }

        public static StiNotice AuthOAuthIdNotSpecified()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthOAuthIdNotSpecified };
        }

        public static StiNotice AuthPasswordIsTooShort()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthPasswordIsTooShort };
        }

        public static StiNotice AuthPasswordNotSpecified()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthPasswordIsNotSpecified };
        }

        public static StiNotice AuthRoleNameAlreadyExists(string argument)
        {
            return Create(StiNoticeIdent.AuthRoleNameAlreadyExists, argument);
        }

        public static StiNotice AuthRoleCantBeDeletedBecauseUsedByUsers()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthRoleCantBeDeletedBecauseUsedByUsers };
        }

        public static StiNotice AuthRequestsLimitIsExceeded()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthRequestsLimitIsExceeded };
        }

        internal static StiNotice AuthTokenIsNotCorrect()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthTokenIsNotCorrect };
        }

        public static StiNotice AuthRoleNameIsSystemRole(string argument)
        {
            return Create(StiNoticeIdent.AuthRoleNameIsSystemRole, argument);
        }

        public static StiNotice AuthUserNameAlreadyExists()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthUserNameAlreadyExists };
        }

        public static StiNotice AuthUserNameNotSpecified()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthUserNameIsNotSpecified };
        }

        internal static StiNotice AuthUserNameNotAssociatedWithYourAccount(string userName, string social)
        {
            return Create(StiNoticeIdent.AuthUserNameNotAssociatedWithYourAccount, new string[] { userName, social });
        }

        public static StiNotice AuthUserNameOrPasswordIsNotCorrect()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthUserNameOrPasswordIsNotCorrect };
        }

        public static StiNotice AuthUserNameShouldLookLikeAnEmailAddress()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthUserNameShouldLookLikeAnEmailAddress };
        }

        public static StiNotice AuthUserNameEmailIsBlocked()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthUserNameEmailIsBlocked };
        }

        public static StiNotice AuthWorkspaceNameAlreadyInUse()
        {
            return new StiNotice { Ident = StiNoticeIdent.AuthWorkspaceNameAlreadyInUse };
        }

        public static StiNotice Congratulations()
        {
            return Create(StiNoticeIdent.Congratulations);
        }

        public static StiNotice ExecutionError()
        {
            return Create(StiNoticeIdent.ExecutionError);
        }        

        public static StiNotice ItemCantBeDeletedBecauseAttachedToOtherItems(string itemNames)
        {
            return Create(StiNoticeIdent.ItemCantBeDeletedBecauseItemIsAttachedToOtherItems, itemNames);
        }

        public static StiNotice IsNotAuthorized(string argument)
        {
            return Create(StiNoticeIdent.IsNotAuthorized, argument);
        }

        public static StiNotice IsNotEqual(string argument)
        {
            return Create(StiNoticeIdent.IsNotEqual, argument);
        }

        public static StiNotice IsNotSpecified(string argument)
        {
            return Create(StiNoticeIdent.IsNotSpecified, argument);
        }
       
        public static StiNotice IsNotDeleted(string argument)
        {
            return Create(StiNoticeIdent.IsNotDeleted, argument);
        }

        public static StiNotice IsNotCorrect(string argument)
        {
            return Create(StiNoticeIdent.IsNotCorrect, argument);
        }

        public static StiNotice IsNotFound(string argument)
        {
            return Create(StiNoticeIdent.IsNotFound, argument);
        }

        public static StiNotice IsNotRecognized(string argument)
        {
            return Create(StiNoticeIdent.IsNotRecognized, argument);
        }

        public static StiNotice ItemDoesNotSupport(string argument)
        {
            return Create(StiNoticeIdent.ItemDoesNotSupport, argument);
        }

        public static StiNotice NewProduct()
        {
            return Create(StiNoticeIdent.NewProduct);
        }

        public static StiNotice NewVersionsAvailable()
        {
            return Create(StiNoticeIdent.NewVersionsAvailable);
        }

        public static StiNotice NotificationFileUploading(string argument)
        {
            return Create(StiNoticeIdent.NotificationFileUploading, argument);
        }

        public static StiNotice NotificationFilesUploadingComplete(string argument)
        {
            return Create(StiNoticeIdent.NotificationFilesUploadingComplete, argument);
        }

        public static StiNotice NotificationItemWaitingProcessing(string argument)
        {
            return Create(StiNoticeIdent.NotificationItemWaitingProcessing, argument);
        }

        public static StiNotice NotificationMailing()
        {
            return Create(StiNoticeIdent.NotificationMailing);
        }

        public static StiNotice NotificationMailingComplete(string argument)
        {
            return Create(StiNoticeIdent.NotificationMailingComplete, argument);
        }

        public static StiNotice NotificationMailingWaitingProcessing(string argument)
        {
            return Create(StiNoticeIdent.NotificationMailingWaitingProcessing, argument);
        }

        public static StiNotice NotificationReportExporting(string argument)
        {
            return Create(StiNoticeIdent.NotificationReportExporting, argument);
        }

        public static StiNotice NotificationReportExportingComplete(string argument)
        {
            return Create(StiNoticeIdent.NotificationReportExportingComplete, argument);
        }

        public static StiNotice NotificationReportRendering(string argument)
        {
            return Create(StiNoticeIdent.NotificationReportRendering, argument);
        }

        public static StiNotice NotificationReportRenderingComplete(string argument)
        {
            return Create(StiNoticeIdent.NotificationReportRenderingComplete, argument);
        }

        public static StiNotice NotificationReportWaitingProcessing(string argument)
        {
            return Create(StiNoticeIdent.NotificationReportWaitingProcessing, argument);
        }

        public static StiNotice NotificationSchedulerRunning(string argument)
        {
            return Create(StiNoticeIdent.NotificationSchedulerRunning, argument);
        }

        public static StiNotice NotificationSchedulerRunningComplete(string argument)
        {
            return Create(StiNoticeIdent.NotificationSchedulerRunningComplete, argument);
        }

        public static StiNotice NotificationSchedulerWaitingProcessing(string argument)
        {
            return Create(StiNoticeIdent.NotificationSchedulerWaitingProcessing, argument);
        }

        public static StiNotice NotificationTitleFilesUploading(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleFilesUploading, argument);
        }

        public static StiNotice NotificationTitleItemRefreshing(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleItemRefreshing, argument);
        }

        public static StiNotice NotificationTitleItemTransferring(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleItemTransferring, argument);
        }

        public static StiNotice NotificationTitleMailing(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleMailing, argument);
        }
        
        public static StiNotice NotificationTitleReportExporting(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleReportExporting, argument);
        }

        public static StiNotice NotificationTitleReportRendering(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleReportRendering, argument);
        }

        public static StiNotice NotificationTitleSchedulerRunning(string argument)
        {
            return Create(StiNoticeIdent.NotificationTitleSchedulerRunning, argument);
        }

        public static StiNotice NotificationTransferring(string argument1, string argument2)
        {
            return Create(StiNoticeIdent.NotificationTransferring, argument1, argument2);
        }
        
        public static StiNotice OutOfRange(string argument)
        {
            return Create(StiNoticeIdent.OutOfRange, argument);
        }
        
        public static StiNotice SpecifiedItemIsNot(string argument)
        {
            return Create(StiNoticeIdent.SpecifiedItemIsNot, argument);
        }

        public static StiNotice SubscriptionExpired()
        {
            return Create(StiNoticeIdent.SubscriptionExpired);
        }

        public static StiNotice SubscriptionExpiredDate()
        {
            return Create(StiNoticeIdent.SubscriptionExpiredDate);
        }

        public static StiNotice SubscriptionsOut10()
        {
            return Create(StiNoticeIdent.SubscriptionsOut10);
        }

        public static StiNotice SubscriptionsOut20()
        {
            return Create(StiNoticeIdent.SubscriptionsOut20);
        }

        public static StiNotice SuccessfullyRenewed()
        {
            return Create(StiNoticeIdent.SuccessfullyRenewed);
        }

        public static StiNotice TrialToLicense()
        {
            return Create(StiNoticeIdent.TrialToLicense);
        }

        public static StiNotice TinyKeyIsNotCorrect()
        {
            return Create(StiNoticeIdent.TinyKeyIsNotCorrect);
        }

        public static StiNotice NoAccessToShareItem()
        {
            return Create(StiNoticeIdent.NoAccessToShareItem);
        }

        public static StiNotice Alert()
        {
            return Create(StiNoticeIdent.Alert);
        }

        public static StiNotice Warning()
        {
            return Create(StiNoticeIdent.Warning);
        }

        public static StiNotice WouldYouLikeToUpdateNow()
        {
            return Create(StiNoticeIdent.WouldYouLikeToUpdateNow);
        }

        public static StiNotice WithSpecifiedKeyIsNotFound(string argument)
        {
            return Create(StiNoticeIdent.WithSpecifiedKeyIsNotFound, argument);
        }

        public static StiNotice VersionCopyFromItem(string argument)
        {
            return Create(StiNoticeIdent.VersionCopyFromItem, argument);
        }

        public static StiNotice VersionCreatedFromFile(string argument)
        {
            return Create(StiNoticeIdent.VersionCreatedFromFile, argument);
        }

        public static StiNotice VersionCreatedFromItem(string argument)
        {
            return Create(StiNoticeIdent.VersionCreatedFromItem, argument);
        }

        public static StiNotice VersionNewItemCreation()
        {
            return Create(StiNoticeIdent.VersionNewItemCreation);
        }

        public static StiNotice VersionLoadedFromFile(string argument)
        {
            return Create(StiNoticeIdent.VersionLoadedFromFile, argument);
        }
        
        public static bool Compare(StiNotice message1, StiNotice message2)
        {
            if (message1 == message2) return true;
            if (message1 == null && message2 == null) return true;
            if (message1 == null || message2 == null) return false;

            return message1.Equals(message2);
        }
        #endregion
    }
}
