#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Stimulsoft.Report.Export
{
	public static class StiBidirectionalConvert2
	{
		#region Constants
		// Max explicity depth (embedding level)
		private const int MAX_DEPTH = 125;
		private const int MAX_NESTED_BRACKET_PAIRS = 63;

		private const string LeftBracketsList = "\u0028\u005B\u007B\u0F3A\u0F3C\u169B\u2045\u207D\u208D\u2308\u230A\u2329\u2768\u276A\u276C\u276E\u2770\u2772\u2774\u27C5\u27E6\u27E8\u27EA\u27EC\u27EE\u2983\u2985\u2987\u2989\u298B\u298D\u298F\u2991\u2993\u2995\u2997\u29D8\u29DA\u29FC\u2E22\u2E24\u2E26\u2E28\u3008\u300A\u300C\u300E\u3010\u3014\u3016\u3018\u301A\uFE59\uFE5B\uFE5D\uFF08\uFF3B\uFF5B\uFF5F\uFF62";
		private const string RightBracketsList = "\u0029\u005D\u007D\u0F3B\u0F3D\u169C\u2046\u207E\u208E\u2309\u230B\u232A\u2769\u276B\u276D\u276F\u2771\u2773\u2775\u27C6\u27E7\u27E9\u27EB\u27ED\u27EF\u2984\u2986\u2988\u298A\u298C\u298E\u2990\u2992\u2994\u2996\u2998\u29D9\u29DB\u29FD\u2E23\u2E25\u2E27\u2E29\u3009\u300B\u300D\u300F\u3011\u3015\u3017\u3019\u301B\uFE5A\uFE5C\uFE5E\uFF09\uFF3D\uFF5D\uFF60\uFF63";
		#endregion

		#region Structures
		private struct DirectionalStatus
		{
			internal byte paragraphEmbeddingLevel;        // 0 >= value <= MAX_DEPTH
			internal byte directionalOverrideStatus;      // N, R or L
			internal bool directionalIsolateStatus;
		}

		private class IsolatingRunSequence
		{
			public byte level;
			public BidiClass sos;
			public BidiClass eos;
			public int length;
			public int[] indexes;
			public byte[] types;
			public byte[] resolvedLevels;

			public IsolatingRunSequence(byte paragraphEmbeddingLevel, List<int> runIndexList, byte[] types, byte[] levels)
			{
				ComputeIsolatingRunSequence(this, paragraphEmbeddingLevel, runIndexList, types, levels);
			}
		}
		#endregion

		#region Enum BidiClass
		private enum BidiClass : byte
		{
			L,              // 0  Left-to-Right
			LRE,            // 1  Left-to-Right Embedding
			LRO,            // 2  Left-to-Right Override
			R,              // 3  Right-to-Left
			AL,             // 4  Right-to-Left Arabic
			RLE,            // 5  Right-to-Left Embedding
			RLO,            // 6  Right-to-Left Override
			PDF,            // 7  Pop Directional Format
			EN,             // 8  European Number
			ES,             // 9  European Number Separator
			ET,             // 10 European Number Terminator
			AN,             // 11 Arabic Number
			CS,             // 12 Common Number Separator
			NSM,            // 13 Nonspacing Mark
			BN,             // 14 Boundary Neutral
			B,              // 15 Paragraph Separator
			S,              // 16 Segment Separator
			WS,             // 17 Whitespace
			ON,             // 18 Other Neutrals
			LRI,            // 19 Left-to-Right Isolate
			RLI,            // 20 Right-to-left Isolate
			FSI,            // 21 First Strong Isolate
			PDI             // 22 Pop Directional Isolate
		}
		#endregion

		#region Method Convert
		public static StringBuilder ConvertStringBuilder(StringBuilder input, bool rightToLeft, bool modePdf = true)
		{
			return new StringBuilder(ConvertString(input.ToString(), rightToLeft, modePdf));
		}
		public static string ConvertString(string input, bool rightToLeft, bool modePdf = true)
		{
			if (input == null) return string.Empty;
			if (string.IsNullOrWhiteSpace(input)) return input;

			input = ConvertArabicHebrew(input).ToString();

			string output = LogicalToVisual(input, rightToLeft);

			if (modePdf)
			{
				if ((StiOptions.Export.Pdf.ConvertDigitsToArabic) && rightToLeft)
				{
					output = StiExportUtils.ConvertDigitsToArabic(output, StiOptions.Export.Pdf.ArabicDigitsType);
				}
			}

			return output;
		}
        #endregion

        #region Method LogicalToVisual
		//Entry point for algorithm to return at final correct display order
		private static string LogicalToVisual(string input, bool rightToLeft, int[] lineBreaks = null)
		{
			// Optimization:
			// Only continue if an RTL character is present
			bool found = rightToLeft;
			if (!found)
			{
				foreach (char ch in input)
				{
					BidiClass ct = (BidiClass)Bidi_Types.BidiCharTypes[ch];
					if (ct == BidiClass.R || ct == BidiClass.AL || ct == BidiClass.AN)
					{
						found = true;
						break;
					}
				}
			}
			if (!found) return input;

			int inputLength = input.Length;
			byte[] typesList = new byte[input.Length];
			byte[] levelsList = new byte[input.Length];
			int[] matchingPDI;
			int[] matchingIsolateInitiator;

			// Analyze text bidi_class types
			ClassifyCharacters(input, ref typesList);

			// Determine Matching PDI
			GetMatchingPDI(typesList, out matchingPDI, out matchingIsolateInitiator);

			// 3.3.1 Determine paragraph embedding level
			byte baseLevel = GetParagraphEmbeddingLevel(typesList, matchingPDI);
			//if (rightToLeft) baseLevel = 1;
			baseLevel = (byte)(rightToLeft ? 1 : 0);	//for compatibility with Net

			// Initialize levelsList to paragraph embedding level
			SetLevels(ref levelsList, baseLevel);

			// 3.3.2 (X1-X8) Determine explicit embedding levels and directions
			GetExplicitEmbeddingLevels(baseLevel, typesList, ref levelsList, matchingPDI);

			/*
            ** Isolating run sequences
            ** 3.3.3,  3.3.4,  3.3.5,  3.3.6
            ** X9,X10  W1-W7   N0-N2   I1-I2
            */

			// X9 Remove all RLE, LRE, RLO, LRO, PDF and BN characters
			// Instead of removing, assign the embedding level to each formatting 
			// character and turn it (type or level?) to BN.
			// The goal in marking a formatting or control character as BN is that it 
			// has no effect on the rest of the algorithm (ZWJ and ZWNJ are exceptions).
			RemoveX9Characters(ref typesList);

			// X10 steps
			// .1 Compute isolating run sequences according to BD13. Apply next rules to each sequence
			var levelRuns = GetLevelRuns(levelsList);

			// Determine each character belongs to what run
			int[] runCharsArray = GetRunForCharacter(levelRuns, inputLength);

			var sequences = GetIsolatingRunSequences(baseLevel, typesList, levelsList, levelRuns, matchingIsolateInitiator,
													 matchingPDI, runCharsArray);

			foreach (var sequence in sequences)
			{
				// Rules W1-W7
				sequence.ResolveWeaks();

				// Rules N0-N2
				sequence.ResolveNeutrals(input);

				// Rules I1-I2
				sequence.ResolveImplicit();

				sequence.ApplyTypesAndLevels(ref typesList, ref levelsList);
			}

			byte[] levels;

			// Rules L1-L2
			var lines = lineBreaks == null ? new int[] { typesList.Length } : lineBreaks;
			int[] newIndexes = GetReorderedIndexes(baseLevel, typesList, levelsList, lines, out levels);

			// Return new text from ordered levels
			var finalStr = GetOrderedString(input, newIndexes, levels);

			return finalStr;
		}
		#endregion

		#region Method ConvertArabic
		public static StringBuilder ConvertArabicHebrew(StringBuilder inputSB)
        {
			return ConvertArabicHebrew(inputSB.ToString());
        }

		public static StringBuilder ConvertArabicHebrew(string inputSB)
		{
			StringBuilder tempSB = new StringBuilder();
			if (inputSB == null) return tempSB;

			//check for symbols ranges
			bool hasArabic = false;
			bool hasLigatures = false;
			foreach (char ch in inputSB)
            {
				byte bb = (byte)((ch >> 8) & 0x00FF);
				if (bb == 0x05 || bb == 0x06 || bb == 0xFB) hasArabic = true;
				if (bb == 0x05 || bb == 0xFB || bb == 0xFE) hasLigatures = true;
			}
			if (hasArabic) hasLigatures = true;		//some arabic symbols also connected in ligatures

			if (hasArabic)
			{
				int index = 0;
				while (index < inputSB.Length)
				{
					ushort tempInt = Bidi_Types.GetArabicTableValue(inputSB[index], 0);
					switch (tempInt)
					{
						case 3:
							{
								//get sequence
								StringBuilder sb = new StringBuilder();
								sb.Append(inputSB[index]);
								while ((index + 1 < inputSB.Length) &&
									(Bidi_Types.GetArabicTableValue(inputSB[index + 1], 0) == 3))
								{
									index++;
									sb.Append(inputSB[index]);
								}
								if ((index + 1 < inputSB.Length) &&
									(Bidi_Types.GetArabicTableValue(inputSB[index + 1], 0) == 2))
								{
									index++;
									sb.Append(inputSB[index]);
								}
								//replace symbols
								if (sb.Length == 1)
								{
									tempSB.Append((char)Bidi_Types.GetArabicTableValue(sb[0], 1));    //isolated
								}
								else
								{
									tempSB.Append((char)Bidi_Types.GetArabicTableValue(sb[0], 3));    //initial
									if (sb.Length > 2)
									{
										for (int tempIndex = 1; tempIndex < sb.Length - 1; tempIndex++)
										{
											tempSB.Append((char)Bidi_Types.GetArabicTableValue(sb[tempIndex], 4));    //medial
										}
									}
									tempSB.Append((char)Bidi_Types.GetArabicTableValue(sb[sb.Length - 1], 2));    //final
								}
							}
							break;
						case 2:
						case 1:
							{
								tempSB.Append((char)Bidi_Types.GetArabicTableValue(inputSB[index], 1));   //isolated
							}
							break;
						default:
							tempSB.Append(inputSB[index]);
							break;
					}
					index++;
				}
			}
			else
            {
				tempSB.Append(inputSB);
            }

			if (hasLigatures)
			{
				for (int index3 = 0; index3 < Bidi_Types.LigaturesTable.GetLength(0); index3++)
				{
					tempSB = tempSB.Replace(Bidi_Types.LigaturesTable[index3, 0], Bidi_Types.LigaturesTable[index3, 1]);
				}
			}

			return tempSB;
		}
		#endregion

		#region Methods
		// 3.2 Determine Bidi_class of each input character
		private static void ClassifyCharacters(string text, ref byte[] typesList)
		{
			typesList = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				int chIndex = Convert.ToInt32(text[i]);
				typesList[i] = Bidi_Types.BidiCharTypes[chIndex];
			}
		}

		// Rules P2, P3 Determine paragraph embedding level given types array and optional 
		// start and end index to treat types as a scoped paragraph (useful for rule X5c)
		private static byte GetParagraphEmbeddingLevel(byte[] types, int[] matchingPDI, int si = -1, int ei = -1)
		{
			int start = si != -1 ? si : 0;
			int end = ei != -1 ? ei : types.Length;

			// Find first L, AL or R character
			for (int i = start; i < end; i++)
			{
				var cct = (BidiClass)types[i];
				if (cct == BidiClass.L ||
				   cct == BidiClass.AL ||
				   cct == BidiClass.R)
				{
					if (cct == BidiClass.L) return 0;
					else return 1;
				}
				else if (cct == BidiClass.LRI ||
						cct == BidiClass.RLI ||
						cct == BidiClass.FSI)
				{
					// Skip characters between isolate initiator and matching PDI (if found)
					i = matchingPDI[i];
				}
			}

			return 0;   // default, no strong character type found
		}

		// 3.3.2 Determine Explicit Embedding Levels and directions
		private static void GetExplicitEmbeddingLevels(byte level, byte[] types, ref byte[] levels, int[] matchingPDI)
		{
			// X1.
			// Directional Status Stack and entry
			Stack<DirectionalStatus> dirStatusStack = new Stack<DirectionalStatus>(MAX_DEPTH + 2);
			DirectionalStatus dirEntry = new DirectionalStatus
			{
				paragraphEmbeddingLevel = level,
				directionalOverrideStatus = (int)BidiClass.ON,
				directionalIsolateStatus = false
			};
			dirStatusStack.Push(dirEntry);

			int overflowIsolateCount = 0;
			int overflowEmbeddingCount = 0;
			int validIsolateCount = 0;

			// X2-X8
			for (int i = 0; i < types.Length; i++)
			{
				BidiClass cct = (BidiClass)types[i];
				switch (cct)
				{
					case BidiClass.RLE:
					case BidiClass.RLO:
					case BidiClass.LRE:
					case BidiClass.LRO:
					case BidiClass.LRI:
					case BidiClass.RLI:
					case BidiClass.FSI:
						{
							byte newLevel;      // New calculated embedding level

							bool isIsolate = (cct == BidiClass.RLI || cct == BidiClass.LRI);

							// X5a, X5b .1 isolate embedding level
							if (isIsolate)
							{
								levels[i] = dirStatusStack.Peek().paragraphEmbeddingLevel;
							}

							// X5c. Get embedding level of characters between FSI and its matching PDI
							// FSI = RLI if embedding level is 1, otherwise LRI

							if (cct == BidiClass.FSI)
							{
								byte el = GetParagraphEmbeddingLevel(types, matchingPDI, i + 1, matchingPDI[i]);
								cct = el == 1 ? BidiClass.RLI : BidiClass.LRI;
							}

							// 1 (RLE RLO RLI, LRE LRO LRI) Compute least odd/even embedding level greater than embedding level
							//  of last entry on directional status stack
							if (cct == BidiClass.RLE || cct == BidiClass.RLO || cct == BidiClass.RLI)
							{
								newLevel = (byte)LeastGreaterOdd(dirStatusStack.Peek().paragraphEmbeddingLevel);
							}
							else
							{
								newLevel = (byte)LeastGreaterEven(dirStatusStack.Peek().paragraphEmbeddingLevel);
							}

							// 2 New level would be valid(level <= max_depth) and overflow isolate count and
							// overflow embedding count are both zero => this RLE is valid, increment isolate counter.
							if (newLevel <= MAX_DEPTH && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
							{
								// X5b .3
								if (isIsolate)
								{
									validIsolateCount++;
								}

								// Push new entry to stack
								byte dos = cct == BidiClass.RLO ? (byte)BidiClass.R  // RLO = R directional override status
										: cct == BidiClass.LRO ? (byte)BidiClass.L   // LRO = L directional override status
										: (byte)BidiClass.ON;                        // All rest are neutrals
								dirStatusStack.Push(new DirectionalStatus()
								{
									paragraphEmbeddingLevel = newLevel,
									directionalOverrideStatus = dos,
									directionalIsolateStatus = isIsolate
								});
							}
							// 3 Otherwise, this is an overflow RLE. If the overflow isolate count is zero, 
							// increment the overflow embedding count by one. Leave all other variables unchanged.
							else
							{
								if (overflowIsolateCount == 0)
								{
									overflowEmbeddingCount++;
								}
							}
						}
						break;

					// X6a Terminating Isolates
					case BidiClass.PDI:
						{
							if (overflowIsolateCount > 0)   // This PDI matches an overflow isolate initiator
							{
								overflowIsolateCount--;
							}
							else if (validIsolateCount == 0)
							{
								// No matching isolator (valid or overflow), do nothing
							}
							else // This PDI matches a valid isolate initiator
							{
								overflowEmbeddingCount = 0;

								while (dirStatusStack.Peek().directionalIsolateStatus == false)
								{
									dirStatusStack.Pop();
								}

								dirStatusStack.Pop();
								validIsolateCount--;
							}

							levels[i] = dirStatusStack.Peek().paragraphEmbeddingLevel;
						}
						break;

					// X7
					case BidiClass.PDF:
						{
							if (overflowIsolateCount > 0) // X7 .1
							{
								// Do nothing
							}
							else if (overflowEmbeddingCount > 0) // X7 .2
							{
								overflowEmbeddingCount--;
							}
							else if (!dirStatusStack.Peek().directionalIsolateStatus && dirStatusStack.Count > 1) // X7 .3
							{
								dirStatusStack.Pop();
							}
							else
							{
								// Do nothing
							}
						}
						break;

					// X8
					case BidiClass.B:
						{
							// Paragraph separators.
							// Applied at the end of paragraph (last character in array).

							// 1 Terminate(reset) all directional embeddings, overrides and isolates 
							overflowEmbeddingCount = 0;
							overflowIsolateCount = 0;
							validIsolateCount = 0;
							dirStatusStack.Clear();
							dirStatusStack.Push(dirEntry);

							// 2 Assign separator character an embedding level equal to paragraph embedding level
							levels[i] = level;
						}
						break;

					// X6 Non-formatting characters
					default:
						{
							levels[i] = dirStatusStack.Peek().paragraphEmbeddingLevel;
							if (dirStatusStack.Peek().directionalOverrideStatus != (int)BidiClass.ON) // X6.b (6.2.0 naming)
							{
								types[i] = dirStatusStack.Peek().directionalOverrideStatus; // reset type to last element status
							}
						}
						break;
				}
			}
		}

		// 3.3.3 Resolve Weak Types
		private static void ResolveWeaks(this IsolatingRunSequence sequence)
		{
			// W1 NSM
			for (int i = 0; i < sequence.length; i++)
			{
				var ct = (BidiClass)sequence.types[i];
				var prevType = i == 0 ? sequence.sos : (BidiClass)sequence.types[i - 1];
				if (ct == BidiClass.NSM)
				{
					// if NSM is at start of sequence resolved to sos type
					// assign ON if previous is isolate initiator or PDI, otherwise type of previous
					bool isIsolateOrPDI = prevType == BidiClass.LRI ||
										  prevType == BidiClass.RLI ||
										  prevType == BidiClass.FSI ||
										  prevType == BidiClass.PDI;

					sequence.types[i] = isIsolateOrPDI ? (byte)BidiClass.ON : (byte)prevType;
				}
			}

			// W2 EN
			// At each EN search in backward until first strong type is found, if AL is found then resolve to AN
			for (int i = 0; i < sequence.length; i++)
			{
				var chType = (BidiClass)sequence.types[i];
				if (chType == BidiClass.EN)
				{
					for (int j = i - 1; j >= 0; j--)
					{
						var type = (BidiClass)sequence.types[j];
						if (type == BidiClass.R || type == BidiClass.AL || type == BidiClass.L)
						{
							if (type == BidiClass.AL)
							{
								sequence.types[i] = (byte)BidiClass.AN;
								break;
							}
						}
					}
				}
			}

			// W3 AL
			// Resolve all ALs to R
			for (int i = 0; i < sequence.length; i++)
			{
				if ((BidiClass)sequence.types[i] == BidiClass.AL)
				{
					sequence.types[i] = (byte)BidiClass.R;
				}
			}

			// W4 ES, CS (Number Separators)
			// ES between EN is resolved to EN
			// Single CS between same numbers type is resolve to that number type
			for (int i = 1; i < sequence.length - 1; i++)
			{
				var cct = (BidiClass)sequence.types[i];
				var prevType = (BidiClass)sequence.types[i - 1];
				var nextType = (BidiClass)sequence.types[i + 1];

				if (cct == BidiClass.ES && prevType == BidiClass.EN && nextType == BidiClass.EN) // EN ES EN -> EN EN EN
				{
					sequence.types[i] = (byte)BidiClass.EN;
				}
				else if (cct == BidiClass.CS && (
				prevType == BidiClass.EN && nextType == BidiClass.EN ||
				prevType == BidiClass.AN && nextType == BidiClass.AN))      // EN CS EN -> EN EN EN, AN CS AN -> AN AN AN
				{
					sequence.types[i] = (byte)prevType;
				}
			}

			// W5 ET(s) adjacent to EN resolve to EN(s)
			var typesSet = new BidiClass[] { BidiClass.ET };
			for (int i = 0; i < sequence.length; i++)
			{
				if ((BidiClass)sequence.types[i] == BidiClass.ET)
				{
					int runStart = i;
					// int runEnd = runStart;
					// runEnd = Array.FindIndex(sequence.types, runStart, t1 => typesSet.Any(t2 => t2 == (BidiClass)t1));
					int runEnd = sequence.GetRunLimit(runStart, sequence.length, typesSet);

					var type = runStart > 0 ? (BidiClass)sequence.types[runStart - 1] : sequence.sos;

					if (type != BidiClass.EN)
					{
						type = runEnd < sequence.length ? (BidiClass)sequence.types[runEnd] : sequence.eos; // End type
					}

					if (type == BidiClass.EN)
					{
						sequence.SetRunTypes(runStart, runEnd, BidiClass.EN); // Resolve to EN
					}

					i = runEnd; // advance to end of sequence
				}
			}

			// W6 Separators and Terminators -> ON
			for (int i = 0; i < sequence.length; i++)
			{
				var t = (BidiClass)sequence.types[i];
				if (t == BidiClass.ET || t == BidiClass.ES || t == BidiClass.CS)
				{
					sequence.types[i] = (byte)BidiClass.ON;
				}
			}

			// W7 same as W2 but EN -> L
			for (int i = 0; i < sequence.length; i++)
			{
				if ((BidiClass)sequence.types[i] == BidiClass.EN)
				{
					var prevStrong = sequence.sos;  // Default to sos if reached start
					for (int j = i - 1; j >= 0; j--)
					{
						var t = (BidiClass)sequence.types[j];
						if (t == BidiClass.R || t == BidiClass.L || t == BidiClass.AL)
						{
							prevStrong = t;
							break;
						}

						if (prevStrong == BidiClass.L)
						{
							sequence.types[i] = (byte)BidiClass.L;
						}
					}
				}
			}

		}

		// 3.3.4 Resolve Neutral Types
		// In final results all NIs are resolved to R or L
		private static void ResolveNeutrals(this IsolatingRunSequence sequence, string input)
		{
			// N0 rule (Paired Brackets algorithm)
			// written by Ishma, пока ещё не полностью разобрался во всех нюансах, поэтому возможно придётся дорабатывать 

			// Build the pair stack.
			int[,] pairStack = new int[MAX_NESTED_BRACKET_PAIRS, 2];
			int stackPointer = 0;
			List<KeyValuePair<int, int>> pairList = new List<KeyValuePair<int, int>>();

			for (int i = 0; i < sequence.length; i++)
            {
				char ch = input[sequence.indexes[i]];
				if (IsBracketTypeOpen(ch))
                {
					if (stackPointer >= MAX_NESTED_BRACKET_PAIRS) break;
					pairStack[stackPointer, 0] = ch;
					pairStack[stackPointer, 1] = i;
					stackPointer++;
				}
				else if (IsBracketTypeClose(ch))
                {
					if (stackPointer == 0) continue;
					int stackP2 = stackPointer - 1;
					char chPair = (char)Bidi_Types.MirrorChars[ch];
					while (stackP2 >= 0)
                    {
						if (pairStack[stackP2, 0] == chPair)
                        {
							//pos stack to current level
							stackPointer = stackP2;
							//store pair
							pairList.Add(new KeyValuePair<int, int>(pairStack[stackP2, 1], i));
							break;
                        }
						stackP2--;
                    }
                }
			}

			// Start the N0
			foreach (var pair in pairList)
            {
				bool found = false;
				BidiClass strongType = BidiClass.ON;
				// Find matching strong
				for (int i = pair.Key + 1; i < pair.Value; i++)
                {
					var ct = (BidiClass)sequence.types[i];
					if (ct == BidiClass.AN || ct == BidiClass.EN) ct = BidiClass.R;
					if (ct == BidiClass.L || ct == BidiClass.R)
					{
						strongType = ct;
						if (sequence.sos == ct)
						{
							found = true;
							break;
						}
                    }
				}
				if (!found)
                {
					// N0c - Search for any strong type preceding and within the bracket pair
					for (int i = pair.Key - 1; i >= 0; i--)
					{
						var ct = (BidiClass)sequence.types[i];
						if (ct == BidiClass.AN || ct == BidiClass.EN) ct = BidiClass.R;
						if (ct == BidiClass.L || ct == BidiClass.R)
						{
							if (strongType == ct)
							{
								found = true;
								break;
							}
						}
					}

				}
				if (!found)
                {
					strongType = sequence.sos;
                }
				sequence.types[pair.Key] = (byte)strongType;
				sequence.types[pair.Value] = (byte)strongType;
			}


			// N1
			// Sequence of NIs will resolve to surrounding "strong" type if text on both sides was of same direction.
			// sos and eos are used at run sequence boundaries. AN and EN will resolve type to R.
			var typesSet = new BidiClass[] { BidiClass.B, BidiClass.S, BidiClass.WS, BidiClass.ON, BidiClass.LRI, BidiClass.RLI, BidiClass.FSI, BidiClass.PDI };
			for (int i = 0; i < sequence.length; i++)
			{
				var ct = (BidiClass)sequence.types[i];
				bool isNI = ct == BidiClass.B ||
							ct == BidiClass.S ||
							ct == BidiClass.WS ||
							ct == BidiClass.ON ||
							ct == BidiClass.LRI ||
							ct == BidiClass.RLI ||
							ct == BidiClass.FSI ||
							ct == BidiClass.PDI;

				if (isNI)
				{
					BidiClass leadType = 0;
					BidiClass trailType = 0;
					int start = i;
					int runEnd = sequence.GetRunLimit(start, sequence.length, typesSet);

					// Start of matching NI
					if (start == 0) // Start boundary, lead type = sos
					{
						leadType = sequence.sos;
					}
					else
					{
						leadType = (BidiClass)sequence.types[start - 1];
						if (leadType == BidiClass.AN || leadType == BidiClass.EN)   // Leading AN, EN resolve type to R
						{
							leadType = BidiClass.R;
						}
					}

					// End of Matching NI
					if (runEnd == sequence.length) // End boundary. trail type = eos
					{
						trailType = sequence.eos;
					}
					else
					{
						trailType = (BidiClass)sequence.types[runEnd];
						if (trailType == BidiClass.AN || trailType == BidiClass.EN)
						{
							trailType = BidiClass.R;
						}
					}

					if (leadType == trailType)
					{
						sequence.SetRunTypes(start, runEnd, leadType);
					}
					else    // N2
					{
						// Remaining NIs take current run embedding level
						var runDirection = GetTypeForLevel(sequence.level);
						sequence.SetRunTypes(start, runEnd, runDirection);
					}

					i = runEnd;
				}
			}
		}

		// 3.3.5 Resolve Implicit Embedding Levels
		private static void ResolveImplicit(this IsolatingRunSequence sequence)
		{
			byte level = sequence.level;

			// Initialize the sequence resolved levels with sequence embedding level
			sequence.resolvedLevels = new byte[sequence.length];
			SetLevels(ref sequence.resolvedLevels, sequence.level);

			for (int i = 0; i < sequence.length; i++)
			{
				var ct = (BidiClass)sequence.types[i];

				// I1
				// Sequence level is even (Left-to-right) then R types go up one level, AN and EN go up two levels
				if (!IsOdd(level))
				{
					if (ct == BidiClass.R)
					{
						sequence.resolvedLevels[i] += 1;
					}
					else if (ct == BidiClass.AN || ct == BidiClass.EN)
					{
						sequence.resolvedLevels[i] += 2;
					}
				}
				// N2
				// Sequence level is odd (Right-to-left) then L, AN, EN go up one level
				else
				{
					if (ct == BidiClass.L || ct == BidiClass.AN || ct == BidiClass.EN)
					{
						sequence.resolvedLevels[i] += 1;
					}
				}
			}
		}

		private static void ApplyTypesAndLevels(this IsolatingRunSequence sequence, ref byte[] typesList, ref byte[] levelsList)
		{
			for (int i = 0; i < sequence.length; i++)
			{
				int idx = sequence.indexes[i];
				typesList[idx] = sequence.types[i];
				levelsList[idx] = sequence.resolvedLevels[i];
			}
		}

		// Entry for Rules L1-L2
		// Return the final ordered levels array including the line breaks
		private static int[] GetReorderedIndexes(byte level, byte[] typesList, byte[] levelsList, int[] lineBreaks, out byte[] levels)
		{
			levels = GetTextLevels(level, typesList, levelsList, lineBreaks);

			var multilineLevels = GetMultiLineReordered(levels, lineBreaks);

			return multilineLevels;
		}

		private static void GetMatchingPDI(byte[] types, out int[] outMatchingPDI, out int[] outMatchingIsolateInitiator)
		{
			int[] matchingPDI = new int[types.Length];
			int[] matchingIsolateInitiator = new int[types.Length];

			// Scan for isolate initiator
			for (int i = 0; i < types.Length; i++)
			{
				var cct = (BidiClass)types[i];
				if (cct == BidiClass.LRI ||
				   cct == BidiClass.RLI ||
				   cct == BidiClass.FSI)
				{
					int counter = 1;
					bool hasMatchingPDI = false;

					// Scan the text following isolate initiator till end of paragraph
					for (int j = i + 1; j < types.Length; j++)
					{
						BidiClass nct = (BidiClass)types[j];
						if (nct == BidiClass.LRI ||
						   nct == BidiClass.RLI ||
						   nct == BidiClass.FSI)        // Increment counter at every isolate initiator
						{
							counter++;
						}
						else if (nct == BidiClass.PDI)   // Decrement counter at every PDI
						{
							counter--;

							if (counter == 0)            // BD9 bullet 3. Stop when counter is 0
							{
								hasMatchingPDI = true;
								matchingPDI[i] = j;      // Matching PDI found
								matchingIsolateInitiator[j] = i;
								break;
							}

						}
					}

					if (!hasMatchingPDI)
					{
						matchingPDI[i] = types.Length;
					}
				}
				else        // Other characters matchingPDI are set to -1
				{
					matchingPDI[i] = -1;
					matchingIsolateInitiator[i] = -1;
				}
			}

			outMatchingPDI = matchingPDI;
			outMatchingIsolateInitiator = matchingIsolateInitiator;
		}

		private static void RemoveX9Characters(ref byte[] buffer)
		{
			// Todo: ZWJ and ZWNJ characters exception from BN overriding

			// Replace Embedding and override type with BN
			for (int i = 0; i < buffer.Length; i++)
			{
				var ct = (BidiClass)buffer[i];
				if (ct == BidiClass.LRE || ct == BidiClass.RLE ||
				   ct == BidiClass.LRO || ct == BidiClass.RLO)
				{
					buffer[i] = (byte)BidiClass.BN;
				}
			}
		}

		private static List<List<int>> GetLevelRuns(byte[] levels)
		{
			List<int> runList = new List<int>();
			List<List<int>> allRunsList = new List<List<int>>();

			sbyte currentLevel = -1;
			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i] != currentLevel)        // New run
				{
					if (currentLevel >= 0)           // Assign last run
					{
						allRunsList.Add(runList);
						runList.Clear();
					}

					currentLevel = (sbyte)levels[i];       // New run level
				}

				runList.Add(i);
			}

			// Append last run
			if (runList.Count > 0)
			{
				allRunsList.Add(runList);
			}

			return allRunsList;
		}

		// Map each character to its belonging run
		private static int[] GetRunForCharacter(List<List<int>> levelRuns, int length)
		{
			int[] runCharsArray = new int[length];
			for (int i = 0; i < levelRuns.Count; i++)
			{
				for (int j = 0; j < levelRuns[i].Count; j++)
				{
					int chPos = levelRuns[i][j];
					runCharsArray[chPos] = chPos;
				}
			}

			return runCharsArray;
		}

		private static List<IsolatingRunSequence> GetIsolatingRunSequences(byte pLevel, byte[] types, byte[] levels,
		List<List<int>> levelRuns, int[] matchingIsolateInitiator, int[] matchingPDI, int[] runCharsArray)
		{
			List<IsolatingRunSequence> allRunSequences = new List<IsolatingRunSequence>(levelRuns.Count);

			foreach (var run in levelRuns)
			{
				List<int> currRunSequence;
				var first = run[0];

				if ((BidiClass)types[first] != BidiClass.PDI || matchingIsolateInitiator[first] == -1) // BD13 bullet 2
				{
					currRunSequence = new List<int>(run);           // initialize a new level run sequence with current run

					int lastCh = currRunSequence[currRunSequence.Count - 1];
					var lastType = (BidiClass)types[lastCh];
					bool isIsolateInitiator = lastType == BidiClass.RLI ||
											   lastType == BidiClass.LRI ||
											   lastType == BidiClass.FSI;

					int lastChMatchingPDI = matchingPDI[lastCh];
					while (isIsolateInitiator && lastChMatchingPDI != types.Length)
					{
						var lChRunIndex = runCharsArray[lastChMatchingPDI]; // Get run index for last character that has matchingPDI
						var newRun = levelRuns[lChRunIndex];
						currRunSequence.AddRange(newRun);
					}

					allRunSequences.Add(new IsolatingRunSequence(pLevel, currRunSequence, types, levels));
				}
			}

			return allRunSequences;
		}

		// X10 bullet 2 Determine start and end of sequence types (R or L) for an isolating run sequence
		// using run sequence indexes
		private static void ComputeIsolatingRunSequence(this IsolatingRunSequence sequence, byte pLevel, List<int> indexList,
		byte[] typesList, byte[] levels)
		{
			sequence.length = indexList.Count;
			sequence.indexes = indexList.ToArray();                     // Indexes of run in original text

			// Character types of run sequence
			sequence.types = new byte[indexList.Count];
			for (int i = 0; i < sequence.length; i++)
			{
				sequence.types[i] = typesList[indexList[i]];
			}

			// sos
			var firstLevel = levels[indexList[0]];      // level of first character
			sequence.level = firstLevel;
			var previous = indexList[0] - 1;
			var prevLevel = previous >= 0 ? levels[previous] : pLevel;
			sequence.sos = GetTypeForLevel(Math.Max(firstLevel, prevLevel));

			// eos
			var lastType = (BidiClass)sequence.types[sequence.length - 1];
			var last = indexList[sequence.length - 1];       // last character in the sequence
			var lastLevel = levels[last];
			var next = indexList[sequence.length - 1] + 1;   // next character after sequence (in paragraph)
			var nextLevel = next < typesList.Length && lastType != BidiClass.PDI ? levels[last] : pLevel;
			sequence.eos = GetTypeForLevel(Math.Max(lastLevel, nextLevel));
		}

		// Override levels list with new level value
		private static void SetLevels(ref byte[] levels, byte newLevel)
		{
			for (int i = 0; i < levels.Length; i++)
			{
				levels[i] = newLevel;
			}
		}

		// Return end index of run consisting of types in typesSet
		// Start from index and check the value, if value not present in set then return index.
		private static int GetRunLimit(this IsolatingRunSequence sequence, int index, int limit, BidiClass[] typesSet)
		{
		loop: for (; index < limit;)
			{
				var type = (BidiClass)sequence.types[index];
				for (int i = 0; i < typesSet.Length; i++)
				{
					if (type == typesSet[i])
					{
						index++;
						goto loop;
					}
				}

				// No match in typesSet
				return index;
			}

			return limit;
		}

		// Override types list from start up to (not including) limit to newType
		private static void SetRunTypes(this IsolatingRunSequence sequence, int start, int limit, BidiClass newType)
		{
			for (int i = start; i < limit; i++)
			{
				sequence.types[i] = (byte)newType;
			}
		}

		// Compute least odd level greater than l
		private static int LeastGreaterOdd(int l)
		{
			return IsOdd(l) ? l + 2 : l + 1;
		}

		// Compute least even level greater than l
		private static int LeastGreaterEven(int l)
		{
			return !IsOdd(l) ? l + 2 : l + 1;
		}

		private static bool IsOdd(int n)
		{
			return (n & 1) != 0;
		}

		// Return L if level is even and R if Odd
		private static BidiClass GetTypeForLevel(byte level)
		{
			return (level & 1) == 0 ? BidiClass.L : BidiClass.R;
		}

		private static byte[] GetTextLevels(byte paragraphEmbeddingLevel, byte[] typesList, byte[] levelsList, int[] lineBreaks)
		{
			byte[] finalLevels = levelsList;

			// Rule L1
			// Level of S and B is changed to the paragraph embedding level.
			// Any sequence of whitespace and/or isolate formatting characters preceding S, B are changed to paragraph level
			for (int i = 0; i < finalLevels.Length; i++)
			{
				var t = (BidiClass)typesList[i];    // Types here are original ones not the output of previous stages

				if (t == BidiClass.S || t == BidiClass.B)
				{
					finalLevels[i] = paragraphEmbeddingLevel;
				}

				// Search backward for whitespace or isolates (LRI, RLI, FSI, PDI)
				for (int j = i - 1; j >= 0; j--)
				{
					t = (BidiClass)typesList[j];
					if (t == BidiClass.LRI ||
						t == BidiClass.RLI ||
						t == BidiClass.FSI ||
						t == BidiClass.FSI)
					{
						finalLevels[j] = paragraphEmbeddingLevel;
					}
					else
					{
						break;
					}
				}
			}

			// Search backward for any sequence of whitespace or isolates at ach line breaks (ends)
			int start = 0;
			for (int i = 0; i < lineBreaks.Length; i++)
			{
				int end = lineBreaks[i];    // Line limit (new line start)
				for (int j = end - 1; j >= start; j--)
				{
					var t = (BidiClass)typesList[j];
					if (t == BidiClass.LRI ||
						t == BidiClass.RLI ||
						t == BidiClass.FSI ||
						t == BidiClass.FSI)
					{
						finalLevels[j] = paragraphEmbeddingLevel;
					}
					else
					{
						break;
					}
				}

				start = end; // Reset start to new line start
			}

			return finalLevels;
		}

		// Compute correct text indexes using levels array and line breaks positions.
		// Line breaks should be calculated and supplied by the rendering system after shaping and bounds calculations
		private static int[] GetMultiLineReordered(byte[] levels, int[] lineBreaks)
		{
			int[] resultIndexes = new int[levels.Length];

			// Calculate lines levels separately and append them at their final offsets in levels array
			int start = 0;
			for (int i = 0; i < lineBreaks.Length; i++)
			{
				int end = lineBreaks[i];

				var tempLevels = new byte[end - start];  // Line levels
				Array.Copy(levels, start, tempLevels, 0, tempLevels.Length); // Copy line levels to work on it

				var tempReorderedIndexes = ComputeReorderingIndexes(tempLevels); // Rule L2 (reversing)
				for (int j = 0; j < tempReorderedIndexes.Length; j++)
				{
					resultIndexes[start + j] = tempReorderedIndexes[j] + start;
				}

				start = end; // Next line start
			}

			return resultIndexes;
		}

		// Rule L2
		private static int[] ComputeReorderingIndexes(byte[] levels)
		{
			int lineLength = levels.Length;

			// Initialize line indexes to logical order 0,1,2, etc..
			int[] resultIndexes = new int[lineLength];
			for (int i = 0; i < lineLength; i++)
			{
				resultIndexes[i] = i;
			}

			// Determine highest level on the text
			// scan for highest level and lowest odd level
			byte highestLevel = 0;
			byte lowestOddLevel = MAX_DEPTH + 2; // max value for odd levels
			foreach (var level in levels)
			{
				if (level > highestLevel) // highest level
				{
					highestLevel = level;
				}

				// lowest odd level (start from max possible odd levels down to lowest level found)
				if (IsOdd(level) && level < lowestOddLevel)
				{
					lowestOddLevel = level;
				}
			}

			for (int l = highestLevel; l >= lowestOddLevel; l--)    // Reverse from highest level down to lowest odd level
			{
				for (int i = 0; i < lineLength; i++)
				{
					if (levels[i] >= l)
					{
						int start = i;
						int end = i + 1;

						while (end < lineLength && levels[end] >= l)    // Text range at this level or above
						{
							end++;
						}

						for (int j = start, k = end - 1; j < k; j++, k--) // Reverse
						{
							int tmp = resultIndexes[j];
							resultIndexes[j] = resultIndexes[k];
							resultIndexes[k] = tmp;
						}

						i = end; // Skip to end
					}
				}
			}

			return resultIndexes;
		}

		// Return final correctly reversed string order
		private static string GetOrderedString(string input, int[] newIndexes, byte[] levels)
		{
			var sb = new StringBuilder(input.Length);
			for (int i = 0; i < newIndexes.Length; i++)
			{
				int index = newIndexes[i];
				char ch = input[index];
				if (levels[index] % 2 == 1)
                {
					char ch2 = (char)Bidi_Types.MirrorChars[ch];
					if (ch2 != 0)
                    {
						ch = ch2;
                    }
                }
				sb.Append(ch);
			}

			return sb.ToString();
		}

		private static bool IsBracketTypeOpen(char ch)
        {
			//return ch == '(' || ch == '{' || ch == '[';
			return LeftBracketsList.IndexOf(ch) != -1;
		}
		private static bool IsBracketTypeClose(char ch)
		{
			//return ch == ')' || ch == '}' || ch == ']';
			return RightBracketsList.IndexOf(ch) != -1;
		}
		#endregion

		#region Data - Bidi_Types
		private static class Bidi_Types
		{
			private const int arabicTableSize = 76 + 21;
			private const int ligaturesTableSize = 8 + 32;

			internal static byte[] BidiCharTypes = null;
			internal static ushort[] MirrorChars = null;

			internal static ushort[,] ArabicTable = null;
			internal static string[,] LigaturesTable = null;

			internal static ushort GetArabicTableValue(int sym, int column)
			{
				int index = 0;
				if ((sym & 0xFF00) == 0x0500 || (sym & 0xFF00) == 0x0600) index = sym - 0x0500;
				if ((sym & 0xFF00) == 0xFB00) index = sym - 0xF900;
				return ArabicTable[index, column];
			}

			static Bidi_Types()
			{
				#region BidiCharTypes
				byte[] packed = global::System.Convert.FromBase64String(
					"eJzt3Yt6myAYgOF/022Rimm4/4udiijgATVq0vZ722XK6QdE6+qepHI+75+PexW43+s0Y5RSplGURVneemWbaGTWmvwuUDWrbMK38evyVdOZ263ZNDdZan4h7Kp8vLv+0JvAeN8vEmXW9MWCrovr3tXr8eJwy7yZ+cm6Y6K3yOov++1ayeYN+V7Uj1qzAnOl8jIPzod" +
					"cJM/r77WmOti2b6mPj6ZQfoUu+ocbSZ1g7Mbw82uqnlSpDup5smGuNvKi5BLI1uubyLLmyuevgh/siQtPW3M4+jbBbQ+llgPsDz/EDxJGpaJUpRazN+jiu1fbkURzozLPx2/iDvM/LhWmhqOXJ+dfT8//lgE+0QGJaq9ryt74qCPuAdzY7SHoR6rT43c3ZM+IG56OFK" +
					"WaI8bt2Xz+nWWYfC0nhtTi37C5s8m/Cp6rjx+vsmPDL7bVTLDeuXx1ILh8+dlRuWlDo3KJNmYX2l7xF4p2FcYZUZrXzkXDAC6yeUUPv5kQ4Nt7zGe96BTQevmkTecvZifzV4luAIabMi9VTV194uvLsN/8gmThaLzaEbO2g3Y3MkNKkNFMo3nba7VJmqmoXzTdlxvdj" +
					"q7Q37V3eyvF52eXKAm2hl4d5jxD/9+hN2jodWZrC76u9nmmvx9dz9P78hhUVSXZ3M+Jx/3Xn3+//5bKPY73lGbWo32I3wazz/Nv9ZZ9/lIUdb4Mz2PsfkxNklmpEyFe8N0z4eBn+DCtdhBivyT85bEtYgu4La9SRzYyL1fEhxcjcoSF5s3htnciMFfdr5/KX3SbIIcy" +
					"+HLCozY6hOJddffqG15eIWuWj7iGpC++b9nJfLUoIzj/lurNkR/OAEdx60nwVszEUTFTid9P6p9DZis51kIgc5Z18b8ymRWXenQbZk5fs1svXSuuMTlLszRFzvyfyic2fR2zguy1o+qW2Lu6ttB+Kt4buairySCpfAAAAAAAAAAAAFzMPEkAAAAAAAAAAAAAAAAAAAA" +
					"AAAAAAAAAAAAAAAAAAAAAAD+C2UdwgPMncvgIWek/4vWlTJJc4qIwE3T3x75MF9GHfmCpOXhW5zt+FPdp2ckCS2W+Njc498fLGW0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACslemsV2Q+6V4l676d/HSpTkuOq4zfxzR/0t4WZKWgkgneuLQVv+9t/KamJq" +
					"E0pdQvjjKmKOq/xShl+vg7x3g4kaoekKq71ijKoixvvW4QMmtdfqLQ2dqxKSWjjtjd/2GsGxo=");

				var ms = new MemoryStream();
				ms.Write(packed, 2, packed.Length - 2);
				ms.Seek(0, SeekOrigin.Begin);
				DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
				BidiCharTypes = new byte[0xffff];
				ds.Read(BidiCharTypes, 0, 0xffff);
				ds.Close();
				#endregion

				#region Arabic table
				ushort[,] tempArabicTable = new ushort[arabicTableSize, 6]
				{
					// presentation form 
					// code, mode, isolated, final, initial, medial
                    {0x0621,1,0xFE80,0x0000,0x0000,0x0000},
					{0x0622,2,0xFE81,0xFE82,0x0000,0x0000},
					{0x0623,2,0xFE83,0xFE84,0x0000,0x0000},
					{0x0624,2,0xFE85,0xFE86,0x0000,0x0000},
					{0x0625,2,0xFE87,0xFE88,0x0000,0x0000},
					{0x0626,3,0xFE89,0xFE8A,0xFE8B,0xFE8C},
					{0x0627,2,0xFE8D,0xFE8E,0x0000,0x0000},
					{0x0628,3,0xFE8F,0xFE90,0xFE91,0xFE92},
					{0x0629,2,0xFE93,0xFE94,0x0000,0x0000},
					{0x062A,3,0xFE95,0xFE96,0xFE97,0xFE98},
					{0x062B,3,0xFE99,0xFE9A,0xFE9B,0xFE9C},
					{0x062C,3,0xFE9D,0xFE9E,0xFE9F,0xFEA0},
					{0x062D,3,0xFEA1,0xFEA2,0xFEA3,0xFEA4},
					{0x062E,3,0xFEA5,0xFEA6,0xFEA7,0xFEA8},
					{0x062F,2,0xFEA9,0xFEAA,0x0000,0x0000},
					{0x0630,2,0xFEAB,0xFEAC,0x0000,0x0000},
					{0x0631,2,0xFEAD,0xFEAE,0x0000,0x0000},
					{0x0632,2,0xFEAF,0xFEB0,0x0000,0x0000},
					{0x0633,3,0xFEB1,0xFEB2,0xFEB3,0xFEB4},
					{0x0634,3,0xFEB5,0xFEB6,0xFEB7,0xFEB8},
					{0x0635,3,0xFEB9,0xFEBA,0xFEBB,0xFEBC},
					{0x0636,3,0xFEBD,0xFEBE,0xFEBF,0xFEC0},
					{0x0637,3,0xFEC1,0xFEC2,0xFEC3,0xFEC4},
					{0x0638,3,0xFEC5,0xFEC6,0xFEC7,0xFEC8},
					{0x0639,3,0xFEC9,0xFECA,0xFECB,0xFECC},
					{0x063A,3,0xFECD,0xFECE,0xFECF,0xFED0},
					{0x0641,3,0xFED1,0xFED2,0xFED3,0xFED4},
					{0x0642,3,0xFED5,0xFED6,0xFED7,0xFED8},
					{0x0643,3,0xFED9,0xFEDA,0xFEDB,0xFEDC},
					{0x0644,3,0xFEDD,0xFEDE,0xFEDF,0xFEE0},
					{0x0645,3,0xFEE1,0xFEE2,0xFEE3,0xFEE4},
					{0x0646,3,0xFEE5,0xFEE6,0xFEE7,0xFEE8},
					{0x0647,3,0xFEE9,0xFEEA,0xFEEB,0xFEEC},
					{0x0648,2,0xFEED,0xFEEE,0x0000,0x0000},
					{0x0649,3,0xFEEF,0xFEF0,0xFBE8,0xFBE9},
					{0x064A,3,0xFEF1,0xFEF2,0xFEF3,0xFEF4},
					{0x0671,2,0xFB50,0xFB51,0x0000,0x0000},
					{0x0677,2,0xFBDD,0x0677,0x0000,0x0000},
					{0x0679,3,0xFB66,0xFB67,0xFB68,0xFB69},
					{0x067A,3,0xFB5E,0xFB5F,0xFB60,0xFB61},
					{0x067B,3,0xFB52,0xFB53,0xFB54,0xFB55},
					{0x067E,3,0xFB56,0xFB57,0xFB58,0xFB59},
					{0x067F,3,0xFB62,0xFB63,0xFB64,0xFB65},
					{0x0680,3,0xFB5A,0xFB5B,0xFB5C,0xFB5D},
					{0x0683,3,0xFB76,0xFB77,0xFB78,0xFB79},
					{0x0684,3,0xFB72,0xFB73,0xFB74,0xFB75},
					{0x0686,3,0xFB7A,0xFB7B,0xFB7C,0xFB7D},
					{0x0687,3,0xFB7E,0xFB7F,0xFB80,0xFB81},
					{0x0688,2,0xFB88,0xFB89,0x0000,0x0000},
					{0x068C,2,0xFB84,0xFB85,0x0000,0x0000},
					{0x068D,2,0xFB82,0xFB83,0x0000,0x0000},
					{0x068E,2,0xFB86,0xFB87,0x0000,0x0000},
					{0x0691,2,0xFB8C,0xFB8D,0x0000,0x0000},
					{0x0698,2,0xFB8A,0xFB8B,0x0000,0x0000},
					{0x06A4,3,0xFB6A,0xFB6B,0xFB6C,0xFB6D},
					{0x06A6,3,0xFB6E,0xFB6F,0xFB70,0xFB71},
					{0x06A9,3,0xFB8E,0xFB8F,0xFB90,0xFB91},
					{0x06AD,3,0xFBD3,0xFBD4,0xFBD5,0xFBD6},
					{0x06AF,3,0xFB92,0xFB93,0xFB94,0xFB95},
					{0x06B1,3,0xFB9A,0xFB9B,0xFB9C,0xFB9D},
					{0x06B3,3,0xFB96,0xFB97,0xFB98,0xFB99},
					{0x06BA,2,0xFB9E,0xFB9F,0x0000,0x0000},
					{0x06BB,3,0xFBA0,0xFBA1,0xFBA2,0xFBA3},
					{0x06BE,3,0xFBAA,0xFBAB,0xFBAC,0xFBAD},
					{0x06C0,2,0xFBA4,0xFBA5,0x0000,0x0000},
					{0x06C1,3,0xFBA6,0xFBA7,0xFBA8,0xFBA9},
					{0x06C5,2,0xFBE0,0xFBE1,0x0000,0x0000},
					{0x06C6,2,0xFBD9,0xFBDA,0x0000,0x0000},
					{0x06C7,2,0xFBD7,0xFBD8,0x0000,0x0000},
					{0x06C8,2,0xFBDB,0xFBDC,0x0000,0x0000},
					{0x06C9,2,0xFBE2,0xFBE3,0x0000,0x0000},
					{0x06CB,2,0xFBDE,0xFBDF,0x0000,0x0000},
					{0x06CC,3,0xFBFC,0xFBFD,0xFBFE,0xFBFF},
					{0x06D0,3,0xFBE4,0xFBE5,0xFBE6,0xFBE7},
					{0x06D2,2,0xFBAE,0xFBAF,0x0000,0x0000},
					{0x06D3,2,0xFBB0,0xFBB1,0x0000,0x0000},

                    //diacritics
                    {0x064B,3,0x064B,0x064B,0x064B,0x064B},
					{0x064C,3,0x064C,0x064C,0x064C,0x064C},
					{0x064D,3,0x064D,0x064D,0x064D,0x064D},
					{0x064E,3,0x064E,0x064E,0x064E,0x064E},
					{0x064F,3,0x064F,0x064F,0x064F,0x064F},
					{0x0650,3,0x0650,0x0650,0x0650,0x0650},
					{0x0651,3,0x0651,0x0651,0x0651,0x0651},
					{0x0652,3,0x0652,0x0652,0x0652,0x0652},
					{0x0653,3,0x0653,0x0653,0x0653,0x0653},
					{0x0654,3,0x0654,0x0654,0x0654,0x0654},
					{0x0655,3,0x0655,0x0655,0x0655,0x0655},
					{0x0656,3,0x0656,0x0656,0x0656,0x0656},
					{0x0657,3,0x0657,0x0657,0x0657,0x0657},
					{0x0658,3,0x0658,0x0658,0x0658,0x0658},
					{0x0659,3,0x0659,0x0659,0x0659,0x0659},
					{0x065A,3,0x065A,0x065A,0x065A,0x065A},
					{0x065B,3,0x065B,0x065B,0x065B,0x065B},
					{0x065C,3,0x065C,0x065C,0x065C,0x065C},
					{0x065D,3,0x065D,0x065D,0x065D,0x065D},
					{0x065E,3,0x065E,0x065E,0x065E,0x065E},
					{0x065F,3,0x065F,0x065F,0x065F,0x065F}
				};

				ArabicTable = new ushort[0x0300, 5];
				for (int index = 0; index < arabicTableSize; index++)
				{
					uint tempInt = tempArabicTable[index, 0] - 0x0500u;
					ArabicTable[tempInt, 0] = tempArabicTable[index, 1];
					ArabicTable[tempInt, 1] = tempArabicTable[index, 2];
					ArabicTable[tempInt, 2] = tempArabicTable[index, 3];
					ArabicTable[tempInt, 3] = tempArabicTable[index, 4];
					ArabicTable[tempInt, 4] = tempArabicTable[index, 5];
				}
				for (ushort index = 0x0590; index <= 0x05ff; index++)
				{
					ArabicTable[index - 0x0500, 0] = 1;
					ArabicTable[index - 0x0500, 1] = index;
				}
				for (ushort index = 0xfb1d; index <= 0xfb4f; index++)
				{
					ArabicTable[index - 0xf900, 0] = 1;
					ArabicTable[index - 0xf900, 1] = index;
				}
				#endregion

				#region Ligatures
				LigaturesTable = new string[ligaturesTableSize, 2]
				{
					// ligatures Arabic
					{   "\uFEDF\uFE82", "\uFEF5"    },
					{   "\uFEE0\uFE82", "\uFEF6"    },
					{   "\uFEDF\uFE84", "\uFEF7"    },
					{   "\uFEE0\uFE84", "\uFEF8"    },
					{   "\uFEDF\uFE88", "\uFEF9"    },
					{   "\uFEE0\uFE88", "\uFEFA"    },
					{   "\uFEDF\uFE8E", "\uFEFB"    },
					{   "\uFEE0\uFE8E", "\uFEFC"    },

					// ligatures Hebrew
					{   "\u05E9\u05C1", "\uFB2A"    },
					{   "\u05E9\u05C2", "\uFB2B"    },
					{   "\uFB49\u05C1", "\uFB2C"    },
					{   "\uFB49\u05C2", "\uFB2D"    },
					{   "\u05D0\u05B7", "\uFB2E"    },
					{   "\u05D0\u05B8", "\uFB2F"    },
					{   "\u05D0\u05BC", "\uFB30"    },
					{   "\u05D1\u05BC", "\uFB31"    },
					{   "\u05D2\u05BC", "\uFB32"    },
					{   "\u05D3\u05BC", "\uFB33"    },
					{   "\u05D4\u05BC", "\uFB34"    },
					{   "\u05D5\u05BC", "\uFB35"    },
					{   "\u05D6\u05BC", "\uFB36"    },
					{   "\u05D8\u05BC", "\uFB38"    },
					{   "\u05D9\u05BC", "\uFB39"    },
					{   "\u05DA\u05BC", "\uFB3A"    },
					{   "\u05DB\u05BC", "\uFB3B"    },
					{   "\u05DC\u05BC", "\uFB3C"    },
					{   "\u05DE\u05BC", "\uFB3E"    },
					{   "\u05E0\u05BC", "\uFB40"    },
					{   "\u05E1\u05BC", "\uFB41"    },
					{   "\u05E3\u05BC", "\uFB43"    },
					{   "\u05E4\u05BC", "\uFB44"    },
					{   "\u05E6\u05BC", "\uFB46"    },
					{   "\u05E7\u05BC", "\uFB47"    },
					{   "\u05E8\u05BC", "\uFB48"    },
					{   "\u05E9\u05BC", "\uFB49"    },
					{   "\u05EA\u05BC", "\uFB4A"    },
					{   "\u05D5\u05B9", "\uFB4B"    },
					{   "\u05D1\u05BF", "\uFB4C"    },
					{   "\u05DB\u05BF", "\uFB4D"    },
					{   "\u05E4\u05BF", "\uFB4E"    }
					//{	"\u05D0\u05DC",	"\uFB4F"	}	//unicode standard, but has approximate value
				};
				#endregion

				#region MirrorChars
				MirrorChars = new ushort[65536];
                MirrorChars[0x0028] = 0x0029; // LEFT PARENTHESIS
				MirrorChars[0x0029] = 0x0028; // RIGHT PARENTHESIS
				MirrorChars[0x003C] = 0x003E; // LESS-THAN SIGN
				MirrorChars[0x003E] = 0x003C; // GREATER-THAN SIGN
				MirrorChars[0x005B] = 0x005D; // LEFT SQUARE BRACKET
				MirrorChars[0x005D] = 0x005B; // RIGHT SQUARE BRACKET
				MirrorChars[0x007B] = 0x007D; // LEFT CURLY BRACKET
				MirrorChars[0x007D] = 0x007B; // RIGHT CURLY BRACKET
				MirrorChars[0x00AB] = 0x00BB; // LEFT-POINTING DOUBLE ANGLE QUOTATION MARK
				MirrorChars[0x00BB] = 0x00AB; // RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK
				MirrorChars[0x2039] = 0x203A; // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
				MirrorChars[0x203A] = 0x2039; // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
				MirrorChars[0x2045] = 0x2046; // LEFT SQUARE BRACKET WITH QUILL
				MirrorChars[0x2046] = 0x2045; // RIGHT SQUARE BRACKET WITH QUILL
				MirrorChars[0x207D] = 0x207E; // SUPERSCRIPT LEFT PARENTHESIS
				MirrorChars[0x207E] = 0x207D; // SUPERSCRIPT RIGHT PARENTHESIS
				MirrorChars[0x208D] = 0x208E; // SUBSCRIPT LEFT PARENTHESIS
				MirrorChars[0x208E] = 0x208D; // SUBSCRIPT RIGHT PARENTHESIS
				MirrorChars[0x2208] = 0x220B; // ELEMENT OF
				MirrorChars[0x2209] = 0x220C; // NOT AN ELEMENT OF
				MirrorChars[0x220A] = 0x220D; // SMALL ELEMENT OF
				MirrorChars[0x220B] = 0x2208; // CONTAINS AS MEMBER
				MirrorChars[0x220C] = 0x2209; // DOES NOT CONTAIN AS MEMBER
				MirrorChars[0x220D] = 0x220A; // SMALL CONTAINS AS MEMBER
				MirrorChars[0x2215] = 0x29F5; // DIVISION SLASH
				MirrorChars[0x223C] = 0x223D; // TILDE OPERATOR
				MirrorChars[0x223D] = 0x223C; // REVERSED TILDE
				MirrorChars[0x2243] = 0x22CD; // ASYMPTOTICALLY EQUAL TO
				MirrorChars[0x2252] = 0x2253; // APPROXIMATELY EQUAL TO OR THE IMAGE OF
				MirrorChars[0x2253] = 0x2252; // IMAGE OF OR APPROXIMATELY EQUAL TO
				MirrorChars[0x2254] = 0x2255; // COLON EQUALS
				MirrorChars[0x2255] = 0x2254; // EQUALS COLON
				MirrorChars[0x2264] = 0x2265; // LESS-THAN OR EQUAL TO
				MirrorChars[0x2265] = 0x2264; // GREATER-THAN OR EQUAL TO
				MirrorChars[0x2266] = 0x2267; // LESS-THAN OVER EQUAL TO
				MirrorChars[0x2267] = 0x2266; // GREATER-THAN OVER EQUAL TO
				MirrorChars[0x2268] = 0x2269; // [BEST FIT] LESS-THAN BUT NOT EQUAL TO
				MirrorChars[0x2269] = 0x2268; // [BEST FIT] GREATER-THAN BUT NOT EQUAL TO
				MirrorChars[0x226A] = 0x226B; // MUCH LESS-THAN
				MirrorChars[0x226B] = 0x226A; // MUCH GREATER-THAN
				MirrorChars[0x226E] = 0x226F; // [BEST FIT] NOT LESS-THAN
				MirrorChars[0x226F] = 0x226E; // [BEST FIT] NOT GREATER-THAN
				MirrorChars[0x2270] = 0x2271; // [BEST FIT] NEITHER LESS-THAN NOR EQUAL TO
				MirrorChars[0x2271] = 0x2270; // [BEST FIT] NEITHER GREATER-THAN NOR EQUAL TO
				MirrorChars[0x2272] = 0x2273; // [BEST FIT] LESS-THAN OR EQUIVALENT TO
				MirrorChars[0x2273] = 0x2272; // [BEST FIT] GREATER-THAN OR EQUIVALENT TO
				MirrorChars[0x2274] = 0x2275; // [BEST FIT] NEITHER LESS-THAN NOR EQUIVALENT TO
				MirrorChars[0x2275] = 0x2274; // [BEST FIT] NEITHER GREATER-THAN NOR EQUIVALENT TO
				MirrorChars[0x2276] = 0x2277; // LESS-THAN OR GREATER-THAN
				MirrorChars[0x2277] = 0x2276; // GREATER-THAN OR LESS-THAN
				MirrorChars[0x2278] = 0x2279; // NEITHER LESS-THAN NOR GREATER-THAN
				MirrorChars[0x2279] = 0x2278; // NEITHER GREATER-THAN NOR LESS-THAN
				MirrorChars[0x227A] = 0x227B; // PRECEDES
				MirrorChars[0x227B] = 0x227A; // SUCCEEDS
				MirrorChars[0x227C] = 0x227D; // PRECEDES OR EQUAL TO
				MirrorChars[0x227D] = 0x227C; // SUCCEEDS OR EQUAL TO
				MirrorChars[0x227E] = 0x227F; // [BEST FIT] PRECEDES OR EQUIVALENT TO
				MirrorChars[0x227F] = 0x227E; // [BEST FIT] SUCCEEDS OR EQUIVALENT TO
				MirrorChars[0x2280] = 0x2281; // [BEST FIT] DOES NOT PRECEDE
				MirrorChars[0x2281] = 0x2280; // [BEST FIT] DOES NOT SUCCEED
				MirrorChars[0x2282] = 0x2283; // SUBSET OF
				MirrorChars[0x2283] = 0x2282; // SUPERSET OF
				MirrorChars[0x2284] = 0x2285; // [BEST FIT] NOT A SUBSET OF
				MirrorChars[0x2285] = 0x2284; // [BEST FIT] NOT A SUPERSET OF
				MirrorChars[0x2286] = 0x2287; // SUBSET OF OR EQUAL TO
				MirrorChars[0x2287] = 0x2286; // SUPERSET OF OR EQUAL TO
				MirrorChars[0x2288] = 0x2289; // [BEST FIT] NEITHER A SUBSET OF NOR EQUAL TO
				MirrorChars[0x2289] = 0x2288; // [BEST FIT] NEITHER A SUPERSET OF NOR EQUAL TO
				MirrorChars[0x228A] = 0x228B; // [BEST FIT] SUBSET OF WITH NOT EQUAL TO
				MirrorChars[0x228B] = 0x228A; // [BEST FIT] SUPERSET OF WITH NOT EQUAL TO
				MirrorChars[0x228F] = 0x2290; // SQUARE IMAGE OF
				MirrorChars[0x2290] = 0x228F; // SQUARE ORIGINAL OF
				MirrorChars[0x2291] = 0x2292; // SQUARE IMAGE OF OR EQUAL TO
				MirrorChars[0x2292] = 0x2291; // SQUARE ORIGINAL OF OR EQUAL TO
				MirrorChars[0x2298] = 0x29B8; // CIRCLED DIVISION SLASH
				MirrorChars[0x22A2] = 0x22A3; // RIGHT TACK
				MirrorChars[0x22A3] = 0x22A2; // LEFT TACK
				MirrorChars[0x22A6] = 0x2ADE; // ASSERTION
				MirrorChars[0x22A8] = 0x2AE4; // TRUE
				MirrorChars[0x22A9] = 0x2AE3; // FORCES
				MirrorChars[0x22AB] = 0x2AE5; // DOUBLE VERTICAL BAR DOUBLE RIGHT TURNSTILE
				MirrorChars[0x22B0] = 0x22B1; // PRECEDES UNDER RELATION
				MirrorChars[0x22B1] = 0x22B0; // SUCCEEDS UNDER RELATION
				MirrorChars[0x22B2] = 0x22B3; // NORMAL SUBGROUP OF
				MirrorChars[0x22B3] = 0x22B2; // CONTAINS AS NORMAL SUBGROUP
				MirrorChars[0x22B4] = 0x22B5; // NORMAL SUBGROUP OF OR EQUAL TO
				MirrorChars[0x22B5] = 0x22B4; // CONTAINS AS NORMAL SUBGROUP OR EQUAL TO
				MirrorChars[0x22B6] = 0x22B7; // ORIGINAL OF
				MirrorChars[0x22B7] = 0x22B6; // IMAGE OF
				MirrorChars[0x22C9] = 0x22CA; // LEFT NORMAL FACTOR SEMIDIRECT PRODUCT
				MirrorChars[0x22CA] = 0x22C9; // RIGHT NORMAL FACTOR SEMIDIRECT PRODUCT
				MirrorChars[0x22CB] = 0x22CC; // LEFT SEMIDIRECT PRODUCT
				MirrorChars[0x22CC] = 0x22CB; // RIGHT SEMIDIRECT PRODUCT
				MirrorChars[0x22CD] = 0x2243; // REVERSED TILDE EQUALS
				MirrorChars[0x22D0] = 0x22D1; // DOUBLE SUBSET
				MirrorChars[0x22D1] = 0x22D0; // DOUBLE SUPERSET
				MirrorChars[0x22D6] = 0x22D7; // LESS-THAN WITH DOT
				MirrorChars[0x22D7] = 0x22D6; // GREATER-THAN WITH DOT
				MirrorChars[0x22D8] = 0x22D9; // VERY MUCH LESS-THAN
				MirrorChars[0x22D9] = 0x22D8; // VERY MUCH GREATER-THAN
				MirrorChars[0x22DA] = 0x22DB; // LESS-THAN EQUAL TO OR GREATER-THAN
				MirrorChars[0x22DB] = 0x22DA; // GREATER-THAN EQUAL TO OR LESS-THAN
				MirrorChars[0x22DC] = 0x22DD; // EQUAL TO OR LESS-THAN
				MirrorChars[0x22DD] = 0x22DC; // EQUAL TO OR GREATER-THAN
				MirrorChars[0x22DE] = 0x22DF; // EQUAL TO OR PRECEDES
				MirrorChars[0x22DF] = 0x22DE; // EQUAL TO OR SUCCEEDS
				MirrorChars[0x22E0] = 0x22E1; // [BEST FIT] DOES NOT PRECEDE OR EQUAL
				MirrorChars[0x22E1] = 0x22E0; // [BEST FIT] DOES NOT SUCCEED OR EQUAL
				MirrorChars[0x22E2] = 0x22E3; // [BEST FIT] NOT SQUARE IMAGE OF OR EQUAL TO
				MirrorChars[0x22E3] = 0x22E2; // [BEST FIT] NOT SQUARE ORIGINAL OF OR EQUAL TO
				MirrorChars[0x22E4] = 0x22E5; // [BEST FIT] SQUARE IMAGE OF OR NOT EQUAL TO
				MirrorChars[0x22E5] = 0x22E4; // [BEST FIT] SQUARE ORIGINAL OF OR NOT EQUAL TO
				MirrorChars[0x22E6] = 0x22E7; // [BEST FIT] LESS-THAN BUT NOT EQUIVALENT TO
				MirrorChars[0x22E7] = 0x22E6; // [BEST FIT] GREATER-THAN BUT NOT EQUIVALENT TO
				MirrorChars[0x22E8] = 0x22E9; // [BEST FIT] PRECEDES BUT NOT EQUIVALENT TO
				MirrorChars[0x22E9] = 0x22E8; // [BEST FIT] SUCCEEDS BUT NOT EQUIVALENT TO
				MirrorChars[0x22EA] = 0x22EB; // [BEST FIT] NOT NORMAL SUBGROUP OF
				MirrorChars[0x22EB] = 0x22EA; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP
				MirrorChars[0x22EC] = 0x22ED; // [BEST FIT] NOT NORMAL SUBGROUP OF OR EQUAL TO
				MirrorChars[0x22ED] = 0x22EC; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP OR EQUAL
				MirrorChars[0x22F0] = 0x22F1; // UP RIGHT DIAGONAL ELLIPSIS
				MirrorChars[0x22F1] = 0x22F0; // DOWN RIGHT DIAGONAL ELLIPSIS
				MirrorChars[0x22F2] = 0x22FA; // ELEMENT OF WITH LONG HORIZONTAL STROKE
				MirrorChars[0x22F3] = 0x22FB; // ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
				MirrorChars[0x22F4] = 0x22FC; // SMALL ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
				MirrorChars[0x22F6] = 0x22FD; // ELEMENT OF WITH OVERBAR
				MirrorChars[0x22F7] = 0x22FE; // SMALL ELEMENT OF WITH OVERBAR
				MirrorChars[0x22FA] = 0x22F2; // CONTAINS WITH LONG HORIZONTAL STROKE
				MirrorChars[0x22FB] = 0x22F3; // CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
				MirrorChars[0x22FC] = 0x22F4; // SMALL CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
				MirrorChars[0x22FD] = 0x22F6; // CONTAINS WITH OVERBAR
				MirrorChars[0x22FE] = 0x22F7; // SMALL CONTAINS WITH OVERBAR
				MirrorChars[0x2308] = 0x2309; // LEFT CEILING
				MirrorChars[0x2309] = 0x2308; // RIGHT CEILING
				MirrorChars[0x230A] = 0x230B; // LEFT FLOOR
				MirrorChars[0x230B] = 0x230A; // RIGHT FLOOR
				MirrorChars[0x2329] = 0x232A; // LEFT-POINTING ANGLE BRACKET
				MirrorChars[0x232A] = 0x2329; // RIGHT-POINTING ANGLE BRACKET
				MirrorChars[0x2768] = 0x2769; // MEDIUM LEFT PARENTHESIS ORNAMENT
				MirrorChars[0x2769] = 0x2768; // MEDIUM RIGHT PARENTHESIS ORNAMENT
				MirrorChars[0x276A] = 0x276B; // MEDIUM FLATTENED LEFT PARENTHESIS ORNAMENT
				MirrorChars[0x276B] = 0x276A; // MEDIUM FLATTENED RIGHT PARENTHESIS ORNAMENT
				MirrorChars[0x276C] = 0x276D; // MEDIUM LEFT-POINTING ANGLE BRACKET ORNAMENT
				MirrorChars[0x276D] = 0x276C; // MEDIUM RIGHT-POINTING ANGLE BRACKET ORNAMENT
				MirrorChars[0x276E] = 0x276F; // HEAVY LEFT-POINTING ANGLE QUOTATION MARK ORNAMENT
				MirrorChars[0x276F] = 0x276E; // HEAVY RIGHT-POINTING ANGLE QUOTATION MARK ORNAMENT
				MirrorChars[0x2770] = 0x2771; // HEAVY LEFT-POINTING ANGLE BRACKET ORNAMENT
				MirrorChars[0x2771] = 0x2770; // HEAVY RIGHT-POINTING ANGLE BRACKET ORNAMENT
				MirrorChars[0x2772] = 0x2773; // LIGHT LEFT TORTOISE SHELL BRACKET
				MirrorChars[0x2773] = 0x2772; // LIGHT RIGHT TORTOISE SHELL BRACKET
				MirrorChars[0x2774] = 0x2775; // MEDIUM LEFT CURLY BRACKET ORNAMENT
				MirrorChars[0x2775] = 0x2774; // MEDIUM RIGHT CURLY BRACKET ORNAMENT
				MirrorChars[0x27D5] = 0x27D6; // LEFT OUTER JOIN
				MirrorChars[0x27D6] = 0x27D5; // RIGHT OUTER JOIN
				MirrorChars[0x27DD] = 0x27DE; // LONG RIGHT TACK
				MirrorChars[0x27DE] = 0x27DD; // LONG LEFT TACK
				MirrorChars[0x27E2] = 0x27E3; // WHITE CONCAVE-SIDED DIAMOND WITH LEFTWARDS TICK
				MirrorChars[0x27E3] = 0x27E2; // WHITE CONCAVE-SIDED DIAMOND WITH RIGHTWARDS TICK
				MirrorChars[0x27E4] = 0x27E5; // WHITE SQUARE WITH LEFTWARDS TICK
				MirrorChars[0x27E5] = 0x27E4; // WHITE SQUARE WITH RIGHTWARDS TICK
				MirrorChars[0x27E6] = 0x27E7; // MATHEMATICAL LEFT WHITE SQUARE BRACKET
				MirrorChars[0x27E7] = 0x27E6; // MATHEMATICAL RIGHT WHITE SQUARE BRACKET
				MirrorChars[0x27E8] = 0x27E9; // MATHEMATICAL LEFT ANGLE BRACKET
				MirrorChars[0x27E9] = 0x27E8; // MATHEMATICAL RIGHT ANGLE BRACKET
				MirrorChars[0x27EA] = 0x27EB; // MATHEMATICAL LEFT DOUBLE ANGLE BRACKET
				MirrorChars[0x27EB] = 0x27EA; // MATHEMATICAL RIGHT DOUBLE ANGLE BRACKET
				MirrorChars[0x2983] = 0x2984; // LEFT WHITE CURLY BRACKET
				MirrorChars[0x2984] = 0x2983; // RIGHT WHITE CURLY BRACKET
				MirrorChars[0x2985] = 0x2986; // LEFT WHITE PARENTHESIS
				MirrorChars[0x2986] = 0x2985; // RIGHT WHITE PARENTHESIS
				MirrorChars[0x2987] = 0x2988; // Z NOTATION LEFT IMAGE BRACKET
				MirrorChars[0x2988] = 0x2987; // Z NOTATION RIGHT IMAGE BRACKET
				MirrorChars[0x2989] = 0x298A; // Z NOTATION LEFT BINDING BRACKET
				MirrorChars[0x298A] = 0x2989; // Z NOTATION RIGHT BINDING BRACKET
				MirrorChars[0x298B] = 0x298C; // LEFT SQUARE BRACKET WITH UNDERBAR
				MirrorChars[0x298C] = 0x298B; // RIGHT SQUARE BRACKET WITH UNDERBAR
				MirrorChars[0x298D] = 0x2990; // LEFT SQUARE BRACKET WITH TICK IN TOP CORNER
				MirrorChars[0x298E] = 0x298F; // RIGHT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
				MirrorChars[0x298F] = 0x298E; // LEFT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
				MirrorChars[0x2990] = 0x298D; // RIGHT SQUARE BRACKET WITH TICK IN TOP CORNER
				MirrorChars[0x2991] = 0x2992; // LEFT ANGLE BRACKET WITH DOT
				MirrorChars[0x2992] = 0x2991; // RIGHT ANGLE BRACKET WITH DOT
				MirrorChars[0x2993] = 0x2994; // LEFT ARC LESS-THAN BRACKET
				MirrorChars[0x2994] = 0x2993; // RIGHT ARC GREATER-THAN BRACKET
				MirrorChars[0x2995] = 0x2996; // DOUBLE LEFT ARC GREATER-THAN BRACKET
				MirrorChars[0x2996] = 0x2995; // DOUBLE RIGHT ARC LESS-THAN BRACKET
				MirrorChars[0x2997] = 0x2998; // LEFT BLACK TORTOISE SHELL BRACKET
				MirrorChars[0x2998] = 0x2997; // RIGHT BLACK TORTOISE SHELL BRACKET
				MirrorChars[0x29B8] = 0x2298; // CIRCLED REVERSE SOLIDUS
				MirrorChars[0x29C0] = 0x29C1; // CIRCLED LESS-THAN
				MirrorChars[0x29C1] = 0x29C0; // CIRCLED GREATER-THAN
				MirrorChars[0x29C4] = 0x29C5; // SQUARED RISING DIAGONAL SLASH
				MirrorChars[0x29C5] = 0x29C4; // SQUARED FALLING DIAGONAL SLASH
				MirrorChars[0x29CF] = 0x29D0; // LEFT TRIANGLE BESIDE VERTICAL BAR
				MirrorChars[0x29D0] = 0x29CF; // VERTICAL BAR BESIDE RIGHT TRIANGLE
				MirrorChars[0x29D1] = 0x29D2; // BOWTIE WITH LEFT HALF BLACK
				MirrorChars[0x29D2] = 0x29D1; // BOWTIE WITH RIGHT HALF BLACK
				MirrorChars[0x29D4] = 0x29D5; // TIMES WITH LEFT HALF BLACK
				MirrorChars[0x29D5] = 0x29D4; // TIMES WITH RIGHT HALF BLACK
				MirrorChars[0x29D8] = 0x29D9; // LEFT WIGGLY FENCE
				MirrorChars[0x29D9] = 0x29D8; // RIGHT WIGGLY FENCE
				MirrorChars[0x29DA] = 0x29DB; // LEFT DOUBLE WIGGLY FENCE
				MirrorChars[0x29DB] = 0x29DA; // RIGHT DOUBLE WIGGLY FENCE
				MirrorChars[0x29F5] = 0x2215; // REVERSE SOLIDUS OPERATOR
				MirrorChars[0x29F8] = 0x29F9; // BIG SOLIDUS
				MirrorChars[0x29F9] = 0x29F8; // BIG REVERSE SOLIDUS
				MirrorChars[0x29FC] = 0x29FD; // LEFT-POINTING CURVED ANGLE BRACKET
				MirrorChars[0x29FD] = 0x29FC; // RIGHT-POINTING CURVED ANGLE BRACKET
				MirrorChars[0x2A2B] = 0x2A2C; // MINUS SIGN WITH FALLING DOTS
				MirrorChars[0x2A2C] = 0x2A2B; // MINUS SIGN WITH RISING DOTS
				MirrorChars[0x2A2D] = 0x2A2C; // PLUS SIGN IN LEFT HALF CIRCLE
				MirrorChars[0x2A2E] = 0x2A2D; // PLUS SIGN IN RIGHT HALF CIRCLE
				MirrorChars[0x2A34] = 0x2A35; // MULTIPLICATION SIGN IN LEFT HALF CIRCLE
				MirrorChars[0x2A35] = 0x2A34; // MULTIPLICATION SIGN IN RIGHT HALF CIRCLE
				MirrorChars[0x2A3C] = 0x2A3D; // INTERIOR PRODUCT
				MirrorChars[0x2A3D] = 0x2A3C; // RIGHTHAND INTERIOR PRODUCT
				MirrorChars[0x2A64] = 0x2A65; // Z NOTATION DOMAIN ANTIRESTRICTION
				MirrorChars[0x2A65] = 0x2A64; // Z NOTATION RANGE ANTIRESTRICTION
				MirrorChars[0x2A79] = 0x2A7A; // LESS-THAN WITH CIRCLE INSIDE
				MirrorChars[0x2A7A] = 0x2A79; // GREATER-THAN WITH CIRCLE INSIDE
				MirrorChars[0x2A7D] = 0x2A7E; // LESS-THAN OR SLANTED EQUAL TO
				MirrorChars[0x2A7E] = 0x2A7D; // GREATER-THAN OR SLANTED EQUAL TO
				MirrorChars[0x2A7F] = 0x2A80; // LESS-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
				MirrorChars[0x2A80] = 0x2A7F; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
				MirrorChars[0x2A81] = 0x2A82; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
				MirrorChars[0x2A82] = 0x2A81; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
				MirrorChars[0x2A83] = 0x2A84; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE RIGHT
				MirrorChars[0x2A84] = 0x2A83; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE LEFT
				MirrorChars[0x2A8B] = 0x2A8C; // LESS-THAN ABOVE DOUBLE-LINE EQUAL ABOVE GREATER-THAN
				MirrorChars[0x2A8C] = 0x2A8B; // GREATER-THAN ABOVE DOUBLE-LINE EQUAL ABOVE LESS-THAN
				MirrorChars[0x2A91] = 0x2A92; // LESS-THAN ABOVE GREATER-THAN ABOVE DOUBLE-LINE EQUAL
				MirrorChars[0x2A92] = 0x2A91; // GREATER-THAN ABOVE LESS-THAN ABOVE DOUBLE-LINE EQUAL
				MirrorChars[0x2A93] = 0x2A94; // LESS-THAN ABOVE SLANTED EQUAL ABOVE GREATER-THAN ABOVE SLANTED EQUAL
				MirrorChars[0x2A94] = 0x2A93; // GREATER-THAN ABOVE SLANTED EQUAL ABOVE LESS-THAN ABOVE SLANTED EQUAL
				MirrorChars[0x2A95] = 0x2A96; // SLANTED EQUAL TO OR LESS-THAN
				MirrorChars[0x2A96] = 0x2A95; // SLANTED EQUAL TO OR GREATER-THAN
				MirrorChars[0x2A97] = 0x2A98; // SLANTED EQUAL TO OR LESS-THAN WITH DOT INSIDE
				MirrorChars[0x2A98] = 0x2A97; // SLANTED EQUAL TO OR GREATER-THAN WITH DOT INSIDE
				MirrorChars[0x2A99] = 0x2A9A; // DOUBLE-LINE EQUAL TO OR LESS-THAN
				MirrorChars[0x2A9A] = 0x2A99; // DOUBLE-LINE EQUAL TO OR GREATER-THAN
				MirrorChars[0x2A9B] = 0x2A9C; // DOUBLE-LINE SLANTED EQUAL TO OR LESS-THAN
				MirrorChars[0x2A9C] = 0x2A9B; // DOUBLE-LINE SLANTED EQUAL TO OR GREATER-THAN
				MirrorChars[0x2AA1] = 0x2AA2; // DOUBLE NESTED LESS-THAN
				MirrorChars[0x2AA2] = 0x2AA1; // DOUBLE NESTED GREATER-THAN
				MirrorChars[0x2AA6] = 0x2AA7; // LESS-THAN CLOSED BY CURVE
				MirrorChars[0x2AA7] = 0x2AA6; // GREATER-THAN CLOSED BY CURVE
				MirrorChars[0x2AA8] = 0x2AA9; // LESS-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
				MirrorChars[0x2AA9] = 0x2AA8; // GREATER-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
				MirrorChars[0x2AAA] = 0x2AAB; // SMALLER THAN
				MirrorChars[0x2AAB] = 0x2AAA; // LARGER THAN
				MirrorChars[0x2AAC] = 0x2AAD; // SMALLER THAN OR EQUAL TO
				MirrorChars[0x2AAD] = 0x2AAC; // LARGER THAN OR EQUAL TO
				MirrorChars[0x2AAF] = 0x2AB0; // PRECEDES ABOVE SINGLE-LINE EQUALS SIGN
				MirrorChars[0x2AB0] = 0x2AAF; // SUCCEEDS ABOVE SINGLE-LINE EQUALS SIGN
				MirrorChars[0x2AB3] = 0x2AB4; // PRECEDES ABOVE EQUALS SIGN
				MirrorChars[0x2AB4] = 0x2AB3; // SUCCEEDS ABOVE EQUALS SIGN
				MirrorChars[0x2ABB] = 0x2ABC; // DOUBLE PRECEDES
				MirrorChars[0x2ABC] = 0x2ABB; // DOUBLE SUCCEEDS
				MirrorChars[0x2ABD] = 0x2ABE; // SUBSET WITH DOT
				MirrorChars[0x2ABE] = 0x2ABD; // SUPERSET WITH DOT
				MirrorChars[0x2ABF] = 0x2AC0; // SUBSET WITH PLUS SIGN BELOW
				MirrorChars[0x2AC0] = 0x2ABF; // SUPERSET WITH PLUS SIGN BELOW
				MirrorChars[0x2AC1] = 0x2AC2; // SUBSET WITH MULTIPLICATION SIGN BELOW
				MirrorChars[0x2AC2] = 0x2AC1; // SUPERSET WITH MULTIPLICATION SIGN BELOW
				MirrorChars[0x2AC3] = 0x2AC4; // SUBSET OF OR EQUAL TO WITH DOT ABOVE
				MirrorChars[0x2AC4] = 0x2AC3; // SUPERSET OF OR EQUAL TO WITH DOT ABOVE
				MirrorChars[0x2AC5] = 0x2AC6; // SUBSET OF ABOVE EQUALS SIGN
				MirrorChars[0x2AC6] = 0x2AC5; // SUPERSET OF ABOVE EQUALS SIGN
				MirrorChars[0x2ACD] = 0x2ACE; // SQUARE LEFT OPEN BOX OPERATOR
				MirrorChars[0x2ACE] = 0x2ACD; // SQUARE RIGHT OPEN BOX OPERATOR
				MirrorChars[0x2ACF] = 0x2AD0; // CLOSED SUBSET
				MirrorChars[0x2AD0] = 0x2ACF; // CLOSED SUPERSET
				MirrorChars[0x2AD1] = 0x2AD2; // CLOSED SUBSET OR EQUAL TO
				MirrorChars[0x2AD2] = 0x2AD1; // CLOSED SUPERSET OR EQUAL TO
				MirrorChars[0x2AD3] = 0x2AD4; // SUBSET ABOVE SUPERSET
				MirrorChars[0x2AD4] = 0x2AD3; // SUPERSET ABOVE SUBSET
				MirrorChars[0x2AD5] = 0x2AD6; // SUBSET ABOVE SUBSET
				MirrorChars[0x2AD6] = 0x2AD5; // SUPERSET ABOVE SUPERSET
				MirrorChars[0x2ADE] = 0x22A6; // SHORT LEFT TACK
				MirrorChars[0x2AE3] = 0x22A9; // DOUBLE VERTICAL BAR LEFT TURNSTILE
				MirrorChars[0x2AE4] = 0x22A8; // VERTICAL BAR DOUBLE LEFT TURNSTILE
				MirrorChars[0x2AE5] = 0x22AB; // DOUBLE VERTICAL BAR DOUBLE LEFT TURNSTILE
				MirrorChars[0x2AEC] = 0x2AED; // DOUBLE STROKE NOT SIGN
				MirrorChars[0x2AED] = 0x2AEC; // REVERSED DOUBLE STROKE NOT SIGN
				MirrorChars[0x2AF7] = 0x2AF8; // TRIPLE NESTED LESS-THAN
				MirrorChars[0x2AF8] = 0x2AF7; // TRIPLE NESTED GREATER-THAN
				MirrorChars[0x2AF9] = 0x2AFA; // DOUBLE-LINE SLANTED LESS-THAN OR EQUAL TO
				MirrorChars[0x2AFA] = 0x2AF9; // DOUBLE-LINE SLANTED GREATER-THAN OR EQUAL TO
				MirrorChars[0x3008] = 0x3009; // LEFT ANGLE BRACKET
				MirrorChars[0x3009] = 0x3008; // RIGHT ANGLE BRACKET
				MirrorChars[0x300A] = 0x300B; // LEFT DOUBLE ANGLE BRACKET
				MirrorChars[0x300B] = 0x300A; // RIGHT DOUBLE ANGLE BRACKET
				MirrorChars[0x300C] = 0x300D; // [BEST FIT] LEFT CORNER BRACKET
				MirrorChars[0x300D] = 0x300C; // [BEST FIT] RIGHT CORNER BRACKET
				MirrorChars[0x300E] = 0x300F; // [BEST FIT] LEFT WHITE CORNER BRACKET
				MirrorChars[0x300F] = 0x300E; // [BEST FIT] RIGHT WHITE CORNER BRACKET
				MirrorChars[0x3010] = 0x3011; // LEFT BLACK LENTICULAR BRACKET
				MirrorChars[0x3011] = 0x3010; // RIGHT BLACK LENTICULAR BRACKET
				MirrorChars[0x3014] = 0x3015; // LEFT TORTOISE SHELL BRACKET
				MirrorChars[0x3015] = 0x3014; // RIGHT TORTOISE SHELL BRACKET
				MirrorChars[0x3016] = 0x3017; // LEFT WHITE LENTICULAR BRACKET
				MirrorChars[0x3017] = 0x3016; // RIGHT WHITE LENTICULAR BRACKET
				MirrorChars[0x3018] = 0x3019; // LEFT WHITE TORTOISE SHELL BRACKET
				MirrorChars[0x3019] = 0x3018; // RIGHT WHITE TORTOISE SHELL BRACKET
				MirrorChars[0x301A] = 0x301B; // LEFT WHITE SQUARE BRACKET
				MirrorChars[0x301B] = 0x301A; // RIGHT WHITE SQUARE BRACKET
				MirrorChars[0xFF08] = 0xFF09; // FULLWIDTH LEFT PARENTHESIS
				MirrorChars[0xFF09] = 0xFF08; // FULLWIDTH RIGHT PARENTHESIS
				MirrorChars[0xFF1C] = 0xFF1E; // FULLWIDTH LESS-THAN SIGN
				MirrorChars[0xFF1E] = 0xFF1C; // FULLWIDTH GREATER-THAN SIGN
				MirrorChars[0xFF3B] = 0xFF3D; // FULLWIDTH LEFT SQUARE BRACKET
				MirrorChars[0xFF3D] = 0xFF3B; // FULLWIDTH RIGHT SQUARE BRACKET
				MirrorChars[0xFF5B] = 0xFF5D; // FULLWIDTH LEFT CURLY BRACKET
				MirrorChars[0xFF5D] = 0xFF5B; // FULLWIDTH RIGHT CURLY BRACKET
				MirrorChars[0xFF5F] = 0xFF60; // FULLWIDTH LEFT WHITE PARENTHESIS
				MirrorChars[0xFF60] = 0xFF5F; // FULLWIDTH RIGHT WHITE PARENTHESIS
				MirrorChars[0xFF62] = 0xFF63; // [BEST FIT] HALFWIDTH LEFT CORNER BRACKET
				MirrorChars[0xFF63] = 0xFF62; // [BEST FIT] HALFWIDTH RIGHT CORNER BRACKET 
                #endregion
            }
        }
        #endregion
    }
}
