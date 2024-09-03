using QuantLib;

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
            riskFreeRate = 0.1;
            double spotPrice = 815.45;
            //double spotPrice = 814.95;
            double strikePrice = strikePrices[i];
            int expiryDays = 8;
            //int expiryDays = 27;
            double dividend = 0;
            double callPremium = call_premiums[i];
            double putPremium = put_premiums[i];

            //var volatility = GetVolatility(spotPrice, strikePrice, expiryDays, riskFreeRate, dividend, Option.Type.Call, callPremium);
            //Console.WriteLine(ConvertToPercentage(volatility));

            var volatility = GetVolatility(spotPrice, strikePrice, expiryDays, riskFreeRate, dividend, Option.Type.Put, putPremium);
            Console.WriteLine(ConvertToPercentage(volatility));

            //riskFreeRate = 0.066342;
            //riskFreeRate = 0.066152;

            //Console.WriteLine("\nCALL Greeks");
            //var option = GetOption(spotPrice, strikePrice, expiryDays, riskFreeRate, volatility, dividend, Option.Type.Call);
            //var option = GetOption(spotPrice, strikePrice, expiryDays, riskFreeRate, volatility, dividend, Option.Type.Put);

            //Console.WriteLine(GetDelta(option));
            //Console.WriteLine(GetTheta(option));
            //Console.WriteLine(GetGamma(option));
            //Console.WriteLine(GetVega(option));

            //Co0nsole.WriteLine("\nPUT Greeks");
            //CalculateGreeks(spotPrice, strikePrice, expiryDays, riskFreeRate, volatility, dividend, Option.Type.Put);

            //Console.WriteLine("\n\n");
        }
    }

    private static double ConvertToPercentage(double value) => Math.Round(value * 100, 2);

    private static VanillaOption GetOption(double spotPrice, double strikePrice, int expiryDays, double rate, double volatility, double dividend, Option.Type optionType)
    {
        // Payoff and Exercise
        var payoff = new PlainVanillaPayoff(optionType, strikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + expiryDays);

        // Bootstrapped risk-free rate curve with more accurate day count convention
        var riskFreeRate = new FlatForward(Date.todaysDate(), rate, new Actual365Fixed());
        var discountTermStructure = new YieldTermStructureHandle(riskFreeRate);

        // Bootstrapped dividend yield curve with more accurate day count convention
        var dividendYield = new FlatForward(Date.todaysDate(), dividend, new Actual365Fixed());
        var dividendTermStructure = new YieldTermStructureHandle(dividendYield);

        // Use a more accurate guess for the flat volatility term structure
        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), volatility, new Actual365Fixed());
        var volatilityTermStructure = new BlackVolTermStructureHandle(flatVolTS);

        var process = new BlackScholesMertonProcess(
            new QuoteHandle(new SimpleQuote(spotPrice)),
            dividendTermStructure,
            discountTermStructure,
            volatilityTermStructure);

        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        return option;
    }

    public static double GetDelta(VanillaOption vanillaOption)
    {
        return Math.Round(vanillaOption.delta(), 3);
    }

    public static double GetGamma(VanillaOption vanillaOption)
    {
        return Math.Round(vanillaOption.gamma(), 3);
    }

    public static double GetTheta(VanillaOption vanillaOption)
    {
        return Math.Round(vanillaOption.thetaPerDay(), 3);
    }

    public static double GetVega(VanillaOption vanillaOption)
    {
        return Math.Round(vanillaOption.vega() / 100, 3);
    }

    public static double GetRho(VanillaOption vanillaOption)
    {
        return Math.Round(vanillaOption.rho(), 3);
    }

    public static double GetVolatility(
        double spotPrice, double strikePrice, int expiryDays, double rate, double dividend,
        Option.Type optionType, double currentMarketPrice)
    {
        // Payoff and exercise
        var payoff = new PlainVanillaPayoff(optionType, strikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + expiryDays);

        // Bootstrapped risk-free rate curve with more accurate day count convention
        var riskFreeRate = new FlatForward(Date.todaysDate(), rate, new Actual365Fixed());
        var discountTermStructure = new YieldTermStructureHandle(riskFreeRate);

        // Bootstrapped dividend yield curve with more accurate day count convention
        var dividendYield = new FlatForward(Date.todaysDate(), dividend, new Actual365Fixed());
        var dividendTermStructure = new YieldTermStructureHandle(dividendYield);

        // Use a more accurate guess for the flat volatility term structure
        var initialVolGuess = 0.20; // Initial guess at 20%, can be adjusted
        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), initialVolGuess, new Actual365Fixed());
        var volatilityTermStructure = new BlackVolTermStructureHandle(flatVolTS);

        var process = new BlackScholesMertonProcess(
            new QuoteHandle(new SimpleQuote(spotPrice)),
            dividendTermStructure,
            discountTermStructure,
            volatilityTermStructure);

        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        double impliedVolatility = option.impliedVolatility(currentMarketPrice, process);

        return impliedVolatility;
    }
}
