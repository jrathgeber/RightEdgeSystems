using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using RightEdge.Common;
using RightEdge.Indicators;

public class BreakOutModel
{
	
	public int bars_to_use { get; set; }
	public int bars_to_sell { get; set; }

	public double bo_buy_height_param { get; set; }
	public double bo_sell_height_param { get; set; }
	
	public double adx_param { get; set; }
	
	
	public BreakOutModel(double buy_bars, double sell_bars, double buy_height, double sell_height, double adx_value)
	{
        bars_to_use = (int)buy_bars;
		bars_to_sell = (int)sell_bars;

		bo_buy_height_param = buy_height;
		bo_sell_height_param = sell_height;
		
		adx_param = adx_value;
		
    }

	
	public bool calcBuy (BarData[] LookBackData , double body, double AdxValue, double Vwap) {
	
		bool result = true;

		if (LookBackData[0].BarStartTime.Hour < 10 /* || LookBackData[0].BarStartTime.Hour > 15 */) {
            result = false;
		}

		if (LookBackData[0].Close < Vwap) {
            result = false;
		}
		
		return result;
	}
	
	
		
	// Determins a BreakOut Buy Signal for an Instrument
	public bool calcShort (BarData[] LookBackData , double body, double AdxValue) {
		
		// Default to Shorting
		bool result = true;
		
		bool down = true;
		
		// Check Level of ADX
		if (AdxValue > adx_param) {
			result = false;
		}
				
		// Check Height of Bar
		if (body < bo_buy_height_param  ) {
		//	result = false;
		}
		
		if (LookBackData[0].Close > LookBackData[0].Open) {
            down = false;
		}
		
		// Need to bars to calc rank and start value
		if ((LookBackData[0].BarStartTime.Hour < 10 && LookBackData[0].BarStartTime.Minute < 32) ) {
            result = false;
		}
		
	
		// Only Trade in 9s & 10s
		if (LookBackData[0].BarStartTime.Hour > 13 /* || LookBackData[0].BarStartTime.Hour > 15 */) {
        //    result = false;
		}
				
		
		// Check the BarData for BreakOut
		foreach (BarData bd in LookBackData)
        {
			if( LookBackData[0].BarStartTime !=  bd.BarStartTime) {
		
				if (  LookBackData[0].High < bd.High /*|| bd.Volume == 0*/ ) {
					result = false;
				}
			}
		}
		
        return result && down;
	}
	
	
		// Determins a BreakOut Buy Signal for an Instrument
	public bool calcShortInitial (BarData[] LookBackData , double body, double AdxValue) {
		
		// Default to Shorting
		//bool lookback = true;
		bool lookback = false;
		
		bool downbar = false;
		bool firstbar = false;
		bool secondbar = false;
		bool shortAll = false;
		
		// Check Height of Bar
		if (body < bo_buy_height_param  ) {
		//	lookback = false;
		}

				
		// Wait for first down bar
		if (LookBackData[0].Close < LookBackData[0].Open) {
            downbar = true;
		}

		
		// Short Everything !!
		if ((LookBackData[0].BarStartTime.Hour < 10 && LookBackData[0].BarStartTime.Minute < 32) ) {
            shortAll = true;
		}

		
		// If first bar is down short !!
		//if ((LookBackData[0].BarStartTime.Hour < 10 && LookBackData[0].BarStartTime.Minute < 31) && (LookBackData[0].Close < LookBackData[0].Open)) {
        //    firstbar = true;
		//}

		
		// If Second bar is down short !!
		//if ((LookBackData[0].BarStartTime.Hour < 10 && LookBackData[0].BarStartTime.Minute < 32 && LookBackData[0].BarStartTime.Minute > 30) && (LookBackData[0].Close < LookBackData[0].Open)) {
        //    secondbar = true;
		//}
		
		
		// Only Trade in 9s or 10s
		if ( LookBackData[0].BarStartTime.Hour > 11) {
            lookback = false;
		}
	
		
		// Check the BarData for BreakOut
		foreach (BarData bd in LookBackData)
        {
			if( LookBackData[0].BarStartTime !=  bd.BarStartTime) {
		
				if (  LookBackData[0].High < bd.High /*|| bd.Volume == 0*/ ) {
					lookback = false;
				}
			}
		}
		
		
		
				
		// Check the BarData for Break Down
		foreach (BarData bd in LookBackData)
        {
			// Ignore current bar
			if( LookBackData[0].BarStartTime !=  bd.BarStartTime) {
			
				// previous bar was a breakout
				if (  LookBackData[1].High < bd.High || bd.Volume == 0) {
             		lookback = false;
				}	
			
			}
			
		}
				
		
	    //return (firstbar && secondbar) || (lookback && downbar);	
        //return (firstbar) || (lookback && downbar);	
		return (shortAll && downbar);	
	}
	
	
	
	
	
	
	
	public bool calcTimeup2 (BarData[] LookBackData , double body, SymbolScriptBase sb ) {
		
		bool timeup = false;
		
		DateTime uptime = new DateTime(LookBackData[0].BarStartTime.Year, LookBackData[0].BarStartTime.Month, LookBackData[0].BarStartTime.Day, 15, 30, 0 );
		
		// time is up
		if (LookBackData[0].BarStartTime.Hour >= 15 && LookBackData[0].BarStartTime.Minute >= 55 ) {
            timeup = true;
		}
	
		return timeup;
		
	}
	
	
	
	
	
	
	// Determins a BreakOut Buy Signal for an Instrument
	public bool calcSell (BarData[] LookBackData , double body, double trailPrice, SymbolScriptBase sb ) {
		
		// Default to Selling
		bool result = true;
		
			
		// Check Height of Bar
		if (body < bo_sell_height_param) {
		//	result = false;
		}
				
		if (LookBackData[0].Close > LookBackData[0].Open) {
            result = false;
		}
		
		
		foreach (BarData bd in LookBackData)
        {
			
			if( LookBackData[0].BarStartTime !=  bd.BarStartTime) {
			
				if (  LookBackData[0].Close > bd.Close ) {
				
					result = false;
					
				}
			}
		}


		
		// If past trail then sell
		if (LookBackData[0].Close <= trailPrice ) {
		
			result = true;
			
		}
		
		
		
		return result;
	}	

	
		// Determins a BreakOut Buy Signal for an Instrument
	public bool calcCover (BarData[] LookBackData , double body, double trailPrice, SymbolScriptBase sb ) {
		
		// Default to Selling
		bool upbar = false;
		bool lookback = true;
		bool trail = false;
			
		// Check Height of Bar
		if (body < bo_sell_height_param) {
			 upbar = false;
		}
			
		
		// is current bar up ? 
		if ( LookBackData[0].Close > LookBackData[0].Open) {
			
			upbar = true;
		}
		
		
		// is it a break out ? 
		foreach (BarData bd in LookBackData)
        {
        	if( LookBackData[0].BarStartTime !=  bd.BarStartTime) {
			
				if ( LookBackData[0].Close < bd.High) {
						
					lookback = false;
				}
			}
		}
		
		// Trail it
		if (LookBackData[0].Close >= trailPrice ) {
			trail = true;
		}
		

		return trail || (upbar && lookback);
		//return false;
	}	

	
	
	public bool calcTimeup (BarData[] LookBackData , double body, SymbolScriptBase sb ) {
		
		bool timeup = false;
		
		DateTime uptime = new DateTime(LookBackData[0].BarStartTime.Year, LookBackData[0].BarStartTime.Month, LookBackData[0].BarStartTime.Day, 15, 30, 0 );
		
		// time is up
		if (LookBackData[0].BarStartTime.Hour >= 15 && LookBackData[0].BarStartTime.Minute >= 55 ) {
        //    timeup = true;
		}
	
		return timeup;
		
	}
	
}
