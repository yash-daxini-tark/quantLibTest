using QuantLib;
using QuantLibTest;

namespace MyApp;
public class Program
{
    static void Main(string[] args)
    {
        double riskFreeRate = 0.1;

        //double[] strikePrices = [740, 750, 760, 770, 780, 790, 800, 810, 820, 830, 840, 850, 860, 870, 880];
        //double[] call_premiums = [84.60, 72.30, 63.45, 54.20, 45.80, 37.70, 30.55, 24.55, 19.30, 14.60, 10.90, 8.10, 5.90, 4.40, 3.35];
        //double[] put_premiums = [1.75, 2.30, 3.05, 4.10, 5.60, 7.65, 10.55, 14.20, 18.90, 24.20, 30.40, 37.95, 45.20, 53.75, 62.40];

        double[] strikePrices = [700, 750, 760, 770, 780, 790, 800, 810, 820, 830, 840, 850, 860, 870, 950];
        double[] call_premiums = [118.80, 68.25, 58.45, 49.20, 39.55, 30.55, 22.55, 15.45, 10, 5.90, 3.40, 2.15, 1.35, 0.9, 0.25];
        double[] put_premiums = [0.30, 0.55, 0.75, 1.15, 1.75, 2.80, 4.60, 7.40, 11.85, 17.90, 25.35, 34.20, 44.05, 53, 133.50];

        for (int i = 0; i < strikePrices.Length; i++)
        {
            double spotPrice = 815.45;
            //double spotPrice = 814.95;
            double strikePrice = strikePrices[i];
            int expiryDays = 8;
            //int expiryDays = 27;
            //double dividend = 0;
            double callPremium = call_premiums[i];
            double putPremium = put_premiums[i];

            GreeksCalculationInput callInput = new(spotPrice, strikePrices[i], 6.6342, expiryDays, call_premiums[i], Option.Type.Call);
            var delta = new OptionGreekCalculator().CalculateDelta(callInput);

            Console.WriteLine(Math.Round(delta, 3));
            //Console.WriteLine(greeks.Gamma);
            //Console.WriteLine(greeks.Theta);
            //Console.WriteLine(greeks.Vega);
            //Console.WriteLine(greeks.Rho);
        }
    }
}
