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

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes class that contains an expression.
	/// </summary>
	public abstract class StiUnifiedExpression : StiExpression
	{	
		#region Methods
		public void Set(StiComponent parent, string propertyName, string value)
		{
            this.parent = parent;
			this.propertyName = propertyName;
			this.Value = value;
		}
		#endregion

		#region Fields
		private string propertyName = string.Empty;
		private StiComponent parent;
		#endregion

		#region Properties
		public override string Value
		{
			get
			{
			    return parent != null 
			        ? parent.Properties.Get(propertyName, string.Empty) as string 
			        : base.Value;
			}
			set
			{
				if (parent != null)
				    parent.Properties.Set(propertyName, value, string.Empty);
				else
				    base.Value = value;
			}
		}		
		#endregion

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiUnifiedExpression() : this(string.Empty)
		{
		}
		
		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Gets or sets the expression value.</param>
		public StiUnifiedExpression(string value) : base(value)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiUnifiedExpression(StiComponent parent, string propertyName)
		{
			this.parent = parent;
			this.propertyName = propertyName;
		}
	}
}
