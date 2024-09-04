using QuantLib;

namespace QuantLibTest;

public class GreeksCalculationInput
{
    public double SpotPrice { get; }
    public double StrikePrice { get; }
    public double RiskFreeRate { get; }
    public int ExpiryInDays { get; }
    public double PremiumPrice { get; }
    public Option.Type OptionType { get; }

    public GreeksCalculationInput(double spotPrice, double strikePrice, double riskFreeRate, int expiryInDays, double premium, Option.Type optionType)
    {
        SpotPrice = spotPrice;
        StrikePrice = strikePrice;
        RiskFreeRate = ConvertPercentageToDecimal(riskFreeRate);
        ExpiryInDays = expiryInDays;
        PremiumPrice = premium;
        OptionType = optionType;
    }

    private double ConvertPercentageToDecimal(double value)
    {
        return value / 100;
    }
}
