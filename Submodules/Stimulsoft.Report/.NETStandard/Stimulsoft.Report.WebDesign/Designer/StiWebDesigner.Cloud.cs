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

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using StiResource = Stimulsoft.Report.Dictionary.StiResource;
using System.Linq;

#if SERVER
using Stimulsoft.Server.Objects;
using Stimulsoft.Server;
using Stimulsoft.Server.Connect;
#endif

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region Stimulsoft Server Client Methods

#if SERVER
        private StiCommand RunCommand(StiCommand command)
        {
            return StiCommandToServer.RunCommand(command);
        }

        public static void CreateDataSourcesFromAttachedItem(StiReport report, ArrayList attachedItems)
        {
            if (attachedItems != null && attachedItems.Count > 0)
            {
                List<string> attachedKeys = new List<string>();
                foreach (Hashtable attachedItem in attachedItems)
                {
                    attachedKeys.Add(attachedItem["key"] as string);
                }

                StiDataWorker.Report.RegisterData(report, attachedKeys, null, null, false);
            }
        }

        private static void AddItemToAttachedItemsCollection(Hashtable attachedItems, StiItem item)
        {
            string itemGroup = StiAttachedItemHelper.GetDictionaryCloudItemsGroupName(item);
            if (itemGroup != "Unknown")
            {
                Hashtable itemObj = new Hashtable();
                itemObj["name"] = item.Name;
                itemObj["description"] = item.Description;
                itemObj["key"] = item.Key;
                itemObj["typeItem"] = itemGroup;
                itemObj["typeIcon"] = "Cloud" + itemGroup;
                itemObj["isCloudAttachedItem"] = true;
                if (item is StiFileItem) itemObj["fileType"] = ((StiFileItem)item).FileType;

                if (attachedItems[itemGroup] == null) attachedItems[itemGroup] = new ArrayList();
                ((ArrayList)attachedItems[itemGroup]).Add(itemObj);
            }
        }

        public static Hashtable GetReportAttachedItems(ArrayList attachedItemsKeys, string sessionKey)
        {
            Hashtable attachedItems = new Hashtable();

            if (attachedItemsKeys != null && attachedItemsKeys.Count > 0 && !string.IsNullOrEmpty(sessionKey))
            {
                var commands = new List<StiCommand>();

                for (int i = 0; i < attachedItemsKeys.Count; i++)
                {
                    var getItemCommand = new StiItemCommands.Get
                    {
                        ItemKey = (string)attachedItemsKeys[i],
                        SessionKey = sessionKey
                    };
                    commands.Add(getItemCommand);
                }

                var runCommandList = new StiCommandListCommands.Run
                {
                    Commands = commands,
                    ContinueAfterError = true,
                    SessionKey = sessionKey
                };

                var resultCommandList = StiCommandToServer.RunCommand(runCommandList) as StiCommandListCommands.Run;

                if (resultCommandList == null || !resultCommandList.ResultSuccess)
                    return null;

                if (resultCommandList.ResultCommands != null)
                {
                    foreach (var stiCommand in runCommandList.ResultCommands)
                    {
                        var command = (StiItemCommands.Get)stiCommand;
                        if (!command.ResultSuccess && command.ResultItem != null) continue;
                        if (!StiAttachedItemHelper.CanAttachToReportTemplate(command.ResultItem)) continue;

                        AddItemToAttachedItemsCollection(attachedItems, command.ResultItem);
                    }
                }
            }

            return attachedItems;
        }

        public static Hashtable GetReportAttachedItemsFormSharedItem(string parentItemKey, ArrayList attachedItemsKeys, string sessionKey)
        {
            Hashtable attachedItems = new Hashtable();

            if (attachedItemsKeys != null && attachedItemsKeys.Count > 0 && !string.IsNullOrEmpty(sessionKey))
            {
                var getAttachedItemsCommand = new StiItemCommands.GetAttachedItems()
                {
                    ItemKey = parentItemKey,
                    SessionKey = sessionKey
                };

                getAttachedItemsCommand = (StiItemCommands.GetAttachedItems)StiCommandToServer.RunCommand(getAttachedItemsCommand);

                
                if (getAttachedItemsCommand.ResultSuccess && getAttachedItemsCommand.ResultItems != null)
                {
                    foreach (var resultItem in getAttachedItemsCommand.ResultItems)
                    {
                        if (!StiAttachedItemHelper.CanAttachToReportTemplate(resultItem)) continue;

                        AddItemToAttachedItemsCollection(attachedItems, resultItem);
                    }
                }
            }

            return attachedItems;
        }

        public static void AddResourcesToReport(StiReport report, ArrayList resourceItems, string sessionKey)
        {
            if (resourceItems != null && resourceItems.Count > 0)
            {
                var itemKeys = new List<string>();
                resourceItems.Cast<Hashtable>().ToList().ForEach(i => { itemKeys.Add(i["key"] as string); });

                var fetchCommand = new StiItemResourceCommands.Fetch()
                {
                    ItemKeys = itemKeys,
                    SessionKey = sessionKey
                };

                fetchCommand = StiCommandToServer.RunCommand(fetchCommand) as StiItemResourceCommands.Fetch;

                if (fetchCommand.ResultSuccess && fetchCommand.ResultResources != null)
                {
                    var xsdResource = string.Empty;

                    fetchCommand.ResultResources.ForEach(r =>
                    {
                        var currResItem = resourceItems.Cast<Hashtable>().ToList().Where(i => (string)i["key"] == r.Key).FirstOrDefault() as Hashtable;
                        if (currResItem != null)
                        {
                            string fileName = currResItem["fileName"] as string;
                            string itemKey = currResItem["key"] as string;
                            string resourceName = fileName.Substring(0, fileName.IndexOf("."));
                            if (String.IsNullOrEmpty(resourceName)) resourceName = StiNameCreation.CreateResourceName(report, "Resource");

                            StiResource newResource = new StiResource()
                            {
                                Name = resourceName,
                                Content = r.Content,
                                Type = StiReportResourcesHelper.GetResourceTypeByFileName(fileName)
                            };
                                                        
                            if (report.Dictionary.Resources.Contains(resourceName)) {
                                int i = 2;
                                while (report.Dictionary.Resources.Contains($"{resourceName}{i}"))
                                {
                                    i++;
                                }
                                newResource.Name = $"{resourceName}{i}";
                            }

                            if (newResource.Type == StiResourceType.Xsd) xsdResource = newResource.Name;

                            report.Dictionary.Resources.Add(newResource);

                            //Add datasources to report
                            if (StiReportResourcesHelper.IsDataResourceType(newResource.Type))
                            {
                                StiFileDatabase database = StiDictionaryHelper.CreateNewDatabaseFromResource(report, newResource);
                                if (database != null)
                                {
                                    database.Name = database.Alias = StiDictionaryHelper.GetNewDatabaseName(report, newResource.Name);
                                    database.PathData = StiHyperlinkProcessor.ResourceIdent + newResource.Name;
                                    report.Dictionary.Databases.Add(database);
                                    database.CreateDataSources(report.Dictionary);
                                }
                            }
                        }
                    });

                    if (!string.IsNullOrEmpty(xsdResource) && report.Dictionary.Databases.Count > 0) {
                        report.Dictionary.Databases.ToList().ForEach(d => {
                            if (d is StiXmlDatabase)
                                ((StiXmlDatabase)d).PathSchema = StiHyperlinkProcessor.ResourceIdent + xsdResource;
                        });
                    }

#if CLOUD
                    StiCloudHelper.ClearCloudLimits(report);
#endif

                    report.Dictionary.Connect(false);
                    report.Dictionary.Synchronize();
                }

                //Remove temp resource items
                List<string> deletingItems = new List<string>();
                foreach (Hashtable resourceItem in resourceItems)
                {
                    deletingItems.Add(resourceItem["key"] as string);
                }
                var commandDelete = new StiItemCommands.Delete()
                {
                    SessionKey = sessionKey,
                    ItemKeys = deletingItems,
                    ResultSuccess = true,
                    AllowMoveToRecycleBin = false,
                    AllowNotifications = false
                };
                var resultDeleteCommand = StiCommandToServer.RunCommand(commandDelete) as StiItemCommands.Delete;
            }
        }
#endif
        #endregion
    }
}
