using QuantLib;

namespace MyApp;
public class Program
{
    static void Main(string[] args)
    {
        double spotPrice = 815.45;
        double strikePrice = 870;
        int expiryDays = 8;
        double riskFreeRate = 0.1;
        double dividend = 0;
        double callPremium = 0.90; //66.54
        double putPremium = 52.61;  //54

        Console.WriteLine("\nCALL");

        var volatility = GetVolatility(spotPrice, strikePrice, expiryDays, riskFreeRate, dividend, Option.Type.Call, callPremium);

        Console.WriteLine("Implied Volatility: " + volatility);

        CalculateGreeks(spotPrice, strikePrice, expiryDays, riskFreeRate, volatility, dividend, Option.Type.Call);

        Console.WriteLine("\nPUT");

        volatility = GetVolatility(spotPrice, strikePrice, expiryDays, riskFreeRate, dividend, Option.Type.Put, putPremium);

        Console.WriteLine("Implied Volatility: " + volatility);

        CalculateGreeks(spotPrice, strikePrice, expiryDays, riskFreeRate, volatility, dividend, Option.Type.Put);
    }

    private static void CalculateGreeks(double spotPrice, double strikePrice, int expiryDays, double rate, double valatility, double divindend, Option.Type optionType)
    {
        // Payoff and Exercise
        var payoff = new PlainVanillaPayoff(optionType, strikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + expiryDays);

        // Risk-free rate curve
        var riskFreeRate = new FlatForward(Date.todaysDate(), rate, new Actual360());

        // Volatility surface
        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), valatility, new Actual360());

        var dividendYield = new FlatForward(Date.todaysDate(), divindend, new Actual360());

        // Black-Sh-oles process
        var process = new BlackScholesMertonProcess(
            new QuoteHandle(new SimpleQuote(spotPrice)),
            new YieldTermStructureHandle(dividendYield),
            new YieldTermStructureHandle(riskFreeRate),
            new BlackVolTermStructureHandle(flatVolTS));

        // Option pricing engine
        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        // Output the Greeks
        Console.WriteLine("Delta: " + option.delta());
        Console.WriteLine("Gamma: " + option.gamma());
        Console.WriteLine("Theta: " + option.thetaPerDay());
        Console.WriteLine("Vega: " + option.vega());
        Console.WriteLine("Rho: " + option.rho());

        Console.WriteLine();
    }

    private static double GetVolatility(double spotPrice, double strikePrice, int expiryDays, double rate, double divindend, Option.Type optionType, double currentMarketPrice)
    {
        var payoff = new PlainVanillaPayoff(optionType, strikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + expiryDays);

        // Risk-free rate curve
        var riskFreeRate = new FlatForward(Date.todaysDate(), rate, new Actual360());

        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), 0, new Actual360());

        var dividendYield = new FlatForward(Date.todaysDate(), divindend, new Actual360());

        // Black-Sholes process
        var process = new BlackScholesMertonProcess(
            new QuoteHandle(new SimpleQuote(spotPrice)),
            new YieldTermStructureHandle(dividendYield),
            new YieldTermStructureHandle(riskFreeRate),
            new BlackVolTermStructureHandle(flatVolTS));

        // Option pricing engine
        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        return option.impliedVolatility(currentMarketPrice, process, 1e-6, 1000, 1e-6, 5.0);
    }
}