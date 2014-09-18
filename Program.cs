using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChangeCalculator
{
    class Program
    {
        static void Main( string[] args )
        {
			// Initialize our currency denominations
			uint[] denominations = { 500, 100, 50, 20, 10, 5, 1 };

			do
			{
				Console.Clear();

				// Amount to be payed
				double subtotal = readPositiveDouble( Assignment1.Resources.promptAmountToPay );

				// Round our total to closest integer value
				uint total = (uint) Math.Round( subtotal );
				double roundingOffAmount = total - subtotal;

				// Amount actually payed
				uint cash = readUint( Assignment1.Resources.promptAmountPayed, total );

				uint change = cash - total;

				// Calculate cash back in largest possible denominations
				uint[] notes = splitIntoDenominations( change, denominations );

				// Finally print the receipt
				viewReceipt( subtotal, roundingOffAmount, total, cash, change, notes, denominations );

				viewMessage( Assignment1.Resources.promptAgain );

			} while ( Console.ReadKey().KeyChar != 27 ); // If user presses esc (27) exit, otherwise go again
        }

		// Read a double value from console input, with specified prompt
        private static double readPositiveDouble( string prompt )
        {
			// Put in a simple loop check, in case we wish to allow manual aborting in the future
			bool check = true;

			// Initialize value to a known invalid value
			double value = -1.0;

			// Implemented as a conditionless loop, because the other way would have to be
			// recursive method calls, and that could potentially blow the lid off the
			// stack. Not very likely... But it could.
			while ( check )
			{
				// Print prompt message
				Console.Write( "{0,-25}: ", prompt );

				// Read input from console
				string input = Console.ReadLine();

				try
				{
					// Attempt to parse out double from entered string
					value = double.Parse( input );

					// If our input successfully parses and rounds to 1 or more, break loop
					if ( Math.Round( value ) >= 1.0 )
						break;

					// If we're still executing, that means the value was valid, but too small
					viewMessage( Assignment1.Resources.errorAmountTooSmall, true );
				}
				catch ( FormatException e )
				{
					// input has an invalid format, inform user
					viewMessage( Assignment1.Resources.errorFormat, true );
				}
				catch ( OverflowException e )
				{
					// input is too large, inform user
					viewMessage( Assignment1.Resources.errorOverflow, true );
				}
				catch ( ArgumentNullException e )
				{
					// This should only happen when pressing ctrl-c while debugging,
					// there's no well defined behaviour here.

					Environment.Exit( 1 );
				}
			}

			// Return our resulting value
			return value;
        }

		private static uint readUint( string prompt, uint minValue )
		{
			// Most of this is copy-paste from above method

			// Put in a simple loop check, in case we wish to allow manual aborting in the future
			bool check = true;

			// Initialize value to a known invalid value
			uint value = 0;

			// Implemented as a conditionless loop, because the other way would have to be
			// recursive method calls, and that could potentially blow the lid off the
			// stack. Not very likely... But it could.
			while ( check )
			{
				// Print prompt message
				Console.Write( "{0,-25}: ", prompt );

				// Read input from console
				string input = Console.ReadLine();

				try
				{
					// Attempt to parse out uint from entered string
					value = uint.Parse( input );
					
					// If entered value is greater or equal to minimum value, break loop
					if ( value >= minValue )
						break;

					viewMessage( Assignment1.Resources.errorAmountInsufficient, true );
				}
				catch ( FormatException e )
				{
					// input has an invalid format, inform user
					viewMessage( Assignment1.Resources.errorFormat, true );
				}
				catch ( OverflowException e )
				{
					// input is too large, inform user
					viewMessage( Assignment1.Resources.errorOverflow, true );
				}
				catch ( ArgumentNullException e )
				{
					// This should only happen when pressing ctrl-c while debugging,
					// there's no well defined behaviour here.

					Environment.Exit( 1 );
				}
			}
			
			// Return our resulting value
			return value;
		}

		private static uint[] splitIntoDenominations( uint change, uint[] denominations )
		{
			// Create new array to hold notes back
			uint[] notes = new uint[denominations.Length];

			// For each denomination, check how many we can fit in remaining amount
			for ( uint i = 0; i < denominations.Length; i++ )
			{
				// If remaining change is less than the denomination, we obviously can't pay any of those notes
				if ( change >= denominations[i] )
				{
					// Calculate how many notes of this denomination we need to hand back
					uint noteCount = change / denominations[i];

					// Stuff in array
					notes[i] = noteCount;

					// Set remining change
					change %= denominations[i];
				}
			}

			// Back whence you came
			return notes;
		}

		private static void viewMessage( string message, bool isError = false )
		{
			// Empty spacer line
			Console.WriteLine( "" );

			// Set text colour to white, for better clarity
			Console.ForegroundColor = ConsoleColor.White;

			// Select background colour depending on error state
			if ( isError )
				Console.BackgroundColor = ConsoleColor.Red;
			else
				Console.BackgroundColor = ConsoleColor.DarkGreen;

			// Print our message
			Console.WriteLine( message );

			// Reset background colour to default
			Console.ResetColor();

			// End with empty spacer line
			Console.WriteLine( "" );
		}

		private static void viewReceipt( double subtotal, double roundingOffAmount, uint total, uint cash, uint change, uint[] notes, uint[] denominations )
		{
			string headerLine = new String( '-', 35 );

			Console.WriteLine( "" );
			Console.WriteLine( Assignment1.Resources.receiptHeader );
			Console.WriteLine( headerLine );

			Console.WriteLine( "{0,-20}: {1,13:c}", Assignment1.Resources.receiptSubtotal, subtotal );
			Console.WriteLine( "{0,-20}: {1,13:c}", Assignment1.Resources.receiptRounding, roundingOffAmount );
			Console.WriteLine( "{0,-20}: {1,13:c}", Assignment1.Resources.receiptTotal, total );
			Console.WriteLine( "{0,-20}: {1,13:c}", Assignment1.Resources.receiptCash, cash );
			Console.WriteLine( "{0,-20}: {1,13:c}", Assignment1.Resources.receiptChange, change );

			Console.WriteLine( headerLine );

			Console.WriteLine( "" );
			for ( uint i = 0; i < denominations.Length; i++ )
			{
				if ( notes[i] < 1 )
					continue;

				string tag = "notes";
				if ( denominations[i] < 20 )
					tag = "coins";

				Console.WriteLine( "{0,4} {1,-15}: {2}", denominations[i], tag, notes[i] );
			}
		}
    }
}
