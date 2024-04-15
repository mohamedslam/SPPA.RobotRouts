#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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
using System.Collections;
using System.Globalization;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    internal class StiGS1ApplicationIdentifiers
    {
        #region StiGS1ApplicationIdentifierItem
        public class StiGS1ApplicationIdentifierItem
        {
            public string AICode;
            public int AILength;
            public int NumericMin;
            public int NumericMax;
            public int CharacterMin;
            public int CharacterMax;
            public bool NeedFNC1;
            public string DataTitle;
            public string DataContent;

            public StiGS1ApplicationIdentifierItem(string AICode, int AILength, int NumericMin, int NumericMax, int CharacterMin, int CharacterMax,
                bool NeedFNC1, string DataTitle, string DataContent)
            {
                this.AICode = AICode;
                this.AILength = AILength;
                this.NumericMin = NumericMin;
                this.NumericMax = NumericMax;
                this.CharacterMin = CharacterMin;
                this.CharacterMax = CharacterMax;
                this.NeedFNC1 = NeedFNC1;
                this.DataTitle = DataTitle;
                this.DataContent = DataContent;
            }
        }
        #endregion

        #region Fields
        private static StiGS1ApplicationIdentifierItem[] GS1ApplicationIdentifiers = new StiGS1ApplicationIdentifierItem[] {
            new StiGS1ApplicationIdentifierItem("00", 2, 18, 18, 0, 0, false, "SSCC", "Serial Shipping Container Code (SSCC)"),
            new StiGS1ApplicationIdentifierItem("01", 2, 14, 14, 0, 0, false, "GTIN", "Global Trade Item Number (GTIN)"),
            new StiGS1ApplicationIdentifierItem("02", 2, 14, 14, 0, 0, false, "CONTENT", "GTIN of contained trade items"),
            new StiGS1ApplicationIdentifierItem("10", 2, 0, 0, 0, 20, true, "BATCH/LOT", "Batch or lot number"),
            new StiGS1ApplicationIdentifierItem("11", 2, 6, 6, 0, 0, false, "PROD DATE", "Production date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("12", 2, 6, 6, 0, 0, false, "DUE DATE", "Due date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("13", 2, 6, 6, 0, 0, false, "PACK DATE", "Packaging date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("15", 2, 6, 6, 0, 0, false, "BEST BEFORE or BEST BY", "Best before date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("16", 2, 6, 6, 0, 0, false, "SELL BY", "Sell by date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("17", 2, 6, 6, 0, 0, false, "USE BY OR EXPIRY", "Expiration date (YYMMDD)"),
            new StiGS1ApplicationIdentifierItem("20", 2, 2, 2, 0, 0, false, "VARIANT", "Variant number"),
            new StiGS1ApplicationIdentifierItem("21", 2, 0, 0, 0, 20, true, "SERIAL", "Serial number"),
            new StiGS1ApplicationIdentifierItem("22", 2, 0, 0, 0, 20, true, "CPV", "Consumer product variant"),
            new StiGS1ApplicationIdentifierItem("235", 3, 0, 0, 0, 28, true, "TPX", "Third Party Controlled, Serialised Extension of Global Trade Item Number"),
            new StiGS1ApplicationIdentifierItem("240", 3, 0, 0, 0, 30, true, "ADDITIONAL ID", "Additional item identification"),
            new StiGS1ApplicationIdentifierItem("241", 3, 0, 0, 0, 30, true, "CUST. PART NO.", "Customer part number"),
            new StiGS1ApplicationIdentifierItem("242", 3, 0, 6, 0, 0, true, "MTO VARIANT", "Made-to-Order variation number"),
            new StiGS1ApplicationIdentifierItem("243", 3, 0, 0, 0, 20, true, "PCN", "Packaging component number"),
            new StiGS1ApplicationIdentifierItem("250", 3, 0, 0, 0, 30, true, "SECONDARY SERIAL", "Secondary serial number"),
            new StiGS1ApplicationIdentifierItem("251", 3, 0, 0, 0, 30, true, "REF. TO SOURCE", "Reference to source entity"),
            new StiGS1ApplicationIdentifierItem("253", 3, 13, 13, 0, 17, true, "GDTI", "Global Document Type Identifier (GDTI)"),
            new StiGS1ApplicationIdentifierItem("254", 3, 0, 0, 0, 20, true, "GLN EXTENSION COMPONENT", "GLN extension component"),
            new StiGS1ApplicationIdentifierItem("255", 3, 13, 25, 0, 0, true, "GCN", "Global Coupon Number (GCN)"),
            new StiGS1ApplicationIdentifierItem("30", 2, 0, 8, 0, 0, true, "VAR. COUNT", "Count of items (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("310", 4, 6, 6, 0, 0, false, "NET WEIGHT (kg)", "Net weight, kilograms (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("311", 4, 6, 6, 0, 0, false, "LENGTH (m)", "Length or first dimension, metres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("312", 4, 6, 6, 0, 0, false, "WIDTH (m)", "Width, diameter, or second dimension, metres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("313", 4, 6, 6, 0, 0, false, "HEIGHT (m)", "Depth, thickness, height, or third dimension, metres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("314", 4, 6, 6, 0, 0, false, "AREA (m2)", "Area, square metres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("315", 4, 6, 6, 0, 0, false, "NET VOLUME (l)", "Net volume, litres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("316", 4, 6, 6, 0, 0, false, "NET VOLUME (m3)", "Net volume, cubic metres (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("320", 4, 6, 6, 0, 0, false, "NET WEIGHT (lb)", "Net weight, pounds (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("321", 4, 6, 6, 0, 0, false, "LENGTH (i)", "Length or first dimension, inches (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("322", 4, 6, 6, 0, 0, false, "LENGTH (f)", "Length or first dimension, feet (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("323", 4, 6, 6, 0, 0, false, "LENGTH (y)", "Length or first dimension, yards (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("324", 4, 6, 6, 0, 0, false, "WIDTH (i)", "Width, diameter, or second dimension, inches (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("325", 4, 6, 6, 0, 0, false, "WIDTH (f)", "Width, diameter, or second dimension, feet (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("326", 4, 6, 6, 0, 0, false, "WIDTH (y)", "Width, diameter, or second dimension, yards (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("327", 4, 6, 6, 0, 0, false, "HEIGHT (i)", "Depth, thickness, height, or third dimension, inches (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("328", 4, 6, 6, 0, 0, false, "HEIGHT (f)", "Depth, thickness, height, or third dimension, feet (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("329", 4, 6, 6, 0, 0, false, "HEIGHT (y)", "Depth, thickness, height, or third dimension, yards (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("330", 4, 6, 6, 0, 0, false, "GROSS WEIGHT (kg)", "Logistic weight, kilograms"),
            new StiGS1ApplicationIdentifierItem("331", 4, 6, 6, 0, 0, false, "LENGTH (m), log", "Length or first dimension, metres"),
            new StiGS1ApplicationIdentifierItem("332", 4, 6, 6, 0, 0, false, "WIDTH (m), log", "Width, diameter, or second dimension, metres"),
            new StiGS1ApplicationIdentifierItem("333", 4, 6, 6, 0, 0, false, "HEIGHT (m), log", "Depth, thickness, height, or third dimension, metres"),
            new StiGS1ApplicationIdentifierItem("334", 4, 6, 6, 0, 0, false, "AREA (m2), log", "Area, square metres"),
            new StiGS1ApplicationIdentifierItem("335", 4, 6, 6, 0, 0, false, "VOLUME (l), log", "Logistic volume, litres"),
            new StiGS1ApplicationIdentifierItem("336", 4, 6, 6, 0, 0, false, "VOLUME (m3), log", "Logistic volume, cubic metres"),
            new StiGS1ApplicationIdentifierItem("337", 4, 6, 6, 0, 0, false, "KG PER m²", "Kilograms per square metre"),
            new StiGS1ApplicationIdentifierItem("340", 4, 6, 6, 0, 0, false, "GROSS WEIGHT (lb)", "Logistic weight, pounds"),
            new StiGS1ApplicationIdentifierItem("341", 4, 6, 6, 0, 0, false, "LENGTH (i), log", "Length or first dimension, inches"),
            new StiGS1ApplicationIdentifierItem("342", 4, 6, 6, 0, 0, false, "LENGTH (f), log", "Length or first dimension, feet"),
            new StiGS1ApplicationIdentifierItem("343", 4, 6, 6, 0, 0, false, "LENGTH (y), log", "Length or first dimension, yards"),
            new StiGS1ApplicationIdentifierItem("344", 4, 6, 6, 0, 0, false, "WIDTH (i), log", "Width, diameter, or second dimension, inches"),
            new StiGS1ApplicationIdentifierItem("345", 4, 6, 6, 0, 0, false, "WIDTH (f), log", "Width, diameter, or second dimension, feet"),
            new StiGS1ApplicationIdentifierItem("346", 4, 6, 6, 0, 0, false, "WIDTH (y), log", "Width, diameter, or second dimension, yard"),
            new StiGS1ApplicationIdentifierItem("347", 4, 6, 6, 0, 0, false, "HEIGHT (i), log", "Depth, thickness, height, or third dimension, inches"),
            new StiGS1ApplicationIdentifierItem("348", 4, 6, 6, 0, 0, false, "HEIGHT (f), log", "Depth, thickness, height, or third dimension, feet"),
            new StiGS1ApplicationIdentifierItem("349", 4, 6, 6, 0, 0, false, "HEIGHT (y), log", "Depth, thickness, height, or third dimension, yards"),
            new StiGS1ApplicationIdentifierItem("350", 4, 6, 6, 0, 0, false, "AREA (i2)", "Area, square inches (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("351", 4, 6, 6, 0, 0, false, "AREA (f2)", "Area, square feet (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("352", 4, 6, 6, 0, 0, false, "AREA (y2)", "Area, square yards (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("353", 4, 6, 6, 0, 0, false, "AREA (i2), log", "Area, square inches"),
            new StiGS1ApplicationIdentifierItem("354", 4, 6, 6, 0, 0, false, "AREA (f2), log", "Area, square feet"),
            new StiGS1ApplicationIdentifierItem("355", 4, 6, 6, 0, 0, false, "AREA (y2), log", "Area, square yards"),
            new StiGS1ApplicationIdentifierItem("356", 4, 6, 6, 0, 0, false, "NET WEIGHT (t)", "Net weight, troy ounces (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("357", 4, 6, 6, 0, 0, false, "NET VOLUME (oz)", "Net weight (or volume), ounces (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("360", 4, 6, 6, 0, 0, false, "NET VOLUME (q)", "Net volume, quarts (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("361", 4, 6, 6, 0, 0, false, "NET VOLUME (g)", "Net volume, gallons U.S. (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("362", 4, 6, 6, 0, 0, false, "VOLUME (q), log", "Logistic volume, quarts"),
            new StiGS1ApplicationIdentifierItem("363", 4, 6, 6, 0, 0, false, "VOLUME (g), log", "Logistic volume, gallons U.S."),
            new StiGS1ApplicationIdentifierItem("364", 4, 6, 6, 0, 0, false, "VOLUME (i3)", "Net volume, cubic inches (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("365", 4, 6, 6, 0, 0, false, "VOLUME (f3)", "Net volume, cubic feet (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("366", 4, 6, 6, 0, 0, false, "VOLUME (y3)", "Net volume, cubic yards (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("367", 4, 6, 6, 0, 0, false, "VOLUME (i3), log", "Logistic volume, cubic inches"),
            new StiGS1ApplicationIdentifierItem("368", 4, 6, 6, 0, 0, false, "VOLUME (f3), log", "Logistic volume, cubic feet"),
            new StiGS1ApplicationIdentifierItem("369", 4, 6, 6, 0, 0, false, "VOLUME (y3), log", "Logistic volume, cubic yards"),
            new StiGS1ApplicationIdentifierItem("37", 2, 0, 8, 0, 0, true, "COUNT", "Count of trade items"),
            new StiGS1ApplicationIdentifierItem("390", 4, 0, 15, 0, 0, true, "AMOUNT", "Applicable amount payable or Coupon value, local currency"),
            new StiGS1ApplicationIdentifierItem("391", 4, 3, 18, 0, 0, true, "AMOUNT", "Applicable amount payable with ISO currency code"),
            new StiGS1ApplicationIdentifierItem("392", 4, 0, 15, 0, 0, true, "PRICE", "Applicable amount payable, single monetary area (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("393", 4, 3, 18, 0, 0, true, "PRICE", "Applicable amount payable with ISO currency code (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("394", 4, 4, 4, 0, 0, true, "PRCNT OFF", "Percentage discount of a coupon"),
            new StiGS1ApplicationIdentifierItem("395", 4, 6, 6, 0, 0, true, "PRICE/UoM", "Amount payable per unit of measure single monetary area (variable measure trade item)"),
            new StiGS1ApplicationIdentifierItem("400", 3, 0, 0, 0, 30, true, "ORDER NUMBER", "Customer's purchase order number"),
            new StiGS1ApplicationIdentifierItem("401", 3, 0, 0, 0, 30, true, "GINC", "Global Identification Number for Consignment (GINC)"),
            new StiGS1ApplicationIdentifierItem("402", 3, 17, 17, 0, 0, true, "GSIN", "Global Shipment Identification Number (GSIN)"),
            new StiGS1ApplicationIdentifierItem("403", 3, 0, 0, 0, 30, true, "ROUTE", "Routing code"),
            new StiGS1ApplicationIdentifierItem("410", 3, 13, 13, 0, 0, false, "SHIP TO LOC", "Ship to - Deliver to Global Location Number"),
            new StiGS1ApplicationIdentifierItem("411", 3, 13, 13, 0, 0, false, "BILL TO", "Bill to - Invoice to Global Location Number"),
            new StiGS1ApplicationIdentifierItem("412", 3, 13, 13, 0, 0, false, "PURCHASE FROM", "Purchased from Global Location Number"),
            new StiGS1ApplicationIdentifierItem("413", 3, 13, 13, 0, 0, false, "SHIP FOR LOC", "Ship for - Deliver for - Forward to Global Location Number"),
            new StiGS1ApplicationIdentifierItem("414", 3, 13, 13, 0, 0, false, "LOC No", "Identification of a physical location - Global Location Number"),
            new StiGS1ApplicationIdentifierItem("415", 3, 13, 13, 0, 0, false, "PAY TO", "Global Location Number of the invoicing party"),
            new StiGS1ApplicationIdentifierItem("416", 3, 13, 13, 0, 0, false, "PROD/SERV LOC", "Global Location Number (GLN) of the production or service location"),
            new StiGS1ApplicationIdentifierItem("417", 3, 13, 13, 0, 0, false, "PARTY", "Party Global Location Number"),
            new StiGS1ApplicationIdentifierItem("420", 3, 0, 0, 0, 20, true, "SHIP TO POST", "Ship to - Deliver to postal code within a single postal authority"),
            new StiGS1ApplicationIdentifierItem("421", 3, 3, 3, 0, 9, true, "SHIP TO POST", "Ship to - Deliver to postal code with ISO country code"),
            new StiGS1ApplicationIdentifierItem("422", 3, 3, 3, 0, 0, true, "ORIGIN", "Country of origin of a trade item"),
            new StiGS1ApplicationIdentifierItem("423", 3, 3, 15, 0, 0, true, "COUNTRY - INITIAL PROCESS.", "Country of initial processing"),
            new StiGS1ApplicationIdentifierItem("424", 3, 3, 3, 0, 0, true, "COUNTRY - PROCESS.", "Country of processing"),
            new StiGS1ApplicationIdentifierItem("425", 3, 3, 15, 0, 0, true, "COUNTRY - DISASSEMBLY", "Country of disassembly"),
            new StiGS1ApplicationIdentifierItem("426", 3, 3, 3, 0, 0, true, "COUNTRY – FULL PROCESS", "Country covering full process chain"),
            new StiGS1ApplicationIdentifierItem("427", 3, 0, 0, 0, 3, true, "ORIGIN SUBDIVISION", "Country subdivision Of origin"),
            new StiGS1ApplicationIdentifierItem("4300", 4, 0, 0, 0, 35, true, "SHIP TO COMP", "Ship-to / Deliver-to Company name"),
            new StiGS1ApplicationIdentifierItem("4301", 4, 0, 0, 0, 35, true, "SHIP TO NAME", "Ship-to / Deliver-to contact name"),
            new StiGS1ApplicationIdentifierItem("4302", 4, 0, 0, 0, 70, true, "SHIP TO ADD1", "Ship-to / Deliver-to address line 1"),
            new StiGS1ApplicationIdentifierItem("4303", 4, 0, 0, 0, 70, true, "SHIP TO ADD2", "Ship-to / Deliver-to address line 2"),
            new StiGS1ApplicationIdentifierItem("4304", 4, 0, 0, 0, 70, true, "SHIP TO SUB", "Ship-to / Deliver-to suburb"),
            new StiGS1ApplicationIdentifierItem("4305", 4, 0, 0, 0, 70, true, "SHIP TO LOC", "Ship-to / Deliver-to locality"),
            new StiGS1ApplicationIdentifierItem("4306", 4, 0, 0, 0, 70, true, "SHIP TO REG", "Ship-to / Deliver-to region"),
            new StiGS1ApplicationIdentifierItem("4307", 4, 0, 0, 2, 2, true, "SHIP TO COUNTRY", "Ship-to / Deliver-to country code"),
            new StiGS1ApplicationIdentifierItem("4308", 4, 0, 0, 0, 30, true, "SHIP TO PHONE", "Ship-to / Deliver-to telephone number"),
            new StiGS1ApplicationIdentifierItem("4310", 4, 0, 0, 0, 35, true, "RTN TO COMP", "Return-to company name"),
            new StiGS1ApplicationIdentifierItem("4311", 4, 0, 0, 0, 35, true, "RTN TO NAME", "Return-to contact name"),
            new StiGS1ApplicationIdentifierItem("4312", 4, 0, 0, 0, 70, true, "RTN TO ADD1", "Return-to address line 1"),
            new StiGS1ApplicationIdentifierItem("4313", 4, 0, 0, 0, 70, true, "RTN TO ADD2", "Return-to address line 2"),
            new StiGS1ApplicationIdentifierItem("4314", 4, 0, 0, 0, 70, true, "RTN TO SUB", "Return-to suburb"),
            new StiGS1ApplicationIdentifierItem("4315", 4, 0, 0, 0, 70, true, "RTN TO LOC", "Return-to locality"),
            new StiGS1ApplicationIdentifierItem("4316", 4, 0, 0, 0, 70, true, "RTN TO REG", "Return-to region"),
            new StiGS1ApplicationIdentifierItem("4317", 4, 0, 0, 2, 2, true, "RTN TO COUNTRY", "Return-to country code"),
            new StiGS1ApplicationIdentifierItem("4318", 4, 0, 0, 0, 20, true, "RTN TO POST", "Return-to postal code"),
            new StiGS1ApplicationIdentifierItem("4319", 4, 0, 0, 0, 30, true, "RTN TO PHONE", "Return-to telephone number"),
            new StiGS1ApplicationIdentifierItem("4320", 4, 0, 0, 0, 35, true, "SRV DESCRIPTION", "Service code description"),
            new StiGS1ApplicationIdentifierItem("4321", 4, 1, 1, 0, 0, true, "DANGEROUS GOODS", "Dangerous goods flag"),
            new StiGS1ApplicationIdentifierItem("4322", 4, 1, 1, 0, 0, true, "AUTH LEAV", "Authority to leave flag"),
            new StiGS1ApplicationIdentifierItem("4323", 4, 1, 1, 0, 0, true, "SIG REQUIRED", "Signature required flag"),
            new StiGS1ApplicationIdentifierItem("4324", 4, 10, 10, 0, 0, true, "NBEF DEL DT", "Not before delivery date/time"),
            new StiGS1ApplicationIdentifierItem("4325", 4, 10, 10, 0, 0, true, "NAFT DEL DT", "Not after delivery date/time"),
            new StiGS1ApplicationIdentifierItem("4326", 4, 6, 6, 0, 0, true, "REL DATE", "Release date"),
            new StiGS1ApplicationIdentifierItem("7001", 4, 13, 13, 0, 0, true, "NSN", "NATO Stock Number (NSN)"),
            new StiGS1ApplicationIdentifierItem("7002", 4, 0, 0, 0, 30, true, "MEAT CUT", "UN/ECE meat carcasses and cuts classification"),
            new StiGS1ApplicationIdentifierItem("7003", 4, 10, 10, 0, 0, true, "EXPIRY TIME", "Expiration date and time"),
            new StiGS1ApplicationIdentifierItem("7004", 4, 0, 4, 0, 0, true, "ACTIVE POTENCY", "Active potency"),
            new StiGS1ApplicationIdentifierItem("7005", 4, 0, 0, 0, 12, true, "CATCH AREA", "Catch area"),
            new StiGS1ApplicationIdentifierItem("7006", 4, 6, 6, 0, 0, true, "FIRST FREEZE DATE", "First freeze date"),
            new StiGS1ApplicationIdentifierItem("7007", 4, 6, 12, 0, 0, true, "HARVEST DATE", "Harvest date"),
            new StiGS1ApplicationIdentifierItem("7008", 4, 0, 0, 0, 3, true, "AQUATIC SPECIES", "Species for fishery purposes"),
            new StiGS1ApplicationIdentifierItem("7009", 4, 0, 0, 0, 10, true, "FISHING GEAR TYPE", "Fishing gear type"),
            new StiGS1ApplicationIdentifierItem("7010", 4, 0, 0, 0, 2, true, "PROD METHOD", "Production method"),
            new StiGS1ApplicationIdentifierItem("7020", 4, 0, 0, 0, 20, true, "REFURB LOT", "Refurbishment lot ID"),
            new StiGS1ApplicationIdentifierItem("7021", 4, 0, 0, 0, 20, true, "FUNC STAT", "Functional status"),
            new StiGS1ApplicationIdentifierItem("7022", 4, 0, 0, 0, 20, true, "REV STAT", "Revision status"),
            new StiGS1ApplicationIdentifierItem("7023", 4, 0, 0, 0, 30, true, "GIAI – ASSEMBLY", "Global Individual Asset Identifier of an assembly"),
            new StiGS1ApplicationIdentifierItem("703", 4, 3, 3, 0, 27, true, "PROCESSOR # s", "Number of processor with three-digit ISO country code"),
            new StiGS1ApplicationIdentifierItem("7040", 4, 1, 1, 3, 3, true, "UIC+EXT", "GS1 UIC with Extension 1 and Importer index"),
            new StiGS1ApplicationIdentifierItem("710", 3, 0, 0, 0, 20, true, "NHRN PZN", "National Healthcare Reimbursement Number (NHRN) – Germany PZN"),
            new StiGS1ApplicationIdentifierItem("711", 3, 0, 0, 0, 20, true, "NHRN CIP", "National Healthcare Reimbursement Number (NHRN) – France CIP"),
            new StiGS1ApplicationIdentifierItem("712", 3, 0, 0, 0, 20, true, "NHRN CN", "National Healthcare Reimbursement Number (NHRN) – Spain CN"),
            new StiGS1ApplicationIdentifierItem("713", 3, 0, 0, 0, 20, true, "NHRN DRN", "National Healthcare Reimbursement Number (NHRN) – Brasil DRN"),
            new StiGS1ApplicationIdentifierItem("714", 3, 0, 0, 0, 20, true, "NHRN AIM", "National Healthcare Reimbursement Number (NHRN) – Portugal AIM"),
            new StiGS1ApplicationIdentifierItem("715", 3, 0, 0, 0, 20, true, "NHRN NDC", "National Healthcare Reimbursement Number (NHRN) – United States of America NDC"),
            new StiGS1ApplicationIdentifierItem("723", 4, 0, 0, 2, 30, true, "CERT # s", "Certification reference"),
            new StiGS1ApplicationIdentifierItem("7240", 4, 0, 0, 0, 20, true, "PROTOCOL", "Protocol ID"),
            new StiGS1ApplicationIdentifierItem("8001", 4, 14, 14, 0, 0, true, "DIMENSIONS", "Roll products (width, length, core diameter, direction, splices)"),
            new StiGS1ApplicationIdentifierItem("8002", 4, 0, 0, 0, 20, true, "CMT No", "Cellular mobile telephone identifier"),
            new StiGS1ApplicationIdentifierItem("8003", 4, 14, 14, 0, 16, true, "GRAI", "Global Returnable Asset Identifier (GRAI)"),
            new StiGS1ApplicationIdentifierItem("8004", 4, 0, 0, 0, 30, true, "GIAI", "Global Individual Asset Identifier (GIAI)"),
            new StiGS1ApplicationIdentifierItem("8005", 4, 6, 6, 0, 0, true, "PRICE PER UNIT", "Price per unit of measure"),
            new StiGS1ApplicationIdentifierItem("8006", 4, 18, 18, 0, 0, true, "GCTIN", "Identification of the components of a trade item"),
            new StiGS1ApplicationIdentifierItem("8007", 4, 0, 0, 0, 34, true, "IBAN", "International Bank Account Number (IBAN)"),
            new StiGS1ApplicationIdentifierItem("8008", 4, 8, 12, 0, 0, true, "PROD TIME", "Date and time of production"),
            new StiGS1ApplicationIdentifierItem("8009", 4, 0, 0, 0, 50, true, "OPTSEN", "Optically readable sensor indicator"),
            new StiGS1ApplicationIdentifierItem("8010", 4, 0, 0, 0, 30, true, "CPID", "Component / Part Identifier (CPID)"),
            new StiGS1ApplicationIdentifierItem("8011", 4, 0, 12, 0, 0, true, "CPID SERIAL", "Component / Part Identifier serial number (CPID SERIAL)"),
            new StiGS1ApplicationIdentifierItem("8012", 4, 0, 0, 0, 20, true, "VERSION", "Software version"),
            new StiGS1ApplicationIdentifierItem("8013", 4, 0, 0, 0, 25, true, "GMN", "Global Model Number (GMN)"),
            new StiGS1ApplicationIdentifierItem("8017", 4, 18, 18, 0, 0, true, "GSRN - PROVIDER", "Global Service Relation Number to identify the relationship between an organisation offering services and the provider of services"),
            new StiGS1ApplicationIdentifierItem("8018", 4, 18, 18, 0, 0, true, "GSRN - RECIPIENT", "Global Service Relation Number to identify the relationship between an organisation offering services and the recipient of services"),
            new StiGS1ApplicationIdentifierItem("8019", 4, 0, 10, 0, 0, true, "SRIN", "Service Relation Instance Number (SRIN)"),
            new StiGS1ApplicationIdentifierItem("8020", 4, 0, 0, 0, 25, true, "REF No", "Payment slip reference number"),
            new StiGS1ApplicationIdentifierItem("8026", 4, 18, 18, 0, 0, true, "ITIP CONTENT", "Identification of pieces of a trade item (ITIP) contained in a logistic unit"),
            new StiGS1ApplicationIdentifierItem("8110", 4, 0, 0, 0, 70, true, "-", "Coupon code identification for use in North America"),
            new StiGS1ApplicationIdentifierItem("8111", 4, 4, 4, 0, 0, true, "POINTS", "Loyalty points of a coupon"),
            new StiGS1ApplicationIdentifierItem("8112", 4, 0, 0, 0, 70, true, "-", "Positive offer file coupon code identification for use in North America"),
            new StiGS1ApplicationIdentifierItem("8200", 4, 0, 0, 0, 70, true, "PRODUCT URL", "Extended Packaging URL"),
            new StiGS1ApplicationIdentifierItem("90", 2, 0, 0, 0, 30, true, "INTERNAL", "Information mutually agreed between trading partners"),
            new StiGS1ApplicationIdentifierItem("91", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("92", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("93", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("94", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("95", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("96", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("97", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("98", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
            new StiGS1ApplicationIdentifierItem("99", 2, 0, 0, 0, 90, true, "INTERNAL", "Company internal information"),
        };

        private static Hashtable AICodeToItem = new Hashtable();
        #endregion

        #region Methods
        public static StiGS1ApplicationIdentifierItem GetApplicationIdentifierItemByCode(string code)
        {
            if (AICodeToItem.ContainsKey(code)) return AICodeToItem[code] as StiGS1ApplicationIdentifierItem;

            foreach (var item in GS1ApplicationIdentifiers)
            {
                if ((code.Length >= item.AILength) && (code.StartsWith(item.AICode)))
                {
                    AICodeToItem[code] = item;
                    return item;
                }
            }

            return null;
        }

        public static string ParseCode(string code, StringBuilder outputCode, StringBuilder outputText, char fnc1, bool addLeadingFnc1)
        {
            var parts = code.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            int count = parts.Length / 2;

            if (addLeadingFnc1) outputCode.Append(fnc1);

            if (!code.StartsWith("["))
            {
                return string.Format("Incorrect data: {0} \r\nThe first bracket is missing.", code);
            }
            if (count == 0)
            {
                return string.Format("Incorrect data: {0}", code);
            }

            for (int indexPart = 0; indexPart < count; indexPart++)
            {
                var key = parts[indexPart * 2].Trim();
                var value = parts[indexPart * 2 + 1].Trim();
                var ai = GetApplicationIdentifierItemByCode(key);

                if ((key == "01" || key == "02") && (value.Length == 8 || value.Length == 12 || value.Length == 13))
                {
                    value = new string('0', 14 - value.Length) + value;

                    #region Correction of checksum
                    var dig = new int[14];
                    for (int tempIndex = 0; tempIndex < 14; tempIndex++)
                    {
                        dig[tempIndex] = int.Parse(value[tempIndex].ToString(CultureInfo.InvariantCulture));
                    }
                    int sum = (dig[1] + dig[3] + dig[5] + dig[7] + dig[9] + dig[11]) +
                        (dig[0] + dig[2] + dig[4] + dig[6] + dig[8] + dig[10] + dig[12]) * 3;
                    int checkDigit = 10 - (sum % 10);
                    if (checkDigit == 10)
                    {
                        checkDigit = 0;
                    }
                    value = value.Substring(0, 13) + (char)(checkDigit + 48);
                    #endregion
                }

                //check AI
                foreach (char ch in key)
                {
                    if (!char.IsDigit(ch))
                    {
                        return string.Format("Incorrect AI: {0}", key);
                    }
                }
                if ((ai != null) && (key.Length > ai.AILength))
                {
                    return string.Format("Incorrect AI: {0}", key);
                }

                if (ai != null)
                {
                    //check data
                    int minLength = ai.NumericMin + ai.CharacterMin;
                    int maxLength = ai.NumericMax + ai.CharacterMax;
                    if (value.Length < minLength || value.Length > maxLength)
                    {
                        return string.Format("Incorrect data length: ({0}){1}", key, value);
                    }
                    if (ai.NumericMax > 0)
                    {
                        int numCount = ai.NumericMax;
                        if (ai.NumericMin != ai.NumericMax) numCount = value.Length;
                        for (int indexChar = 0; indexChar < numCount; indexChar++)
                        {
                            if (!char.IsDigit(value, indexChar))
                            {
                                return string.Format("Incorrect data: ({0}){1}", key, value);
                            }
                        }
                    }
                }

                outputCode.Append(key + value);
                if ((indexPart < count - 1) && (ai == null || ai.NeedFNC1)) outputCode.Append(fnc1);
                outputText.Append("(" + key + ")" + value);
                if (indexPart < count - 1) outputText.Append(" ");
            }

            return null;
        }
        #endregion
    }
}
