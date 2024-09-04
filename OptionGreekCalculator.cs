using QuantLib;

namespace QuantLibTest;

public class OptionGreekCalculator : IOptionGreekCalculator
{
    public Greeks Calculate(GreeksCalculationInput input)
    {
        return new Greeks
        {
            Delta = CalculateDelta(input),
            Gamma = CalculateGamma(input),
            Theta = CalculateTheta(input),
            Rho = CalculateRho(input),
            Vega = CalculateVega(input),
        };
    }

    public double CalculateDelta(GreeksCalculationInput input)
    {
        double volatility = GetVolatility(input);
        VanillaOption vanillaOption = CreateVanillaOption(input, volatility);
        return vanillaOption.delta();
    }

    public double CalculateGamma(GreeksCalculationInput input)
    {
        double volatility = GetVolatility(input);
        VanillaOption vanillaOption = CreateVanillaOption(input, volatility);
        return vanillaOption.gamma();
    }

    public double CalculateRho(GreeksCalculationInput input)
    {
        double volatility = GetVolatility(input);
        VanillaOption vanillaOption = CreateVanillaOption(input, volatility);
        return vanillaOption.rho();
    }

    public double CalculateTheta(GreeksCalculationInput input)
    {
        double volatility = GetVolatility(input);
        VanillaOption vanillaOption = CreateVanillaOption(input, volatility);
        return vanillaOption.thetaPerDay();
    }

    public double CalculateVega(GreeksCalculationInput input)
    {
        double volatility = GetVolatility(input);
        VanillaOption vanillaOption = CreateVanillaOption(input, volatility);
        return vanillaOption.vega();
    }

    private static VanillaOption CreateVanillaOption(GreeksCalculationInput input, double volatility)
    {
        // Payoff and Exercise
        var payoff = new PlainVanillaPayoff(input.OptionType, input.StrikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + input.ExpiryInDays);

        // Bootstrapped risk-free rate curve with more accurate day count convention
        var riskFreeRate = new FlatForward(Date.todaysDate(), input.RiskFreeRate, new Actual365Fixed());
        var discountTermStructure = new YieldTermStructureHandle(riskFreeRate);

        // Use a more accurate guess for the flat volatility term structure
        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), volatility, new Actual365Fixed());
        var volatilityTermStructure = new BlackVolTermStructureHandle(flatVolTS);

        var process = new BlackScholesProcess(
            new QuoteHandle(new SimpleQuote(input.SpotPrice)),
            discountTermStructure,
            volatilityTermStructure);

        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        return option;
    }

    public static double GetVolatility(GreeksCalculationInput input)
    {
        double riskFreeRate = 0.1;
        double initialVolatilityGuess = 0.5;

        // Payoff and exercise
        var payoff = new PlainVanillaPayoff(input.OptionType, input.StrikePrice);
        var exercise = new EuropeanExercise(Date.todaysDate() + input.ExpiryInDays);

        // Bootstrapped risk-free rate curve with more accurate day count convention
        var riskFreeRateCurve = new FlatForward(Date.todaysDate(), riskFreeRate, new Actual365Fixed());
        var discountTermStructure = new YieldTermStructureHandle(riskFreeRateCurve);

        var flatVolTS = new BlackConstantVol(Date.todaysDate(), new NullCalendar(), initialVolatilityGuess, new Actual365Fixed());
        var volatilityTermStructure = new BlackVolTermStructureHandle(flatVolTS);

        var process = new BlackScholesProcess(
            new QuoteHandle(new SimpleQuote(input.SpotPrice)),
            discountTermStructure,
            volatilityTermStructure);

        var engine = new AnalyticEuropeanEngine(process);
        var option = new VanillaOption(payoff, exercise);
        option.setPricingEngine(engine);

        double impliedVolatility = option.impliedVolatility(input.PremiumPrice, process);

        return impliedVolatility;
    }
}
