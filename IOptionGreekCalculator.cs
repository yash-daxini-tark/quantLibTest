namespace QuantLibTest;

public interface IOptionGreekCalculator
{
    Greeks Calculate(GreeksCalculationInput input);
    double CalculateTheta(GreeksCalculationInput input);
    double CalculateGamma(GreeksCalculationInput input);
    double CalculateVega(GreeksCalculationInput input);
    double CalculateDelta(GreeksCalculationInput input);
    double CalculateRho(GreeksCalculationInput input);
}
